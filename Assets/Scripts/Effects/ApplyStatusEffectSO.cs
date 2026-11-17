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
    // Ritual doesn't add to it (a cost that buffs the enemy, Challenger's Mark).
    public bool ignoreRitual;

    public override void Execute(EffectContext context)
    {
        int amount = Amount(context.value);
        if(applyToSelf){
            amount = context.GainSelf(statusToApply, amount);
        }
        else if(ignoreRitual){
            context.target.ApplyStatus(statusToApply, amount);
        }
        else{
            amount = context.ApplyToTarget(statusToApply, amount);
        }
        Debug.Log(context.source.entityName + " applied " + amount + " " + statusToApply + (applyToSelf ? " to itself." : "."));
    }

    public override string Describe(EffectContext context){
        int amount = Amount(context.value) + (applyToSelf ? (EffectContext.IsEncouraged(statusToApply) ? context.EncourageBonus : 0) : ignoreRitual ? 0 : context.RitualBonus);
        context.NotePreviewGain(applyToSelf ? context.source : context.target, statusToApply, amount);
        return (applyToSelf ? "gain " : "") + amount + " " + statusToApply;
    }

    private int Amount(int value){
        return fixedAmount > 0 ? fixedAmount : value;
    }

    // A beast or a stance on itself does nothing for an enemy (the Mimic copying Whistle or a Deflect face plays its
    // value as a hit instead).
    public override bool WorksForEnemies => !(applyToSelf && (statusToApply == StatusType.Hound || statusToApply == StatusType.Deflect || statusToApply == StatusType.PerfectGuard));
}
