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
}
