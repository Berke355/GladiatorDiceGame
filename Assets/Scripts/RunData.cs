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
    // Paid rerolls of the starting offers so far; each costs more (the Market's reroll price).
    public int startingOfferRerolls;
    // Passive items (see GDD "Equipment"); at most EquipmentSlots of them.
    public const int EquipmentSlots = 3;
    public List<EquipmentSO> equipment = new List<EquipmentSO>();
    // Market offers the player locked: they survive rerolls and wait in the next Market that sells that kind
    // (equipment only in a Grand Market). See GDD "Market".
    public List<DieFaceSO> lockedFaces = new List<DieFaceSO>();
    public List<EnchantmentSO> lockedEnchantments = new List<EnchantmentSO>();
    public List<EquipmentSO> lockedEquipment = new List<EquipmentSO>();
    // Rerolls the player gets at the start of every battle (see GDD "Rerolls").
    public int rerollsPerBattle = 2;
    // Set by a boss victory: the next Market starts with a free equipment pick.
    public bool bossRewardPending;
    // The next battle, picked ahead (when the Market opens) so the Market can show it: a boss or an encounter.
    public bool nextBattlePicked;
    public BossSO nextBoss;
    public EnemyGroup nextEncounter;

    // The run's starting die (before the chosen sixth face), so the Market can tell build faces from starter faces.
    public List<DieFaceSO> startingDie = new List<DieFaceSO>();

    public RunData(List<DieFaceSO> startingDieFaces, int startingMaxHP){
        startingDie = new List<DieFaceSO>(startingDieFaces);
        // Each face on the die is its own instance, so upgrading one never touches the asset or other copies.
        dieFaces = new List<DieFace>();
        foreach(DieFaceSO face in startingDieFaces){
            dieFaces.Add(new DieFace(face));
        }
        maxHP = startingMaxHP;
        currentHP = startingMaxHP;
    }
}
