using UnityEngine;
using System.Collections.Generic;

// All economy numbers in one asset so they can be tuned in the Inspector (see GDD "Rarity & Economy").
[CreateAssetMenu(fileName = "EconomyConfig", menuName = "Dice/Economy Config")]
public class EconomyConfigSO : ScriptableObject
{
    public int goldPerVictory = 10;
    // Extra gold for winning a boss battle, so the Grand Market that follows is affordable.
    public int bossVictoryBonus = 0;

    [Header("Market Offers")]
    // Every face the Market can offer.
    public List<DieFaceSO> marketPool = new List<DieFaceSO>();
    public int offersPerMarket = 3;
    // Normal Market rarity weights. The Grand Market only offers Epic faces.
    public int commonWeight = 70;
    public int rareWeight = 25;
    public int epicWeight = 5;
    // Chance that an offer is picked among faces sharing a tag with the player's die.
    [Range(0f, 1f)]
    public float tagMatchChance = 0.6f;

    [Header("Action Modules")]
    // Every module the Normal Market can offer, and how many it offers per visit.
    public List<ActionModuleSO> modulePool = new List<ActionModuleSO>();
    public int modulesPerMarket = 2;

    [Header("Equipment")]
    // Every item the boss reward and the Grand Market can offer.
    public List<EquipmentSO> equipmentPool = new List<EquipmentSO>();
    // A boss reward is a free pick of one of this many items.
    public int bossRewardChoices = 3;
    // The Grand Market sells this many items at this price (the Normal Market sells none).
    public int equipmentPerGrandMarket = 2;
    public int equipmentPrice = 30;

    [Header("Market Reroll")]
    // Cost of the first reroll in a visit, and how much each further reroll adds (resets every Market).
    public int rerollBaseCost = 5;
    public int rerollCostStep = 5;
    // The Grand Market only offers Epics, so fishing there costs much more.
    public int grandRerollBaseCost = 15;
    public int grandRerollCostStep = 15;

    [Header("Starting Face")]
    // The starting face choice uses the Market pool, minus faces already on the starting die.
    public int startingFaceOfferCount = 3;
    public int startingCommonWeight = 85;
    public int startingRareWeight = 13;
    public int startingEpicWeight = 2;

    [Header("Buy Price")]
    public int commonPrice = 8;
    public int rarePrice = 15;
    public int epicPrice = 30;

    [Header("Upgrade Cost Multiplier (x current value)")]
    public int commonUpgradeMultiplier = 3;
    public int rareUpgradeMultiplier = 5;
    public int epicUpgradeMultiplier = 10;

    public int GetBuyPrice(Rarity rarity){
        switch(rarity){
            case Rarity.Rare: return rarePrice;
            case Rarity.Epic: return epicPrice;
            default: return commonPrice;
        }
    }

    // Each upgrade costs more than the last because it scales with the current value. There is no cap.
    public int GetUpgradeCost(DieFace face){
        int multiplier;
        switch(face.definition.rarity){
            case Rarity.Rare: multiplier = rareUpgradeMultiplier; break;
            case Rarity.Epic: multiplier = epicUpgradeMultiplier; break;
            default: multiplier = commonUpgradeMultiplier; break;
        }
        return multiplier * face.value;
    }

    // Rising cost within one Market visit, e.g. 5 / 10 / 15 (Grand: 15 / 30 / 45).
    public int GetRerollCost(bool grandMarket, int rerollsThisVisit){
        return grandMarket
            ? grandRerollBaseCost + grandRerollCostStep * rerollsThisVisit
            : rerollBaseCost + rerollCostStep * rerollsThisVisit;
    }

}
