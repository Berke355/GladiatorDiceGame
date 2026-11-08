using System.Collections.Generic;

// Consumable actions used this battle (see GDD "Consumable and Change"). Plain C#, owned by BattleFlow; battle-only,
// never in RunData. Each face instance on the die has its own uses, so two Health Potions are two separate drinks.
public class Consumables
{
    private readonly Dictionary<KeyValuePair<DieFace, ActionType>, int> uses = new Dictionary<KeyValuePair<DieFace, ActionType>, int>();
    // How many times a Consumable action works before it turns into Change (Alchemist's Belt adds to it).
    public int usesPerBattle = 1;

    public bool IsUsed(DieFace face, ActionType action){
        int count;
        return uses.TryGetValue(new KeyValuePair<DieFace, ActionType>(face, action), out count) && count >= usesPerBattle;
    }

    public void MarkUsed(DieFace face, ActionType action){
        KeyValuePair<DieFace, ActionType> key = new KeyValuePair<DieFace, ActionType>(face, action);
        int count;
        uses.TryGetValue(key, out count);
        uses[key] = count + 1;
    }

    // Whether any action of this face was used up (the die strip marks it).
    public bool AnyUsed(DieFace face){
        foreach(KeyValuePair<KeyValuePair<DieFace, ActionType>, int> entry in uses){
            if(entry.Key.Key == face && entry.Value >= usesPerBattle) return true;
        }
        return false;
    }
}
