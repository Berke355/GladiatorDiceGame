using UnityEngine;

public class PlayerRollState : BattleState
{
    public PlayerRollState(BattleManager manager) : base(manager){

    }

    public override void Enter(){
        Debug.Log("Zar atma durumuna girildi.");
    }
    public override void Execute(){
        if(Input.GetKeyDown(KeyCode.Space)){
            int index = Random.Range(0,6);
            DieFaceSO face = battleManager.currentDiceFaces[index];
            Debug.Log("Zar Atıldı. Gelen yüz: " + face.faceName);

            battleManager.ChangeState(new PlayerActionSelectState(battleManager));
        }
    }
    public override void Exit(){
        Debug.Log("Çıkış durumuna girildi.");
    }
}
