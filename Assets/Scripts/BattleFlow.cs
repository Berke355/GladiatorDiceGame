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

    public DieFace RolledFace { get; private set; }
    // The last action asked to reroll the die and act again (Steady Hand). Cleared by the next roll.
    public bool RerollPending { get; private set; }
    // The last action asked for the rolled face to keep its Charge (Resonance).
    private bool keepRolledCharge;
    public bool IsOver => player.IsDead || enemy.IsDead;

    public BattleFlow(Entity player, Entity enemy, IEnemyBrain brain, List<DieFace> die, List<EquipmentSO> equipment){
        this.player = player;
        this.enemy = enemy;
        this.brain = brain;
        this.die = die;
        triggers = new TriggerSystem(player, enemy, die, equipment, charge);
    }

    public void StartBattle(){
        triggers.Fire(TriggerType.BattleStart);
    }

    // The player's Block resets, then TurnStart items run, then the enemy shows its intent.
    public void StartPlayerTurn(){
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

    // ActionChosen items run first, then the rolled face's effects (and its enchantment's) for the action.
    public void PlayAction(ActionType action){
        triggers.Fire(TriggerType.ActionChosen, action);
        if(IsOver) return;

        EffectContext context = new EffectContext{ source = player, target = enemy, value = charge.ValueOf(RolledFace), die = die, face = RolledFace, charge = charge };
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
        IDisposable disposableBrain = brain as IDisposable;
        if(disposableBrain != null) disposableBrain.Dispose();
    }
}
