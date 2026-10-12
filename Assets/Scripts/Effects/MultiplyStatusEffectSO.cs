using UnityEngine;

[CreateAssetMenu(fileName = "NewMultiplyStatusEffect", menuName = "Dice/Effects/Multiply Status Effect")]
public class MultiplyStatusEffectSO : EffectSO
{
    public StatusType statusToMultiply;

    public override void Execute(EffectContext context)
    {
        int currentStatus = context.target.GetStatus(statusToMultiply);
        if (currentStatus > 0)
        {
            // value is the multiplier (e.g. 2 for Fuel)
            int amountToAdd = (currentStatus * context.value) - currentStatus;
            context.target.ApplyStatus(statusToMultiply, amountToAdd);
            Debug.Log(context.source.entityName + " multiplied " + context.target.entityName + "'s " + statusToMultiply + " by " + context.value + ".");
        }
    }
}
