using UnityEngine;

[CreateAssetMenu(fileName = "NewApplyStatusEffect", menuName = "Dice/Effects/Apply Status Effect")]
public class ApplyStatusEffectSO : EffectSO
{
    public StatusType statusToApply;
    // Applies the status to the source instead of the target (e.g. FireShield on yourself).
    // Ritual only boosts statuses applied to the opponent, never these.
    public bool applyToSelf;
    // When above 0, this exact amount is applied instead of the value (e.g. Sword's 1 Bleed).
    public int fixedAmount;

    public override void Execute(EffectContext context)
    {
        int amount = Amount(context.value);
        if(applyToSelf){
            context.source.ApplyStatus(statusToApply, amount);
        }
        else{
            amount = context.ApplyToTarget(statusToApply, amount);
        }
        Debug.Log(context.source.entityName + " applied " + amount + " " + statusToApply + (applyToSelf ? " to itself." : "."));
    }

    public override string Describe(EffectContext context){
        int amount = Amount(context.value) + (applyToSelf ? 0 : context.RitualBonus);
        return (applyToSelf ? "gain " : "") + amount + " " + statusToApply;
    }

    private int Amount(int value){
        return fixedAmount > 0 ? fixedAmount : value;
    }
}
