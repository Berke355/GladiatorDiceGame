using System.Collections.Generic;
using UnityEngine;

// Picks and runs a normal enemy's telegraphed intents (see GDD "Normal Enemies"). Plain C#, one per enemy per battle.
public class EnemyBrain : IEnemyBrain
{
    // The same intent can be picked at most this many turns in a row.
    private const int MaxRepeats = 2;

    public readonly EnemySO definition;
    public EnemyIntent currentIntent;

    // The enemy this brain controls and its opponent (the player).
    private readonly Entity self;
    private readonly Entity opponent;
    private int currentIndex = -1;
    private int repeatCount;
    // Intent cooldowns: the turn (count of picks) each intent was last picked.
    private int picks;
    private readonly Dictionary<int, int> lastPicked = new Dictionary<int, int>();

    // The brain for an enemy asset: the Mimic fights with a copy of the player's die, every other enemy with intents.
    public static IEnemyBrain Create(EnemySO enemy, Entity self, Entity opponent, List<DieFace> playerDie){
        if(enemy.mirrorsPlayerDie) return new MimicBrain(enemy, self, opponent, playerDie);
        return new EnemyBrain(enemy, self, opponent);
    }

    public EnemyBrain(EnemySO enemy, Entity self, Entity opponent){
        definition = enemy;
        this.self = self;
        this.opponent = opponent;
        // The simulator reuses its enemy objects, and a boss may have set this on one.
        self.maxHPLossPerRound = 0;
    }

    // Forgets the current intent, so the enemy doesn't act until it picks a new one (a Hydra head growing back).
    public void ClearIntent(){
        currentIndex = -1;
        repeatCount = 0;
        currentIntent = default(EnemyIntent);
    }

    public bool HasIntent => currentIndex >= 0;

    public void ChooseNextIntent(){
        if(!InSecondWind && definition.secondWindIntents.Count > 0 && self.currentHP * 2 <= self.maxHP){
            InSecondWind = true;
            currentIndex = -1;
            repeatCount = 0;
            Debug.Log(definition.enemyName + " gets a second wind.");
        }
        List<EnemyIntent> intents = InSecondWind ? definition.secondWindIntents : definition.intents;
        if(intents.Count == 0){
            return;
        }

        // Weighted pick among the allowed intents: not the current one if it already ran MaxRepeats times in a row,
        // not one whose required status the player lacks, and not an attack while a pack member already attacks.
        // Each rule is dropped again if it would leave nothing to pick.
        bool blockCurrent = repeatCount >= MaxRepeats && intents.Count > 1;
        bool avoidAttack = PackTacticsHold && PackMemberAttacks();
        List<int> allowed = Allowed(intents, blockCurrent, true, avoidAttack);
        if(allowed.Count == 0) allowed = Allowed(intents, blockCurrent, true, false);
        if(allowed.Count == 0) allowed = Allowed(intents, blockCurrent, false, false);
        if(allowed.Count == 0) allowed = Allowed(intents, false, false, false);

        int totalWeight = 0;
        foreach(int i in allowed) totalWeight += Weight(intents[i]);
        int roll = Random.Range(0, totalWeight);
        int picked = allowed[0];
        foreach(int i in allowed){
            roll -= Weight(intents[i]);
            if(roll < 0){
                picked = i;
                break;
            }
        }

        repeatCount = picked == currentIndex ? repeatCount + 1 : 1;
        currentIndex = picked;
        picks++;
        lastPicked[picked] = picks;
        currentIntent = intents[picked];
        Debug.Log(definition.enemyName + "'s new intent: " + DescribeCurrentIntent());
    }

