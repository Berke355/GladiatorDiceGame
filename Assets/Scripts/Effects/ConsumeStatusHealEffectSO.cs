using UnityEngine;

[CreateAssetMenu(fileName = "NewConsumeStatusHealEffect", menuName = "Dice/Effects/Consume Status Heal Effect")]
public class ConsumeStatusHealEffectSO : EffectSO
{
    public StatusType statusToConsume;

    public override void Execute(Entity source, Entity target, int value)
    {
        int consumedAmount = target.GetStatus(statusToConsume);
        int totalHeal = value * consumedAmount;

        target.ConsumeStatus(statusToConsume);
        source.Heal(totalHeal);

        Debug.Log(source.entityName + " consumed " + consumedAmount + " " + statusToConsume + " from " + target.entityName + " and healed for " + totalHeal + " HP.");
    }
}
