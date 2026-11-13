using System.Collections.Generic;
using UnityEngine;

// Hits once for every stack of the listed statuses on the target, each hit for the value (Haul In: one hit per
// Vulnerable and Weak). Multiplicative, so it belongs on Epics. Each hit is separate, so each one gets Rage and the
// target's Vulnerable. The stacks are counted when it runs, after the action's earlier effects (its own fuel).
[CreateAssetMenu(fileName = "NewMultiHitPerStatusEffect", menuName = "Dice/Effects/Multi Hit Per Status Effect")]
public class MultiHitPerStatusEffectSO : EffectSO
{
    public List<StatusType> statuses = new List<StatusType>();
    // Hits even when the target has none of the statuses.
    public int minHits = 1;

    public override void Execute(EffectContext context){
        int hits = Hits(context);
        int perHit = 0;
        for(int i = 0; i < hits && !context.target.IsDead; i++){
            perHit = context.Hit(context.value);
        }
        Debug.Log(context.source.entityName + " hit " + hits + " times for " + perHit + " damage each.");
    }

    public override string Describe(EffectContext context){
        if(context.target == null) return context.value + " damage per " + string.Join(" and ", statuses) + " stack";
        return Hits(context) + " x " + context.PreviewHit(context.value) + " damage";
    }

    private int Hits(EffectContext context){
        int stacks = 0;
        if(context.target != null){
            foreach(StatusType status in statuses) stacks += context.Stacks(context.target, status);
        }
        return Mathf.Max(minHits, stacks);
    }
}
