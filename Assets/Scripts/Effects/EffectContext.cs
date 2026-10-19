using System.Collections.Generic;

// Everything an effect may need when it runs. New information for effects is added
// here instead of changing every effect's Execute signature.
public class EffectContext
{
    public Entity source;
    public Entity target;
    public int value;
    // The source's die. Null for enemies, which have no die.
    public List<DieFace> die;
    // The die face that triggered the effect. Null for enemy intents.
    public DieFace face;
    // The source's Charge for this battle (Hammer). Null for enemies.
    public DieCharge charge;
    // Set by an effect that rerolls the die (Steady Hand); BattleFlow reads it after the action.
    public bool rerollRequested;

    // A face's value in battle: its value plus its Charge.
    public int ValueOf(DieFace dieFace) => charge != null ? charge.ValueOf(dieFace) : dieFace.value;

    // The source's Rage: added to every hit, so each hit of a multi-hit gets it.
    public int RageBonus => source != null ? source.GetStatus(StatusType.Rage) : 0;
    // The source's Ritual: added to every status applied to the opponent (never to the source itself).
    public int RitualBonus => source != null ? source.GetStatus(StatusType.Ritual) : 0;

    // Every damage effect hits through here so Rage is never forgotten. Returns the damage of the hit.
    public int Hit(int baseDamage){
        int total = baseDamage + RageBonus;
        target.TakeDamage(total, source);
        return total;
    }

    // Every status an effect puts on the opponent goes through here so Ritual is never forgotten.
    public int ApplyToTarget(StatusType status, int baseAmount){
        int total = baseAmount + RitualBonus;
        target.ApplyStatus(status, total);
        return total;
    }
}
