using UnityEngine;

[CreateAssetMenu(fileName = "NewApplyStatusEffect", menuName = "Dice/Effects/Apply Status Effect")]
public class ApplyStatusEffectSO : EffectSO
{
    public StatusType statusToApply;
    // Applies the status to the source instead of the target (e.g. FireShield on yourself).
    public bool applyToSelf;

    public override void Execute(EffectContext context)
    {
        Entity receiver = applyToSelf ? context.source : context.target;
        receiver.ApplyStatus(statusToApply, context.value);
        Debug.Log(context.source.entityName + " applied " + context.value + " " + statusToApply + " to " + receiver.entityName + ".");
    }
}
