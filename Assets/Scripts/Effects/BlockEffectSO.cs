using UnityEngine;

[CreateAssetMenu(fileName = "NewBlockEffect", menuName = "Dice/Effects/Block Effect")]
public class BlockEffectSO : EffectSO
{
    // Adds the source's stacks of this status to the block (None: just the value), e.g. Guard Dog: value + Hound.
    public StatusType plusOwnStatus = StatusType.None;

    public override void Execute(EffectContext context){
        int amount = Amount(context);
        context.source.GainBlock(amount);
        Debug.Log("Gained " + amount + " block.");
    }

    public override string Describe(EffectContext context){
        return Amount(context) + " block";
    }

    private int Amount(EffectContext context){
        if(plusOwnStatus == StatusType.None || context.source == null) return context.value;
        return context.value + context.Stacks(context.source, plusOwnStatus);
    }
}
