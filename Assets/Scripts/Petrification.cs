using System.Collections.Generic;

// Petrify (Medusa, see GDD): stone faces on the player's die for one battle. Plain C#, owned by BattleFlow,
// so it never touches the run's DieFace instances. A stone face's only action is Defense: gain StoneBlock block.
public class Petrification
{
    public const int StoneBlock = 2;

    // Player turns each stone face stays stone.
    private readonly Dictionary<DieFace, int> turnsLeft = new Dictionary<DieFace, int>();

    public bool IsStone(DieFace face){
        return face != null && turnsLeft.ContainsKey(face);
    }

    public int TurnsLeft(DieFace face){
        int turns;
        return face != null && turnsLeft.TryGetValue(face, out turns) ? turns : 0;
    }

    public void Petrify(DieFace face, int turns){
        if(face == null || turns <= 0) return;
        turnsLeft[face] = System.Math.Max(TurnsLeft(face), turns);
    }

    public int CountStone(List<DieFace> die){
        return die != null ? die.FindAll(IsStone).Count : 0;
    }

    // The face with the highest battle value (value plus Charge) that isn't stone yet; null if there is none.
    public DieFace HighestNotStone(List<DieFace> die, DieCharge charge){
        if(die == null) return null;
        DieFace best = null;
        foreach(DieFace face in die){
            if(IsStone(face)) continue;
            if(best == null || ValueOf(face, charge) > ValueOf(best, charge)) best = face;
        }
        return best;
    }

    // Called at the end of each player turn: every stone face has one turn less.
    public void EndPlayerTurn(){
        List<DieFace> faces = new List<DieFace>(turnsLeft.Keys);
        foreach(DieFace face in faces){
            if(--turnsLeft[face] <= 0) turnsLeft.Remove(face);
        }
    }

    private static int ValueOf(DieFace face, DieCharge charge){
        return charge != null ? charge.ValueOf(face) : face.value;
    }
}
