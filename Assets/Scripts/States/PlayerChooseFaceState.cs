using UnityEngine;

// An action let the player choose the next face (a Chosen steer): waits for a click on a face of the die.
public class PlayerChooseFaceState : BattleState
{
    public PlayerChooseFaceState(BattleManager manager) : base(manager){

    }

    public override void Enter(){
        if(UIManager.Instance != null){
            UIManager.Instance.ShowChooseFace(battleManager.currentDiceFaces);
        }
    }

    public override void Execute(){

    }

    public override void Exit(){

    }

    public void Choose(int position){
        if(position < 0 || position >= battleManager.currentDiceFaces.Count) return;
        battleManager.flow.ChooseNextFace(battleManager.currentDiceFaces[position]);
        battleManager.RollAgainAtOnce();
    }
}
