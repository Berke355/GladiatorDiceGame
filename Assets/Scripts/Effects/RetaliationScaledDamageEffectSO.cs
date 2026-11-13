using UnityEngine;

// Deals value + the number of times Riposte, Vigil or Reversal struck back this battle (Grudge): the Provocator's
// Common that grows during a fight.
[CreateAssetMenu(fileName = "NewRetaliationScaledDamageEffect", menuName = "Dice/Effects/Retaliation Scaled Damage Effect")]
public class RetaliationScaledDamageEffectSO : EffectSO
{
    public override void Execute(EffectContext context){
        int dealt = context.Hit(context.value + context.retaliations);
        Debug.Log(context.source.entityName + " struck with a grudge for " + dealt + " damage.");
    }

    public override string Describe(EffectContext context){
        return context.PreviewHit(context.value + context.retaliations) + " damage";
    }
}
