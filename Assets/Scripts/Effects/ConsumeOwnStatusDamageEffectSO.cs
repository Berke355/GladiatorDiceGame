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
        int totalDamage = context.value + consumedAmount * context.value;

        context.source.ConsumeStatus(statusToConsume);
        context.target.TakeDamage(totalDamage, context.source);

        Debug.Log(context.source.entityName + " consumed " + consumedAmount + " of its own " + statusToConsume + " and dealt " + totalDamage + " damage.");
    }
}
