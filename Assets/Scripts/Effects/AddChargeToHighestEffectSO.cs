using UnityEngine;

// Adds value as Charge to the source's highest-value other face (e.g. Anvil). Does nothing without a die.
[CreateAssetMenu(fileName = "NewAddChargeToHighestEffect", menuName = "Dice/Effects/Add Charge To Highest Effect")]
public class AddChargeToHighestEffectSO : EffectSO
{
    public override void Execute(EffectContext context){
        if(context.charge == null || context.die == null) return;
        DieFace target = context.charge.HighestOther(context.die, context.face);
        if(target == null) return;
        context.charge.Add(target, context.value);
        Debug.Log(context.source.entityName + " charged " + target.definition.faceName + " by " + context.value + ".");
    }

    public override string Describe(EffectContext context){
        if(context.charge == null || context.die == null){
            return context.value + " Charge to your highest face";
        }
        DieFace target = context.charge.HighestOther(context.die, context.face);
        return target != null ? context.value + " Charge to " + target.definition.faceName : "";
    }
}
