using UnityEngine;

// Deals value damage several times. Each hit is separate, so each one gets Rage, wears down block
// and triggers FireShield on its own (e.g. Dimachaerus' Twin Strike).
[CreateAssetMenu(fileName = "NewMultiHitDamageEffect", menuName = "Dice/Effects/Multi Hit Damage Effect")]
public class MultiHitDamageEffectSO : EffectSO
{
    public int hits = 2;

    public override void Execute(EffectContext context){
        int perHit = 0;
        for(int i = 0; i < hits; i++){
            perHit = context.Hit(context.value);
        }
        Debug.Log(context.source.entityName + " hit " + hits + " times for " + perHit + " damage each.");
    }

    public override string Describe(EffectContext context){
        return hits + " x " + (context.value + context.RageBonus) + " damage";
    }
}
