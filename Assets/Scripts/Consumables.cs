using System.Collections.Generic;

// Consumable faces used this battle (see GDD "Consumable and Change"). Plain C#, owned by BattleFlow; battle-only,
// never in RunData. Each face instance on the die has its own uses, so two Health Potions are two separate drinks.
public class Consumables
{
    private readonly Dictionary<DieFace, int> uses = new Dictionary<DieFace, int>();
    // How many times a Consumable face works before it turns into Change (Alchemist's Belt adds to it).
    public int usesPerBattle = 1;

    public bool IsUsed(DieFace face){
        int count;
        return uses.TryGetValue(face, out count) && count >= usesPerBattle;
    }

    public void MarkUsed(DieFace face){
        int count;
        uses.TryGetValue(face, out count);
        uses[face] = count + 1;
    }
}
