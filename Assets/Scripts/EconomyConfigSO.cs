using UnityEngine;

// All economy numbers in one asset so they can be tuned in the Inspector (see GDD "Prices").
[CreateAssetMenu(fileName = "EconomyConfig", menuName = "Dice/Economy Config")]
public class EconomyConfigSO : ScriptableObject
{
    public int goldPerVictory = 10;

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
