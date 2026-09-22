using UnityEngine;

[CreateAssetMenu(fileName = "NewConsumeStatusDamageEffect", menuName = "Dice/Effects/Consume Status Damage Effect")]
public class ConsumeStatusDamageEffectSO : EffectSO
{
    public StatusType statusToConsume;

    public override void Execute(Entity source, Entity target, int value)
    {
        int consumedAmount = target.GetStatus(statusToConsume);
        int totalDamage = value + consumedAmount;

        target.ConsumeStatus(statusToConsume);
        target.TakeDamage(totalDamage, source);

        Debug.Log(source.entityName + " consumed " + consumedAmount + " " + statusToConsume + " from " + target.entityName + " and dealt " + totalDamage + " damage.");
    }
}
