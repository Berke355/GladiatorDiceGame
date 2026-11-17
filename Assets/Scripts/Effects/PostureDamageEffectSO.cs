using UnityEngine;

// The Deflect build's payoffs (GDD "Deflect"), read from the target's posture.
// PlusPosture (Grudge): value + the target's posture. StunnedMultiplier (Deathblow): value, times `multiplier` against
// a stunned target (one whose posture broke).
[CreateAssetMenu(fileName = "NewPostureDamageEffect", menuName = "Dice/Effects/Posture Damage Effect")]
public class PostureDamageEffectSO : EffectSO
{
    public enum Mode { PlusPosture, StunnedMultiplier }
    public Mode mode;
    public int multiplier = 3;

    public override void Execute(EffectContext context){
        if(context.target == null) return;
        context.Hit(Damage(context));
    }

    public override string Describe(EffectContext context){
        if(context.target == null){
            return mode == Mode.PlusPosture ? "damage equal to my value + the enemy's posture" : "damage equal to my value, x" + multiplier + " against a stunned enemy";
        }
        int damage = context.PreviewHit(Damage(context));
        if(mode == Mode.PlusPosture) return damage + " damage (" + context.target.Posture + " posture)";
        return damage + " damage" + (context.target.StunTurns > 0 ? " (stunned, x" + multiplier + ")" : "");
    }

    private int Damage(EffectContext context){
        if(mode == Mode.PlusPosture) return context.value + context.target.Posture;
        return context.target.StunTurns > 0 ? context.value * multiplier : context.value;
    }

    // Needs the player's own systems (see EffectSO.WorksForEnemies).
    public override bool WorksForEnemies => false;
}