    // Readable intent text with the real numbers (Rage, Ritual and current stacks included),
    // e.g. "Shield Slam: 4 damage" or "Shield Rush: 2 damage, 2 block".
    public string DescribeCurrentIntent(){
        // No intent picked yet (before the first turn).
        if(currentIndex < 0) return "";
        List<string> parts = new List<string>();
        EffectContext context = CreateContext();
        if(currentIntent.effects != null){
            foreach(EffectSO effect in currentIntent.effects){
                string description = effect != null ? effect.Describe(context) : "";
                if(description != ""){
                    parts.Add(description);
                }
            }
        }

        if(parts.Count == 0){
            return currentIntent.intentName + " (" + currentIntent.value + ")";
        }
        return currentIntent.intentName + ": " + string.Join(", ", parts);
    }

    // Runs the current intent. Enemies have no die, so die and face stay null in the context.
    public void ExecuteCurrentIntent(){
        if(!HasIntent) return;
        EffectContext context = CreateContext();
        foreach(EffectSO effect in currentIntent.effects){
            if(effect != null){
                effect.Execute(context);
            }
        }
    }

    // Called after the enemy's turn: escalating enemies gain Rage, which is visible as a status; plated ones get plates
    // back, up to their starting count.
    public void OnTurnEnd(){
        if(definition.escalationPerTurn > 0){
            self.ApplyStatus(StatusType.Rage, definition.escalationPerTurn);
        }
        if(definition.platesPerTurn > 0){
            int room = definition.startingPlates - self.GetStatus(StatusType.Plating);
            if(room > 0) self.ApplyStatus(StatusType.Plating, System.Math.Min(room, definition.platesPerTurn));
        }
    }

    // ---------- Minions (the Necromancer) ----------

    private BattleEnemy Me => flow != null ? flow.enemies.Find(enemy => enemy.brain == this) : null;

    public int LivingMinions{
        get{
            BattleEnemy me = Me;
            return me == null ? 0 : flow.enemies.FindAll(enemy => enemy.summonedBy == me && !enemy.IsDead).Count;
        }
    }

    // Raises one minion in front of this enemy, if there is room. Returns whether one rose.
    public bool RaiseMinion(){
        BattleEnemy me = Me;
        if(me == null || definition.minion == null || LivingMinions >= definition.maxMinions) return false;
        int number = flow.enemies.FindAll(enemy => enemy.summonedBy == me).Count + 1;
        return flow.SpawnEnemy(definition.minion, definition.minion.enemyName + " " + number, null, me) != null;
    }

    private EffectContext CreateContext(){
        return new EffectContext{
            source = self,
            target = opponent,
            value = currentIntent.value,
            targetLastFace = flow != null ? flow.LastActedFace : null,
            targetLastValue = flow != null ? flow.LastActedValue : 0,
            targetLastAction = flow != null ? flow.LastAction : ActionType.Attack,
            allies = Allies(),
            targetRerolls = flow != null ? flow.rerolls : null,
            summon = RaiseMinion
        };
    }

    // The other living enemies of this battle.
    private List<Entity> Allies(){
        List<Entity> allies = new List<Entity>();
        if(flow == null) return allies;
        foreach(BattleEnemy enemy in flow.enemies){
            if(!enemy.IsDead && enemy.entity != self) allies.Add(enemy.entity);
        }
        return allies;
    }

    // Threat intents (PunishChoiceEffectSO) only hit if the player's choice sets them off, so they don't count here.
    public int IncomingDamage => HasIntent && currentIntent.intentType == ActionType.Attack && !IsThreat(currentIntent) ? currentIntent.value + self.GetStatus(StatusType.Rage) : 0;

    public int ThreatAgainst(DieFace face, int battleValue, ActionType action){
        int damage = 0;
        if(!HasIntent || currentIntent.effects == null) return 0;
        foreach(EffectSO effect in currentIntent.effects){
            PunishChoiceEffectSO threat = effect as PunishChoiceEffectSO;
            if(threat != null && threat.Triggers(face, battleValue, action)) damage += currentIntent.value + self.GetStatus(StatusType.Rage);
        }
        return damage;
    }

    public bool IntendsAttack => HasIntent && currentIntent.intentType == ActionType.Attack;

