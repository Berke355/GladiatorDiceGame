using UnityEngine;

// The source gains rerolls for this battle (e.g. Loaded Die, Lucky Charm). Does nothing without a reroll pool (enemies).
[CreateAssetMenu(fileName = "NewGainRerollEffect", menuName = "Dice/Effects/Gain Reroll Effect")]
public class GainRerollEffectSO : EffectSO
{
    // > 0: always this many, instead of my value.
    public int fixedAmount;

    public override void Execute(EffectContext context){
        if(context.rerolls == null) return;
        context.rerolls.Left += Amount(context);
    }

    public override string Describe(EffectContext context){
        int amount = Amount(context);
        return "gain " + amount + (amount == 1 ? " reroll" : " rerolls");
    }

    private int Amount(EffectContext context){
        return fixedAmount > 0 ? fixedAmount : context.value;
    }
}
