using UnityEngine;
using UnityEditor;
using System.Collections.Generic;
using System.Text;

// Balance tool (Editor only): simulates full runs with bots that fight with the real battle rules
// (Entity, EnemyBrain, effects) and shop in the real Market between battles. See DevLog Day 22–23.
// Battles are simulated directly, without loading scenes, so thousands of runs take seconds.
public static class RunSimulator
{
    public enum Shopping { None, Focused, Unplanned }

    public class Bot
    {
        public string label;
        public Shopping shopping;
        // Archetype tag a Focused bot builds around.
        public FaceTag focusTag;

        public Bot(string label, Shopping shopping, FaceTag focusTag = FaceTag.Physical){
            this.label = label;
            this.shopping = shopping;
            this.focusTag = focusTag;
        }
    }

    private const int MaxTurnsPerBattle = 100;
    private const int BattlesPerRun = 11;

    [MenuItem("Tools/Gladiator Dice/Run Balance Simulation")]
    public static void RunFromMenu(){
        Debug.Log(DefaultReport(300));
    }

    public static Bot[] DefaultBots(){
        return new[]{
            new Bot("No shopping", Shopping.None),
            new Bot("Unplanned buyer", Shopping.Unplanned),
            new Bot("Focused: Burn", Shopping.Focused, FaceTag.Burn),
            new Bot("Focused: Poison", Shopping.Focused, FaceTag.Poison),
            new Bot("Focused: Shield", Shopping.Focused, FaceTag.Block),
        };
    }

    // hpScale / damageScale are per tier (Weak, Medium, Strong); damageScale applies to every intent number.
    // Both only change simulation copies
    // of the enemies, so numbers can be explored before editing the assets. Null means 1.
    public static string DefaultReport(int runs, float[] hpScale = null, float[] damageScale = null){
        StringBuilder report = new StringBuilder();
        foreach(Bot bot in DefaultBots()){
            report.Append(Simulate(bot, runs, hpScale, damageScale));
        }
        return report.ToString();
    }

    public static string Simulate(Bot bot, int runs, float[] hpScale = null, float[] damageScale = null){
        RunManager config = AssetDatabase.LoadAssetAtPath<RunManager>("Assets/Resources/RunManager.prefab");
        List<EnemySO> pool = BuildEnemyPool(config.encounters, hpScale, damageScale);

        GameObject playerObject = new GameObject("SimPlayer"){ hideFlags = HideFlags.HideAndDontSave };
        GameObject enemyObject = new GameObject("SimEnemy"){ hideFlags = HideFlags.HideAndDontSave };
        bool logWasEnabled = Debug.unityLogger.logEnabled;
        Debug.unityLogger.logEnabled = false;

        int[] reached = new int[BattlesPerRun + 2];
        int[] deathsAt = new int[BattlesPerRun + 2];
        int cleared = 0;
        float weakHpLost = 0;
        int weakCleared = 0;
        float[] turnsByTier = new float[3];
        int[] battlesByTier = new int[3];
        Dictionary<string, int> deathsByEnemy = new Dictionary<string, int>();

        try{
            for(int r = 0; r < runs; r++){
                RunData run = new RunData(config.startingDieFaces, config.startingMaxHP);
                run.dieFaces.Add(new DieFace(ChooseStartingFace(bot, StartingFaceOffers.Generate(config.economy, config.startingDieFaces))));
                int hpBeforeWeak = run.currentHP;

                for(int battle = 1; battle <= BattlesPerRun; battle++){
                    reached[battle]++;
                    EnemyTier tier = config.GetTierForBattle(battle);
                    EnemySO enemyData = PickEnemy(pool, run, tier);

                    int turns;
                    bool won = FightBattle(run, enemyData, playerObject, enemyObject, out turns);
                    if(!won){
                        deathsAt[battle]++;
                        deathsByEnemy[enemyData.enemyName] = (deathsByEnemy.ContainsKey(enemyData.enemyName) ? deathsByEnemy[enemyData.enemyName] : 0) + 1;
                        break;
                    }

                    turnsByTier[(int)tier] += turns;
                    battlesByTier[(int)tier]++;

                    // Same rewards as RunManager.OnBattleEnded.
                    run.battlesWon++;
                    run.gold += config.economy.goldPerVictory;
                    if(config.bossBattles.Contains(run.battlesWon)){
                        run.currentHP = Mathf.Min(run.maxHP, run.currentHP + Mathf.RoundToInt(run.maxHP * config.bossHealFraction));
                    }

                    if(battle == 3){
                        weakHpLost += hpBeforeWeak - run.currentHP;
                        weakCleared++;
                    }
                    if(battle == BattlesPerRun){
                        cleared++;
                        break;
                    }

                    Market market = new Market(run, config.economy, config.bossBattles.Contains(run.battlesWon));
                    Shop(bot, market, run);
                }
            }
        }
        finally{
            Debug.unityLogger.logEnabled = logWasEnabled;
            Object.DestroyImmediate(playerObject);
            Object.DestroyImmediate(enemyObject);
        }

        return FormatReport(bot, runs, reached, deathsAt, cleared, weakHpLost, weakCleared, turnsByTier, battlesByTier, deathsByEnemy);
    }

