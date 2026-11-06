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

    // Spends a reroll: the new face replaces the rolled one and must be used.
    public void Reroll(){
        if(!battleManager.flow.CanReroll) return;
        DieFace face = battleManager.flow.RerollFace();
        battleManager.currentRolledFace = face;
        Debug.Log("Rerolled. Face: " + face.definition.faceName);
        if(UIManager.Instance != null){
            UIManager.Instance.ShowRolledFace(face, battleManager.currentDiceFaces);
        }
        battleManager.CheckBattleOver();
    }

    public void ExecuteAction(ActionType selectedAction){
        this.selectedAction=selectedAction;

        // ActionChosen items, then the rolled face's own effects and its enchantment's.
        battleManager.flow.PlayAction(selectedAction);
        if(battleManager.CheckBattleOver()) return;

        // Steady Hand: roll again within the same turn.
        if(battleManager.flow.RerollPending){
            battleManager.ChangeState(new PlayerRollState(battleManager, false));
            return;
        }

        battleManager.flow.EndPlayerTurn();
        if(battleManager.CheckBattleOver()) return;

        battleManager.ChangeState(new EnemyTurnState(battleManager));
    }
}
