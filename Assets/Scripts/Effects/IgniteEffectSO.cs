using UnityEngine;

// Ignites the target's Burn: deals damage equal to its Burn without consuming it (see GDD "Status Effects").
// Burn deals no damage on its own, so ignitions like this are how it turns into damage.
[CreateAssetMenu(fileName = "NewIgniteEffect", menuName = "Dice/Effects/Ignite Effect")]
public class IgniteEffectSO : EffectSO
{
    // Burns it all up: the Burn is gone after the hit (Fuel, whose multiplied Burn would otherwise snowball).
    public bool consume;

    public override void Execute(EffectContext context){
        int burn = context.target.GetStatus(StatusType.Burn);
        if(burn <= 0){
            Debug.Log(context.source.entityName + " ignited, but there was no Burn.");
            return;
        }
        if(consume) context.target.ConsumeStatus(StatusType.Burn);
        int dealt = context.Hit(burn);
        Debug.Log(context.source.entityName + " ignited " + burn + " Burn for " + dealt + " damage.");
    }

    public override string Describe(EffectContext context){
        if(context.target == null){
            return "ignite Burn";
        }
        int burn = context.Stacks(context.target, StatusType.Burn);
        return "ignite Burn (" + (burn > 0 ? context.PreviewHit(burn) : 0) + " damage" + (consume ? ", consume Burn)" : ")");
    }
}
