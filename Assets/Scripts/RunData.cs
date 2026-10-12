using System;
using System.Collections.Generic;

// Everything that must survive between battles in a single run.
// Plain C# data so it can later be saved as JSON or created in tests without a scene.
[Serializable]
public class RunData
{
    public List<DieFaceSO> dieFaces;
    public int maxHP;
    public int currentHP;
    public int gold;
    public int battlesWon;

    public RunData(List<DieFaceSO> startingDieFaces, int startingMaxHP){
        // Copy the list so the run never edits the starting die it was created from.
        dieFaces = new List<DieFaceSO>(startingDieFaces);
        maxHP = startingMaxHP;
        currentHP = startingMaxHP;
    }
}
