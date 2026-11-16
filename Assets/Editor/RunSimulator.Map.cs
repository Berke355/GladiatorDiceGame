using System.Collections.Generic;
using System.Text;
using UnityEditor;
using UnityEngine;

// The simulator on the map (GDD "Map", Roadmap map step 5): bots walk three acts of generated maps the way RunManager
// runs them, choosing their route node by node, so economy and difficulty are tuned on the run the player plays. The
// old battle line (Simulate) stays for comparisons.
public static partial class RunSimulator
{
    // Route choices, for experiments.
    public static bool TakeElites = true;
    // Rests heal below this share of max HP, else upgrade a face.
    public static float RestHealBelow = 0.7f;

    public class MapStats
    {
        public int runs, cleared;
        public int[] actsReached = new int[4];
        public Dictionary<string, int> deathsByNode = new Dictionary<string, int>();
        public Dictionary<string, int> deathsByEnemy = new Dictionary<string, int>();
        public Dictionary<MapNodeType, int> visits = new Dictionary<MapNodeType, int>();
        public int eliteFights, eliteDeaths, restHeals, restUpgrades;
        public long battlesFought;
        public long[] goldAtBoss = new long[3], hpAtBoss = new long[3];
        public int[] bossArrivals = new int[3];
        // Gold: earned in total, and at each kind of shop the gold brought in and taken out.
        public long goldEarned;
        public Dictionary<string, long[]> shops = new Dictionary<string, long[]>();
        public void Shopped(string kind, int goldIn, int goldOut){
            if(!shops.ContainsKey(kind)) shops[kind] = new long[3];
            shops[kind][0]++; shops[kind][1] += goldIn; shops[kind][2] += goldOut;
        }
        public string GoldReport(){
            StringBuilder s = new StringBuilder("gold earned per run " + (goldEarned / Mathf.Max(1, runs)) + " |");
            foreach(KeyValuePair<string, long[]> shop in shops){
                long n = Mathf.Max(1, (int)shop.Value[0]);
                s.Append(" " + shop.Key + ": " + ((float)shop.Value[0] / runs).ToString("0.0") + " visits/run, in " + (shop.Value[1] / n) + "g, spent " + ((shop.Value[1] - shop.Value[2]) / n) + "g |");
            }
            return s.ToString();
        }

        public string Report(string label){
            StringBuilder s = new StringBuilder("== " + label + " (" + runs + " runs, map) ==\n");
            s.Append("cleared: " + Pct(cleared) + " | reached act 2: " + Pct(actsReached[1]) + ", act 3: " + Pct(actsReached[2]) + "\n");
            s.Append("deaths by node:");
            foreach(KeyValuePair<string, int> d in deathsByNode) s.Append(" " + d.Key + " " + Pct(d.Value));
            s.Append("\nbattles fought per run: " + ((float)battlesFought / runs).ToString("0.0") + " | elites per run " + ((float)eliteFights / runs).ToString("0.00")
                + " (die " + (eliteFights > 0 ? (100f * eliteDeaths / eliteFights).ToString("0") : "-") + "%) | rests: heal " + restHeals + ", upgrade " + restUpgrades + "\n");
            s.Append("visits per run:");
            foreach(KeyValuePair<MapNodeType, int> v in visits) s.Append(" " + v.Key + " " + ((float)v.Value / runs).ToString("0.0"));
            s.Append("\nat each boss (arrivals, avg gold, avg HP):");
            for(int a = 0; a < 3; a++){
                if(bossArrivals[a] == 0) continue;
                s.Append(" act " + (a + 1) + ": " + Pct(bossArrivals[a]) + ", " + (goldAtBoss[a] / bossArrivals[a]) + "g, " + (hpAtBoss[a] / bossArrivals[a]) + " HP |");
            }
            List<KeyValuePair<string, int>> killers = new List<KeyValuePair<string, int>>(deathsByEnemy);
            killers.Sort((x, y) => y.Value.CompareTo(x.Value));
            s.Append("\ntop killers:");
            for(int i = 0; i < Mathf.Min(6, killers.Count); i++) s.Append(" " + killers[i].Key + " " + killers[i].Value);
            return s.AppendLine().ToString();
        }

        private string Pct(int count){ return (100f * count / Mathf.Max(1, runs)).ToString("0") + "%"; }
    }

    public static string SimulateMap(Bot bot, int runs){
        MapStats stats = SimulateMapStats(bot, runs);
        return stats.Report(bot.label);
    }

    // Clear rates on the map, one line: "Bleed 21% / Rage 12% / ...".
    public static string MapClearRates(IEnumerable<Bot> bots, int runs){
        List<string> parts = new List<string>();
        foreach(Bot bot in bots){
            MapStats stats = SimulateMapStats(bot, runs);
            parts.Add(bot.label.Replace("Focused: ", "").Replace("Hybrid: ", "") + " " + (100f * stats.cleared / runs).ToString("0") + "%");
        }
        return string.Join(" / ", parts.ToArray());
    }

