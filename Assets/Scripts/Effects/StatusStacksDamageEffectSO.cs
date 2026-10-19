using UnityEngine;

// Deals damage from a status's stacks without consuming them, so the hit grows as the status builds up.
// Additive (value + stacks, e.g. Lacerate) or with the value as a minimum (stacks, at least value, e.g. Spiked Shield).
[CreateAssetMenu(fileName = "NewStatusStacksDamageEffect", menuName = "Dice/Effects/Status Stacks Damage Effect")]
public class StatusStacksDamageEffectSO : EffectSO
{
    public StatusType status;
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
            return addValue ? context.value + " + " + status + " damage" : status + " damage (at least " + context.value + ")";
        }
        return (BaseDamage(context) + context.RageBonus) + " damage";
    }

    private int BaseDamage(EffectContext context){
        Entity counted = countOnTarget ? context.target : context.source;
        int stacks = counted != null ? counted.GetStatus(status) : 0;
        return addValue ? context.value + stacks : Mathf.Max(context.value, stacks);
    }
}
