using UnityEngine;
using System.Collections.Generic;

// Charge (Hammer, see GDD): temporary value on die faces for one battle. Plain C#, owned by BattleFlow,
// so it never touches the run's DieFace instances or the assets.
public class DieCharge
{
    public const int MaxCharge = 5;

    private readonly Dictionary<DieFace, int> charge = new Dictionary<DieFace, int>();
    // The bonus from the die's enchantments for a face (see GDD "Die Enchantments"), set by BattleFlow.
    public System.Func<DieFace, int> placementBonus;

    public int Bonus(DieFace face){
        return placementBonus != null ? placementBonus(face) : 0;
    }

    public int Get(DieFace face){
        int amount;
        return face != null && charge.TryGetValue(face, out amount) ? amount : 0;
    }

    // Capped Charge stops at MaxCharge but never lowers Charge that is already above it.
    // Uncapped Charge (Resonance) can go past the cap.
    public void Add(DieFace face, int amount, bool capped = true){
        int current = Get(face);
        charge[face] = capped ? Mathf.Max(current, Mathf.Min(MaxCharge, current + amount)) : current + amount;
    }

    public void Reset(DieFace face){
        charge.Remove(face);
    }

    // The value a face has in this battle, which is what its effects use (Value Is the Effect): its value, its Charge
    // and its place on the die. Never below 0.
    public int ValueOf(DieFace face){
        return Mathf.Max(0, face.value + Get(face) + Bonus(face));
    }

    // The face with the highest battle value other than exclude (the first one on ties); null if there is none.
    public DieFace HighestOther(List<DieFace> die, DieFace exclude){
        DieFace best = null;
        foreach(DieFace face in die){
            if(face == exclude) continue;
            if(best == null || ValueOf(face) > ValueOf(best)) best = face;
        }
        return best;
    }
}
