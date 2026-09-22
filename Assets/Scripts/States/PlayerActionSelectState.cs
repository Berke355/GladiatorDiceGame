using UnityEngine;

public class PlayerActionSelectState : BattleState
{
    public ActionType selectedAction;

    public PlayerActionSelectState(BattleManager manager) : base(manager){

    }

    public override void Enter(){
        Debug.Log("Entered Action Select state.");
        Debug.Log("Rolled face description: " + battleManager.currentRolledFace.description);
    }

    public override void Execute(){
        
    }

    public override void Exit(){
        Debug.Log("Exited Action Select state.");
    }

    public void ExecuteAction(ActionType selectedAction){
        this.selectedAction=selectedAction;

        foreach(ActionEffect effect in battleManager.currentRolledFace.effects){
            if(effect.actionType == selectedAction){
                if(effect.effectLogic != null){
                    effect.effectLogic.Execute(battleManager.player, battleManager.enemy, battleManager.currentRolledFace.baseValue);
                }
            }
        }

        if(battleManager.enemy.currentHP <= 0){
            Debug.Log("Battle ended. Enemy died.");
        }
        else{
            battleManager.player.OnTurnEnd();
            battleManager.ChangeState(new EnemyTurnState(battleManager));
        }
    }
}
