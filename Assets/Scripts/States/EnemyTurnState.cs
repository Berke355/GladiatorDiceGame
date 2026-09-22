using UnityEngine;

public class EnemyTurnState : BattleState
{
    public EnemyTurnState(BattleManager manager) : base(manager) {

    }

    public override void Enter(){
        battleManager.enemy.OnTurnStart();
        
        EnemyIntent intent = battleManager.enemyBrain.currentIntent;
        
        if(intent.effectLogic != null){
            intent.effectLogic.Execute(battleManager.enemy, battleManager.player, intent.value);
        }
        
        battleManager.enemy.OnTurnEnd();
        battleManager.ChangeState(new PlayerRollState(battleManager));
    }


    public override void Execute(){

    }

    public override void Exit(){
        
    }
}
