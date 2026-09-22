using UnityEngine;

public class PlayerRollState : BattleState
{
    public PlayerRollState(BattleManager manager) : base(manager){

    }

    public override void Enter(){
        Debug.Log("Entered the roll state.");

        battleManager.player.OnTurnStart();

        battleManager.enemyBrain.ChooseNextIntent();

        if(UIManager.Instance != null){
            UIManager.Instance.UpdateEnemyIntentText(battleManager.enemyBrain.currentIntent);
        }
    }

    public override void Execute(){
        
    }

    public override void Exit(){
        Debug.Log("Entered the exit state.");
    }

    public void RollTheDice(){
        int index = Random.Range(0,6);
        DieFaceSO face = battleManager.currentDiceFaces[index];
        Debug.Log("The dice were rolled. Face: " + face.faceName);
        battleManager.currentRolledFace = face;

        if(UIManager.Instance != null){
            UIManager.Instance.UpdateDiceText(battleManager.currentRolledFace);
        }

        battleManager.ChangeState(new PlayerActionSelectState(battleManager));
    }
}
