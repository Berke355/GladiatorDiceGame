using UnityEngine;
using System.Collections.Generic;

public class BattleManager : MonoBehaviour
{
    private BattleState currentState;

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
}
