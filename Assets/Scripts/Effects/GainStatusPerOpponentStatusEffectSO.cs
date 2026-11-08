using UnityEngine;
using System.Collections.Generic;

// The source gains value x (how many of the counted statuses the opponent has), at least the value.
// Rewards stacking different statuses on the enemy (e.g. Dark Communion gains Ritual per Burn/Poison/Bleed type).
// countOwn counts the source's own statuses instead (Crowd's Favor: Encourage per Rage/Fortify/FireShield type).
[CreateAssetMenu(fileName = "NewGainStatusPerOpponentStatusEffect", menuName = "Dice/Effects/Gain Status Per Opponent Status Effect")]
public class GainStatusPerOpponentStatusEffectSO : EffectSO
{
    public StatusType statusToGain = StatusType.Ritual;
    public List<StatusType> countedStatuses = new List<StatusType>{ StatusType.Burn, StatusType.Poison, StatusType.Bleed };
    public bool countOwn;

    public override void Execute(EffectContext context){
        int amount = Amount(context);
        amount = context.GainSelf(statusToGain, amount);
        Debug.Log(context.source.entityName + " gained " + amount + " " + statusToGain + ".");
    }

    public override string Describe(EffectContext context){
        string per = countOwn ? "per buff type you have" : "per status type on the enemy";
        if((countOwn ? context.source : context.target) == null){
            return "gain " + context.value + " " + statusToGain + " " + per;
        }
        return "gain " + Amount(context) + " " + statusToGain + " (" + context.value + " " + per + ")";
    }

    private int Amount(EffectContext context){
        int types = 0;
        Entity counted = countOwn ? context.source : context.target;
        if(counted != null){
            foreach(StatusType status in countedStatuses){
                if(context.Stacks(counted, status) > 0) types++;
            }
        }
        return context.value * Mathf.Max(1, types);
    }
}
