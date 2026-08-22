using UnityEngine;
using System.Collections.Generic;

public class BattleManager : MonoBehaviour
{
    public Entity enemy;
    public Entity player;

    private BattleState currentState;
    public DieFaceSO currentRolledFace;

    [SerializeField]
    public DieFaceSO[] currentDiceFaces = new DieFaceSO[6];

    public void ChangeState(BattleState newState){
        if(currentState != null){
            currentState.Exit();
        }

        currentState = newState;

        currentState.Enter();
    }

    void Start(){
        ChangeState(new PlayerRollState(this));
    }

    void Update(){
        if(currentState != null){
            currentState.Execute();
        }
    }

    public void OnActionSelected(int actionIndex){
        ActionType selectedAction = (ActionType)actionIndex;

        if(currentState is PlayerActionSelectState playerState){
            playerState.ExecuteAction(selectedAction);
        }
    }

    public void OnRolled(){
        if(currentState is PlayerRollState playerState){
            playerState.RollTheDice();
        }
    }
}
