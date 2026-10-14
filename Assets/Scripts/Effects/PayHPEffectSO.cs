using UnityEngine;

// The source loses HP equal to the value, ignoring Block and Fortify (a cost, e.g. Bloodrage).
[CreateAssetMenu(fileName = "NewPayHPEffect", menuName = "Dice/Effects/Pay HP Effect")]
public class PayHPEffectSO : EffectSO
{
    public override void Execute(EffectContext context){
        context.source.PayHP(context.value);
        Debug.Log(context.source.entityName + " paid " + context.value + " HP.");
    }

    public override string Describe(EffectContext context){
        return "lose " + context.value + " HP";
    }
}
