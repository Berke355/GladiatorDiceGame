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
    // Consumable faces used this battle; a used one is Change (see GDD "Consumable and Change").
    public readonly Consumables consumables = new Consumables();
    // Change can be used once per turn (see GDD "Consumable and Change").
    public const int MaxChangesPerTurn = 1;
    // The face a Change came from: the next roll lands on any other face.
    private DieFace changeFrom;
    // The player passed this turn (no action): the rolled face counts as unused.
    public bool Passed { get; private set; }
    // Whether the player chose Attack or Magic at least once this turn: the beast (Hound) stays by your side on a turn
    // you only defended (or passed), and bites on the others.
    public bool AttackedThisTurn { get; private set; }
    public int ChangesThisTurn { get; private set; }
    // The last action steered the die (Change, Flip, Tumble): it is rolled again at once and the turn goes on.
    public bool RollsAgainAtOnce { get; private set; }

    public DieFace RolledFace { get; private set; }
    // The player's action this turn (cleared at the start of each player turn), for enemy threats on that choice.
    public DieFace LastActedFace { get; private set; }
    public int LastActedValue { get; private set; }
    public ActionType LastAction { get; private set; }
    private bool hasActedThisBattle;
    // Whether this action differs from the last one this battle (Feint, Balance Scales). Also used by the previews.
    public bool WouldChangeAction(ActionType action){
        return hasActedThisBattle && LastAction != action;
    }
    // The last action asked to reroll the die and act again (Steady Hand). Cleared by the next roll.
    public bool RerollPending { get; private set; }
    // Faces that acted this turn spend their Charge at the turn's end, unless their action kept it (Resonance).
    private readonly HashSet<DieFace> actedThisTurn = new HashSet<DieFace>();
    private readonly HashSet<DieFace> keptChargeThisTurn = new HashSet<DieFace>();
    // Several actions can chain in one turn (effects that also Change); this ends the chain (see GDD "Rerolls").
    public const int MaxActionsPerTurn = 3;
    public int ActionsThisTurn { get; private set; }
    // The player's turn number in this battle (1 on the first turn).
    public int TurnNumber { get; private set; }
    // Provocator: Riposte, Vigil and Reversal strikes this battle, and the hits the player took on the enemies' last turn.
    public int Retaliations { get; private set; }
    public int HitsTakenLastEnemyTurn { get; private set; }
    private int hitsSinceTurnStart;
    private void OnRetaliated(int damage){ Retaliations++; }
    private void OnStruck(Entity attacker){ hitsSinceTurnStart++; }
    // How many times each face came up this turn.
    private readonly Dictionary<DieFace, int> landingsThisTurn = new Dictionary<DieFace, int>();
    public int LandingsThisTurn(DieFace face){
        int count;
        return face != null && landingsThisTurn.TryGetValue(face, out count) ? count : 0;
    }
    // The face the next roll lands on (a steer), and whether a steer brought the current face here.
    private DieFace steerTo;
    public bool ArrivedBySteer { get; private set; }
    // An action let the player choose the next face; ChooseNextFace answers it.
    public bool ChoicePending { get; private set; }
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

    // The enchantments on the die (see GDD "Die Enchantments").
    private readonly List<DieEnchantmentSO> dieEnchantments;

    public BattleFlow(Entity player, List<BattleEnemy> enemies, List<DieFace> die, List<EquipmentSO> equipment, int rerollsPerBattle = 0, Func<Entity> entityFactory = null, List<DieEnchantmentSO> dieEnchantments = null){
        this.dieEnchantments = dieEnchantments;
        int enchantedFaceBonus = 0;
        if(equipment != null){
            foreach(EquipmentSO item in equipment) enchantedFaceBonus += item.enchantedFaceBonus;
        }
        charge.placementBonus = face => DieEnchantmentSO.BonusAt(dieEnchantments, die, die.IndexOf(face)) + (face.enchantment != null ? enchantedFaceBonus : 0);
        charge.placementMultiplier = face => DieEnchantmentSO.MultiplierAt(dieEnchantments, die.IndexOf(face));
        rerolls = new RerollPool(rerollsPerBattle);
        this.player = player;
        this.enemies = enemies;
        this.die = die;
        this.entityFactory = entityFactory;
        triggers = new TriggerSystem(player, () => Target.entity, LivingEnemyEntities, die, equipment, charge, rerolls);
        player.Retaliated += OnRetaliated;
        player.Struck += OnStruck;
        if(equipment != null){
            foreach(EquipmentSO item in equipment) consumables.usesPerBattle += item.extraConsumableUses;
        }
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
        BattleEnemy enemy = new BattleEnemy(entity, new EnemyBrain(data, entity, player)){ sprite = data.sprite, spriteHeight = data.spriteHeight };
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

    // Whether the face can take this action now: a stone face can only defend (for StoneBlock block); every action of
    // a used Consumable face is Change, until this turn's Changes run out.
    public bool CanAct(DieFace face, ActionType action){
        if(petrification.IsStone(face)) return action == ActionType.Defense;
        if(IsChange(face, action)) return ChangesThisTurn < MaxChangesPerTurn;
        return face.HasAction(action);
    }

    // Consumable by its own keyword or by its place on the die (Burnout).
    public bool IsConsumableFace(DieFace face){
        return face != null && (face.definition.IsConsumable || DieEnchantmentSO.ConsumableAt(dieEnchantments, die.IndexOf(face)));
    }

    // An action of a Consumable face already used this battle: it now rolls the die again (Change).
    public bool IsChange(DieFace face, ActionType action){
        return face != null && !petrification.IsStone(face) && consumables.IsUsed(face) && face.HasAction(action);
    }

    public void StartBattle(){
        triggers.Fire(TriggerType.BattleStart);
    }

    // The player's Block resets, then TurnStart items run, then every living enemy shows its intent.
    public void StartPlayerTurn(){
        rerolls.UsedThisTurn = 0;
        ChangesThisTurn = 0;
        ActionsThisTurn = 0;
        TurnNumber++;
        HitsTakenLastEnemyTurn = hitsSinceTurnStart;
        hitsSinceTurnStart = 0;
        landingsThisTurn.Clear();
        actedThisTurn.Clear();
        keptChargeThisTurn.Clear();
        steerTo = null;
        ChoicePending = false;
        Passed = false;
        AttackedThisTurn = false;
        LastActedFace = null;
        triggers.StartRound();
        triggers.turnNumber = TurnNumber;
        foreach(BattleEnemy enemy in enemies) enemy.entity.StartRound();
        player.OnTurnStart();
        triggers.Fire(TriggerType.TurnStart);
        foreach(BattleEnemy enemy in enemies){
            if(!enemy.IsDead) enemy.brain.ChooseNextIntent();
        }
    }

    // Also used for a reroll within the same turn, which is not a new turn (no turn start, no Block reset).
    public DieFace Roll(){
        RerollPending = false;
        RollsAgainAtOnce = false;
        ArrivedBySteer = steerTo != null || changeFrom != null;
        if(steerTo != null){
            RolledFace = steerTo;
        }
        else{
            RolledFace = PickFace(changeFrom);
        }
        steerTo = null;
        changeFrom = null;
        landingsThisTurn[RolledFace] = LandingsThisTurn(RolledFace) + 1;
        triggers.currentFace = RolledFace;
        triggers.arrivedBySteer = ArrivedBySteer;
        triggers.Fire(TriggerType.FaceRolled);
        return RolledFace;
    }

    // A random face, weighted by the die's enchantments (Weighted Die). Change guarantees a face other than the one it
    // came from. If the weights rule out everything left, every candidate counts the same.
    private DieFace PickFace(DieFace exclude){
        List<DieFace> faces = exclude != null && die.Count > 1 ? die.FindAll(face => face != exclude) : die;
        int total = 0;
        foreach(DieFace face in faces) total += DieEnchantmentSO.WeightAt(dieEnchantments, die.IndexOf(face));
        if(total <= 0) return faces[UnityEngine.Random.Range(0, faces.Count)];
        int roll = UnityEngine.Random.Range(0, total);
        foreach(DieFace face in faces){
            roll -= DieEnchantmentSO.WeightAt(dieEnchantments, die.IndexOf(face));
            if(roll < 0) return face;
        }
        return faces[faces.Count - 1];
    }

    // A reroll can be spent once the die has landed and before an action is chosen; the new face must be used.
    public bool CanReroll => RolledFace != null && rerolls.Left > 0 && !IsOver && player.GetStatus(StatusType.Entangled) == 0;

    // Spends a reroll and rolls again. Not a new turn: Block stays, the intents stay; FaceRolled items fire again.
    public DieFace RerollFace(){
        rerolls.Left--;
        rerolls.UsedThisTurn++;
        rerolls.UsedThisBattle++;
        DieFace face = Roll();
        triggers.Fire(TriggerType.Rerolled);
        foreach(BattleEnemy enemy in LivingEnemies){
            enemy.traits.PlayerRerolled();
        }
        return face;
    }

    // ActionChosen items run first, then the enemies' traits, then the rolled face's effects (and its enchantment's)
    // on the selected target.
    public void PlayAction(ActionType action){
        // Change: not an action for items, traits or threats, and not a reroll; the die just rolls again.
        if(IsChange(RolledFace, action)){
            if(ChangesThisTurn >= MaxChangesPerTurn) return;
            ChangesThisTurn++;
            changeFrom = RolledFace;
            RollsAgainAtOnce = true;
            RerollPending = true;
            return;
        }
        ActionsThisTurn++;
        LastActedFace = RolledFace;
        LastActedValue = charge.ValueOf(RolledFace);
        bool actionChanged = WouldChangeAction(action);
        triggers.actionChanged = actionChanged;
        hasActedThisBattle = true;
        LastAction = action;
        if(action != ActionType.Defense) AttackedThisTurn = true;
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
            actedThisTurn.Add(RolledFace);
            RerollPending = false;
            return;
        }

        EffectContext context = new EffectContext{ source = player, target = Target.entity, opponents = LivingEnemyEntities(), value = charge.ValueOf(RolledFace), die = die, face = RolledFace, charge = charge, rerolls = rerolls, faceLandings = LandingsThisTurn(RolledFace), arrivedBySteer = ArrivedBySteer, turnNumber = TurnNumber, actionChanged = actionChanged, targetBrain = Target.brain, retaliations = Retaliations, hitsTakenLastEnemyTurn = HitsTakenLastEnemyTurn };
        foreach(EffectSO effect in RolledFace.GetEffects(action)){
            effect.Execute(context);
        }
        if(IsConsumableFace(RolledFace)) consumables.MarkUsed(RolledFace);
        actedThisTurn.Add(RolledFace);
        if(context.keepCharge) keptChargeThisTurn.Add(RolledFace);

        // The chain goes on only below the actions-per-turn limit; at the limit the turn just ends.
        bool canChain = ActionsThisTurn < MaxActionsPerTurn;
        RerollPending = canChain && (context.rerollRequested || context.steer != RollSteer.None);
        if(canChain && context.steer != RollSteer.None) ApplySteer(context.steer, context.steerPosition);
    }

    // Decides where the next roll lands. A position the die doesn't have (fewer than six faces) falls back to any other face.
    private void ApplySteer(RollSteer steer, int position){
        int here = die.IndexOf(RolledFace);
        if(steer == RollSteer.Chosen){
            ChoicePending = true;
            return;
        }
        RollsAgainAtOnce = true;
        int next = -1;
        if(steer == RollSteer.Opposite && here >= 0 && here < DieLayout.Count){
            next = DieLayout.OppositeOf(here);
        }
        else if(steer == RollSteer.Neighbor && here >= 0 && here < DieLayout.Count){
            List<int> neighbors = DieLayout.NeighborsOf(here).FindAll(position2 => position2 < die.Count);
            if(neighbors.Count > 0) next = neighbors[UnityEngine.Random.Range(0, neighbors.Count)];
        }
        else if(steer == RollSteer.Position){
            next = position;
        }
        if(next >= 0 && next < die.Count) steerTo = die[next];
        else changeFrom = RolledFace;
    }

    // Answers a Chosen steer: the next roll lands on this face.
    public void ChooseNextFace(DieFace face){
        if(!ChoicePending) return;
        ChoicePending = false;
        steerTo = face;
        RollsAgainAtOnce = true;
    }

    // Ends the turn without an action (e.g. when the rolled face can't act): no items, traits or threats fire for an
    // action, and the rolled face keeps its Charge as if it hadn't come up. EndPlayerTurn follows as usual.
    public void PassTurn(){
        Passed = true;
        RerollPending = false;
        RollsAgainAtOnce = false;
    }

    // TurnEnd items run before the player's statuses tick.
    // The face you acted with spends its Charge (unless its action kept it); every other charging face gains 1.
    public void EndPlayerTurn(){
        if(AttackedThisTurn && !Target.IsDead){
            Beast.Bite(new EffectContext{ source = player, target = Target.entity, opponents = LivingEnemyEntities(), die = die, charge = charge, rerolls = rerolls });
        }
        triggers.Fire(TriggerType.TurnEnd);
        player.OnTurnEnd();
        foreach(DieFace face in die){
            if(keptChargeThisTurn.Contains(face)) continue;
            if(actedThisTurn.Contains(face)) charge.Reset(face);
            else if(face.definition.Charges) charge.Add(face, 1);
        }
        petrification.EndPlayerTurn();
        triggers.currentFace = null;
    }

    // Every living enemy acts in turn order; stops when the battle is over. Enemies can join during the loop
    // (Hydra heads growing back); they have no intent yet, so they don't act.
    public void EnemyAct(){
        for(int i = 0; i < enemies.Count; i++){
            EnemyActOne(i);
            if(IsOver) return;
        }
    }

    // One enemy's turn (the list can grow while enemies act, so callers loop by index). Returns false for a dead one,
    // which doesn't act. The battle scene paces these one by one; the simulator runs them all through EnemyAct.
    public bool EnemyActOne(int index){
        BattleEnemy enemy = enemies[index];
        if(enemy.IsDead) return false;
        enemy.entity.OnTurnStart();
        enemy.traits.RunPending();
        enemy.brain.ExecuteCurrentIntent();
        return true;
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
        player.Retaliated -= OnRetaliated;
        player.Struck -= OnStruck;
        foreach(BattleEnemy enemy in enemies){
            if(enemy.traits != null) enemy.traits.Dispose();
            IDisposable disposableBrain = enemy.brain as IDisposable;
            if(disposableBrain != null) disposableBrain.Dispose();
        }
    }
}
