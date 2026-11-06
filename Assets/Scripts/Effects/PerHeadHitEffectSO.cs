using UnityEngine;

// Hits once for my value per head the source has (e.g. the Hydra's Bite); Rage counts on every hit.
// A source without heads (context.heads 0) hits once.
[CreateAssetMenu(fileName = "NewPerHeadHitEffect", menuName = "Dice/Effects/Per Head Hit Effect")]
public class PerHeadHitEffectSO : EffectSO
{
    public override void Execute(EffectContext context){
        int hits = Hits(context);
        for(int i = 0; i < hits; i++){
            context.Hit(context.value);
        }
        Debug.Log(context.source.entityName + " bit " + hits + " times.");
    }

    public override string Describe(EffectContext context){
        return Hits(context) + " x " + (context.value + context.RageBonus) + " damage";
    }

    public static int Hits(EffectContext context){
        return Mathf.Max(1, context.heads);
    }
}
