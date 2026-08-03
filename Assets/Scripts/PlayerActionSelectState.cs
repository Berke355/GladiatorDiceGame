using UnityEngine;

public class PlayerActionSelectState : BattleState
{
    public PlayerActionSelectState(BattleManager manager) : base(manager){

    }

    public override void Enter(){
        Debug.Log("Aksiyon seçme durumununa girildi.");
    }
    public override void Execute(){
        Debug.Log("Aksiyon seçimi yapılıyor.");
    }
    public override void Exit(){
        Debug.Log("Aksiyon seçimi durumundan çıkıldı.");
    }
}
