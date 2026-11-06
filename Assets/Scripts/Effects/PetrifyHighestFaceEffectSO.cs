using UnityEngine;

// Petrifies the target's highest-value face that isn't stone yet, by battle value, for my value in turns
// (e.g. Medusa's Gaze, value 2). A target locked in the intent (context.targetFace) is used instead, so the shown
// target is the one that turns to stone. Does nothing if the target has no die (effects stay source/target-agnostic).
[CreateAssetMenu(fileName = "NewPetrifyHighestFaceEffect", menuName = "Dice/Effects/Petrify Highest Face Effect")]
public class PetrifyHighestFaceEffectSO : EffectSO
{
    public override void Execute(EffectContext context){
        if(context.targetPetrification == null) return;
        DieFace face = Target(context);
        if(face == null) return;
        context.targetPetrification.Petrify(face, context.value);
        Debug.Log(context.source.entityName + " petrified " + face.definition.faceName + " for " + context.value + " turns.");
    }

    public override string Describe(EffectContext context){
        if(context.targetPetrification == null || context.targetDie == null){
            return "petrify your highest face for " + context.value + " turns";
        }
        DieFace face = Target(context);
        return face != null ? "petrify " + face.definition.faceName + " for " + context.value + " turns" : "";
    }

    // The face this petrifies now: the locked target, or else the highest-value face that isn't stone.
    public static DieFace Target(EffectContext context){
        if(context.targetFace != null) return context.targetFace;
        return context.targetPetrification != null ? context.targetPetrification.HighestNotStone(context.targetDie, context.targetCharge) : null;
    }
}
