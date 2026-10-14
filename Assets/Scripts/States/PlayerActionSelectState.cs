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

        DieFace rolledFace = battleManager.currentRolledFace;
        EffectContext context = new EffectContext{
            source = battleManager.player,
            target = battleManager.enemy,
            value = rolledFace.value,
            die = battleManager.currentDiceFaces,
            face = rolledFace
        };
        // The face's own effects and any attached module's, for the chosen action.
        foreach(EffectSO effect in rolledFace.GetEffects(selectedAction)){
            effect.Execute(context);
        }

        if(battleManager.CheckBattleOver()) return;

        battleManager.player.OnTurnEnd();
        if(battleManager.CheckBattleOver()) return;

        battleManager.ChangeState(new EnemyTurnState(battleManager));
    }
}
