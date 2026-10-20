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

    // The intent brain of a normal enemy (enemyBrain) or the dice brain of a boss.
    [System.NonSerialized]
    public IEnemyBrain brain;

    // The turn steps and the equipment triggers, shared with the balance simulator.
    [System.NonSerialized]
    public BattleFlow flow;

    void Awake(){
        // A run always starts by choosing the starting face. If this scene was played directly
        // (e.g. Play pressed with Battle open in the Editor), send the player to the run start screen.
        // Disabling this component also skips Start, so no turn begins.
        if(!RunManager.Instance.run.startingFaceChosen){
            enabled = false;
            RunManager.Instance.GoToRunStart();
            return;
        }

        // Load the run's die and the player's HP before UIManager.Start reads them.
        RunData run = RunManager.Instance.run;
        currentDiceFaces = run.dieFaces;
        player.maxHP = run.maxHP;
        player.currentHP = run.currentHP;

        // The run picks this battle's boss or enemy; the scene's Enemy object takes its stats.
        BossSO boss = RunManager.Instance.PickBossForNextBattle();
        if(boss != null){
            enemy.entityName = boss.bossName;
            enemy.maxHP = boss.maxHP;
            enemy.currentHP = boss.maxHP;
            enemyBrain.enabled = false;
            brain = new BossBrain(boss, enemy, player, currentDiceFaces);
        }
        else{
            EnemySO enemyData = RunManager.Instance.PickEnemyForNextBattle();
            enemy.entityName = enemyData.enemyName;
            enemy.maxHP = enemyData.maxHP;
            enemy.currentHP = enemyData.maxHP;
            enemyBrain.Setup(enemyData, enemy, player);
            brain = enemyBrain;
        }

        flow = new BattleFlow(player, enemy, brain, currentDiceFaces, run.equipment);
    }

    void OnDestroy(){
        if(flow != null){
            flow.Dispose();
        }
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
        // Hook the UI up before the battle starts, so BattleStart items flash too.
        if(UIManager.Instance != null){
            UIManager.Instance.ShowEquipment(RunManager.Instance.run.equipment);
            UIManager.Instance.charge = flow.charge;
            flow.triggers.ItemTriggered += UIManager.Instance.FlashEquipment;
        }
        flow.StartBattle();
        if(CheckBattleOver()) return;
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
