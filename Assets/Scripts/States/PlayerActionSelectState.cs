using UnityEngine;

public class PlayerActionSelectState : BattleState
{
    public ActionType selectedAction;

    public PlayerActionSelectState(BattleManager manager) : base(manager){

    }

    public override void Enter(){
        Debug.Log("Entered Action Select state.");
        Debug.Log("Rolled face description: " + battleManager.currentRolledFace.FullDescription);
    }

    public override void Execute(){
        
    }

    public override void Exit(){
        Debug.Log("Exited Action Select state.");
    }

    public void ExecuteAction(ActionType selectedAction){
        this.selectedAction=selectedAction;

        // ActionChosen items, then the rolled face's own effects and any attached module's.
        battleManager.flow.PlayAction(selectedAction);
        if(battleManager.CheckBattleOver()) return;

        battleManager.flow.EndPlayerTurn();
        if(battleManager.CheckBattleOver()) return;

        battleManager.ChangeState(new EnemyTurnState(battleManager));
    }
}
