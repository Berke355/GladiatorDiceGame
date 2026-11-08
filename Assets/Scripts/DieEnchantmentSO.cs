using UnityEngine;
using System;
using System.Collections.Generic;

// A number for one position on the net.
[Serializable]
public struct PositionAmount
{
    public int position;
    public int amount;
}

// An enchantment inscribed on the die itself, not on a face (see GDD "Die Enchantments"). It stays on its positions
// when faces move, which makes the arrangement a puzzle for every build. Design data only: what a run owns is in
// RunData.dieEnchantments.
[CreateAssetMenu(fileName = "NewDieEnchantment", menuName = "Dice/Die Enchantment")]
public class DieEnchantmentSO : ScriptableObject
{
    public string enchantmentName;
    [TextArea]
    public string description;
    public Rarity rarity;
    // Added to the battle value of the face on each position (Value Is the Effect: shown on the die).
    public List<PositionAmount> positionBonuses = new List<PositionAmount>();
    // Added to every face sharing a tag with its opposite face (Kindred).
    public int oppositeTagBonus;
    // How often each listed position comes up, relative to the others' 1: 2 is twice as often, 0 never (Weighted Die).
    public List<PositionAmount> rollWeights = new List<PositionAmount>();

    // The total bonus of these enchantments for the face on a position.
    public static int BonusAt(List<DieEnchantmentSO> enchantments, List<DieFace> die, int position){
        if(enchantments == null) return 0;
        int bonus = 0;
        foreach(DieEnchantmentSO enchantment in enchantments){
            foreach(PositionAmount entry in enchantment.positionBonuses){
                if(entry.position == position) bonus += entry.amount;
            }
            if(enchantment.oppositeTagBonus != 0 && SharesTagWithOpposite(die, position)) bonus += enchantment.oppositeTagBonus;
        }
        return bonus;
    }

    // How often a position comes up under these enchantments (1 is normal).
    public static int WeightAt(List<DieEnchantmentSO> enchantments, int position){
        int weight = 1;
        if(enchantments == null) return weight;
        foreach(DieEnchantmentSO enchantment in enchantments){
            foreach(PositionAmount entry in enchantment.rollWeights){
                if(entry.position == position) weight *= entry.amount;
            }
        }
        return weight;
    }

    private static bool SharesTagWithOpposite(List<DieFace> die, int position){
        if(position < 0 || position >= DieLayout.Count || position >= die.Count) return false;
        int opposite = DieLayout.OppositeOf(position);
        if(opposite >= die.Count) return false;
        foreach(FaceTag tag in die[position].definition.tags){
            if(die[opposite].definition.tags.Contains(tag)) return true;
        }
        return false;
    }
}
