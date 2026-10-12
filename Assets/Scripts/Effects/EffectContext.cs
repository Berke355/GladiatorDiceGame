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
}
