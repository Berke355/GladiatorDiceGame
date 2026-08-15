using UnityEngine;

public class PlayerActionSelectState : BattleState
{
    public ActionType selectedAction;

    public PlayerActionSelectState(BattleManager manager) : base(manager){

    }

    public override void Enter(){
        Debug.Log("Aksiyon seçme durumununa girildi.");
        Debug.Log("Gelen Yüzün açıklaması: " + battleManager.currentRolledFace.description);
    }

    public override void Execute(){

        if(Input.GetKeyDown(KeyCode.Alpha1)){
            ExecuteAction(ActionType.Attack);
        }
        else if(Input.GetKeyDown(KeyCode.Alpha2)){
            ExecuteAction(ActionType.Defense);
        }
        else if(Input.GetKeyDown(KeyCode.Alpha3)){
            ExecuteAction(ActionType.Magic);
        }
    }

    public override void Exit(){
        Debug.Log("Aksiyon seçimi durumundan çıkıldı.");
    }

    public void ExecuteAction(ActionType selectedAction){
        this.selectedAction=selectedAction;

        foreach(ActionEffect effect in battleManager.currentRolledFace.effects){
            if(effect.actionType == selectedAction){
                if(effect.effectLogic != null){
                    int finalValue = Mathf.RoundToInt(battleManager.currentRolledFace.baseValue * effect.multiplier);
                    effect.effectLogic.Execute(battleManager.player, battleManager.enemy, finalValue);
                }
            }
        }

        if(battleManager.enemy.currentHP <= 0){
            Debug.Log("Savaş bitti. Düşman öldü.");
        }
        else{
            battleManager.ChangeState(new EnemyTurnState(battleManager));
        }
    }
}
