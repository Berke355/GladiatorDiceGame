using UnityEngine;

// Deals value damage several times. Each hit is separate, so each one wears down block
// and triggers FireShield on its own (e.g. Dimachaerus' Twin Strike).
[CreateAssetMenu(fileName = "NewMultiHitDamageEffect", menuName = "Dice/Effects/Multi Hit Damage Effect")]
public class MultiHitDamageEffectSO : EffectSO
{
    public int hits = 2;

    public override void Execute(EffectContext context){
        for(int i = 0; i < hits; i++){
            context.target.TakeDamage(context.value, context.source);
        }
        Debug.Log(context.source.entityName + " hit " + hits + " times for " + context.value + " damage each.");
    }
}
