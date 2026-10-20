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
        // Archetype tags a Focused bot builds around. Several tags make a hybrid bot (e.g. Ritual + Poison).
        public FaceTag[] focusTags;

        public Bot(string label, Shopping shopping, params FaceTag[] focusTags){
            this.label = label;
            this.shopping = shopping;
            this.focusTags = focusTags;
        }

        public bool Fits(DieFaceSO face){
            return Fits(face.tags);
        }

        public bool Fits(EnchantmentSO enchantment){
            return Fits(enchantment.tags);
        }

        // How much a focused bot wants an item: 2 for its archetype, 1 for a general item (no tags), 0 otherwise.
        public int Want(EquipmentSO item){
            if(Fits(item.tags)) return 2;
            return item.tags.Count == 0 ? 1 : 0;
        }

        private bool Fits(List<FaceTag> tags){
            foreach(FaceTag tag in focusTags){
                if(tags.Contains(tag)) return true;
            }
            return false;
        }
    }

    // A Focused bot rerolls at most this many times per Market, and only if it keeps this much gold after paying.
    // Public so experiments can compare reroll strategies.
    public static int MaxRerollsPerVisit = 3;
    // Rage-building faces switch to attacking once the bot has this much Rage.
    public static int RageToStartAttacking = 3;
    // 40 measured best: rerolling down to little gold costs the upgrades a build needs (DevLog Day 29).
    public static int GoldToKeepAfterReroll = 40;
    // Bots buy enchantments; off to measure what enchantments add.
    public static bool BuyEnchantments = true;
    // Bots play the enchantments they bought; off to separate the gold cost from playing them.
    public static bool PlayEnchantments = true;
    // Bots take boss rewards and buy equipment; off to measure what equipment adds.
    public static bool UseEquipment = true;

    // Per-battle measurements for diagnosing a build (null = off). Only battles of statsTier are counted.
    public class FightStats
    {
        public EnemyTier statsTier = EnemyTier.Strong;
        // Optional status on the enemy to follow: stacks each face adds, and stacks when the player acts.
        public StatusType trackedStatus = StatusType.None;
        public long trackedStacksAtAction;
        // Per battle turn (1, 2, 3...): summed tracked stacks at the player's action, and how many battles got that far.
        public long[] trackedByTurn = new long[16];
        public int[] battlesByTurn = new int[16];
        public int battles, won, turns, actions, bleedingAtAction;
        // Turns where an enchantment was played (the face's Magic came from it), and their damage.
        public int enchantmentActions;
        public long enchantmentDamage;
        // Enemy HP lost from the player's actions vs. from ticks at the end of the enemy's turn (Poison, Bleed).
        public long actionDamage, tickDamage, hpTaken;
        // Per face name: { times rolled, enemy HP lost from its actions, tracked stacks added }.
        public Dictionary<string, int[]> perFace = new Dictionary<string, int[]>();

        public string Report(){
            if(battles == 0) return "no " + statsTier + " battles";
            StringBuilder sb = new StringBuilder();
            if(enchantmentActions > 0) sb.AppendLine("enchantments played " + (100f * enchantmentActions / actions).ToString("0.0") + "% of turns, " + ((float)enchantmentDamage / enchantmentActions).ToString("0.0") + " dmg each");
            sb.AppendLine(statsTier + " battles " + battles + ", won " + (100f * won / battles).ToString("0") + "%, avg turns " + ((float)turns / battles).ToString("0.0"));
            sb.AppendLine("per player turn: action dmg " + ((float)actionDamage / actions).ToString("0.00") + ", tick dmg " + ((float)tickDamage / actions).ToString("0.00")
                + ", HP taken " + ((float)hpTaken / actions).ToString("0.00") + ", enemy bleeding at action " + (100f * bleedingAtAction / actions).ToString("0") + "%");
            List<KeyValuePair<string, int[]>> faces = new List<KeyValuePair<string, int[]>>(perFace);
            faces.Sort((a, b) => b.Value[0].CompareTo(a.Value[0]));
            foreach(KeyValuePair<string, int[]> face in faces){
                sb.Append(face.Key + " " + (100f * face.Value[0] / actions).ToString("0") + "% rolls, " + ((float)face.Value[1] / face.Value[0]).ToString("0.0") + " dmg/roll");
                if(trackedStatus != StatusType.None) sb.Append(", +" + ((float)face.Value[2] / face.Value[0]).ToString("0.0") + " " + trackedStatus + "/roll");
                sb.Append("; ");
            }
            sb.AppendLine();
            if(trackedStatus != StatusType.None){
                sb.Append("avg enemy " + trackedStatus + " at action " + ((float)trackedStacksAtAction / actions).ToString("0.0") + " | by turn: ");
                for(int t = 1; t < trackedByTurn.Length; t++){
                    if(battlesByTurn[t] == 0) break;
                    sb.Append(t + ":" + ((float)trackedByTurn[t] / battlesByTurn[t]).ToString("0.0") + " ");
                }
                sb.AppendLine();
            }
            return sb.ToString();
        }
    }
    public static FightStats Stats;
    // Optional change to every run's starting state (e.g. a combo on the die), for combo tests. Null = off.
    public static System.Action<RunData> RunSetup;
    // Optional change before each battle (run, battle number), e.g. a combo that arrives at the first Grand Market.
    public static System.Action<RunData, int> BeforeBattle;

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
            new Bot("Focused: Rage", Shopping.Focused, FaceTag.Rage),
            new Bot("Focused: Ritual", Shopping.Focused, FaceTag.Ritual),
            new Bot("Focused: Bleed", Shopping.Focused, FaceTag.Bleed),
            new Bot("Focused: Hammer", Shopping.Focused, FaceTag.Hammer),
            // Ritual only boosts statuses other faces apply, so it is measured together with a status archetype.
            new Bot("Hybrid: Ritual + Poison", Shopping.Focused, FaceTag.Ritual, FaceTag.Poison),
            new Bot("Hybrid: Ritual + Bleed", Shopping.Focused, FaceTag.Ritual, FaceTag.Bleed),
            new Bot("Hybrid: Shield + Rage", Shopping.Focused, FaceTag.Block, FaceTag.Rage),
            // Lose HP for power, heal it back from a bleeding enemy (Bloodrage + Bloodscent loop).
            new Bot("Hybrid: Bleed + Rage", Shopping.Focused, FaceTag.Bleed, FaceTag.Rage),
        };
    }

    // ---------- Balance experiments ----------

    // Short clear-rate line for the given bots, e.g. "Bleed 12% / Rage 30%".
    public static string ClearRates(IEnumerable<Bot> bots, int runs){
        List<string> parts = new List<string>();
        foreach(Bot bot in bots){
            string report = Simulate(bot, runs);
            int start = report.IndexOf("cleared all 11: ") + "cleared all 11: ".Length;
            parts.Add(bot.label.Replace("Focused: ", "").Replace("Hybrid: ", "") + " " + report.Substring(start, report.IndexOf('%', start) - start) + "%");
        }
        return string.Join(" / ", parts);
    }

    // Runs measure() with some faces' base value temporarily changed by the given amounts
    // (keys are face names, e.g. "Open Wound"), then always restores them. The assets are never saved.
    public static string WithFaceValueChanges(Dictionary<string, int> valueDeltas, System.Func<string> measure){
        List<DieFaceSO> changed = new List<DieFaceSO>();
        List<int> oldBase = new List<int>();
        foreach(string guid in AssetDatabase.FindAssets("t:DieFaceSO", new[]{ "Assets/Faces" })){
            DieFaceSO face = AssetDatabase.LoadAssetAtPath<DieFaceSO>(AssetDatabase.GUIDToAssetPath(guid));
            if(face == null || !valueDeltas.ContainsKey(face.faceName)) continue;
            changed.Add(face);
            oldBase.Add(face.baseValue);
            face.baseValue += valueDeltas[face.faceName];
        }
        try{
            return measure();
        }
        finally{
            for(int i = 0; i < changed.Count; i++){
                changed[i].baseValue = oldBase[i];
            }
        }
    }

    public static Bot[] BotsMatching(params string[] labelParts){
        return System.Array.FindAll(DefaultBots(), bot => System.Array.Exists(labelParts, part => bot.label.Contains(part)));
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
        // What the die looks like when a run reaches the Strong tier (battle 8): is the build actually there?
        Dictionary<string, int> facesAtBattle8 = new Dictionary<string, int>();
        int buildFacesAtBattle8 = 0;
        int enchantedAtBattle8 = 0;
        int equipmentAtBattle8 = 0;
        Dictionary<string, int> itemsAtBattle8 = new Dictionary<string, int>();
        float goldAtFirstGrandMarket = 0;
        int firstGrandMarketVisits = 0;

        try{
            for(int r = 0; r < runs; r++){
                RunData run = new RunData(config.startingDieFaces, config.startingMaxHP);
                run.dieFaces.Add(new DieFace(ChooseStartingFace(bot, StartingFaceOffers.Generate(config.economy, config.startingDieFaces))));
                if(RunSetup != null) RunSetup(run);
                int hpBeforeWeak = run.currentHP;

                for(int battle = 1; battle <= BattlesPerRun; battle++){
                    reached[battle]++;
                    if(BeforeBattle != null) BeforeBattle(run, battle);
                    if(battle == 8){
                        foreach(DieFace face in run.dieFaces){
                            string name = face.definition.faceName;
                            facesAtBattle8[name] = (facesAtBattle8.ContainsKey(name) ? facesAtBattle8[name] : 0) + 1;
                            if(bot.Fits(face.definition)) buildFacesAtBattle8++;
                            if(face.enchantment != null) enchantedAtBattle8++;
                        }
                        equipmentAtBattle8 += run.equipment.Count;
                        foreach(EquipmentSO item in run.equipment){
                            itemsAtBattle8[item.equipmentName] = (itemsAtBattle8.ContainsKey(item.equipmentName) ? itemsAtBattle8[item.equipmentName] : 0) + 1;
                        }
                    }
                    EnemyTier tier = config.GetTierForBattle(battle);
                    // Boss slots with a boss fight it; the others (and boss slots without a boss yet) use a normal enemy.
                    BossSO boss = config.encounters.PickBoss(battle);
                    EnemySO enemyData = boss == null ? PickEnemy(pool, run, tier) : null;
                    string enemyName = boss != null ? boss.bossName : enemyData.enemyName;

                    int turns;
                    bool won = FightBattle(run, enemyData, boss, playerObject, enemyObject, out turns);
                    if(!won){
                        deathsAt[battle]++;
                        deathsByEnemy[enemyName] = (deathsByEnemy.ContainsKey(enemyName) ? deathsByEnemy[enemyName] : 0) + 1;
                        break;
                    }

                    turnsByTier[(int)tier] += turns;
                    battlesByTier[(int)tier]++;

                    // Same rewards as RunManager.OnBattleEnded.
                    run.battlesWon++;
                    run.gold += config.economy.goldPerVictory;
                    if(config.bossBattles.Contains(run.battlesWon)){
                        run.currentHP = Mathf.Min(run.maxHP, run.currentHP + Mathf.RoundToInt(run.maxHP * config.bossHealFraction));
                        run.gold += config.economy.bossVictoryBonus;
                        run.bossRewardPending = true;
                        // Gold in hand when the first Grand Market opens.
                        if(run.battlesWon == config.bossBattles[0]){
                            goldAtFirstGrandMarket += run.gold;
                            firstGrandMarketVisits++;
                        }
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
                    // The game does not let the player leave with the reward open; bots that ignore it skip it.
                    if(market.IsRewardPending) market.SkipReward();
                }
            }
        }
        finally{
            Debug.unityLogger.logEnabled = logWasEnabled;
            Object.DestroyImmediate(playerObject);
            Object.DestroyImmediate(enemyObject);
        }

        string report = FormatReport(bot, runs, reached, deathsAt, cleared, weakHpLost, weakCleared, turnsByTier, battlesByTier, deathsByEnemy);
        string gold = "avg gold entering the first Grand Market: " + (firstGrandMarketVisits > 0 ? (goldAtFirstGrandMarket / firstGrandMarketVisits).ToString("0") : "-") + "\n";
        string onHand = "avg enchanted faces at battle 8: " + (reached[8] > 0 ? ((float)enchantedAtBattle8 / reached[8]).ToString("0.0") : "-") + "\n";
        onHand += "avg equipment at battle 8: " + (reached[8] > 0 ? ((float)equipmentAtBattle8 / reached[8]).ToString("0.0") : "-");
        foreach(KeyValuePair<string, int> item in itemsAtBattle8){
            onHand += " | " + item.Key + " " + ((float)item.Value / reached[8]).ToString("0.00");
        }
        onHand += "\n";
        return report + gold + onHand + FormatDieAtBattle8(reached[8], buildFacesAtBattle8, facesAtBattle8);
    }

    // Average number of build faces at battle 8 and the most common faces on the die there.
    private static string FormatDieAtBattle8(int runsReached, int buildFaces, Dictionary<string, int> faces){
        if(runsReached == 0){
            return "die at battle 8: -\n";
        }
        List<KeyValuePair<string, int>> sorted = new List<KeyValuePair<string, int>>(faces);
        sorted.Sort((a, b) => b.Value.CompareTo(a.Value));
        StringBuilder sb = new StringBuilder("die at battle 8: avg build faces " + ((float)buildFaces / runsReached).ToString("0.0") + "/6 | per run: ");
        for(int i = 0; i < sorted.Count && i < 8; i++){
            sb.Append(sorted[i].Key + " " + ((float)sorted[i].Value / runsReached).ToString("0.0") + ", ");
        }
        return sb.AppendLine().ToString();
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
    // A normal enemy (enemyData) or a boss (boss); exactly one of them is set.
    private static bool FightBattle(RunData run, EnemySO enemyData, BossSO boss, GameObject playerObject, GameObject enemyObject, out int turns){
        Entity player = Reset<Entity>(playerObject);
        player.entityName = "Player";
        player.maxHP = run.maxHP;
        player.currentHP = run.currentHP;

        Entity enemy = Reset<Entity>(enemyObject);
        enemy.entityName = boss != null ? boss.bossName : enemyData.enemyName;
        enemy.maxHP = boss != null ? boss.maxHP : enemyData.maxHP;
        enemy.currentHP = enemy.maxHP;
        IEnemyBrain brain;
        if(boss != null){
            brain = new BossBrain(boss, enemy, player, run.dieFaces);
        }
        else{
            EnemyBrain intentBrain = Reset<EnemyBrain>(enemyObject);
            intentBrain.Setup(enemyData, enemy, player);
            brain = intentBrain;
        }

        // The same turn steps as the battle states (BattleFlow), so equipment triggers fire at the same points.
        BattleFlow flow = new BattleFlow(player, enemy, brain, run.dieFaces, run.equipment);
        EnemyTier statsTier = boss != null ? EnemyTier.Boss : enemyData.tier;
        FightStats stats = Stats != null && statsTier == Stats.statsTier ? Stats : null;
        turns = 0;
        flow.StartBattle();
        while(turns < MaxTurnsPerBattle && !flow.IsOver){
            turns++;
            flow.StartPlayerTurn();
            if(flow.IsOver) break;

            // Steady Hand rerolls within the same turn, so a turn can hold several rolls and actions.
            int rerolls = 0;
            do{
                DieFace face = flow.Roll();
                if(flow.IsOver) break;
                ActionType action = ChooseAction(face, run.dieFaces, player, enemy, brain, flow);
                int enemyHP = enemy.currentHP;
                int trackedBefore = stats != null && stats.trackedStatus != StatusType.None ? enemy.GetStatus(stats.trackedStatus) : 0;
                if(stats != null){
                    stats.actions++;
                    if(enemy.GetStatus(StatusType.Bleed) > 0) stats.bleedingAtAction++;
                    stats.trackedStacksAtAction += trackedBefore;
                    if(turns < stats.trackedByTurn.Length){
                        stats.trackedByTurn[turns] += trackedBefore;
                        stats.battlesByTurn[turns]++;
                    }
                }
                flow.PlayAction(action);
                if(stats != null){
                    string name = face.definition.faceName;
                    if(!stats.perFace.ContainsKey(name)) stats.perFace[name] = new int[3];
                    if(stats.trackedStatus != StatusType.None) stats.perFace[name][2] += Mathf.Max(0, enemy.GetStatus(stats.trackedStatus) - trackedBefore);
                    stats.perFace[name][0]++;
                    stats.perFace[name][1] += enemyHP - enemy.currentHP;
                    stats.actionDamage += enemyHP - enemy.currentHP;
                    if(action == ActionType.Magic && face.enchantment != null){
                        stats.enchantmentActions++;
                        stats.enchantmentDamage += enemyHP - enemy.currentHP;
                    }
                }
            } while(flow.RerollPending && !flow.IsOver && ++rerolls < 10);
            if(player.IsDead || enemy.IsDead) break;

            flow.EndPlayerTurn();
            if(player.IsDead || enemy.IsDead) break;

            int playerHP = player.currentHP;
            flow.EnemyAct();
            if(stats != null) stats.hpTaken += playerHP - player.currentHP;
            if(player.IsDead || enemy.IsDead) break;

            int enemyHPBeforeTicks = enemy.currentHP;
            flow.EndEnemyTurn();
            if(stats != null) stats.tickDamage += enemyHPBeforeTicks - enemy.currentHP;
            if(player.IsDead || enemy.IsDead) break;
        }

        bool won = enemy.IsDead && !player.IsDead;
        if(won) flow.WinBattle();
        flow.Dispose();
        run.currentHP = player.currentHP;
        if(stats != null){
            stats.battles++;
            stats.turns += turns;
            if(won) stats.won++;
        }
        return won;
    }

    private static T Reset<T>(GameObject target) where T : Component{
        T existing = target.GetComponent<T>();
        if(existing != null){
            Object.DestroyImmediate(existing);
        }
        return target.AddComponent<T>();
    }

    // A simple, reasonable player: uses payoffs when their resource is ready, blocks big telegraphed hits.
    private static ActionType ChooseAction(DieFace face, List<DieFace> die, Entity player, Entity enemy, IEnemyBrain brain, BattleFlow flow){
        DieFaceSO definition = face.definition;
        int burn = enemy.GetStatus(StatusType.Burn);
        int fortify = player.GetStatus(StatusType.Fortify);
        int rage = player.GetStatus(StatusType.Rage);
        bool bigHit = brain.IncomingDamage >= 6;
        // Stacking Fortify only pays off if the die has a face that spends it.
        bool usesFortify = die.Exists(f => f.definition.faceName == "Shield Burst" || f.definition.faceName == "Shield Bash" || f.definition.faceName == "Spiked Shield");

        if(PlayEnchantments && face.enchantment != null && PlaysEnchantment(face, die, player, flow, bigHit, rage)) return ActionType.Magic;

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
            // Fortify never resets, so stack it first; blocking a big hit also builds it.
            case "Spiked Shield": return !bigHit && fortify >= 6 ? ActionType.Attack : ActionType.Defense;
            case "Bite": return player.currentHP < player.maxHP / 2 ? ActionType.Magic : ActionType.Attack;
            // Rage and Ritual last the battle, so build some early, then start hitting with it.
            case "Fury": return rage < RageToStartAttacking ? ActionType.Magic : ActionType.Attack;
            case "Incantation": return ActionType.Magic;
            // Paying HP for Rage is only worth it with HP to spare and while Rage is still low.
            // Aim the Charge unless a big hit needs blocking.
            case "Anvil": return bigHit ? ActionType.Defense : ActionType.Magic;
            // Fish for a charged face; otherwise block.
            case "Steady Hand": return !bigHit && die.Exists(f => f != face && flow.charge.Get(f) >= 2) ? ActionType.Magic : ActionType.Defense;
            case "Bloodrage": return rage < RageToStartAttacking && player.currentHP > face.value + 15 ? ActionType.Magic : ActionType.Attack;
        }

        if(bigHit && face.HasAction(ActionType.Defense)) return ActionType.Defense;
        if(face.HasAction(ActionType.Attack)) return ActionType.Attack;
        if(face.HasAction(ActionType.Defense)) return ActionType.Defense;
        return ActionType.Magic;
    }

    // When to play an enchantment. Epic ones are combo pieces with their own timing; the others bring an
    // archetype onto a face outside it (e.g. Venom Coating on a Sword), so they are that face's build action.
    private static bool PlaysEnchantment(DieFace face, List<DieFace> die, Entity player, BattleFlow flow, bool bigHit, int rage){
        EnchantmentSO enchantment = face.enchantment;
        switch(enchantment.enchantmentName){
            case "War Cry": return !bigHit && rage < RageToStartAttacking;
            case "Dark Rite": return player.GetStatus(StatusType.Ritual) == 0;
            case "Whirlwind": return !bigHit || !face.HasAction(ActionType.Defense);
            // A turn without a hit: worth it when the Charge it hands out (my battle value to each other Charge
            // face, past the cap) is more than the hit I'd deal now. This face keeps its Charge either way.
            case "Resonance":
                int others = die.FindAll(f => f != face && f.definition.charges).Count;
                return others > 0 && flow.charge.ValueOf(face) * others > flow.charge.ValueOf(face) + player.GetStatus(StatusType.Rage);
        }
        bool offBuild = !enchantment.tags.Exists(tag => face.definition.tags.Contains(tag));
        return offBuild && !(bigHit && face.HasAction(ActionType.Defense));
    }

    // ---------- Run start ----------

    // Focused bots take a face of their archetype (or the rarest offer), the others pick at random.
    private static DieFaceSO ChooseStartingFace(Bot bot, List<DieFaceSO> offers){
        if(bot.shopping == Shopping.Focused){
            DieFaceSO best = null;
            foreach(DieFaceSO face in offers){
                bool fits = bot.Fits(face);
                bool bestFits = best != null && bot.Fits(best);
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
            case Shopping.Focused:
                if(UseEquipment) TakeBestReward(market, run, bot);
                ShopFocused(market, run, bot);
                break;
            case Shopping.Unplanned:
                if(UseEquipment && market.IsRewardPending){
                    int slot = market.HasFreeEquipmentSlot ? -1 : Random.Range(0, run.equipment.Count);
                    market.ClaimReward(Random.Range(0, market.rewardChoices.Count), slot);
                }
                ShopUnplanned(market, run);
                break;
        }
    }

    // Takes the reward the bot wants most. With full slots it replaces the item it wants least, if that one is worth less.
    private static void TakeBestReward(Market market, RunData run, Bot bot){
        if(!market.IsRewardPending) return;
        int best = 0;
        for(int i = 1; i < market.rewardChoices.Count; i++){
            if(bot.Want(market.rewardChoices[i]) > bot.Want(market.rewardChoices[best])) best = i;
        }
        int slot = EquipmentSlotFor(run, bot, bot.Want(market.rewardChoices[best]));
        if(slot >= -1) market.ClaimReward(best, slot);
    }

    // Buys an affordable item the bot wants (archetype first) if it has a slot for it.
    private static bool BuyWantedEquipment(Market market, RunData run, Bot bot){
        int best = -1;
        for(int i = 0; i < market.equipmentOffers.Count; i++){
            if(!market.CanBuyEquipment(i) || bot.Want(market.equipmentOffers[i].item) == 0) continue;
            if(best < 0 || bot.Want(market.equipmentOffers[i].item) > bot.Want(market.equipmentOffers[best].item)) best = i;
        }
        if(best < 0) return false;
        int slot = EquipmentSlotFor(run, bot, bot.Want(market.equipmentOffers[best].item));
        return slot >= -1 && market.BuyEquipment(best, slot);
    }

    // -1: a free slot. Otherwise the slot of the least wanted item, if it is wanted less than the new one; -2: no slot.
    private static int EquipmentSlotFor(RunData run, Bot bot, int newWant){
        if(run.equipment.Count < RunData.EquipmentSlots) return -1;
        int worst = 0;
        for(int i = 1; i < run.equipment.Count; i++){
            if(bot.Want(run.equipment[i]) < bot.Want(run.equipment[worst])) worst = i;
        }
        return bot.Want(run.equipment[worst]) < newWant ? worst : -2;
    }

    // Buys the best affordable face of its archetype into a face outside it, otherwise upgrades its archetype's faces.
    // If the Market shows nothing of its archetype, it rerolls while it can afford it and keep some gold.
    private static void ShopFocused(Market market, RunData run, Bot bot){
        int rerolls = 0;
        for(int guard = 0; guard < 20; guard++){
            int bestOffer = -1;
            bool fittingOfferOnShow = false;
            for(int i = 0; i < market.offers.Count; i++){
                if(market.offers[i].sold || !bot.Fits(market.offers[i].face)) continue;
                fittingOfferOnShow = true;
                if(!market.CanBuy(i)) continue;
                if(bestOffer < 0 || market.offers[i].face.rarity > market.offers[bestOffer].face.rarity){
                    bestOffer = i;
                }
            }
            if(bestOffer >= 0){
                int slot = PickSlotToReplace(run, market.offers[bestOffer].face, bot);
                if(slot >= 0){
                    market.Buy(bestOffer, slot);
                    continue;
                }
            }

            if(UseEquipment && BuyWantedEquipment(market, run, bot)){
                continue;
            }

            if(!fittingOfferOnShow && rerolls < MaxRerollsPerVisit && run.gold - market.GetRerollCost() >= GoldToKeepAfterReroll){
                market.Reroll();
                rerolls++;
                continue;
            }

            int cheapestUpgrade = -1;
            for(int i = 0; i < run.dieFaces.Count; i++){
                if(!bot.Fits(run.dieFaces[i].definition) || !market.CanUpgrade(i)) continue;
                if(cheapestUpgrade < 0 || market.GetUpgradeCost(i) < market.GetUpgradeCost(cheapestUpgrade)){
                    cheapestUpgrade = i;
                }
            }
            if(cheapestUpgrade >= 0){
                market.Upgrade(cheapestUpgrade);
                continue;
            }

            // Enchantments get the gold left after upgrades.
            if(BuyEnchantments && BuyFittingEnchantment(market, run, bot)){
                continue;
            }
            break;
        }
    }

    // Buys an affordable enchantment of the archetype (the rarest one). Epic enchantments are combo pieces, so
    // they go on the face with the highest value (a Charge face counts as +3, its usual Charge); the others go on
    // the highest-value face outside the archetype, which they turn into a build face.
    private static bool BuyFittingEnchantment(Market market, RunData run, Bot bot){
        int bestOffer = -1;
        for(int i = 0; i < market.enchantmentOffers.Count; i++){
            // Only Epic ones: Common and Rare enchantments are sidegrades on the low-value faces bots have, and
            // buying them made bots play Kindling instead of hitting (Burn died at battle 4 twice as often).
            EnchantmentSO offered = market.enchantmentOffers[i].enchantment;
            if(offered.rarity != Rarity.Epic || !bot.Fits(offered) || !market.CanBuyEnchantment(i)) continue;
            if(bestOffer < 0 || market.enchantmentOffers[i].enchantment.rarity > market.enchantmentOffers[bestOffer].enchantment.rarity){
                bestOffer = i;
            }
        }
        if(bestOffer < 0) return false;

        bool epic = market.enchantmentOffers[bestOffer].enchantment.rarity == Rarity.Epic;
        int slot = -1;
        for(int i = 0; i < run.dieFaces.Count; i++){
            DieFace face = run.dieFaces[i];
            if(!market.CanEnchant(i) || (!epic && bot.Fits(face.definition))) continue;
            if(slot < 0 || HostValue(face) > HostValue(run.dieFaces[slot])) slot = i;
        }
        return slot >= 0 && market.BuyEnchantment(bestOffer, slot);
    }

    private static int HostValue(DieFace face){
        return face.value + (face.definition.charges ? 3 : 0);
    }

    // Replaces the weakest face outside the archetype (a face carrying an archetype enchantment counts as inside it,
    // since replacing it loses the enchantment); if the die is all archetype, only a lower-rarity one.
    private static int PickSlotToReplace(RunData run, DieFaceSO incoming, Bot bot){
        int best = -1;
        for(int i = 0; i < run.dieFaces.Count; i++){
            DieFace face = run.dieFaces[i];
            if(bot.Fits(face.definition) || (face.enchantment != null && bot.Fits(face.enchantment))) continue;
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
            if(BuyEnchantments && Random.value < 0.3f && BuyRandomEnchantment(market, run)) continue;
            if(buyable.Count == 0 && upgradable.Count == 0) break;

            if(buyable.Count > 0 && (upgradable.Count == 0 || Random.value < 0.6f)){
                market.Buy(buyable[Random.Range(0, buyable.Count)], Random.Range(0, run.dieFaces.Count));
            }
            else{
                market.Upgrade(upgradable[Random.Range(0, upgradable.Count)]);
            }
        }
    }

    private static bool BuyRandomEnchantment(Market market, RunData run){
        for(int i = 0; i < market.enchantmentOffers.Count; i++){
            if(!market.CanBuyEnchantment(i)) continue;
            List<int> slots = new List<int>();
            for(int slot = 0; slot < run.dieFaces.Count; slot++){
                if(market.CanEnchant(slot)) slots.Add(slot);
            }
            return market.BuyEnchantment(i, slots[Random.Range(0, slots.Count)]);
        }
        return false;
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
