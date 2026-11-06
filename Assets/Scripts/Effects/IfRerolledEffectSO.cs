using UnityEngine;
using System.Collections.Generic;

// Runs its inner effects only if the source rerolled this turn (e.g. Gambler's Strike: "if you rerolled, hit again").
[CreateAssetMenu(fileName = "NewIfRerolledEffect", menuName = "Dice/Effects/If Rerolled Effect")]
public class IfRerolledEffectSO : EffectSO
{
    public List<EffectSO> effects = new List<EffectSO>();

    public override void Execute(EffectContext context){
        if(!Rerolled(context)) return;
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
        string inner = string.Join(", ", parts.ToArray());
        if(context.rerolls == null) return "if you rerolled: " + inner;
        return Rerolled(context) ? inner : "";
    }

    private static bool Rerolled(EffectContext context){
        return context.rerolls != null && context.rerolls.UsedThisTurn > 0;
    }
}
