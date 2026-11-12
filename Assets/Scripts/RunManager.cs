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
    // Winning this battle (the final boss's) completes the run.
    public int finalBattle = 11;
    public bool IsRunComplete => run.battlesWon >= finalBattle;

    [Header("Scenes")]
    public string runStartSceneName = "RunStart";
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

        // The settings screen (gear and Esc) lives through scene loads like the run.
        SettingsUI settings = Resources.Load<SettingsUI>("SettingsOverlay");
        if(settings != null){
            SettingsUI overlay = Instantiate(settings);
            overlay.name = "SettingsOverlay";
            DontDestroyOnLoad(overlay.gameObject);
        }
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

    // The starting die has five faces; the sixth is picked from startingFaceOffers on the run start screen.
    public void StartNewRun(){
        run = new RunData(startingDieFaces, startingMaxHP);
        run.gold = economy.startingGold;
        run.startingFaceOffers = StartingFaceOffers.Generate(economy, startingDieFaces);
    }

    // The run start screen's paid reroll of the three offers.
    public bool RerollStartingFaces(){
        return StartingFaceOffers.Reroll(economy, run, startingDieFaces);
    }

    public int StartingRerollCost => StartingFaceOffers.RerollCost(economy, run);

    public void ChooseStartingFace(DieFaceSO face){
        if(run.startingFaceChosen){
            return;
        }
        run.dieFaces.Add(new DieFace(face));
        run.startingFaceChosen = true;
    }

    // Called once when a battle ends. Returns the gold earned.
    public int OnBattleEnded(bool playerWon, int playerHP){
        run.currentHP = playerHP;

        if(!playerWon){
            return 0;
        }

        run.battlesWon++;
        int goldEarned = economy.goldPerVictory + EquipmentSO.TotalGoldPerVictory(run.equipment);

        // Bosses heal and pay a bonus, so the player reaches the Grand Market with gold to spend.
        if(bossBattles.Contains(run.battlesWon)){
            int heal = Mathf.RoundToInt(run.maxHP * bossHealFraction);
            run.currentHP = Mathf.Min(run.maxHP, run.currentHP + heal);
            goldEarned += economy.bossVictoryBonus;
            // The Grand Market that follows opens with a free pick of equipment.
            run.bossRewardPending = true;
        }
        run.gold += goldEarned;
        return goldEarned;
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

    // The boss for the upcoming battle, or null if it is a normal battle (or its boss isn't built yet).
    public BossSO PickBossForNextBattle(){
        return encounters.PickBoss(run.battlesWon + 1);
    }

    // Picks the enemy or enemy group for the upcoming battle and remembers its members so they won't appear again this run.
    public EnemyGroup PickEncounterForNextBattle(){
        EnemyTier tier = GetTierForBattle(run.battlesWon + 1);
        return EncounterConfigSO.PickEncounter(encounters.enemyPool, encounters.groups, tier, run.metEnemies);
    }

    public void GoToRunStart(){
        SceneManager.LoadScene(runStartSceneName);
    }

    public void GoToMarket(){
        PickNextBattle();
        SceneManager.LoadScene(marketSceneName);
    }

    // Picks the next battle's boss or encounter once, ahead of time (the Market shows it); the battle takes it.
    public void PickNextBattle(){
        if(run.nextBattlePicked) return;
        run.nextBoss = PickBossForNextBattle();
        run.nextEncounter = run.nextBoss == null ? PickEncounterForNextBattle() : null;
        run.nextBattlePicked = true;
    }

    // The picked next battle, handed to the battle once.
    public void TakeNextBattle(out BossSO boss, out EnemyGroup encounter){
        PickNextBattle();
        boss = run.nextBoss;
        encounter = run.nextEncounter;
        run.nextBattlePicked = false;
        run.nextBoss = null;
        run.nextEncounter = null;
    }

    public void GoToNextBattle(){
        SceneManager.LoadScene(battleSceneName);
    }

    // A new run begins on the run start screen, where the starting face is picked.
    public void RestartRun(){
        StartNewRun();
        SceneManager.LoadScene(runStartSceneName);
    }
}
