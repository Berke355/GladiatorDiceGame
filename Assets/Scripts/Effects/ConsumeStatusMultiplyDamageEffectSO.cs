using UnityEngine;

// Consumes one of the target's statuses and deals value x the consumed stacks as one hit (at least the value),
// e.g. Exsanguinate cashing in Bleed's remaining duration. Multiplicative, so it belongs on Epic faces.
// Compare ConsumeStatusDamageEffectSO (Detonate), which is additive: value + stacks.
[CreateAssetMenu(fileName = "NewConsumeStatusMultiplyDamageEffect", menuName = "Dice/Effects/Consume Status Multiply Damage Effect")]
public class ConsumeStatusMultiplyDamageEffectSO : EffectSO
{
    public StatusType statusToConsume = StatusType.Bleed;

    public override void Execute(EffectContext context){
        int stacks = context.target.GetStatus(statusToConsume);
        context.target.ConsumeStatus(statusToConsume);
        int dealt = context.Hit(context.value * Mathf.Max(1, stacks));
        Debug.Log(context.source.entityName + " consumed " + stacks + " " + statusToConsume + " for " + dealt + " damage.");
    }

    public override string Describe(EffectContext context){
        if(context.target == null){
            return context.value + " x " + statusToConsume + " damage, consume " + statusToConsume;
        }
        int damage = context.value * Mathf.Max(1, context.target.GetStatus(statusToConsume)) + context.RageBonus;
        return damage + " damage, consume " + statusToConsume;
    }
}
