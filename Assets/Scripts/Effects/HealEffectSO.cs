using UnityEngine;

[CreateAssetMenu(fileName = "NewHealEffect", menuName = "Dice/Effects/Heal Effect")]
public class HealEffectSO : EffectSO
{
    // When above 0, this exact amount is healed instead of the value (Bite's Attack).
    public int fixedAmount;

    public override void Execute(EffectContext context){
        context.source.Heal(Amount(context));
    }

    public override string Describe(EffectContext context){
        return "heal " + Amount(context);
    }

    private int Amount(EffectContext context){
        return fixedAmount > 0 ? fixedAmount : context.value;
    }
}
