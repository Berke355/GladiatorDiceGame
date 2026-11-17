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

    [Header("Map")]
    // The map's shape and node types (see GDD "Map"). Without it the run is the old line of battles and Markets
    // (bossBattles, finalBattle), which the simulator still uses.
    public MapConfigSO mapConfig;
    public int acts = 3;
    public bool UsesMap => mapConfig != null;
    // Share of max HP a rest heals.
    [Range(0f, 1f)]
    public float restHealFraction = 0.3f;

    [Header("Run Structure")]
    // Boss battle numbers. Winning one opens the Grand Market instead of a normal one and heals the player.
    // Boss enemies themselves are not implemented yet.
    public List<int> bossBattles = new List<int>{ 4, 8 };
    // Share of max HP restored after winning a boss battle.
    [Range(0f, 1f)]
    public float bossHealFraction = 0.5f;

    public bool IsGrandMarket => UsesMap ? run.grandMarketPending : bossBattles.Contains(run.battlesWon);
    // Winning this battle (the final boss's) completes the run.
    public int finalBattle = 11;
    public bool IsRunComplete => UsesMap ? run.runComplete : run.battlesWon >= finalBattle;

    [Header("Scenes")]
    public string runStartSceneName = "RunStart";
    public string battleSceneName = "Battle";
    public string marketSceneName = "Market";
    public string mapSceneName = "Map";

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
        if(UsesMap) BeginAct(0);
    }

    // ---------- Map ----------

    // A new act: its map, its boss, and the player before the first floor.
    public void BeginAct(int act){
        run.act = act;
        run.map = MapGenerator.Generate(mapConfig, act, new System.Random(UnityEngine.Random.Range(int.MinValue, int.MaxValue)));
        run.mapNode = -1;
        run.visitedNodes.Clear();
        openShop = null;
        openShopNode = -1;
        mapInk = null;
        run.restPending = false;
        run.actBoss = encounters.PickBossForAct(act);
        PrepareReachable();
    }

    public MapNode CurrentNode => run.map != null && run.mapNode >= 0 ? run.map.nodes[run.mapNode] : null;
    public bool AtBoss => UsesMap && CurrentNode != null && CurrentNode.type == MapNodeType.Boss;

    // Picks the enemies of the battle nodes the player can go to next, each different, so the map can show them.
    // Members count as met only when the player enters the node.
    public void PrepareReachable(){
        if(run.map == null) return;
        List<EnemySO> reserved = new List<EnemySO>(run.metEnemies);
        EnemyTier tier = ActTier;
        foreach(int index in run.map.Reachable(run.mapNode)){
            MapNode node = run.map.nodes[index];
            if(node.type != MapNodeType.Battle && node.type != MapNodeType.Elite) continue;
            if(node.encounter == null || node.encounter.members.Count == 0){
                if(node.type == MapNodeType.Elite){
                    EnemySO elite = encounters.PickElite(run.act, reserved);
                    // An act without elites yet fights a normal battle there.
                    node.encounter = elite != null ? EnemyGroup.Alone(elite) : EncounterConfigSO.PickEncounter(encounters.enemyPool, encounters.groups, tier, reserved);
                }
                else node.encounter = EncounterConfigSO.PickEncounter(encounters.enemyPool, encounters.groups, tier, reserved);
            }
            if(node.encounter != null) reserved.AddRange(node.encounter.members);
        }
    }

    public bool CanEnter(int index){
        return run.map != null && (run.map.Reachable(run.mapNode).Contains(index) || CanReenterShop(index));
    }

    // A shop left for the map stays open until the player steps on: its node can be entered again, with the same
    // offers, rerolls and purchases (the Market object lives on), so walking back in is never a free reroll. Runtime only.
    // The player's pencil drawings on this act's map (MapDrawing); runtime only, gone with the act.
    [System.NonSerialized] public Color32[] mapInk;
    [System.NonSerialized] public int mapInkAct = -1;

    private Market openShop;
    private int openShopNode = -1;

    public bool CanReenterShop(int index){
        return run.map != null && index == run.mapNode && index == openShopNode && openShop != null && !run.grandMarketPending;
    }

    // MarketUI's Market: the open shop of this node when the player walks back in, else a new one, kept for that.
    public Market TakeShop(){
        if(UsesMap && openShop != null && openShopNode == run.mapNode && !IsGrandMarket) return openShop;
        Market market = new Market(run, economy, IsGrandMarket);
        openShop = UsesMap && !IsGrandMarket ? market : null;
        openShopNode = openShop != null ? run.mapNode : -1;
        return market;
    }

    // The player steps onto a reachable node: a fight, a shop, or a rest (played on the map screen, which stays open).
    public void EnterNode(int index){
        if(!CanEnter(index)) return;
        if(index != openShopNode){
            openShop = null;
            openShopNode = -1;
        }
        run.mapNode = index;
        if(!run.visitedNodes.Contains(index)) run.visitedNodes.Add(index);
        MapNode node = run.map.nodes[index];
        switch(node.type){
            case MapNodeType.Battle:
            case MapNodeType.Elite:
                run.nextBoss = null;
                run.nextEncounter = node.encounter;
                if(node.encounter != null) run.metEnemies.AddRange(node.encounter.members);
                run.nextBattlePicked = node.encounter != null;
                GoToNextBattle();
                break;
            case MapNodeType.Boss:
                run.nextBoss = run.actBoss;
                run.nextEncounter = null;
                run.nextBattlePicked = run.actBoss != null;
                GoToNextBattle();
                break;
            case MapNodeType.Market:
            case MapNodeType.EnchantingTable:
                run.atEnchantingTable = node.type == MapNodeType.EnchantingTable;
                SceneManager.LoadScene(marketSceneName);
                break;
            case MapNodeType.Rest:
                run.restPending = true;
                break;
            default:
                PrepareReachable();
                break;
        }
    }

    // A rest's two choices (see GDD "Map"): heal a share of max HP, or upgrade one face for free.
    public int RestHealAmount => Mathf.RoundToInt(run.maxHP * restHealFraction);

    public void RestHeal(){
        run.currentHP = Mathf.Min(run.maxHP, run.currentHP + RestHealAmount);
    }

    public bool CanRestUpgrade(int dieSlot){
        return dieSlot >= 0 && dieSlot < run.dieFaces.Count && CanRestUpgradeFace(run.dieFaces[dieSlot]);
    }

    // A rest upgrades any face below its upgrade cap (GDD "Upgrade Caps").
    public static bool CanRestUpgradeFace(DieFace face){
        return face != null && !face.IsBlank && !face.AtMaxValue;
    }

    public void RestUpgrade(int dieSlot){
        if(CanRestUpgrade(dieSlot)) run.dieFaces[dieSlot].value++;
    }

    public void FinishRest(){
        run.restPending = false;
        PrepareReachable();
    }

    public void GoToMap(){
        PrepareReachable();
        SceneManager.LoadScene(mapSceneName);
    }

    // The run start screen's choice is made: the first act's map, or the first battle without a map.
    public void BeginRun(){
        if(UsesMap) GoToMap();
        else GoToNextBattle();
    }

    // The battle screen's continue after a victory: a boss opens the Grand Market, any other fight goes back to the map.
    public void ContinueAfterVictory(){
        if(UsesMap && !AtBoss) GoToMap();
        else GoToMarket();
    }

    // The shop's continue: back to the map; after a boss's Grand Market the next act begins.
    public void LeaveShop(){
        if(!UsesMap){
            GoToNextBattle();
            return;
        }
        run.atEnchantingTable = false;
        if(run.grandMarketPending){
            run.grandMarketPending = false;
            BeginAct(run.act + 1);
        }
        GoToMap();
    }

    // The battle's title in the top bar.
    public string BattleTitle(bool boss){
        if(!UsesMap || run.map == null) return "BATTLE " + (run.battlesWon + 1) + " / " + finalBattle;
        if(boss) return "ACT " + (run.act + 1) + " / " + acts;
        string title = "ACT " + (run.act + 1) + "  FLOOR " + ((CurrentNode != null ? CurrentNode.floor : 0) + 1);
        return CurrentNode != null && CurrentNode.type == MapNodeType.Elite ? title + "  <color=#C77DFF>ELITE</color>" : title;
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

        bool bossWon = UsesMap ? AtBoss : bossBattles.Contains(run.battlesWon);
        // An elite pays more and offers an item (picked on the map screen).
        if(UsesMap && CurrentNode != null && CurrentNode.type == MapNodeType.Elite){
            goldEarned += economy.eliteGoldBonus;
            run.eliteRewardChoices = EliteReward.Pick(economy, run, economy.eliteRewardChoices);
        }
        // The final boss ends the run.
        if(UsesMap && bossWon && run.act >= acts - 1){
            run.runComplete = true;
            run.gold += goldEarned;
            return goldEarned;
        }
        // Bosses heal and pay a bonus, so the player reaches the Grand Market with gold to spend.
        if(bossWon){
            run.grandMarketPending = true;
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
    // On the map, every normal fight of an act has the act's tier (GDD "Map").
    public EnemyTier ActTier => (EnemyTier)Mathf.Min(run.act, (int)EnemyTier.Strong);

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
        if(UsesMap) return AtBoss ? run.actBoss : null;
        return encounters.PickBoss(run.battlesWon + 1);
    }

    // Picks the enemy or enemy group for the upcoming battle and remembers its members so they won't appear again this run.
    public EnemyGroup PickEncounterForNextBattle(){
        EnemyTier tier = UsesMap ? ActTier : GetTierForBattle(run.battlesWon + 1);
        return EncounterConfigSO.PickEncounter(encounters.enemyPool, encounters.groups, tier, run.metEnemies);
    }

    public void GoToRunStart(){
        SceneManager.LoadScene(runStartSceneName);
    }

    public void GoToMarket(){
        if(!UsesMap) PickNextBattle();
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

    // A test shortcut (only in the Editor and development builds, RunStartUI.bossTestButton): takes the first starting
    // offer and goes straight to the final boss with the starting die, with extra HP so the fight lasts long enough to
    // watch its mechanics.
    public const int BossTestHP = 150;
    public void JumpToFinalBoss(){
        if(!run.startingFaceChosen && run.startingFaceOffers.Count > 0) ChooseStartingFace(run.startingFaceOffers[0]);
        run.battlesWon = finalBattle - 1;
        run.maxHP = BossTestHP;
        run.currentHP = BossTestHP;
        run.nextBattlePicked = false;
        if(UsesMap){
            BeginAct(acts - 1);
            run.mapNode = run.map.bossNode;
            run.visitedNodes.Add(run.mapNode);
        }
        PickNextBattle();
        GoToNextBattle();
    }

    public void GoToNextBattle(){
        SceneManager.LoadScene(battleSceneName);
    }

    // Settings' Abandon Run: the run is dropped and the title menu opens; New Run there starts a fresh one.
    public void AbandonRun(){
        StartNewRun();
        SceneManager.LoadScene(SettingsUI.TitleSceneName);
    }

    // A new run begins on the run start screen, where the starting face is picked.
    public void RestartRun(){
        StartNewRun();
        SceneManager.LoadScene(runStartSceneName);
    }
}
