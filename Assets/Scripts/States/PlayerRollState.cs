using UnityEngine;

public class PlayerRollState : BattleState
{
    public PlayerRollState(BattleManager manager) : base(manager){

    }

    public override void Enter(){
        Debug.Log("Entered the roll state.");

        battleManager.flow.StartPlayerTurn();
        if(battleManager.CheckBattleOver()) return;

        if(UIManager.Instance != null){
            UIManager.Instance.UpdateEnemyIntentText(battleManager.enemyBrain.DescribeCurrentIntent());
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
