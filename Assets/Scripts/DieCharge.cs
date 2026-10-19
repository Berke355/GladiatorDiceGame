using UnityEngine;
using System.Collections.Generic;

// Charge (Hammer, see GDD): temporary value on die faces for one battle. Plain C#, owned by BattleFlow,
// so it never touches the run's DieFace instances or the assets.
public class DieCharge
{
    public const int MaxCharge = 5;

    private readonly Dictionary<DieFace, int> charge = new Dictionary<DieFace, int>();

    public int Get(DieFace face){
        int amount;
        return face != null && charge.TryGetValue(face, out amount) ? amount : 0;
    }

    public void Add(DieFace face, int amount){
        charge[face] = Mathf.Min(MaxCharge, Get(face) + amount);
    }

    public void Reset(DieFace face){
        charge.Remove(face);
    }

    // The value a face has in this battle, which is what its effects use (Value Is the Effect).
    public int ValueOf(DieFace face){
        return face.value + Get(face);
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
