using UnityEngine;
using System.Collections.Generic;

public class BattleManager : MonoBehaviour
{
    public EnemyBrain enemyBrain;
    public Entity enemy;
    public Entity player;

    private BattleState currentState;
    [System.NonSerialized]
    public DieFace currentRolledFace;

    // Borrowed from the run in Awake; the run owns the die.
    [System.NonSerialized]
    public List<DieFace> currentDiceFaces;

    void Awake(){
        // Load the run's die and the player's HP before UIManager.Start reads them.
        RunManager.Instance.EnsureStartingFace();
        RunData run = RunManager.Instance.run;
        currentDiceFaces = run.dieFaces;
        player.maxHP = run.maxHP;
        player.currentHP = run.currentHP;

        // The run picks this battle's enemy; the scene's Enemy object takes its stats.
        EnemySO enemyData = RunManager.Instance.PickEnemyForNextBattle();
        enemy.entityName = enemyData.enemyName;
        enemy.maxHP = enemyData.maxHP;
        enemy.currentHP = enemyData.maxHP;
        enemyBrain.Setup(enemyData);
    }

    public void ChangeState(BattleState newState){
        if(currentState != null){
            currentState.Exit();
        }

        currentState = newState;

        currentState.Enter();
    }

    // Ends the battle if either side is dead. Callers must stop their turn flow when this returns true.
    public bool CheckBattleOver(){
        if(player.IsDead || enemy.IsDead){
            ChangeState(new BattleOverState(this, !player.IsDead));
            return true;
        }
        return false;
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

    // Called by the button on the battle result screen.
    public void OnResultButton(){
        if(currentState is BattleOverState overState){
            overState.Continue();
        }
    }

    public void OnRolled(){
        if(currentState is PlayerRollState playerState){
            playerState.RollTheDice();
        }
    }
}
