using UnityEngine;
using System.Collections.Generic;

// Runs its inner effects only if a status is present, e.g. "if the enemy is bleeding, hit again".
// One reusable "if" box instead of a new effect class for every conditional face.
[CreateAssetMenu(fileName = "NewConditionalEffect", menuName = "Dice/Effects/Conditional Effect")]
public class ConditionalEffectSO : EffectSO
{
    public StatusType requiredStatus = StatusType.Bleed;
    // Check the target (the opponent) by default; false checks the source itself (e.g. "if you have Fortify").
    public bool checkTarget = true;
    public List<EffectSO> effects = new List<EffectSO>();

    public override void Execute(EffectContext context){
        if(!ConditionMet(context)){
            return;
        }
        foreach(EffectSO effect in effects){
            if(effect != null){
                effect.Execute(context);
            }
        }
    }

    public override string Describe(EffectContext context){
        List<string> parts = new List<string>();
        foreach(EffectSO effect in effects){
            string description = effect != null ? effect.Describe(context) : "";
            if(description != "") parts.Add(description);
        }
        string condition = "if " + (checkTarget ? "enemy has " : "you have ") + requiredStatus;
        return condition + ": " + string.Join(", ", parts);
    }

    private bool ConditionMet(EffectContext context){
        Entity checkedEntity = checkTarget ? context.target : context.source;
        return checkedEntity != null && checkedEntity.GetStatus(requiredStatus) > 0;
    }
}
