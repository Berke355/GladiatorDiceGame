using UnityEngine;

// The source loses HP equal to the value, ignoring Block and Fortify (a cost, e.g. Bloodrage).
[CreateAssetMenu(fileName = "NewPayHPEffect", menuName = "Dice/Effects/Pay HP Effect")]
public class PayHPEffectSO : EffectSO
{
    // Leaves at least 1 HP (an item paying on its own at the start of a battle, Blood Pact, should never kill).
    public bool neverLethal;
    // When above 0, this exact amount instead of the value (Feed: upgrades don't raise the cost).
    public int fixedAmount;

    private int Amount(EffectContext context){ return fixedAmount > 0 ? fixedAmount : context.value; }

    public override void Execute(EffectContext context){
        int amount = neverLethal ? Mathf.Min(Amount(context), context.source.currentHP - 1) : Amount(context);
        context.source.PayHP(amount);
        Debug.Log(context.source.entityName + " paid " + amount + " HP.");
    }

    public override string Describe(EffectContext context){
        return "lose " + Amount(context) + " HP";
    }
}
