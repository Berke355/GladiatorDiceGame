using UnityEngine;

// Block that grows with the rerolls the source still has: my value + 1 per reroll left (Hedge, the defensive twin of
// Patient Blade: saving rerolls also keeps you alive). Without a reroll pool (enemies) it gives my value.
[CreateAssetMenu(fileName = "NewRerollScaledBlockEffect", menuName = "Dice/Effects/Reroll Scaled Block Effect")]
public class RerollScaledBlockEffectSO : EffectSO
{
    public override void Execute(EffectContext context){
        int block = Amount(context);
        context.source.GainBlock(block);
        Debug.Log(context.source.entityName + " gained " + block + " block scaled by rerolls.");
    }

    public override string Describe(EffectContext context){
        return context.rerolls == null ? context.value + " + rerolls left block" : Amount(context) + " block";
    }

    private static int Amount(EffectContext context){
        return context.value + (context.rerolls != null ? context.rerolls.Left : 0);
    }
}
