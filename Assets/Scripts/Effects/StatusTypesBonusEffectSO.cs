using UnityEngine;
using System.Collections.Generic;

// Deals damage (or gains block) equal to the value plus how many of the counted status types are present, on the
// target or on the source. It reads a state many builds produce, so it fits many dice (Exploit, Veteran).
[CreateAssetMenu(fileName = "NewStatusTypesBonusEffect", menuName = "Dice/Effects/Status Types Bonus Effect")]
public class StatusTypesBonusEffectSO : EffectSO
{
    public List<StatusType> countedStatuses = new List<StatusType>();
    // Counts the source's own statuses instead of the target's.
    public bool countOwn;
    // Gains block instead of dealing damage.
    public bool block;

    public override void Execute(EffectContext context){
        int amount = context.value + Types(context);
        if(block) context.source.GainBlock(amount);
        else context.Hit(amount);
    }

    public override string Describe(EffectContext context){
        int amount = context.value + Types(context);
        return block ? amount + " block" : (amount + context.RageBonus) + " damage";
    }

    private int Types(EffectContext context){
        Entity counted = countOwn ? context.source : context.target;
        if(counted == null) return 0;
        int types = 0;
        foreach(StatusType status in countedStatuses){
            if(context.Stacks(counted, status) > 0) types++;
        }
        return types;
    }
}
