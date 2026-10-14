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
        int taggedFaces = 1;
        if(context.die != null){
            taggedFaces = 0;
            foreach(DieFace face in context.die){
                if(face.definition.tags.Contains(tagToCount)){
                    taggedFaces++;
                }
            }
        }

        int amount = context.value * taggedFaces;
        context.target.ApplyStatus(statusToApply, amount);
        Debug.Log(context.source.entityName + " applied " + amount + " " + statusToApply + " (" + context.value + " x " + taggedFaces + " " + tagToCount + " faces) to " + context.target.entityName + ".");
    }

    public override string Describe(int value){
        return value + " " + statusToApply + " per " + tagToCount + " face";
    }
}
