using UnityEngine;

public class PlayerRollState : BattleState
{
    // False for a reroll within the same turn (Steady Hand): no turn start, the intent stays.
    private readonly bool startsTurn;

    public PlayerRollState(BattleManager manager, bool startsTurn = true) : base(manager){
        this.startsTurn = startsTurn;
    }

    public override void Enter(){
        Debug.Log("Entered the roll state.");

        if(startsTurn){
            battleManager.flow.StartPlayerTurn();
            if(battleManager.CheckBattleOver()) return;
        }

        if(UIManager.Instance != null){
            UIManager.Instance.UpdateEnemyIntentText(battleManager.brain.DescribeCurrentIntent());
            UIManager.Instance.ShowBossDie(battleManager.brain.Die, battleManager.brain.RolledFace);
            UIManager.Instance.ShowRollPhase(battleManager.currentDiceFaces);
        }
    }

    public override void Execute(){
        
    }

    public override void Exit(){
        Debug.Log("Entered the exit state.");
    }

    public void RollTheDice(){
        DieFace face = battleManager.flow.Roll();
        Debug.Log("The dice were rolled. Face: " + face.definition.faceName);
        battleManager.currentRolledFace = face;

        if(UIManager.Instance != null){
            UIManager.Instance.ShowRolledFace(battleManager.currentRolledFace, battleManager.currentDiceFaces);
        }
        if(battleManager.CheckBattleOver()) return;

        battleManager.ChangeState(new PlayerActionSelectState(battleManager));
    }
}
