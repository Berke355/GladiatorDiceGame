using UnityEngine;
using System;
using System.Collections.Generic;

// The steps of a battle turn, shared by the battle states and the balance simulator, so both run the same
// rules and fire every equipment trigger at the same point. Plain C#: the states decide when each step runs
// (they wait for input and check for the end of the battle between steps).
public class BattleFlow
{
    public readonly Entity player;
    // Every enemy in the battle, in turn order (the first one is the front). Dead enemies stay in the list.
    public readonly List<BattleEnemy> enemies;
    public readonly List<DieFace> die;
    public readonly TriggerSystem triggers;
    // Charge on the die faces for this battle (Hammer).
    public readonly DieCharge charge = new DieCharge();
    // The player's rerolls for this battle (see GDD "Rerolls").
    public readonly RerollPool rerolls;
    // Stone faces on the die for this battle (Medusa's Petrify).
    public readonly Petrification petrification = new Petrification();

    public DieFace RolledFace { get; private set; }
    // The player's action this turn (cleared at the start of each player turn), for enemy threats on that choice.
    public DieFace LastActedFace { get; private set; }
    public int LastActedValue { get; private set; }
    public ActionType LastAction { get; private set; }
    // The last action asked to reroll the die and act again (Steady Hand). Cleared by the next roll.
    public bool RerollPending { get; private set; }
    // The last action asked for the rolled face to keep its Charge (Resonance).
    private bool keepRolledCharge;
    public bool IsOver => player.IsDead || enemies.TrueForAll(enemy => enemy.IsDead) || enemies.Exists(enemy => enemy.endsBattleOnDeath && enemy.IsDead);
    // Creates the entity of an enemy spawned during the battle (a Hydra head): a scene object in battle, a hidden one
    // in the simulator. Null: nothing can spawn.
    private readonly Func<Entity> entityFactory;
    // Raised when an enemy joins the battle after it started (SpawnEnemy), e.g. for the UI to add its card.
    public event Action<BattleEnemy> EnemyAdded;

    // The enemy the player's attacks, statuses and items hit. Defaults to the front living enemy, and moves
    // there again when the selected one dies.
    private BattleEnemy selectedTarget;
    public BattleEnemy Target{
        get{
            if(selectedTarget == null || selectedTarget.IsDead) selectedTarget = enemies.Find(enemy => !enemy.IsDead) ?? selectedTarget;
            return selectedTarget;
        }
    }

    public BattleFlow(Entity player, Entity enemy, IEnemyBrain brain, List<DieFace> die, List<EquipmentSO> equipment, int rerollsPerBattle = 0)
        : this(player, new List<BattleEnemy>{ new BattleEnemy(enemy, brain) }, die, equipment, rerollsPerBattle){ }

    public BattleFlow(Entity player, List<BattleEnemy> enemies, List<DieFace> die, List<EquipmentSO> equipment, int rerollsPerBattle = 0, Func<Entity> entityFactory = null){
        rerolls = new RerollPool(rerollsPerBattle);
        this.player = player;
        this.enemies = enemies;
        this.die = die;
        this.entityFactory = entityFactory;
        triggers = new TriggerSystem(player, () => Target.entity, LivingEnemyEntities, die, equipment, charge, rerolls);
        // Binding can spawn enemies (the Hydra grows its heads), which SpawnEnemy sets up itself.
        int initial = enemies.Count;
        for(int i = 0; i < initial; i++){
            Setup(enemies[i]);
        }
    }

    private void Setup(BattleEnemy enemy){
        enemy.traits = new EnemyTraitSystem(enemy.entity, player, enemy.brain.Traits);
        enemy.brain.Bind(this);
    }

    // Adds a normal enemy to the battle (at the back of the turn order) with full HP. It picks its first intent at the
    // next player turn, so it doesn't act on the turn it appears.
    public BattleEnemy SpawnEnemy(EnemySO data, string name){
        if(entityFactory == null) return null;
        Entity entity = entityFactory();
        entity.entityName = name;
        entity.maxHP = data.maxHP;
        entity.currentHP = data.maxHP;
        BattleEnemy enemy = new BattleEnemy(entity, new EnemyBrain(data, entity, player));
        enemies.Add(enemy);
        Setup(enemy);
        EnemyAdded?.Invoke(enemy);
        return enemy;
    }

    // Selects the target of the player's attacks; returns false (and keeps the old one) for a dead or unknown enemy.
    public bool SelectTarget(BattleEnemy enemy){
        if(enemy == null || enemy.IsDead || !enemies.Contains(enemy)) return false;
        selectedTarget = enemy;
        return true;
    }

    public List<BattleEnemy> LivingEnemies => enemies.FindAll(enemy => !enemy.IsDead);

    // Every living enemy's entity, for area effects (EffectContext.opponents).
    public List<Entity> LivingEnemyEntities(){
        List<Entity> living = new List<Entity>();
        foreach(BattleEnemy enemy in enemies){
            if(!enemy.IsDead) living.Add(enemy.entity);
        }
        return living;
    }

    // Damage every living enemy is about to deal this turn, with Rage (threats not included). Used by bots.
    public int IncomingDamage{
        get{
            int damage = 0;
            foreach(BattleEnemy enemy in enemies){
                if(!enemy.IsDead) damage += enemy.brain.IncomingDamage;
            }
            return damage;
        }
    }

