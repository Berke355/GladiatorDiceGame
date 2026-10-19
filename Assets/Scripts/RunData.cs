using System;
using System.Collections.Generic;

// Everything that must survive between battles in a single run.
// Plain C# data so it can later be saved as JSON or created in tests without a scene.
[Serializable]
public class RunData
{
    public List<DieFace> dieFaces;
    public int maxHP;
    public int currentHP;
    public int gold;
    public int battlesWon;
    // Enemies already fought this run; an enemy never appears twice in one run.
    public List<EnemySO> metEnemies = new List<EnemySO>();
    // The faces offered at the start of the run, and whether one was picked for the die.
    public List<DieFaceSO> startingFaceOffers = new List<DieFaceSO>();
    public bool startingFaceChosen;
    // Passive items (see GDD "Equipment"); at most EquipmentSlots of them.
    public const int EquipmentSlots = 3;
    public List<EquipmentSO> equipment = new List<EquipmentSO>();
    // Set by a boss victory: the next Market starts with a free equipment pick.
    public bool bossRewardPending;

    public RunData(List<DieFaceSO> startingDieFaces, int startingMaxHP){
        // Each face on the die is its own instance, so upgrading one never touches the asset or other copies.
        dieFaces = new List<DieFace>();
        foreach(DieFaceSO face in startingDieFaces){
            dieFaces.Add(new DieFace(face));
        }
        maxHP = startingMaxHP;
        currentHP = startingMaxHP;
    }
}
