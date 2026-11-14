using System.Collections.Generic;
using UnityEngine;

// The source removes half (rounded up) of each listed status on itself (Venefica's Antidote: Poison, Burn, Bleed).
[CreateAssetMenu(fileName = "NewCleanseSelfEffect", menuName = "Dice/Effects/Cleanse Self Effect")]
public class CleanseSelfEffectSO : EffectSO
{
    public List<StatusType> statuses = new List<StatusType>();

    public override void Execute(EffectContext context){
        foreach(StatusType status in statuses){
            int stacks = context.source.GetStatus(status);
            context.source.ReduceStatus(status, (stacks + 1) / 2);
        }
    }

    public override string Describe(EffectContext context){
        return "cleanses half of the " + string.Join(", ", statuses) + " on itself";
    }
}
