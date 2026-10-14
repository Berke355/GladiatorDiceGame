using System.Collections.Generic;
using UnityEngine;

// Picks and runs a normal enemy's telegraphed intents (see GDD "Normal Enemies").
public class EnemyBrain : MonoBehaviour
{
    // The same intent can be picked at most this many turns in a row.
    private const int MaxRepeats = 2;

    [System.NonSerialized]
    public EnemySO definition;
    public EnemyIntent currentIntent;

    // The enemy this brain controls and its opponent (the player).
    private Entity self;
    private Entity opponent;
    private int currentIndex = -1;
    private int repeatCount;

    public void Setup(EnemySO enemy, Entity self, Entity opponent){
        definition = enemy;
        this.self = self;
        this.opponent = opponent;
        currentIndex = -1;
        repeatCount = 0;
    }

    public void ChooseNextIntent(){
        List<EnemyIntent> intents = definition.intents;
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
            value = currentIntent.value
        };
    }

    private static int Weight(EnemyIntent intent){
        return intent.weight > 0 ? intent.weight : 1;
    }
}
