using UnityEngine;
using System.Collections.Generic;

// Does what the opposite face on the net does on Attack, with this face's value (Mimic). Only the effects are copied, so a
// Consumable stays unused. Falls back to a plain hit if there is nothing to copy: no die, no Attack, or another Mimic.
[CreateAssetMenu(fileName = "NewCopyOppositeAttackEffect", menuName = "Dice/Effects/Copy Opposite Attack Effect")]
public class CopyOppositeAttackEffectSO : EffectSO
{
    public override void Execute(EffectContext context){
        List<EffectSO> copied = Copied(context);
        if(copied == null){
            context.Hit(context.value);
            return;
        }
        foreach(EffectSO effect in copied) effect.Execute(context);
    }

    public override string Describe(EffectContext context){
        if(context.die == null) return "copy the opposite face's Attack";
        List<EffectSO> copied = Copied(context);
        if(copied == null) return (context.value + context.RageBonus) + " damage";
        List<string> parts = new List<string>();
        foreach(EffectSO effect in copied) parts.Add(effect.Describe(context));
        return string.Join(", ", parts);
    }

    private List<EffectSO> Copied(EffectContext context){
        if(context.die == null || context.face == null) return null;
        int here = context.die.IndexOf(context.face);
        if(here < 0 || here >= DieLayout.Count) return null;
        int opposite = DieLayout.OppositeOf(here);
        if(opposite >= context.die.Count) return null;
        DieFace other = context.die[opposite];
        if(other.definition == context.face.definition) return null;
        List<EffectSO> effects = other.GetEffects(ActionType.Attack);
        foreach(EffectSO effect in effects){
            if(effect is CopyOppositeAttackEffectSO) return null;
        }
        return effects.Count > 0 ? effects : null;
    }
}
