using UnityEngine;

[CreateAssetMenu(fileName = "NewApplyStatusEffect", menuName = "Dice/Effects/Apply Status Effect")]
public class ApplyStatusEffectSO : EffectSO
{
    public StatusType statusToApply;
    // Applies the status to the source instead of the target (e.g. FireShield on yourself).
    public bool applyToSelf;

    public override void Execute(Entity source, Entity target, int value)
    {
        Entity receiver = applyToSelf ? source : target;
        receiver.ApplyStatus(statusToApply, value);
        Debug.Log(source.entityName + " applied " + value + " " + statusToApply + " to " + receiver.entityName + ".");
    }
}
