using UnityEngine;
using System.Collections.Generic;

// Follow Through: hits the target for my value; if the hit kills it, the damage it didn't need (beyond its HP,
// Block and Fortify) carries over to the next living enemy in turn order. The carried damage already includes
// Rage and Weak, so it lands through TakeDamage instead of a second context.Hit (no Rage twice); it is still a
// hit from the source, so hit triggers fire.
[CreateAssetMenu(fileName = "NewOverkillDamageEffect", menuName = "Dice/Effects/Overkill Damage Effect")]
public class OverkillDamageEffectSO : EffectSO
{
    public override void Execute(EffectContext context){
        Entity target = context.target;
        int absorbable = target.currentHP + target.currentBlock + target.GetStatus(StatusType.Fortify);
        int dealt = context.Hit(context.value);
        if(target.GetStatus(StatusType.Vulnerable) > 0) dealt += 1;
        if(!target.IsDead) return;
        int excess = dealt - absorbable;
        Entity next = NextOpponent(context.AllOpponents, target);
        if(excess <= 0 || next == null) return;
        next.TakeDamage(excess, context.source);
        Debug.Log(context.source.entityName + "'s Follow Through carried " + excess + " damage to " + next.entityName + ".");
    }

    public override string Describe(EffectContext context){
        return context.PreviewHit(context.value) + " damage, the excess carries to the next enemy";
    }

    // The first living opponent after the target in turn order, wrapping around.
    private static Entity NextOpponent(List<Entity> opponents, Entity target){
        int start = opponents.IndexOf(target);
        for(int i = 1; i <= opponents.Count; i++){
            Entity candidate = opponents[(start + i + opponents.Count) % opponents.Count];
            if(candidate != target && !candidate.IsDead) return candidate;
        }
        return null;
    }
}
