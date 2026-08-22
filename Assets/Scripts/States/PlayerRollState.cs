using UnityEngine;

public class PlayerRollState : BattleState
{
    public PlayerRollState(BattleManager manager) : base(manager){

    }

    public override void Enter(){
        Debug.Log("Zar atma durumuna girildi.");
    }
    public override void Execute(){
        
    }
    public override void Exit(){
        Debug.Log("Çıkış durumuna girildi.");
    }

    public void RollTheDice(){
        int index = Random.Range(0,6);
        DieFaceSO face = battleManager.currentDiceFaces[index];
        Debug.Log("Zar Atıldı. Gelen yüz: " + face.faceName);
        battleManager.currentRolledFace = face;

        if(UIManager.Instance != null){
            UIManager.Instance.UpdateDiceText(battleManager.currentRolledFace);
        }

        battleManager.ChangeState(new PlayerActionSelectState(battleManager));
    }
}
