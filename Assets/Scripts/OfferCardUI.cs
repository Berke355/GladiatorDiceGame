using UnityEngine;
using UnityEngine.UI;
using TMPro;

// References for one offer card (Market faces, enchantments and equipment, boss rewards, starting face choice). The owning UI fills it.
public class OfferCardUI : MonoBehaviour
{
    public TextMeshProUGUI infoText;
    public Button buyButton;
    public TextMeshProUGUI buyButtonText;
    // Optional (Market cards only): keeps the offer through rerolls and into the next Market.
    public Button lockButton;
    public TextMeshProUGUI lockButtonText;

    // Card text shared by every screen that offers a face.
    public static string DescribeFace(DieFaceSO face){
        return "<b>" + face.faceName + "</b>\n"
            + RarityLabel(face.rarity) + "\n"
            + "Value " + face.baseValue + "\n"
            + "<size=80%>" + GameText.Colorize(face.description) + "</size>";
    }

    // Card text for an enchantment offer. An enchantment uses the value of the face it is inscribed on.
    public static string DescribeEnchantment(EnchantmentSO enchantment){
        return "<b>" + enchantment.enchantmentName + "</b>\n"
            + RarityLabel(enchantment.rarity) + "\n"
            + "<size=80%>" + GameText.Colorize(enchantment.description) + "</size>";
    }

    public static string DescribeDieEnchantment(DieEnchantmentSO enchantment){
        return "<b>" + enchantment.enchantmentName + "</b>\n"
            + RarityLabel(enchantment.rarity) + "\n"
            + "<size=80%>" + GameText.Colorize(enchantment.description) + "</size>";
    }

    // Card text for an equipment item (Grand Market offers, boss reward).
    public static string DescribeEquipment(EquipmentSO item){
        return "<b>" + item.equipmentName + "</b>\n"
            + RarityLabel(item.rarity) + "\n"
            + "<size=80%>" + GameText.Colorize(item.description) + "</size>";
    }

    // One-line version for the equipment slots.
    private static string RarityLabel(Rarity rarity){
        switch(rarity){
            case Rarity.Rare: return "<color=#5DADE2>Rare</color>";
            case Rarity.Epic: return "<color=#BB8FCE>Epic</color>";
            default: return "<color=#D5D8DC>Common</color>";
        }
    }
}
