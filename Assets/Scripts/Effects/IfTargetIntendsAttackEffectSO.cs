using System.Collections.Generic;
using UnityEngine;

// Runs its inner effects only if the target enemy's current intent is an attack (Bait). Without a brain to read
// (an enemy's own effect), the condition counts as not met.
[CreateAssetMenu(fileName = "NewIfTargetIntendsAttackEffect", menuName = "Dice/Effects/If Target Intends Attack Effect")]
public class IfTargetIntendsAttackEffectSO : EffectSO
{
    public List<EffectSO> effects = new List<EffectSO>();

    public override void Execute(EffectContext context){
        if(!ConditionMet(context)) return;
        foreach(EffectSO effect in effects){
            if(effect != null) effect.Execute(context);
        }
    }

    public override string Describe(EffectContext context){
        List<string> parts = new List<string>();
        foreach(EffectSO effect in effects){
            string description = effect != null ? effect.Describe(context) : "";
            if(description != "") parts.Add(description);
        }
        // Like ConditionalEffectSO: a condition already met shows just its result.
        if(ConditionMet(context)) return string.Join(", ", parts);
        return "(if the enemy intends to attack: " + string.Join(", ", parts) + ")";
    }

    private static bool ConditionMet(EffectContext context){
        return context.targetBrain != null && context.targetBrain.IntendsAttack;
    }
}
