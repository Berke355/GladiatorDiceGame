using UnityEngine;
using System.Collections.Generic;

// The faces next to this one on the net gain value for this battle (Drill Sergeant). Unlike Charge, acting with them
// doesn't spend it. Does nothing without a die.
[CreateAssetMenu(fileName = "NewAddValueToNeighborsEffect", menuName = "Dice/Effects/Add Value To Neighbors Effect")]
public class AddValueToNeighborsEffectSO : EffectSO
{
    public int amount = 1;

    public override void Execute(EffectContext context){
        foreach(DieFace neighbor in Neighbors(context)) context.charge.AddBattleBonus(neighbor, amount);
    }

    public override string Describe(EffectContext context){
        if(context.die == null || context.face == null) return "+" + amount + " value to every face except me and my opposite";
        int count = Neighbors(context).Count;
        return "+" + amount + " value to " + count + (count == 1 ? " face" : " faces");
    }

    private List<DieFace> Neighbors(EffectContext context){
        List<DieFace> neighbors = new List<DieFace>();
        if(context.die == null || context.face == null || context.charge == null) return neighbors;
        int here = context.die.IndexOf(context.face);
        if(here < 0 || here >= DieLayout.Count) return neighbors;
        foreach(int position in DieLayout.NeighborsOf(here)){
            if(position < context.die.Count) neighbors.Add(context.die[position]);
        }
        return neighbors;
    }
}
