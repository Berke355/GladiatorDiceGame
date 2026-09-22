using UnityEngine;

[CreateAssetMenu(fileName = "NewMultiplyStatusEffect", menuName = "Dice/Effects/Multiply Status Effect")]
public class MultiplyStatusEffectSO : EffectSO
{
    public StatusType statusToMultiply;

    public override void Execute(Entity source, Entity target, int value)
    {
        int currentStatus = target.GetStatus(statusToMultiply);
        if (currentStatus > 0)
        {
            // value is the multiplier (e.g. 2 for Fuel)
            int amountToAdd = (currentStatus * value) - currentStatus;
            target.ApplyStatus(statusToMultiply, amountToAdd);
            Debug.Log(source.entityName + " multiplied " + target.entityName + "'s " + statusToMultiply + " by " + value + ".");
        }
    }
}
