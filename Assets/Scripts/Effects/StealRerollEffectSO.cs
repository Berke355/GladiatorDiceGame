using UnityEngine;

// The target loses rerolls for the rest of the battle (Essedarius's Kick Up Dust): the value, never below 0. A target
// with no rerolls left gets the fallback status instead (Vulnerable), so the intent is never empty.
[CreateAssetMenu(fileName = "NewStealRerollEffect", menuName = "Dice/Effects/Steal Reroll Effect")]
public class StealRerollEffectSO : EffectSO
{
    public StatusType fallbackStatus = StatusType.Vulnerable;
    public int fallbackAmount = 2;

    public override void Execute(EffectContext context){
        if(context.targetRerolls == null) return;
        if(context.targetRerolls.Left <= 0){
            if(fallbackStatus != StatusType.None && fallbackAmount > 0) context.ApplyToTarget(fallbackStatus, fallbackAmount);
            return;
        }
        context.targetRerolls.Left = Mathf.Max(0, context.targetRerolls.Left - context.value);
    }

    public override string Describe(EffectContext context){
        if(context.targetRerolls != null && context.targetRerolls.Left <= 0 && fallbackStatus != StatusType.None){
            return (fallbackAmount + context.RitualBonus) + " " + fallbackStatus;
        }
        return "you lose " + context.value + (context.value == 1 ? " reroll" : " rerolls");
    }
}
