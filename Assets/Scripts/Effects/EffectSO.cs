using UnityEngine;

public abstract class EffectSO : ScriptableObject
{
    public abstract void Execute(EffectContext context);

    // Short player-facing text for what this effect does with the given value (e.g. "4 damage").
    // Used to make enemy intents readable. Effects without a description return an empty string.
    public virtual string Describe(int value){
        return "";
    }
}