    public static MapStats SimulateMapStats(Bot bot, int runs){
        RunManager config = AssetDatabase.LoadAssetAtPath<RunManager>("Assets/Resources/RunManager.prefab");
        EconomyConfigSO economy = config.economy;
        EncounterConfigSO encounters = config.encounters;
        List<EnemySO> pool = BuildEnemyPool(encounters, null, null);
        List<EnemyGroup> groups = BuildGroups(encounters, pool);
        MapStats stats = new MapStats{ runs = runs };
        System.Random rng = new System.Random(Random.Range(int.MinValue, int.MaxValue));

        GameObject playerObject = new GameObject("SimPlayer"){ hideFlags = HideFlags.HideAndDontSave };
        GameObject[] enemyObjects = new GameObject[EnemyGroup.MaxMembers];
        for(int i = 0; i < enemyObjects.Length; i++) enemyObjects[i] = new GameObject("SimEnemy" + i){ hideFlags = HideFlags.HideAndDontSave };
        bool logWasEnabled = Debug.unityLogger.logEnabled;
        Debug.unityLogger.logEnabled = false;
        // The map's Market has no Healer.
        bool healerWas = UseHealer;
        UseHealer = false;
        try{
            for(int r = 0; r < runs; r++){
                RunData run = new RunData(config.startingDieFaces, config.startingMaxHP);
                run.gold = economy.startingGold;
                run.startingFaceOffers = StartingFaceOffers.Generate(economy, config.startingDieFaces);
                while(UseStartingReroll && bot.shopping == Shopping.Focused && run.startingOfferRerolls < MaxStartingRerolls
                    && !run.startingFaceOffers.Exists(face => bot.Fits(face))
                    && StartingFaceOffers.Reroll(economy, run, config.startingDieFaces)){ }
                run.dieFaces.Add(new DieFace(ChooseStartingFace(bot, run.startingFaceOffers)));
                run.startingFaceChosen = true;
                if(RunSetup != null) RunSetup(run);
                bool alive = true;
                for(int act = 0; act < config.acts && alive; act++){
                    stats.actsReached[act]++;
                    ActMap map = MapGenerator.Generate(config.mapConfig, act, rng);
                    int at = -1;
                    while(alive){
                        List<int> options = map.Reachable(at);
                        if(options.Count == 0) break;
                        at = ChooseNode(bot, run, map, options);
                        MapNode node = map.nodes[at];
                        stats.visits[node.type] = (stats.visits.ContainsKey(node.type) ? stats.visits[node.type] : 0) + 1;
                        alive = PlayNode(bot, run, node, act, config, pool, groups, playerObject, enemyObjects, stats);
                        if(node.type == MapNodeType.Boss) break;
                    }
                }
                if(alive) stats.cleared++;
            }
        }
        finally{
            UseHealer = healerWas;
            Debug.unityLogger.logEnabled = logWasEnabled;
            Object.DestroyImmediate(playerObject);
            foreach(GameObject enemyObject in enemyObjects) Object.DestroyImmediate(enemyObject);
        }
        return stats;
    }

    // A reasonable route: rest when hurt, shop with gold in hand, take elites when healthy, otherwise fight.
    private static int ChooseNode(Bot bot, RunData run, ActMap map, List<int> options){
        float hp = (float)run.currentHP / run.maxHP;
        int best = options[0];
        float bestScore = float.MinValue;
        foreach(int index in options){
            float score;
            switch(map.nodes[index].type){
                case MapNodeType.Rest: score = hp < 0.5f ? 4f : 1.5f; break;
                case MapNodeType.Market: score = run.gold >= 35 ? 3f : 0.5f; break;
                case MapNodeType.EnchantingTable: score = run.gold >= 40 && bot.shopping != Shopping.None ? 1.5f : 0.3f; break;
                case MapNodeType.Elite: score = !TakeElites ? -10f : hp >= 0.65f ? 2.5f : hp >= 0.45f ? 0.6f : -5f; break;
                default: score = 1f; break;
            }
            score += Random.value * 0.5f;
            if(score > bestScore){ bestScore = score; best = index; }
        }
        return best;
    }

