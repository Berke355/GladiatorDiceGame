using UnityEngine;
using System.Collections.Generic;

// Gains value block for every few different faces on the source's die, which rewards a die without duplicates
// (Motley Cloak). Copies of a face count once. Does nothing without a die.
[CreateAssetMenu(fileName = "NewBlockPerDifferentFaceEffect", menuName = "Dice/Effects/Block Per Different Face Effect")]
public class BlockPerDifferentFaceEffectSO : EffectSO
{
    public int facesPerBlock = 2;

    public override void Execute(EffectContext context){
        int block = Amount(context);
        if(block <= 0) return;
        context.source.GainBlock(block);
        Debug.Log(context.source.entityName + " gained " + block + " block from different faces.");
    }

    public override string Describe(EffectContext context){
        return context.die == null ? context.value + " block per " + facesPerBlock + " different faces" : Amount(context) + " block";
    }

    private int Amount(EffectContext context){
        if(context.die == null) return 0;
        HashSet<DieFaceSO> different = new HashSet<DieFaceSO>();
        foreach(DieFace face in context.die) different.Add(face.definition);
        return context.value * (different.Count / facesPerBlock);
    }
}
