using UnityEngine;

// Deals the source's highest-value other face's battle value times my value as damage (e.g. Earthshaker).
// That face's Charge is not used up. Without a die (enemies), it deals at least my value.
[CreateAssetMenu(fileName = "NewHighestFaceDamageEffect", menuName = "Dice/Effects/Highest Face Damage Effect")]
public class HighestFaceDamageEffectSO : EffectSO
{
    public override void Execute(EffectContext context){
        int dealt = context.Hit(BaseDamage(context));
        Debug.Log(context.source.entityName + " shook the earth for " + dealt + " damage.");
    }

    public override string Describe(EffectContext context){
        if(context.die == null){
            return context.value + " x your highest face damage";
        }
        return (BaseDamage(context) + context.RageBonus) + " damage";
    }

    private int BaseDamage(EffectContext context){
        if(context.die == null) return context.value;
        DieFace highest = HighestOther(context);
        int faceValue = highest != null ? context.ValueOf(highest) : 1;
        return Mathf.Max(context.value, faceValue * context.value);
    }

    private static DieFace HighestOther(EffectContext context){
        DieFace best = null;
        foreach(DieFace face in context.die){
            if(face == context.face) continue;
            if(best == null || context.ValueOf(face) > context.ValueOf(best)) best = face;
        }
        return best;
    }
}
