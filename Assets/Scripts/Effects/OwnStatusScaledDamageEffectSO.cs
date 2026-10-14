using UnityEngine;

// Deals value x the source's own stacks of a status as one hit (at least the value), e.g. Unleash with Rage.
// The hit also gets Rage through EffectContext.Hit, so with Rage this grows fast: it is an Epic payoff.
[CreateAssetMenu(fileName = "NewOwnStatusScaledDamageEffect", menuName = "Dice/Effects/Own Status Scaled Damage Effect")]
public class OwnStatusScaledDamageEffectSO : EffectSO
{
    public StatusType statusToScaleWith = StatusType.Rage;

    public override void Execute(EffectContext context){
        int dealt = context.Hit(BaseDamage(context));
        Debug.Log(context.source.entityName + " unleashed " + dealt + " damage.");
    }

    public override string Describe(EffectContext context){
        if(context.source == null){
            return context.value + " x " + statusToScaleWith + " damage";
        }
        return (BaseDamage(context) + context.RageBonus) + " damage";
    }

    private int BaseDamage(EffectContext context){
        int stacks = context.source != null ? context.source.GetStatus(statusToScaleWith) : 0;
        return context.value * Mathf.Max(1, stacks);
    }
}
