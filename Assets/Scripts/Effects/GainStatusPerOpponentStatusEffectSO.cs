using UnityEngine;
using System.Collections.Generic;

// The source gains value x (how many of the counted statuses the opponent has), at least the value.
// Rewards stacking different statuses on the enemy (e.g. Dark Communion gains Ritual per Burn/Poison/Bleed type).
[CreateAssetMenu(fileName = "NewGainStatusPerOpponentStatusEffect", menuName = "Dice/Effects/Gain Status Per Opponent Status Effect")]
public class GainStatusPerOpponentStatusEffectSO : EffectSO
{
    public StatusType statusToGain = StatusType.Ritual;
    public List<StatusType> countedStatuses = new List<StatusType>{ StatusType.Burn, StatusType.Poison, StatusType.Bleed };

    public override void Execute(EffectContext context){
        int amount = Amount(context);
        context.source.ApplyStatus(statusToGain, amount);
        Debug.Log(context.source.entityName + " gained " + amount + " " + statusToGain + ".");
    }

    public override string Describe(EffectContext context){
        if(context.target == null){
            return "gain " + context.value + " " + statusToGain + " per status type on the enemy";
        }
        return "gain " + Amount(context) + " " + statusToGain + " (" + context.value + " per status type on the enemy)";
    }

    private int Amount(EffectContext context){
        int types = 0;
        if(context.target != null){
            foreach(StatusType status in countedStatuses){
                if(context.target.GetStatus(status) > 0) types++;
            }
        }
        return context.value * Mathf.Max(1, types);
    }
}
