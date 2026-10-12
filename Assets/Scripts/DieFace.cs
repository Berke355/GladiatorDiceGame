using System;

// One face on the player's die: a DieFaceSO definition plus this copy's current value.
// Upgrades change the instance, never the shared asset, so two copies of the same
// face can have different values and the asset stays untouched.
[Serializable]
public class DieFace
{
    public DieFaceSO definition;
    public int value;

    public DieFace(DieFaceSO definition){
        this.definition = definition;
        value = definition.baseValue;
    }
}
