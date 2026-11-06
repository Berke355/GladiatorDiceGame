using UnityEngine;

// Damage that scales with the source's rerolls, as one hit:
// RerollsLeft: my value + 1 per reroll still left (saving pays, e.g. Patient Blade).
// UsedThisBattle: my value × rerolls used this battle, at least my value (spending pays, e.g. Fate's Edge).
// Without a reroll pool (enemies) it deals my value.
[CreateAssetMenu(fileName = "NewRerollScaledDamageEffect", menuName = "Dice/Effects/Reroll Scaled Damage Effect")]
public class RerollScaledDamageEffectSO : EffectSO
{
    public enum Scaling { RerollsLeft, UsedThisBattle }
    public Scaling scaling;

    public override void Execute(EffectContext context){
        int dealt = context.Hit(BaseDamage(context));
        Debug.Log(context.source.entityName + " dealt " + dealt + " damage scaled by rerolls.");
    }

    public override string Describe(EffectContext context){
        if(context.rerolls == null){
            return scaling == Scaling.RerollsLeft ? context.value + " + rerolls left damage" : context.value + " x rerolls used damage";
        }
        return (BaseDamage(context) + context.RageBonus) + " damage";
    }

    private int BaseDamage(EffectContext context){
        if(context.rerolls == null) return context.value;
        return scaling == Scaling.RerollsLeft
            ? context.value + context.rerolls.Left
            : Mathf.Max(context.value, context.value * context.rerolls.UsedThisBattle);
    }
}
