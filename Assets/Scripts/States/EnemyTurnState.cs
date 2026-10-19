using UnityEngine;

public class EnemyTurnState : BattleState
{
    public EnemyTurnState(BattleManager manager) : base(manager) {

    }

    public override void Enter(){
        battleManager.flow.EnemyAct();
        if(battleManager.CheckBattleOver()) return;

        battleManager.flow.EndEnemyTurn();
        if(battleManager.CheckBattleOver()) return;

        battleManager.ChangeState(new PlayerRollState(battleManager));
    }


    public override void Execute(){

    }

    public override void Exit(){
        
    }
}
