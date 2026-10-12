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
    public int startingMaxHP = 25;

    [Header("Economy")]
    public EconomyConfigSO economy;

    [Header("Run Structure")]
    // Boss battle numbers. Winning one opens the Grand Market instead of a normal one.
    // Boss enemies themselves are not implemented yet.
    public List<int> bossBattles = new List<int>{ 4, 8 };

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
        return economy.goldPerVictory;
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
