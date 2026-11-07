using System.Collections.Generic;

// Everything an effect may need when it runs. New information for effects is added
// here instead of changing every effect's Execute signature.
public class EffectContext
{
    public Entity source;
    public Entity target;
    // Every living opponent of the source, the target included, for area effects. Null: the target is the only one.
    public List<Entity> opponents;
    public int value;
    // The source's die. Null for enemies, which have no die.
    public List<DieFace> die;
    // The die face that triggered the effect. Null for enemy intents.
    public DieFace face;
    // The source's Charge for this battle (Hammer). Null for enemies.
    public DieCharge charge;
    // The source's rerolls for this battle. Null for enemies.
    public RerollPool rerolls;
    // Set by an effect that rerolls the die (Steady Hand); BattleFlow reads it after the action.
    public bool rerollRequested;
    // Set by an effect after which the face keeps its Charge instead of spending it (Resonance).
    public bool keepCharge;
    // The target's die, its stone faces and its Charge, for effects that act on it (Medusa). Null if the target has no die.
    public List<DieFace> targetDie;
    public Petrification targetPetrification;
    public DieCharge targetCharge;
    // The player's last action this turn (the face, its battle value then, and the action), for enemy threats on
    // the player's choice (PunishChoiceEffectSO). Null face when the player hasn't acted.
    public DieFace targetLastFace;
    public int targetLastValue;
    public ActionType targetLastAction;
    // A face of the target's die chosen in advance (the target shown in a boss intent), so the effect hits it
    // even if the die's values changed since. Null: the effect picks its own target when it runs.
    public DieFace targetFace;
    // Set by an effect that starts a boss countdown to a hit of this much damage (Stampede); BossBrain runs it.
    public int countdownDamage;

    // The entities an area effect hits: every living opponent, or just the target.
    public List<Entity> AllOpponents => opponents != null && opponents.Count > 0 ? opponents : new List<Entity>{ target };

    // A face's value in battle: its value plus its Charge.
    public int ValueOf(DieFace dieFace) => charge != null ? charge.ValueOf(dieFace) : dieFace.value;

    // The source's Rage: added to every hit, so each hit of a multi-hit gets it.
    public int RageBonus => source != null ? source.GetStatus(StatusType.Rage) : 0;
    // The source's Ritual: added to every status applied to the opponent (never to the source itself).
    public int RitualBonus => source != null ? source.GetStatus(StatusType.Ritual) : 0;

    // The source's Encourage: added to every buff it gains on itself (Rage, Fortify, FireShield), never to Block,
    // Ritual or Encourage itself.
    public int EncourageBonus => source != null ? source.GetStatus(StatusType.Encourage) : 0;

    public static bool IsEncouraged(StatusType status){
        return status == StatusType.Rage || status == StatusType.Fortify || status == StatusType.FireShield;
    }

    // Every status an effect gives its own source goes through here so Encourage is never forgotten.
    public int GainSelf(StatusType status, int baseAmount){
        int total = baseAmount + (IsEncouraged(status) ? EncourageBonus : 0);
        source.ApplyStatus(status, total);
        return total;
    }

    // Every damage effect hits through here so Rage is never forgotten. Returns the damage of the hit.
    public int Hit(int baseDamage){
        // Weak takes 1 off every hit the source deals.
        int weak = source != null && source.GetStatus(StatusType.Weak) > 0 ? 1 : 0;
        int total = System.Math.Max(0, baseDamage + RageBonus - weak);
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
