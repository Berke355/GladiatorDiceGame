using UnityEngine;

public class EnemyTurnState : BattleState
{
    public EnemyTurnState(BattleManager manager) : base(manager) {

    }

    public override void Enter(){
        battleManager.enemy.OnTurnStart();
        
        EnemyIntent intent = battleManager.enemyBrain.currentIntent;
        
        if(intent.effectLogic != null){
            // Enemies have no die, so die and face stay null.
            intent.effectLogic.Execute(new EffectContext{
                source = battleManager.enemy,
                target = battleManager.player,
                value = intent.value
            });
        }
        if(battleManager.CheckBattleOver()) return;

        battleManager.enemy.OnTurnEnd();
        if(battleManager.CheckBattleOver()) return;

        battleManager.ChangeState(new PlayerRollState(battleManager));
    }


    public override void Execute(){

    }

    public override void Exit(){
        
    }
}
