using UnityEngine;

// The lowest-value face on the source's die (the first one on ties) gains value for this battle (Drill Banner). It helps a
// patchwork die's weak face instead of a stacked one. Does nothing without a die.
[CreateAssetMenu(fileName = "NewAddValueToLowestFaceEffect", menuName = "Dice/Effects/Add Value To Lowest Face Effect")]
public class AddValueToLowestFaceEffectSO : EffectSO
{
    public override void Execute(EffectContext context){
        if(context.die == null || context.charge == null || context.die.Count == 0) return;
        DieFace lowest = context.die[0];
        foreach(DieFace face in context.die){
            if(context.charge.ValueOf(face) < context.charge.ValueOf(lowest)) lowest = face;
        }
        context.charge.AddBattleBonus(lowest, context.value);
    }

    public override string Describe(EffectContext context){
        return "+" + context.value + " value to your lowest face";
    }
}
