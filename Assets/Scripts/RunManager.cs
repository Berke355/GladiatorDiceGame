using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections.Generic;

// Owns the current run and survives scene loads (DontDestroyOnLoad).
// It is created from Resources/RunManager before the first scene loads,
// so any scene can be played directly from the Editor.
public class RunManager : MonoBehaviour
{
    public static RunManager Instance { get; private set; }

    [Header("New Run")]
    public List<DieFaceSO> startingDieFaces = new List<DieFaceSO>();
    public int startingMaxHP = 50;

    [Header("Economy")]
    public EconomyConfigSO economy;

    [Header("Encounters")]
    public EncounterConfigSO encounters;

    [Header("Run Structure")]
    // Boss battle numbers. Winning one opens the Grand Market instead of a normal one and heals the player.
    // Boss enemies themselves are not implemented yet.
    public List<int> bossBattles = new List<int>{ 4, 8 };
    // Share of max HP restored after winning a boss battle.
    [Range(0f, 1f)]
    public float bossHealFraction = 0.5f;

    public bool IsGrandMarket => bossBattles.Contains(run.battlesWon);

    [Header("Scenes")]
    public string battleSceneName = "Battle";
    public string marketSceneName = "Market";

    public RunData run;

    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
    private static void Bootstrap(){
        if(Instance != null){
            return;
        }

        RunManager prefab = Resources.Load<RunManager>("RunManager");
        if(prefab == null){
            Debug.LogError("RunManager prefab not found in a Resources folder.");
            return;
        }
        Instantiate(prefab).name = "RunManager";
    }

    void Awake(){
        if(Instance != null && Instance != this){
            Destroy(gameObject);
            return;
        }
        Instance = this;
        DontDestroyOnLoad(gameObject);

        StartNewRun();
    }

    public void StartNewRun(){
        run = new RunData(startingDieFaces, startingMaxHP);
    }

    // Called once when a battle ends. Returns the gold earned.
    public int OnBattleEnded(bool playerWon, int playerHP){
        run.currentHP = playerHP;

        if(!playerWon){
            return 0;
        }

        run.battlesWon++;
        run.gold += economy.goldPerVictory;

        if(bossBattles.Contains(run.battlesWon)){
            int heal = Mathf.RoundToInt(run.maxHP * bossHealFraction);
            run.currentHP = Mathf.Min(run.maxHP, run.currentHP + heal);
        }
        return economy.goldPerVictory;
    }

    // Weak before the first boss, Medium between the bosses, Strong after the second.
    // Until bosses exist, a boss slot uses the tier that follows it (battle 4: Medium, battle 8: Strong).
    public EnemyTier GetTierForBattle(int battleNumber){
        int bossesReached = 0;
        foreach(int bossBattle in bossBattles){
            if(bossBattle <= battleNumber){
                bossesReached++;
            }
        }
        return (EnemyTier)Mathf.Min(bossesReached, (int)EnemyTier.Strong);
    }

    // Picks the enemy for the upcoming battle and remembers it so it won't appear again this run.
    public EnemySO PickEnemyForNextBattle(){
        EnemyTier tier = GetTierForBattle(run.battlesWon + 1);
        List<EnemySO> candidates = encounters.enemyPool.FindAll(enemy => enemy.tier == tier && !run.metEnemies.Contains(enemy));
        if(candidates.Count == 0){
            // Every enemy of this tier was already met (e.g. a run longer than planned): allow repeats.
            candidates = encounters.enemyPool.FindAll(enemy => enemy.tier == tier);
        }

        EnemySO picked = candidates[Random.Range(0, candidates.Count)];
        run.metEnemies.Add(picked);
        return picked;
    }

    public void GoToMarket(){
        SceneManager.LoadScene(marketSceneName);
    }

    public void GoToNextBattle(){
        SceneManager.LoadScene(battleSceneName);
    }

    public void RestartRun(){
        StartNewRun();
        SceneManager.LoadScene(battleSceneName);
    }
}
