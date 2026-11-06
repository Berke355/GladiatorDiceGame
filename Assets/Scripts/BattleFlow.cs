using UnityEngine;
using System;
using System.Collections.Generic;

// The steps of a battle turn, shared by the battle states and the balance simulator, so both run the same
// rules and fire every equipment trigger at the same point. Plain C#: the states decide when each step runs
// (they wait for input and check for the end of the battle between steps).
public class BattleFlow
{
    public readonly Entity player;
    public readonly Entity enemy;
    public readonly IEnemyBrain brain;
    public readonly List<DieFace> die;
    public readonly TriggerSystem triggers;
    // Charge on the die faces for this battle (Hammer).
    public readonly DieCharge charge = new DieCharge();
    // The player's rerolls for this battle (see GDD "Rerolls").
    public readonly RerollPool rerolls;
    // The enemy's traits, reacting to the player's hits, actions and rerolls (see GDD "Enemy Traits").
    public readonly EnemyTraitSystem enemyTraits;
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
    public bool IsOver => player.IsDead || enemy.IsDead;

    public BattleFlow(Entity player, Entity enemy, IEnemyBrain brain, List<DieFace> die, List<EquipmentSO> equipment, int rerollsPerBattle = 0){
        rerolls = new RerollPool(rerollsPerBattle);
        this.player = player;
        this.enemy = enemy;
        this.brain = brain;
        this.die = die;
        triggers = new TriggerSystem(player, enemy, die, equipment, charge, rerolls);
        brain.Bind(this);
        enemyTraits = new EnemyTraitSystem(enemy, player, brain.Traits);
    }

    // Whether the face can take this action now: a stone face can only defend (for StoneBlock block).
    public bool CanAct(DieFace face, ActionType action){
        return petrification.IsStone(face) ? action == ActionType.Defense : face.HasAction(action);
    }

    public void StartBattle(){
        triggers.Fire(TriggerType.BattleStart);
    }

    // The player's Block resets, then TurnStart items run, then the enemy shows its intent.
    public void StartPlayerTurn(){
        rerolls.UsedThisTurn = 0;
        LastActedFace = null;
        triggers.StartRound();
        player.OnTurnStart();
        triggers.Fire(TriggerType.TurnStart);
        brain.ChooseNextIntent();
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

    // Spends a reroll and rolls again. Not a new turn: Block stays, the intent stays; FaceRolled items fire again.
    public DieFace RerollFace(){
        rerolls.Left--;
        rerolls.UsedThisTurn++;
        rerolls.UsedThisBattle++;
        DieFace face = Roll();
        enemyTraits.PlayerRerolled();
        return face;
    }

    // ActionChosen items run first, then the rolled face's effects (and its enchantment's) for the action.
    public void PlayAction(ActionType action){
        LastActedFace = RolledFace;
        LastActedValue = charge.ValueOf(RolledFace);
        LastAction = action;
        triggers.Fire(TriggerType.ActionChosen, action);
        if(IsOver) return;
        enemyTraits.PlayerChoseAction(action);
        if(IsOver) return;

        // A stone face only gives a little block; its own actions and its enchantment don't work.
        if(petrification.IsStone(RolledFace)){
            if(action == ActionType.Defense) player.GainBlock(Petrification.StoneBlock);
            RerollPending = false;
            keepRolledCharge = false;
            return;
        }

        EffectContext context = new EffectContext{ source = player, target = enemy, value = charge.ValueOf(RolledFace), die = die, face = RolledFace, charge = charge, rerolls = rerolls };
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

    public void EnemyAct(){
        enemy.OnTurnStart();
        brain.ExecuteCurrentIntent();
    }

    public void EndEnemyTurn(){
        enemy.OnTurnEnd();
        brain.OnTurnEnd();
    }

    // Before the run saves the player's HP, so healing items count.
    public void WinBattle(){
        triggers.Fire(TriggerType.BattleWon);
    }

    public void Dispose(){
        triggers.Dispose();
        enemyTraits.Dispose();
        IDisposable disposableBrain = brain as IDisposable;
        if(disposableBrain != null) disposableBrain.Dispose();
    }
}
