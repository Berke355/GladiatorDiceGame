using UnityEngine;

[CreateAssetMenu(fileName = "NewApplyStatusEffect", menuName = "Dice/Effects/Apply Status Effect")]
public class ApplyStatusEffectSO : EffectSO
{
    public StatusType statusToApply;
    // Applies the status to the source instead of the target (e.g. FireShield on yourself).
    public bool applyToSelf;
    // When above 0, this exact amount is applied instead of the value (e.g. Sword's 1 Bleed).
    public int fixedAmount;

    public override void Execute(EffectContext context)
    {
        Entity receiver = applyToSelf ? context.source : context.target;
        int amount = Amount(context.value);
        receiver.ApplyStatus(statusToApply, amount);
        Debug.Log(context.source.entityName + " applied " + amount + " " + statusToApply + " to " + receiver.entityName + ".");
    }

    public override string Describe(int value){
        return Amount(value) + " " + statusToApply;
    }

    private int Amount(int value){
        return fixedAmount > 0 ? fixedAmount : value;
    }
}