    // Damage the living enemies' threats would deal if the player acted with this face and action.
    public int ThreatAgainst(DieFace face, int battleValue, ActionType action){
        int damage = 0;
        foreach(BattleEnemy enemy in enemies){
            if(!enemy.IsDead) damage += enemy.brain.ThreatAgainst(face, battleValue, action);
        }
        return damage;
    }

    // Whether the face can take this action now: a stone face can only defend (for StoneBlock block).
    public bool CanAct(DieFace face, ActionType action){
        return petrification.IsStone(face) ? action == ActionType.Defense : face.HasAction(action);
    }

    public void StartBattle(){
        triggers.Fire(TriggerType.BattleStart);
    }

    // The player's Block resets, then TurnStart items run, then every living enemy shows its intent.
    public void StartPlayerTurn(){
        rerolls.UsedThisTurn = 0;
        LastActedFace = null;
        triggers.StartRound();
        player.OnTurnStart();
        triggers.Fire(TriggerType.TurnStart);
        foreach(BattleEnemy enemy in enemies){
            if(!enemy.IsDead) enemy.brain.ChooseNextIntent();
        }
    }

    // Also used for a reroll within the same turn, which is not a new turn (no turn start, no Block reset).
    public DieFace Roll(){
        RerollPending = false;
        keepRolledCharge = false;
        RolledFace = die[UnityEngine.Random.Range(0, die.Count)];
        triggers.currentFace = RolledFace;
        triggers.Fire(TriggerType.FaceRolled);
        return RolledFace;
    }

    // A reroll can be spent once the die has landed and before an action is chosen; the new face must be used.
    public bool CanReroll => RolledFace != null && rerolls.Left > 0 && !IsOver && player.GetStatus(StatusType.Entangled) == 0;

    // Spends a reroll and rolls again. Not a new turn: Block stays, the intents stay; FaceRolled items fire again.
    public DieFace RerollFace(){
        rerolls.Left--;
        rerolls.UsedThisTurn++;
        rerolls.UsedThisBattle++;
        DieFace face = Roll();
        foreach(BattleEnemy enemy in LivingEnemies){
            enemy.traits.PlayerRerolled();
        }
        return face;
    }

    // ActionChosen items run first, then the enemies' traits, then the rolled face's effects (and its enchantment's)
    // on the selected target.
    public void PlayAction(ActionType action){
        LastActedFace = RolledFace;
        LastActedValue = charge.ValueOf(RolledFace);
        LastAction = action;
        triggers.Fire(TriggerType.ActionChosen, action);
        if(IsOver) return;
        BattleEnemy target = Target;
        foreach(BattleEnemy enemy in LivingEnemies){
            enemy.traits.PlayerChoseAction(action, enemy == target);
        }
        if(IsOver) return;

        // A stone face only gives a little block; its own actions and its enchantment don't work.
        if(petrification.IsStone(RolledFace)){
            if(action == ActionType.Defense) player.GainBlock(Petrification.StoneBlock);
            RerollPending = false;
            keepRolledCharge = false;
            return;
        }

        EffectContext context = new EffectContext{ source = player, target = Target.entity, opponents = LivingEnemyEntities(), value = charge.ValueOf(RolledFace), die = die, face = RolledFace, charge = charge, rerolls = rerolls };
        foreach(EffectSO effect in RolledFace.GetEffects(action)){
            effect.Execute(context);
        }
        RerollPending = context.rerollRequested;
        keepRolledCharge = context.keepCharge;
    }

    // TurnEnd items run before the player's statuses tick.
    // The face you acted with spends its Charge (unless its action kept it); every other charging face gains 1.
    public void EndPlayerTurn(){
        triggers.Fire(TriggerType.TurnEnd);
        player.OnTurnEnd();
        foreach(DieFace face in die){
            if(face == RolledFace){
                if(!keepRolledCharge) charge.Reset(face);
            }
            else if(face.definition.charges) charge.Add(face, 1);
        }
        petrification.EndPlayerTurn();
        triggers.currentFace = null;
    }

    // Every living enemy acts in turn order; stops when the battle is over. Enemies can join during the loop
    // (Hydra heads growing back); they have no intent yet, so they don't act.
    public void EnemyAct(){
        for(int i = 0; i < enemies.Count; i++){
            BattleEnemy enemy = enemies[i];
            if(enemy.IsDead) continue;
            enemy.entity.OnTurnStart();
            enemy.brain.ExecuteCurrentIntent();
            if(IsOver) return;
        }
    }

    // Every living enemy's statuses tick and its brain ends its turn, in turn order.
    public void EndEnemyTurn(){
        for(int i = 0; i < enemies.Count; i++){
            BattleEnemy enemy = enemies[i];
            if(enemy.IsDead) continue;
            enemy.entity.OnTurnEnd();
            if(enemy.IsDead) continue;
            enemy.brain.OnTurnEnd();
            if(IsOver) return;
        }
    }

    // Before the run saves the player's HP, so healing items count.
    public void WinBattle(){
        triggers.Fire(TriggerType.BattleWon);
    }

    public void Dispose(){
        triggers.Dispose();
        foreach(BattleEnemy enemy in enemies){
            if(enemy.traits != null) enemy.traits.Dispose();
            IDisposable disposableBrain = enemy.brain as IDisposable;
            if(disposableBrain != null) disposableBrain.Dispose();
        }
    }
}
