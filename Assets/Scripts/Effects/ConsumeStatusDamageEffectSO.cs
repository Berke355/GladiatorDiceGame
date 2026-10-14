using UnityEngine;

[CreateAssetMenu(fileName = "NewConsumeStatusDamageEffect", menuName = "Dice/Effects/Consume Status Damage Effect")]
public class ConsumeStatusDamageEffectSO : EffectSO
{
    public StatusType statusToConsume;

    public override void Execute(EffectContext context)
    {
        int consumedAmount = context.target.GetStatus(statusToConsume);
        int totalDamage = context.value + consumedAmount;

        context.target.ConsumeStatus(statusToConsume);
        context.target.TakeDamage(totalDamage, context.source);

        Debug.Log(context.source.entityName + " consumed " + consumedAmount + " " + statusToConsume + " from " + context.target.entityName + " and dealt " + totalDamage + " damage.");
    }

    public override string Describe(int value){
        return value + " + " + statusToConsume + " damage, consume " + statusToConsume;
    }
}