    // ---------- Enemies ----------

    private static List<EnemySO> BuildEnemyPool(EncounterConfigSO encounters, float[] hpScale, float[] damageScale){
        List<EnemySO> pool = new List<EnemySO>();
        foreach(EnemySO original in encounters.enemyPool){
            int tier = (int)original.tier;
            float hp = hpScale != null ? hpScale[tier] : 1f;
            float damage = damageScale != null ? damageScale[tier] : 1f;
            if(hp == 1f && damage == 1f){
                pool.Add(original);
                continue;
            }

            // Scaled copy for this simulation only; the asset is never changed.
            EnemySO copy = Object.Instantiate(original);
            copy.hideFlags = HideFlags.HideAndDontSave;
            copy.maxHP = Mathf.Max(1, Mathf.RoundToInt(original.maxHP * hp));
            // Every intent number scales together (damage, block, Fortify, heal, statuses, escalation),
            // otherwise defensive intents end up huge next to the scaled-down HP and attacks.
            copy.escalationPerTurn = ScaleValue(original.escalationPerTurn, damage);
            copy.intents = new List<EnemyIntent>();
            foreach(EnemyIntent intent in original.intents){
                EnemyIntent scaled = intent;
                scaled.value = ScaleValue(intent.value, damage);
                copy.intents.Add(scaled);
            }
            pool.Add(copy);
        }
        return pool;
    }

    // Zero stays zero; anything else stays at least 1.
    private static int ScaleValue(int value, float scale){
        return value <= 0 ? 0 : Mathf.Max(1, Mathf.RoundToInt(value * scale));
    }

    // Same rules as RunManager.PickEnemyForNextBattle.
    private static EnemySO PickEnemy(List<EnemySO> pool, RunData run, EnemyTier tier){
        List<EnemySO> candidates = pool.FindAll(enemy => enemy.tier == tier && !run.metEnemies.Contains(enemy));
        if(candidates.Count == 0){
            candidates = pool.FindAll(enemy => enemy.tier == tier);
        }
        EnemySO picked = candidates[Random.Range(0, candidates.Count)];
        run.metEnemies.Add(picked);
        return picked;
    }

    // ---------- Battle ----------

    // Mirrors the battle states: player turn (roll, act, OnTurnEnd), then enemy turn.
    private static bool FightBattle(RunData run, EnemySO enemyData, GameObject playerObject, GameObject enemyObject, out int turns){
        Entity player = Reset<Entity>(playerObject);
        player.entityName = "Player";
        player.maxHP = run.maxHP;
        player.currentHP = run.currentHP;

        Entity enemy = Reset<Entity>(enemyObject);
        enemy.entityName = enemyData.enemyName;
        enemy.maxHP = enemyData.maxHP;
        enemy.currentHP = enemyData.maxHP;
        EnemyBrain brain = Reset<EnemyBrain>(enemyObject);
        brain.Setup(enemyData, enemy, player);

        turns = 0;
        while(turns < MaxTurnsPerBattle){
            turns++;
            player.OnTurnStart();
            brain.ChooseNextIntent();

            DieFace face = run.dieFaces[Random.Range(0, run.dieFaces.Count)];
            ActionType action = ChooseAction(face, run.dieFaces, player, enemy, brain);
            EffectContext context = new EffectContext{ source = player, target = enemy, value = face.value, die = run.dieFaces, face = face };
            foreach(ActionEffect effect in face.definition.effects){
                if(effect.actionType == action && effect.effectLogic != null){
                    effect.effectLogic.Execute(context);
                }
            }
            if(player.IsDead || enemy.IsDead) break;

            player.OnTurnEnd();
            if(player.IsDead || enemy.IsDead) break;

            enemy.OnTurnStart();
            brain.ExecuteCurrentIntent();
            if(player.IsDead || enemy.IsDead) break;

            enemy.OnTurnEnd();
            brain.OnTurnEnd();
            if(player.IsDead || enemy.IsDead) break;
        }

        run.currentHP = player.currentHP;
        return enemy.IsDead && !player.IsDead;
    }

