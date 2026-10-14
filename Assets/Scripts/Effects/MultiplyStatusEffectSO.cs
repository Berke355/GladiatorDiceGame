using UnityEngine;

// Multiplies one of the target's statuses by the value (e.g. Fuel). Not a new application, so Ritual does not apply.
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

    public override string Describe(EffectContext context){
        return statusToMultiply + " x" + context.value;
    }
}
