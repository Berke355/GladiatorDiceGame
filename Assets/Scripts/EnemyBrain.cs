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

    public EnemyBrain(EnemySO enemy, Entity self, Entity opponent){
        definition = enemy;
        this.self = self;
        this.opponent = opponent;
    }

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

        // Weighted pick, skipping the current intent if it already ran MaxRepeats times in a row.
        bool blockCurrent = repeatCount >= MaxRepeats && intents.Count > 1;
        int totalWeight = 0;
        for(int i = 0; i < intents.Count; i++){
            if(blockCurrent && i == currentIndex) continue;
            totalWeight += Weight(intents[i]);
        }

        int roll = Random.Range(0, totalWeight);
        int picked = 0;
        for(int i = 0; i < intents.Count; i++){
            if(blockCurrent && i == currentIndex) continue;
            roll -= Weight(intents[i]);
            if(roll < 0){
                picked = i;
                break;
            }
        }

        repeatCount = picked == currentIndex ? repeatCount + 1 : 1;
        currentIndex = picked;
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
        EffectContext context = CreateContext();
        foreach(EffectSO effect in currentIntent.effects){
            if(effect != null){
                effect.Execute(context);
            }
        }
    }

    // Called after the enemy's turn: escalating enemies gain Rage, which is visible as a status.
    public void OnTurnEnd(){
        if(definition.escalationPerTurn > 0){
            self.ApplyStatus(StatusType.Rage, definition.escalationPerTurn);
        }
    }

    private EffectContext CreateContext(){
        return new EffectContext{
            source = self,
            target = opponent,
            value = currentIntent.value,
            targetLastFace = flow != null ? flow.LastActedFace : null,
            targetLastValue = flow != null ? flow.LastActedValue : 0,
            targetLastAction = flow != null ? flow.LastAction : ActionType.Attack
        };
    }

    // Threat intents (PunishChoiceEffectSO) only hit if the player's choice sets them off, so they don't count here.
    public int IncomingDamage => currentIntent.intentType == ActionType.Attack && !IsThreat(currentIntent) ? currentIntent.value + self.GetStatus(StatusType.Rage) : 0;

    public int ThreatAgainst(DieFace face, int battleValue, ActionType action){
        int damage = 0;
        if(currentIntent.effects == null) return 0;
        foreach(EffectSO effect in currentIntent.effects){
            PunishChoiceEffectSO threat = effect as PunishChoiceEffectSO;
            if(threat != null && threat.Triggers(face, battleValue, action)) damage += currentIntent.value + self.GetStatus(StatusType.Rage);
        }
        return damage;
    }

    private static bool IsThreat(EnemyIntent intent){
        return intent.effects != null && intent.effects.Exists(effect => effect is PunishChoiceEffectSO);
    }
    // The battle, for the player's last action (threats on the player's choice).
    private BattleFlow flow;
    public void Bind(BattleFlow flow){ this.flow = flow; }

    public List<EnemyTrait> Traits => definition.traits;
    // Second Wind: true once the enemy dropped below half HP and switched to its second intent list.
    public bool InSecondWind { get; private set; }

    // Normal enemies have no die.
    public List<DieFace> Die => null;
    public DieFace RolledFace => null;

    private static int Weight(EnemyIntent intent){
        return intent.weight > 0 ? intent.weight : 1;
    }
}
