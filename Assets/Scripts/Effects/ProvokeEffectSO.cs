using UnityEngine;

// Provoke (see GDD "Provocator"): an enemy that doesn't intend to attack drops its plan, switches to its weakest attack
// and gains Rage (permanent, so provoking often makes it stronger). An enemy already attacking is left alone.
[CreateAssetMenu(fileName = "NewProvokeEffect", menuName = "Dice/Effects/Provoke Effect")]
public class ProvokeEffectSO : EffectSO
{
    // The Rage the provoked enemy gains. Not boosted by the player's Ritual: it is a cost, not an attack.
    public int rageCost = 1;

    public override void Execute(EffectContext context){
        if(context.targetBrain == null || !context.targetBrain.ProvokeToWeakestAttack()) return;
        context.target.ApplyStatus(StatusType.Rage, rageCost);
    }

    public override string Describe(EffectContext context){
        if(context.targetBrain == null) return "if the enemy isn't attacking: it switches to its weakest attack and gains " + rageCost + " Rage";
        if(context.targetBrain.IntendsAttack) return "(the enemy already attacks)";
        return "the enemy switches to its weakest attack and gains " + rageCost + " Rage";
    }
}
