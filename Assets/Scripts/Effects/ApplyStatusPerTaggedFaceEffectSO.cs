using UnityEngine;

// Die-composition scaling (GDD "Build Archetypes"): applies value x (faces on the source's die with the tag).
// The triggering face counts itself, so alone it still applies its value. Enemies have no die and count as 1.
[CreateAssetMenu(fileName = "NewApplyStatusPerTaggedFaceEffect", menuName = "Dice/Effects/Apply Status Per Tagged Face Effect")]
public class ApplyStatusPerTaggedFaceEffectSO : EffectSO
{
    public StatusType statusToApply;
    public FaceTag tagToCount;

    public override void Execute(EffectContext context)
    {
        int taggedFaces = CountTaggedFaces(context);
        int amount = context.ApplyToTarget(statusToApply, context.value * taggedFaces);
        Debug.Log(context.source.entityName + " applied " + amount + " " + statusToApply + " (" + context.value + " x " + taggedFaces + " " + tagToCount + " faces).");
    }

    public override string Describe(EffectContext context){
        int amount = context.value * CountTaggedFaces(context) + context.RitualBonus;
        return amount + " " + statusToApply + " (" + context.value + " per " + tagToCount + " face)";
    }

    private int CountTaggedFaces(EffectContext context){
        if(context.die == null){
            return 1;
        }
        int taggedFaces = 0;
        foreach(DieFace face in context.die){
            if(face.definition.tags.Contains(tagToCount)){
                taggedFaces++;
            }
        }
        return taggedFaces;
    }
}
