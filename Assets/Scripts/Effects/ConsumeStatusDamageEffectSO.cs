using UnityEngine;

// Consumes one of the target's statuses and deals value + the consumed stacks as damage (e.g. Detonate).
[CreateAssetMenu(fileName = "NewConsumeStatusDamageEffect", menuName = "Dice/Effects/Consume Status Damage Effect")]
public class ConsumeStatusDamageEffectSO : EffectSO
{
    public StatusType statusToConsume;

    public override void Execute(EffectContext context)
    {
        int consumedAmount = context.target.GetStatus(statusToConsume);
        context.target.ConsumeStatus(statusToConsume);
        int dealt = context.Hit(context.value + consumedAmount);

        Debug.Log(context.source.entityName + " consumed " + consumedAmount + " " + statusToConsume + " and dealt " + dealt + " damage.");
    }

    public override string Describe(EffectContext context){
        if(context.target == null){
            return context.value + " + " + statusToConsume + " damage, consume " + statusToConsume;
        }
        int damage = context.value + context.target.GetStatus(statusToConsume) + context.RageBonus;
        return damage + " damage, consume " + statusToConsume;
    }
}
