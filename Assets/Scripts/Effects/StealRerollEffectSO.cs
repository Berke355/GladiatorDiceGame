using UnityEngine;

// The target loses rerolls for the rest of the battle (Essedarius's Kick Up Dust): the value, never below 0.
[CreateAssetMenu(fileName = "NewStealRerollEffect", menuName = "Dice/Effects/Steal Reroll Effect")]
public class StealRerollEffectSO : EffectSO
{
    public override void Execute(EffectContext context){
        if(context.targetRerolls == null) return;
        context.targetRerolls.Left = Mathf.Max(0, context.targetRerolls.Left - context.value);
    }

    public override string Describe(EffectContext context){
        return "you lose " + context.value + (context.value == 1 ? " reroll" : " rerolls");
    }
}
