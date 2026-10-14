using UnityEngine;

// Consumes one of the target's statuses and heals the source value x the consumed stacks (e.g. Cauterize).
[CreateAssetMenu(fileName = "NewConsumeStatusHealEffect", menuName = "Dice/Effects/Consume Status Heal Effect")]
public class ConsumeStatusHealEffectSO : EffectSO
{
    public StatusType statusToConsume;

    public override void Execute(EffectContext context)
    {
        int consumedAmount = context.target.GetStatus(statusToConsume);
        int totalHeal = context.value * consumedAmount;

        context.target.ConsumeStatus(statusToConsume);
        context.source.Heal(totalHeal);

        Debug.Log(context.source.entityName + " consumed " + consumedAmount + " " + statusToConsume + " and healed for " + totalHeal + " HP.");
    }

    public override string Describe(EffectContext context){
        if(context.target == null){
            return "consume " + statusToConsume + ", heal " + context.value + " per stack";
        }
        return "consume " + statusToConsume + ", heal " + (context.value * context.target.GetStatus(statusToConsume));
    }
}
