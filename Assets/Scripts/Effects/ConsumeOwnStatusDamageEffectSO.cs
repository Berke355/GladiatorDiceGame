using UnityEngine;

// Consumes all of one of the source's own statuses and deals value + stacks x value damage (e.g. Shield Burst).
// With no stacks it still deals its value, so the face works on its own.
[CreateAssetMenu(fileName = "NewConsumeOwnStatusDamageEffect", menuName = "Dice/Effects/Consume Own Status Damage Effect")]
public class ConsumeOwnStatusDamageEffectSO : EffectSO
{
    public StatusType statusToConsume;

    public override void Execute(EffectContext context)
    {
        int consumedAmount = context.source.GetStatus(statusToConsume);
        context.source.ConsumeStatus(statusToConsume);
        int dealt = context.Hit(context.value + consumedAmount * context.value);

        Debug.Log(context.source.entityName + " consumed " + consumedAmount + " of its own " + statusToConsume + " and dealt " + dealt + " damage.");
    }

    public override string Describe(EffectContext context){
        if(context.source == null){
            return context.value + " + " + statusToConsume + " x" + context.value + " damage, consume " + statusToConsume;
        }
        int damage = context.value + context.source.GetStatus(statusToConsume) * context.value + context.RageBonus;
        return damage + " damage, consume " + statusToConsume;
    }
}
