using UnityEngine;
using System.Collections.Generic;

// Adds the value to each of the listed statuses already on the target, whichever they are (Catalyst). Each addition is a
// new application, so Ritual applies to each. Does nothing on a target without them.
[CreateAssetMenu(fileName = "NewAddToPresentStatusesEffect", menuName = "Dice/Effects/Add To Present Statuses Effect")]
public class AddToPresentStatusesEffectSO : EffectSO
{
    public List<StatusType> statuses = new List<StatusType>{ StatusType.Burn, StatusType.Poison, StatusType.Bleed };

    public override void Execute(EffectContext context){
        foreach(StatusType status in statuses){
            if(context.target.GetStatus(status) > 0) context.ApplyToTarget(status, context.value);
        }
    }

    public override string Describe(EffectContext context){
        if(context.target == null) return "+" + context.value + " to each status on the enemy";
        List<string> parts = new List<string>();
        int amount = context.value + context.RitualBonus;
        foreach(StatusType status in statuses){
            if(context.Stacks(context.target, status) <= 0) continue;
            parts.Add(amount + " " + status);
            context.NotePreviewGain(context.target, status, amount);
        }
        return parts.Count > 0 ? string.Join(", ", parts) : "no status to add to";
    }
}
