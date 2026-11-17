using UnityEngine;

// Looks after the source's beast (GDD "Beastmaster"): heal it, raise its max HP, or shelter it for the enemies' turn.
// Groom and Mend heal, Leather Barding toughens, Heel! shelters. Does nothing without a beast out.
[CreateAssetMenu(fileName = "NewBeastCareEffect", menuName = "Dice/Effects/Beast Care Effect")]
public class BeastCareEffectSO : EffectSO
{
    // Serialized as numbers: append only. HealFull heals the beast to its max HP (Feed).
    public enum Care { Heal, MaxHP, Shelter, HealFull }
    public Care care;
    // When above 0, this exact amount instead of the value.
    public int fixedAmount;

    public override void Execute(EffectContext context){
        BeastCompanion beast = context.source != null ? context.source.beast : null;
        if(beast == null) return;
        int amount = fixedAmount > 0 ? fixedAmount : context.value;
        switch(care){
            case Care.Heal: beast.Heal(amount); break;
            case Care.MaxHP: beast.RaiseMaxHP(amount); break;
            case Care.Shelter: beast.Shelter(); break;
            case Care.HealFull: beast.Heal(beast.MaxHP); break;
        }
    }

    public override string Describe(EffectContext context){
        int amount = fixedAmount > 0 ? fixedAmount : context.value;
        BeastCompanion beast = context.source != null ? context.source.beast : null;
        string none = beast != null && !beast.IsOut ? " (no beast)" : "";
        switch(care){
            case Care.MaxHP: return "your beast gains " + amount + " max HP" + none;
            case Care.Shelter: return "your beast takes no damage until your next turn" + none;
            case Care.HealFull: return "heal your beast fully" + none;
            default: return "heal your beast " + amount + none;
        }
    }

    // Needs the player's own systems (see EffectSO.WorksForEnemies).
    public override bool WorksForEnemies => false;
}
