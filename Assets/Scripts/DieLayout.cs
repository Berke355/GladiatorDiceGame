using System.Collections.Generic;

// The die as a cube laid out as a sideways cross net (see GDD "Die Layout"). The order of RunData.dieFaces is the
// position on the net:
//
//          [0 Top]
//  [1 Left] [2 Middle] [3 Right] [4 Rightmost]
//          [5 Bottom]
//
// The four in the row wrap around the cube, so opposites are Left/Right, Middle/Rightmost and Top/Bottom; every other
// face is a neighbor.
public static class DieLayout
{
    public const int Count = 6;
    public const int Top = 0, Left = 1, Middle = 2, Right = 3, Rightmost = 4, Bottom = 5;

    private static readonly string[] Names = { "Top", "Left", "Middle", "Right", "Rightmost", "Bottom" };
    private static readonly int[] Opposites = { Bottom, Right, Rightmost, Left, Middle, Top };

    public static string Name(int position){
        return position >= 0 && position < Count ? Names[position] : "";
    }

    public static int OppositeOf(int position){
        return Opposites[position];
    }

    // The four faces sharing an edge with this one: all but itself and its opposite.
    public static List<int> NeighborsOf(int position){
        List<int> neighbors = new List<int>();
        for(int i = 0; i < Count; i++){
            if(i != position && i != Opposites[position]) neighbors.Add(i);
        }
        return neighbors;
    }

    // e.g. "Position: Top. Opposite: Bottom (Sword)", for tooltips.
    public static string Describe(List<DieFace> die, int position){
        int opposite = OppositeOf(position);
        string face = opposite < die.Count ? " (" + die[opposite].DisplayName + ")" : "";
        return "Position: " + Name(position) + ". Opposite: " + Name(opposite) + face;
    }
}
