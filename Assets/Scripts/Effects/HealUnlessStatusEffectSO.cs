using UnityEngine;
using System.Collections.Generic;

// Heals the source for my value, unless it has one of the blocking statuses (e.g. the Hydra's Regrow, blocked by
// Burn or Bleed: a burned or bleeding wound doesn't close).
[CreateAssetMenu(fileName = "NewHealUnlessStatusEffect", menuName = "Dice/Effects/Heal Unless Status Effect")]
public class HealUnlessStatusEffectSO : EffectSO
{
    public List<StatusType> blockedBy = new List<StatusType>{ StatusType.Burn, StatusType.Bleed };

    public override void Execute(EffectContext context){
        if(Blocked(context.source)){
            Debug.Log(context.source.entityName + "'s healing was blocked.");
            return;
        }
        context.source.Heal(context.value);
    }

    public override string Describe(EffectContext context){
        if(context.source != null && Blocked(context.source)) return "heal " + context.value + " (blocked)";
        return "heal " + context.value + " unless " + string.Join(" or ", blockedBy.ConvertAll(s => s.ToString()).ToArray());
    }

    private bool Blocked(Entity entity){
        return blockedBy.Exists(status => entity.GetStatus(status) > 0);
    }
}
