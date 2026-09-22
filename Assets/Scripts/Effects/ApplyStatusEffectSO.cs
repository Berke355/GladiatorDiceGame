using UnityEngine;

[CreateAssetMenu(fileName = "NewApplyStatusEffect", menuName = "Dice/Effects/Apply Status Effect")]
public class ApplyStatusEffectSO : EffectSO
{
    public StatusType statusToApply;

    public override void Execute(Entity source, Entity target, int value)
    {
        target.ApplyStatus(statusToApply, value);
        Debug.Log(source.entityName + " applied " + value + " " + statusToApply + " to " + target.entityName + ".");
    }
}
