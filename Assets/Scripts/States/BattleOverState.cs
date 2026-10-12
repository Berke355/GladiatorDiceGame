using UnityEngine;

public class BattleOverState : BattleState
{
    public bool playerWon;

    public BattleOverState(BattleManager manager, bool playerWon) : base(manager){
        this.playerWon = playerWon;
    }

    public override void Enter(){
        Debug.Log(playerWon ? "Battle ended. Player won." : "Battle ended. Player died.");

        int goldEarned = RunManager.Instance.OnBattleEnded(playerWon, battleManager.player.currentHP);

        if(UIManager.Instance != null){
            UIManager.Instance.ShowBattleResult(playerWon, goldEarned);
        }
    }

    // Victory continues to the market; defeat ends the run and starts a new one.
    public void Continue(){
        if(playerWon){
            RunManager.Instance.GoToMarket();
        }
        else{
            RunManager.Instance.RestartRun();
        }
    }

    public override void Execute(){

    }

    public override void Exit(){

    }
}
