using UnityEngine;

// Removes half of one of the target's statuses, rounded up (the Hydra's Corrode asks a stacked Fortify die a question).
[CreateAssetMenu(fileName = "NewRemoveHalfStatusEffect", menuName = "Dice/Effects/Remove Half Status Effect")]
public class RemoveHalfStatusEffectSO : EffectSO
{
    public StatusType status = StatusType.Fortify;

    public override void Execute(EffectContext context){
        int removed = Removed(context.Stacks(context.target, status));
        if(removed > 0) context.target.ApplyStatus(status, -removed);
    }

    public override string Describe(EffectContext context){
        if(context.target == null) return "remove half of the target's " + status;
        int stacks = context.Stacks(context.target, status);
        int removed = Removed(stacks);
        context.NotePreviewGain(context.target, status, -removed);
        return "remove " + removed + " " + status;
    }

    private static int Removed(int stacks){
        return (stacks + 1) / 2;
    }
}
