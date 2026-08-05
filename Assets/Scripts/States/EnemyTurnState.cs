using UnityEngine;

public class EnemyTurnState : BattleState
{
    public EnemyTurnState(BattleManager manager) : base(manager) {

    }

    public override void Enter(){
        battleManager.player.TakeDamage(1);
        Debug.Log(battleManager.enemy.entityName + " saldırdı ve sana 1 hasar verdi.");

        battleManager.player.ResetBlock();
        battleManager.ChangeState(new PlayerRollState(battleManager));
    }

    public override void Execute(){

    }

    public override void Exit(){
        
    }
}
