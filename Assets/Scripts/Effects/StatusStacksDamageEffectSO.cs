using UnityEngine;

// Deals damage from a status's stacks without consuming them, so the hit grows as the status builds up.
// Additive (value + stacks, e.g. Lacerate) or with the value as a minimum (stacks, at least value, e.g. Spiked Shield).
[CreateAssetMenu(fileName = "NewStatusStacksDamageEffect", menuName = "Dice/Effects/Status Stacks Damage Effect")]
public class StatusStacksDamageEffectSO : EffectSO
{
    public StatusType status;
    // A second status whose stacks count too (None: only the first), e.g. Harpoon reads Vulnerable and Weak.
    public StatusType alsoStatus = StatusType.None;
    // On: count the target's stacks (the enemy's Bleed). Off: count the source's own (your Fortify).
    public bool countOnTarget = true;
    // On: value + stacks. Off: stacks, but at least value.
    public bool addValue = true;

    public override void Execute(EffectContext context)
    {
        int dealt = context.Hit(BaseDamage(context));
        Debug.Log(context.source.entityName + " hit with " + status + " and dealt " + dealt + " damage.");
    }

    public override string Describe(EffectContext context){
        Entity counted = countOnTarget ? context.target : context.source;
        if(counted == null){
            string stacks = alsoStatus != StatusType.None ? status + " + " + alsoStatus : status.ToString();
            return addValue ? context.value + " + " + stacks + " damage" : stacks + " damage (at least " + context.value + ")";
        }
        return context.PreviewHit(BaseDamage(context)) + " damage";
    }

    private int BaseDamage(EffectContext context){
        Entity counted = countOnTarget ? context.target : context.source;
        int stacks = 0;
        if(counted != null){
            stacks = context.Stacks(counted, status);
            if(alsoStatus != StatusType.None) stacks += context.Stacks(counted, alsoStatus);
        }
        return addValue ? context.value + stacks : Mathf.Max(context.value, stacks);
    }
}
