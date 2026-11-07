using System.Collections.Generic;

// Consumable actions used this battle (see GDD "Consumable and Change"). Plain C#, owned by BattleFlow; battle-only,
// never in RunData. Each face instance on the die has its own uses, so two Health Potions are two separate drinks.
public class Consumables
{
    private readonly HashSet<KeyValuePair<DieFace, ActionType>> used = new HashSet<KeyValuePair<DieFace, ActionType>>();

    public bool IsUsed(DieFace face, ActionType action){
        return used.Contains(new KeyValuePair<DieFace, ActionType>(face, action));
    }

    public void MarkUsed(DieFace face, ActionType action){
        used.Add(new KeyValuePair<DieFace, ActionType>(face, action));
    }

    // Whether any action of this face was used up (the die strip marks it).
    public bool AnyUsed(DieFace face){
        foreach(KeyValuePair<DieFace, ActionType> entry in used){
            if(entry.Key == face) return true;
        }
        return false;
    }
}
