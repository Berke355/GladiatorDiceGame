using UnityEngine;

[CreateAssetMenu(fileName = "NewBlockEffect", menuName = "Dice/Effects/Block Effect")]
public class BlockEffectSO : EffectSO
{
    public override void Execute(EffectContext context){
        context.source.GainBlock(context.value);
        Debug.Log("Gained " + context.value + " block.");
    }

    public override string Describe(int value){
        return value + " block";
    }
}
