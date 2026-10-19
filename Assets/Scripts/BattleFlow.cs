using UnityEngine;
using System.Collections.Generic;

// The steps of a battle turn, shared by the battle states and the balance simulator, so both run the same
// rules and fire every equipment trigger at the same point. Plain C#: the states decide when each step runs
// (they wait for input and check for the end of the battle between steps).
public class BattleFlow
{
    public readonly Entity player;
    public readonly Entity enemy;
    public readonly EnemyBrain brain;
    public readonly List<DieFace> die;
    public readonly TriggerSystem triggers;

    public DieFace RolledFace { get; private set; }
    public bool IsOver => player.IsDead || enemy.IsDead;

    public BattleFlow(Entity player, Entity enemy, EnemyBrain brain, List<DieFace> die, List<EquipmentSO> equipment){
        this.player = player;
        this.enemy = enemy;
        this.brain = brain;
        this.die = die;
        triggers = new TriggerSystem(player, enemy, die, equipment);
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

    public DieFace Roll(){
        RolledFace = die[Random.Range(0, die.Count)];
        triggers.currentFace = RolledFace;
        triggers.Fire(TriggerType.FaceRolled);
        return RolledFace;
    }

    // ActionChosen items run first, then the rolled face's effects (and its modules') for the action.
    public void PlayAction(ActionType action){
        triggers.Fire(TriggerType.ActionChosen, action);
        if(IsOver) return;

        EffectContext context = new EffectContext{ source = player, target = enemy, value = RolledFace.value, die = die, face = RolledFace };
        foreach(EffectSO effect in RolledFace.GetEffects(action)){
            effect.Execute(context);
        }
    }

    // TurnEnd items run before the player's statuses tick.
    public void EndPlayerTurn(){
        triggers.Fire(TriggerType.TurnEnd);
        player.OnTurnEnd();
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
    }
}
