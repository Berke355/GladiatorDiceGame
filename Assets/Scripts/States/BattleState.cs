using UnityEngine;

public abstract class BattleState
{
    protected BattleManager battleManager;
    
    public BattleState(BattleManager manager){
        this.battleManager = manager;
    }

    public abstract void Enter();
    public abstract void Execute();
    public abstract void Exit();
}