    // Plays one node; false if the run died there. Rewards mirror RunManager.OnBattleEnded on the map.
    private static bool PlayNode(Bot bot, RunData run, MapNode node, int act, RunManager config, List<EnemySO> pool, List<EnemyGroup> groups,
        GameObject playerObject, GameObject[] enemyObjects, MapStats stats){
        EconomyConfigSO economy = config.economy;
        switch(node.type){
            case MapNodeType.Battle:
            case MapNodeType.Elite:
            case MapNodeType.Boss:{
                BossSO boss = null;
                EnemyGroup encounter = null;
                EnemyTier tier = (EnemyTier)Mathf.Min(act, (int)EnemyTier.Strong);
                if(node.type == MapNodeType.Boss){
                    boss = config.encounters.PickBossForAct(act);
                    stats.bossArrivals[act]++;
                    stats.goldAtBoss[act] += run.gold;
                    stats.hpAtBoss[act] += run.currentHP;
                }
                else if(node.type == MapNodeType.Elite){
                    EnemySO elite = config.encounters.PickElite(act, run.metEnemies);
                    if(elite != null){
                        encounter = EnemyGroup.Alone(elite);
                        run.metEnemies.Add(elite);
                    }
                    stats.eliteFights++;
                }
                if(boss == null && encounter == null) encounter = EncounterConfigSO.PickEncounter(pool, groups, tier, run.metEnemies);
                int turns;
                bool won = FightBattle(run, encounter, boss, playerObject, enemyObjects, out turns, config.encounters, run.battlesWon + 1);
                stats.battlesFought++;
                if(!won){
                    string where = node.type.ToString() + " (act " + (act + 1) + ")";
                    stats.deathsByNode[where] = (stats.deathsByNode.ContainsKey(where) ? stats.deathsByNode[where] : 0) + 1;
                    string who = boss != null ? boss.bossName : encounter.DisplayName;
                    stats.deathsByEnemy[who] = (stats.deathsByEnemy.ContainsKey(who) ? stats.deathsByEnemy[who] : 0) + 1;
                    if(node.type == MapNodeType.Elite) stats.eliteDeaths++;
                    return false;
                }
                run.battlesWon++;
                int goldBefore = run.gold;
                run.gold += economy.goldPerVictory + EquipmentSO.TotalGoldPerVictory(run.equipment);
                if(node.type == MapNodeType.Elite){
                    run.gold += economy.eliteGoldBonus;
                    List<EquipmentSO> choices = EliteReward.Pick(economy, run, economy.eliteRewardChoices);
                    if(choices.Count > 0 && UseEquipment && bot.shopping != Shopping.None){
                        EquipmentSO pick = choices[0];
                        foreach(EquipmentSO item in choices) if(bot.Want(item) > bot.Want(pick)) pick = item;
                        run.equipment.Add(pick);
                    }
                }
                if(node.type == MapNodeType.Boss){
                    if(act >= config.acts - 1){ stats.goldEarned += run.gold - goldBefore; return true; }
                    run.currentHP = Mathf.Min(run.maxHP, run.currentHP + Mathf.RoundToInt(run.maxHP * config.bossHealFraction));
                    run.gold += economy.bossVictoryBonus;
                    stats.goldEarned += run.gold - goldBefore;
                    run.bossRewardPending = true;
                    Market grand = new Market(run, economy, true);
                    int grandIn = run.gold;
                    Shop(bot, grand, run, 0);
                    stats.Shopped("Grand", grandIn, run.gold);
                    if(grand.IsRewardPending) grand.SkipReward();
                }
                if(node.type != MapNodeType.Boss) stats.goldEarned += run.gold - goldBefore;
                return true;
            }
            case MapNodeType.Market:{
                // The map's Market: faces, items and upgrades, no Enchanting Table.
                Market market = new Market(run, economy, false);
                market.enchantmentOffers.Clear();
                market.dieEnchantmentOffers.Clear();
                int marketIn = run.gold;
                Shop(bot, market, run, 0);
                stats.Shopped("Market", marketIn, run.gold);
                return true;
            }
            case MapNodeType.EnchantingTable:{
                // The table: enchantments and upgrades, no faces or items.
                Market table = new Market(run, economy, false);
                table.offers.Clear();
                table.equipmentOffers.Clear();
                int tableIn = run.gold;
                Shop(bot, table, run, 0);
                stats.Shopped("Table", tableIn, run.gold);
                return true;
            }
            case MapNodeType.Rest:{
                if((float)run.currentHP / run.maxHP < RestHealBelow){
                    run.currentHP = Mathf.Min(run.maxHP, run.currentHP + Mathf.RoundToInt(run.maxHP * config.restHealFraction));
                    stats.restHeals++;
                }
                else{
                    // The face that matters most: a build face with the highest value (else any).
                    DieFace best = null;
                    foreach(DieFace face in run.dieFaces){
                        if(!RunManager.CanRestUpgradeFace(face)) continue;
                        bool fits = bot.Fits(face.definition);
                        bool bestFits = best != null && bot.Fits(best.definition);
                        if(best == null || (fits && !bestFits) || (fits == bestFits && face.value > best.value)) best = face;
                    }
                    if(best != null) best.value++;
                    stats.restUpgrades++;
                }
                return true;
            }
        }
        return true;
    }
}
