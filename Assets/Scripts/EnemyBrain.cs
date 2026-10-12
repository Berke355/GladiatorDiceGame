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

    private int currentIndex = -1;
    private int repeatCount;
    // Added to the value of Attack intents (escalation and Battle Cry style buffs).
    private int attackBonus;

    public void Setup(EnemySO enemy){
        definition = enemy;
        currentIndex = -1;
        repeatCount = 0;
        attackBonus = 0;
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

    // The value the current intent will use, including the attack bonus.
    public int GetCurrentValue(){
        return currentIntent.value + (currentIntent.intentType == ActionType.Attack ? attackBonus : 0);
    }

    public string DescribeCurrentIntent(){
        if(currentIntent.attackBonusGain > 0 && (currentIntent.effects == null || currentIntent.effects.Count == 0)){
            return currentIntent.intentName + " (+" + currentIntent.attackBonusGain + " damage)";
        }

        int value = GetCurrentValue();
        foreach(EffectSO effect in currentIntent.effects){
            if(effect is MultiHitDamageEffectSO multiHit){
                return currentIntent.intentName + " (" + multiHit.hits + " x " + value + ")";
            }
        }
        return currentIntent.intentName + " (" + value + ")";
    }

    // Runs the current intent. Enemies have no die, so die and face stay null in the context.
    public void ExecuteCurrentIntent(Entity self, Entity target){
        EffectContext context = new EffectContext{
            source = self,
            target = target,
            value = GetCurrentValue()
        };
        foreach(EffectSO effect in currentIntent.effects){
            if(effect != null){
                effect.Execute(context);
            }
        }
        attackBonus += currentIntent.attackBonusGain;
    }

    // Called after the enemy's turn.
    public void OnTurnEnd(){
        attackBonus += definition.escalationPerTurn;
    }

    private static int Weight(EnemyIntent intent){
        return intent.weight > 0 ? intent.weight : 1;
    }
}
