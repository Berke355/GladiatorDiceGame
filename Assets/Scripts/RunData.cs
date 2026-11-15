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
    // Passive items (see GDD "Equipment"), with no limit: they pile up during a run, like relics.
    public List<EquipmentSO> equipment = new List<EquipmentSO>();
    // Market offers the player locked: they survive rerolls and wait in the next Market that sells that kind
    // (equipment only in a Grand Market). See GDD "Market".
    public List<DieFaceSO> lockedFaces = new List<DieFaceSO>();
    public List<EnchantmentSO> lockedEnchantments = new List<EnchantmentSO>();
    public List<EquipmentSO> lockedEquipment = new List<EquipmentSO>();
    // Enchantments on the die itself (see GDD "Die Enchantments"); at most DieEnchantmentSlots of them.
    public const int DieEnchantmentSlots = 2;
    public List<DieEnchantmentSO> dieEnchantments = new List<DieEnchantmentSO>();
    public List<DieEnchantmentSO> lockedDieEnchantments = new List<DieEnchantmentSO>();
    // The bench (see GDD "Bench"): faces replaced at the Market wait here, upgrades and enchantments kept, and can be
    // swapped back onto the die for free at any Market. At most Market.BenchSlots; the oldest is sold when it overflows.
    public List<DieFace> bench = new List<DieFace>();
    // Rerolls the player gets at the start of every battle (see GDD "Rerolls").
    public int rerollsPerBattle = 3;

    // The rerolls a battle starts with: the base, plus items that count the bench (Reserve Roster).
    public int RerollsThisBattle{
        get{
            int perBenched = 0;
            foreach(EquipmentSO item in equipment) if(item != null) perBenched += item.rerollsPerBenchedFace;
            return rerollsPerBattle + perBenched * bench.Count;
        }
    }
    // Set by a boss victory: the next Market starts with a free equipment pick.
    public bool bossRewardPending;
    // The next battle, picked ahead (when the Market opens) so the Market can show it: a boss or an encounter.
    public bool nextBattlePicked;
    public BossSO nextBoss;
    public EnemyGroup nextEncounter;

    // The map (see GDD "Map"): the current act (0, 1, 2), its map, the node the player stands on (-1: before the first
    // floor) and the act's boss, picked with the map so the map can show it.
    public int act;
    public ActMap map;
    public int mapNode = -1;
    public BossSO actBoss;
    // The nodes walked this act, and a rest waiting for its choice (the map screen shows it).
    public List<int> visitedNodes = new List<int>();
    public bool restPending;
    // An elite was beaten: the map offers these items, one to take for free (or none).
    public List<EquipmentSO> eliteRewardChoices = new List<EquipmentSO>();
    // Set by a boss victory: the next shop is the Grand Market, after which the next act's map begins.
    public bool grandMarketPending;
    // The shop scene's mode for this visit: the Enchanting Table node instead of a Market.
    public bool atEnchantingTable;
    // The final boss was beaten: the run is won.
    public bool runComplete;

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
