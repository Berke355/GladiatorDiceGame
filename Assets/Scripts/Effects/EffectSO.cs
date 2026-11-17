using UnityEngine;

public abstract class EffectSO : ScriptableObject
{
    public abstract void Execute(EffectContext context);

    // Short player-facing text of what this effect would do in this context (e.g. "5 damage", "4 Bleed"),
    // used for enemy intents and action previews. It includes Rage, Ritual and current stacks when the
    // context has a source/target. Effects without a description return an empty string.
    public virtual string Describe(EffectContext context){
        return "";
    }

    // Whether an enemy can play this effect as a copy of a player's face (the Mimic). False for effects that need the
    // player's own systems: the die and its steering, rerolls, Charge, the beast, posture, reading an enemy's intent.
    public virtual bool WorksForEnemies => true;
}