    private static T Reset<T>(GameObject target) where T : Component{
        T existing = target.GetComponent<T>();
        if(existing != null){
            Object.DestroyImmediate(existing);
        }
        return target.AddComponent<T>();
    }

    // A simple, reasonable player: uses payoffs when their resource is ready, blocks big telegraphed hits.
    private static ActionType ChooseAction(DieFace face, List<DieFace> die, Entity player, Entity enemy, EnemyBrain brain){
        DieFaceSO definition = face.definition;
        int burn = enemy.GetStatus(StatusType.Burn);
        int fortify = player.GetStatus(StatusType.Fortify);
        bool bigHit = brain.currentIntent.intentType == ActionType.Attack && brain.currentIntent.value + enemy.GetStatus(StatusType.Rage) >= 6;
        // Stacking Fortify only pays off if the die has a face that spends it.
        bool usesFortify = die.Exists(f => f.definition.faceName == "Shield Burst" || f.definition.faceName == "Shield Bash");

        if(definition.faceName == "Shield"){
            if(bigHit) return ActionType.Defense;
            if(!usesFortify) return ActionType.Attack;
            return fortify < 6 ? ActionType.Defense : ActionType.Attack;
        }

        switch(definition.faceName){
            case "Fuel": return burn >= 3 ? ActionType.Magic : ActionType.Defense;
            case "Cauterize": return burn >= 3 && player.currentHP < player.maxHP - 4 ? ActionType.Magic : ActionType.Attack;
            case "Detonate": return burn >= 3 ? ActionType.Attack : ActionType.Magic;
            case "Plague": return ActionType.Magic;
            case "Shield Burst": return fortify >= 3 ? ActionType.Attack : ActionType.Defense;
            case "Bite": return player.currentHP < player.maxHP / 2 ? ActionType.Magic : ActionType.Attack;
        }

        if(bigHit && Has(definition, ActionType.Defense)) return ActionType.Defense;
        if(Has(definition, ActionType.Attack)) return ActionType.Attack;
        if(Has(definition, ActionType.Defense)) return ActionType.Defense;
        return ActionType.Magic;
    }

    private static bool Has(DieFaceSO face, ActionType action){
        return face.effects.Exists(effect => effect.actionType == action);
    }

    // ---------- Run start ----------

    // Focused bots take a face of their archetype (or the rarest offer), the others pick at random.
    private static DieFaceSO ChooseStartingFace(Bot bot, List<DieFaceSO> offers){
        if(bot.shopping == Shopping.Focused){
            DieFaceSO best = null;
            foreach(DieFaceSO face in offers){
                bool fits = face.tags.Contains(bot.focusTag);
                bool bestFits = best != null && best.tags.Contains(bot.focusTag);
                if(best == null || (fits && !bestFits) || (fits == bestFits && face.rarity > best.rarity)){
                    best = face;
                }
            }
            return best;
        }
        return offers[Random.Range(0, offers.Count)];
    }

    // ---------- Shopping ----------

    private static void Shop(Bot bot, Market market, RunData run){
        switch(bot.shopping){
            case Shopping.Focused: ShopFocused(market, run, bot.focusTag); break;
            case Shopping.Unplanned: ShopUnplanned(market, run); break;
        }
    }

