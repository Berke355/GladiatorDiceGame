using UnityEngine;
using System.Collections.Generic;

// All economy numbers in one asset so they can be tuned in the Inspector (see GDD "Rarity & Economy").
[CreateAssetMenu(fileName = "EconomyConfig", menuName = "Dice/Economy Config")]
public class EconomyConfigSO : ScriptableObject
{
    public int goldPerVictory = 10;

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

    // Each upgrade costs more than the last because it scales with the current value.
    public int GetUpgradeCost(DieFace face){
        int multiplier;
        switch(face.definition.rarity){
            case Rarity.Rare: multiplier = rareUpgradeMultiplier; break;
            case Rarity.Epic: multiplier = epicUpgradeMultiplier; break;
            default: multiplier = commonUpgradeMultiplier; break;
        }
        return multiplier * face.value;
    }
}
