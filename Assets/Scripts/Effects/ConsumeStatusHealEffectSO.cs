using UnityEngine;

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

        Debug.Log(context.source.entityName + " consumed " + consumedAmount + " " + statusToConsume + " from " + context.target.entityName + " and healed for " + totalHeal + " HP.");
    }

    public override string Describe(int value){
        return "consume " + statusToConsume + ", heal " + value + " per stack";
    }
}
