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
    // Optional: the picture of an item or an enchantment, where a face offer shows its tile.
    public Image icon;

    // A face offer: its tile and its full rules on the card. Shared by the Market and the run start. The card itself has
    // no tooltip: the rules are written out, and a keyword's definition shows when the pointer is over it.
    public void ShowFace(DieFaceSO face){
        if(tile != null){
            tile.gameObject.SetActive(true);
            tile.Set(face, face.baseValue);
        }
        ShowIcon(null);
        infoText.text = DescribeFace(face);
        if(tooltip != null) tooltip.text = "";
    }

    // Any other offer (an enchantment, an item): its picture, if it has one, and its text.
    public void ShowText(string text, string tooltipText = "", Sprite picture = null){
        if(tile != null) tile.gameObject.SetActive(false);
        ShowIcon(picture);
        infoText.text = text;
        if(tooltip != null) tooltip.text = tooltipText;
    }

    private void ShowIcon(Sprite picture){
        if(icon == null) return;
        icon.sprite = picture;
        icon.gameObject.SetActive(picture != null);
    }

    // Card text shared by every screen that offers a face: the name, the rarity and the face's full rules, one action per
    // line under its colored name (GameText.FaceRules); the value is on the tile.
    public static string DescribeFace(DieFaceSO face){
        return "<font=\"Cinzel SDF\"><b>" + face.faceName + "</b></font>\n" + RarityLabel(face.rarity) + "\n" + GameText.FaceRules(face.description);
    }

    public static string RarityName(Rarity rarity){
        return RarityLabel(rarity);
    }

    // Card text for an enchantment offer. An enchantment uses the value of the face it is inscribed on.
    public static string DescribeEnchantment(EnchantmentSO enchantment){
        return "<font=\"Cinzel SDF\"><b>" + enchantment.enchantmentName + "</b></font>\n"
            + RarityLabel(enchantment.rarity) + "\n"
            + "<size=80%>" + GameText.Colorize(enchantment.description) + "</size>";
    }

    public static string DescribeDieEnchantment(DieEnchantmentSO enchantment){
        return "<font=\"Cinzel SDF\"><b>" + enchantment.enchantmentName + "</b></font>\n"
            + RarityLabel(enchantment.rarity) + "\n"
            + "<size=80%>" + GameText.Colorize(enchantment.description) + "</size>";
    }

    // Card text for an equipment item (Grand Market offers, boss reward).
    public static string DescribeEquipment(EquipmentSO item){
        return "<font=\"Cinzel SDF\"><b>" + item.equipmentName + "</b></font>\n"
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
