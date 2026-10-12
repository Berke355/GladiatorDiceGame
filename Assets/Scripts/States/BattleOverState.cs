using UnityEngine;

public class BattleOverState : BattleState
{
    public bool playerWon;

    public BattleOverState(BattleManager manager, bool playerWon) : base(manager){
        this.playerWon = playerWon;
    }

    public override void Enter(){
        Debug.Log(playerWon ? "Battle ended. Player won." : "Battle ended. Player died.");

        if(UIManager.Instance != null){
            UIManager.Instance.ShowBattleResult(playerWon);
        }
    }

    public override void Execute(){

    }

    public override void Exit(){

    }
}
