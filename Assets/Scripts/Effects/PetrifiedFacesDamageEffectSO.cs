using UnityEngine;

// Deals my value plus a bonus for each stone face on the target's die as one hit (e.g. Medusa's Coil).
[CreateAssetMenu(fileName = "NewPetrifiedFacesDamageEffect", menuName = "Dice/Effects/Petrified Faces Damage Effect")]
public class PetrifiedFacesDamageEffectSO : EffectSO
{
    public int bonusPerStoneFace = 2;

    public override void Execute(EffectContext context){
        int dealt = context.Hit(BaseDamage(context));
        Debug.Log(context.source.entityName + " coiled for " + dealt + " damage.");
    }

    public override string Describe(EffectContext context){
        return (BaseDamage(context) + context.RageBonus) + " damage";
    }

    public int BaseDamage(EffectContext context){
        int stone = context.targetPetrification != null ? context.targetPetrification.CountStone(context.targetDie) : 0;
        return context.value + bonusPerStoneFace * stone;
    }
}
