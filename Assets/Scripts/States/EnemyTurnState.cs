using UnityEngine;

public class EnemyTurnState : BattleState
{
    public EnemyTurnState(BattleManager manager) : base(manager) {

    }

    public override void Enter(){
        battleManager.enemy.OnTurnStart();
        
        battleManager.enemyBrain.ExecuteCurrentIntent(battleManager.enemy, battleManager.player);
        if(battleManager.CheckBattleOver()) return;

        battleManager.enemy.OnTurnEnd();
        battleManager.enemyBrain.OnTurnEnd();
        if(battleManager.CheckBattleOver()) return;

        battleManager.ChangeState(new PlayerRollState(battleManager));
    }


    public override void Execute(){

    }

    public override void Exit(){
        
    }
}
