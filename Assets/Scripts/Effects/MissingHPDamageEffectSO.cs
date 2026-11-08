using UnityEngine;

// Deals the value plus 1 for every few HP the source is missing (Last Stand): a comeback hit that also rewards faces and
// items that spend HP.
[CreateAssetMenu(fileName = "NewMissingHPDamageEffect", menuName = "Dice/Effects/Missing HP Damage Effect")]
public class MissingHPDamageEffectSO : EffectSO
{
    public int missingHPPerBonus = 5;

    public override void Execute(EffectContext context){
        context.Hit(Amount(context));
    }

    public override string Describe(EffectContext context){
        if(context.source == null) return context.value + " damage, +1 per " + missingHPPerBonus + " missing HP";
        return (Amount(context) + context.RageBonus) + " damage";
    }

    private int Amount(EffectContext context){
        int missing = context.source.maxHP - context.source.currentHP;
        return context.value + Mathf.Max(0, missing) / missingHPPerBonus;
    }
}