    // Buys the best affordable face of its archetype into a face outside it, otherwise upgrades its archetype's faces.
    private static void ShopFocused(Market market, RunData run, FaceTag tag){
        for(int guard = 0; guard < 10; guard++){
            int bestOffer = -1;
            for(int i = 0; i < market.offers.Count; i++){
                if(!market.CanBuy(i) || !market.offers[i].face.tags.Contains(tag)) continue;
                if(bestOffer < 0 || market.offers[i].face.rarity > market.offers[bestOffer].face.rarity){
                    bestOffer = i;
                }
            }
            if(bestOffer >= 0){
                int slot = PickSlotToReplace(run, market.offers[bestOffer].face, tag);
                if(slot >= 0){
                    market.Buy(bestOffer, slot);
                    continue;
                }
            }

            int cheapestUpgrade = -1;
            for(int i = 0; i < run.dieFaces.Count; i++){
                if(!run.dieFaces[i].definition.tags.Contains(tag) || !market.CanUpgrade(i)) continue;
                if(cheapestUpgrade < 0 || market.GetUpgradeCost(i) < market.GetUpgradeCost(cheapestUpgrade)){
                    cheapestUpgrade = i;
                }
            }
            if(cheapestUpgrade >= 0){
                market.Upgrade(cheapestUpgrade);
                continue;
            }
            break;
        }
    }

    // Replaces the weakest face outside the archetype; if the die is all archetype, only a lower-rarity one.
    private static int PickSlotToReplace(RunData run, DieFaceSO incoming, FaceTag tag){
        int best = -1;
        for(int i = 0; i < run.dieFaces.Count; i++){
            DieFace face = run.dieFaces[i];
            if(face.definition.tags.Contains(tag)) continue;
            if(best < 0 || face.value < run.dieFaces[best].value) best = i;
        }
        if(best >= 0) return best;

        for(int i = 0; i < run.dieFaces.Count; i++){
            DieFace face = run.dieFaces[i];
            if(face.definition.rarity >= incoming.rarity) continue;
            if(best < 0 || face.definition.rarity < run.dieFaces[best].definition.rarity) best = i;
        }
        return best;
    }

    // Spends on whatever is affordable, replacing random faces: a die bought without a plan.
    private static void ShopUnplanned(Market market, RunData run){
        for(int guard = 0; guard < 6; guard++){
            List<int> buyable = new List<int>();
            for(int i = 0; i < market.offers.Count; i++){
                if(market.CanBuy(i)) buyable.Add(i);
            }
            List<int> upgradable = new List<int>();
            for(int i = 0; i < run.dieFaces.Count; i++){
                if(market.CanUpgrade(i)) upgradable.Add(i);
            }
            if(buyable.Count == 0 && upgradable.Count == 0) break;

            if(buyable.Count > 0 && (upgradable.Count == 0 || Random.value < 0.6f)){
                market.Buy(buyable[Random.Range(0, buyable.Count)], Random.Range(0, run.dieFaces.Count));
            }
            else{
                market.Upgrade(upgradable[Random.Range(0, upgradable.Count)]);
            }
        }
    }

    // ---------- Report ----------

    private static string FormatReport(Bot bot, int runs, int[] reached, int[] deathsAt, int cleared, float weakHpLost, int weakCleared,
        float[] turnsByTier, int[] battlesByTier, Dictionary<string, int> deathsByEnemy){
        StringBuilder sb = new StringBuilder();
        sb.AppendLine("== " + bot.label + " (" + runs + " runs) ==");

        sb.Append("reached: ");
        foreach(int battle in new[]{ 4, 5, 8, 9, 11 }){
            sb.Append(battle + ":" + Percent(reached[battle], runs) + " ");
        }
        sb.AppendLine("| cleared all " + BattlesPerRun + ": " + Percent(cleared, runs));

        sb.Append("deaths by battle: ");
        for(int battle = 1; battle <= BattlesPerRun; battle++){
            if(deathsAt[battle] > 0) sb.Append(battle + ":" + Percent(deathsAt[battle], runs) + " ");
        }
        sb.AppendLine();

        sb.Append("avg HP lost in Weak tier (battles 1-3): " + (weakCleared > 0 ? (weakHpLost / weakCleared).ToString("0.0") : "-"));
        sb.Append(" | avg turns: ");
        string[] tierNames = { "Weak", "Medium", "Strong" };
        for(int t = 0; t < 3; t++){
            sb.Append(tierNames[t] + " " + (battlesByTier[t] > 0 ? (turnsByTier[t] / battlesByTier[t]).ToString("0.0") : "-") + "  ");
        }
        sb.AppendLine();

        sb.Append("deaths by enemy: ");
        foreach(KeyValuePair<string, int> entry in deathsByEnemy){
            sb.Append(entry.Key + " " + entry.Value + ", ");
        }
        sb.AppendLine();
        return sb.ToString();
    }

    private static string Percent(int count, int total){
        return (100f * count / total).ToString("0") + "%";
    }
}