    public bool ProvokeToWeakestAttack(){
        if(!HasIntent || IntendsAttack) return false;
        List<EnemyIntent> intents = InSecondWind ? definition.secondWindIntents : definition.intents;
        int weakest = -1;
        // An attack that needs a status the player lacks (Ignite without Burn) is a last resort.
        foreach(bool checkStatus in new[]{ true, false }){
            for(int i = 0; i < intents.Count; i++){
                if(intents[i].intentType != ActionType.Attack) continue;
                StatusType needed = intents[i].requiresTargetStatus;
                if(checkStatus && needed != StatusType.None && opponent.GetStatus(needed) <= 0) continue;
                if(weakest < 0 || Strength(intents[i]) < Strength(intents[weakest])) weakest = i;
            }
            if(weakest >= 0) break;
        }
        if(weakest < 0) return false;
        currentIndex = weakest;
        repeatCount = 1;
        currentIntent = intents[weakest];
        Debug.Log(definition.enemyName + " was provoked: " + DescribeCurrentIntent());
        return true;
    }

    // An attack's damage before Rage, for picking the weakest one: its value, times its hits for a multi-hit.
    private static int Strength(EnemyIntent intent){
        int hits = 1;
        if(intent.effects != null){
            foreach(EffectSO effect in intent.effects){
                MultiHitDamageEffectSO multi = effect as MultiHitDamageEffectSO;
                if(multi != null) hits = Mathf.Max(hits, multi.hits);
            }
        }
        return intent.value * hits;
    }

    private static bool IsThreat(EnemyIntent intent){
        return intent.effects != null && intent.effects.Exists(effect => effect is PunishChoiceEffectSO);
    }
    // The battle, for the player's last action (threats on the player's choice).
    private BattleFlow flow;
    // Starting plates and minions join here.
    public void Bind(BattleFlow flow){
        this.flow = flow;
        if(definition.startingPlates > 0) self.ApplyStatus(StatusType.Plating, definition.startingPlates);
        for(int i = 0; i < definition.startingMinions; i++) RaiseMinion();
    }

    public List<EnemyTrait> Traits => definition.traits;
    // Second Wind: true once the enemy dropped below half HP and switched to its second intent list.
    public bool InSecondWind { get; private set; }

    // Normal enemies have no die.
    public List<DieFace> Die => null;
    public DieFace RolledFace => null;

    private List<int> Allowed(List<EnemyIntent> intents, bool blockCurrent, bool checkStatus, bool avoidAttack){
        List<int> allowed = new List<int>();
        for(int i = 0; i < intents.Count; i++){
            if(blockCurrent && i == currentIndex) continue;
            StatusType needed = intents[i].requiresTargetStatus;
            if(checkStatus && needed != StatusType.None && opponent.GetStatus(needed) <= 0) continue;
            if(avoidAttack && intents[i].intentType == ActionType.Attack) continue;
            if(checkStatus && intents[i].requiresMinionRoom && LivingMinions >= definition.maxMinions) continue;
            int last;
            if(checkStatus && intents[i].cooldown > 0 && lastPicked.TryGetValue(i, out last) && picks + 1 - last <= intents[i].cooldown) continue;
            allowed.Add(i);
        }
        return allowed;
    }

    // Pack tactics hold for the first packTacticsTurns turns of the battle (0: always).
    private bool PackTacticsHold => definition.packTactics && (definition.packTacticsTurns <= 0 || flow == null || flow.TurnNumber <= definition.packTacticsTurns);

    // Whether a living pack member that picked before this one (earlier in turn order) attacks this turn.
    private bool PackMemberAttacks(){
        if(flow == null) return false;
        foreach(BattleEnemy enemy in flow.enemies){
            if(enemy.brain == this) return false;
            EnemyBrain other = enemy.brain as EnemyBrain;
            if(enemy.IsDead || other == null || !other.definition.packTactics || !other.HasIntent) continue;
            if(other.currentIntent.intentType == ActionType.Attack) return true;
        }
        return false;
    }

    private static int Weight(EnemyIntent intent){
        return intent.weight > 0 ? intent.weight : 1;
    }
}
