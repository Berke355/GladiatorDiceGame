using UnityEngine;

// Damage that scales with the source's rerolls, as one hit:
// RerollsLeft: my value + 1 per reroll still left (saving pays, e.g. Patient Blade).
// UsedThisBattle: my value × rerolls used this battle, at least my value (spending pays, e.g. Fate's Edge).
// UsedThisBattleAdded: my value + 1 per reroll used this battle (the Common rung of spending, e.g. Gambit).
// Without a reroll pool (enemies) it deals my value.
[CreateAssetMenu(fileName = "NewRerollScaledDamageEffect", menuName = "Dice/Effects/Reroll Scaled Damage Effect")]
public class RerollScaledDamageEffectSO : EffectSO
{
    // Assets store this as a number: add new values at the end.
    public enum Scaling { RerollsLeft, UsedThisBattle, UsedThisBattleAdded }
    public Scaling scaling;
    // The bonus per reroll for the added scalings (RerollsLeft, UsedThisBattleAdded).
    public int perReroll = 1;

    public override void Execute(EffectContext context){
        int dealt = context.Hit(BaseDamage(context));
        Debug.Log(context.source.entityName + " dealt " + dealt + " damage scaled by rerolls.");
    }

    public override string Describe(EffectContext context){
        if(context.rerolls == null){
            if(scaling == Scaling.RerollsLeft) return context.value + " + rerolls left damage";
            return scaling == Scaling.UsedThisBattle ? context.value + " x rerolls used damage" : context.value + " + rerolls used damage";
        }
        return (BaseDamage(context) + context.RageBonus) + " damage";
    }

    private int BaseDamage(EffectContext context){
        if(context.rerolls == null) return context.value;
        switch(scaling){
            case Scaling.RerollsLeft: return context.value + perReroll * context.rerolls.Left;
            case Scaling.UsedThisBattle: return Mathf.Max(context.value, context.value * context.rerolls.UsedThisBattle);
            default: return context.value + perReroll * context.rerolls.UsedThisBattle;
        }
    }
}
