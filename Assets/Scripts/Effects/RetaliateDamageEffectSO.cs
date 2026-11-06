using UnityEngine;

// Deals my value to the target as a plain hit without Rage, and without a source, so it can't set off FireShield or
// hit-triggered reactions in a loop (e.g. Thorns, Parry). Block still absorbs it.
[CreateAssetMenu(fileName = "NewRetaliateDamageEffect", menuName = "Dice/Effects/Retaliate Damage Effect")]
public class RetaliateDamageEffectSO : EffectSO
{
    public override void Execute(EffectContext context){
        context.target.TakeDamage(context.value);
    }

    public override string Describe(EffectContext context){
        return context.value + " damage to you";
    }
}
