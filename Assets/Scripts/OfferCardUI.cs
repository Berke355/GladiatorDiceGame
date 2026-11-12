using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections.Generic;

// References for one offer card (Market faces, enchantments and equipment, boss rewards, starting face choice). The owning UI fills it.
public class OfferCardUI : MonoBehaviour
{
    public TextMeshProUGUI infoText;
    public Button buyButton;
    public TextMeshProUGUI buyButtonText;
    // Optional (Market cards only): keeps the offer through rerolls and into the next Market.
    public Button lockButton;
    public TextMeshProUGUI lockButtonText;
    // Optional (face offers): the face as a tile, and its full rules on hover.
    public FaceTileUI tile;
    public TooltipTrigger tooltip;

    // A face offer: its tile, its card text and its full rules in the tooltip. Shared by the Market and the run start.
    public void ShowFace(DieFaceSO face){
        if(tile != null){
            tile.gameObject.SetActive(true);
            tile.Set(face, face.baseValue);
        }
        infoText.text = DescribeFace(face);
        if(tooltip != null) tooltip.text = GameText.FaceTooltip(face, face.baseValue);
    }

    // Any other offer (an enchantment, an item): text only.
    public void ShowText(string text, string tooltipText = ""){
        if(tile != null) tile.gameObject.SetActive(false);
        infoText.text = text;
        if(tooltip != null) tooltip.text = tooltipText;
    }

    // Card text shared by every screen that offers a face, in the card language (GDD "Visual Style"): the name, the
    // rarity and one line of icons per action; the value is on the tile and the rules are on hover.
    public static string DescribeFace(DieFaceSO face){
        System.Text.StringBuilder text = new System.Text.StringBuilder();
        text.Append("<font=\"Cinzel SDF\"><b>" + face.faceName + "</b></font>\n" + RarityLabel(face.rarity));
        DieFace instance = new DieFace(face);
        foreach(ActionType action in System.Enum.GetValues(typeof(ActionType))){
            List<string> parts = new List<string>();
            foreach(EffectSO effect in instance.GetEffects(action)){
                string description = effect.Describe(new EffectContext{ value = face.baseValue, face = instance });
                if(description != "") parts.Add(description);
            }
            if(parts.Count == 0) continue;
            text.Append("\n<size=70%><color=#C9B796>" + action.ToString().ToUpperInvariant() + "</color></size>  " + GameText.Iconize(string.Join(", ", parts)));
        }
        if(face.keywords != null){
            foreach(FaceKeyword keyword in face.keywords) text.Append("\n" + GameText.Colorize(keyword.ToString()));
        }
        return text.ToString();
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
