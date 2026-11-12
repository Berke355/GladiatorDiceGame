using UnityEngine;
using UnityEngine.UI;
using TMPro;

// A die face drawn as a tile (GDD "Visual Style", Face tiles): the rim is the rarity, the band the family (the first
// tag other than Physical; Lifesteal counts as Bleed), the center a pale tint of the family, the emblem the face itself
// and a badge the value. The same tile shows a face on the die net, in the dice tray and in the Market, so a face
// always looks the same. Three corners show which actions the face has (GDD "Visual Style", action pips): Defense top
// left (blue), Attack bottom left (red), Magic top right (purple; an enchanted face shows its seal there instead).
public class FaceTileUI : MonoBehaviour
{
    public Image rim;
    public Image band;
    public Image center;
    public Image emblem;
    public TextMeshProUGUI valueText;
    // Dims the tile (a stone or used face).
    public Image overlay;
    // Optional: the seal of the face's enchantment in a corner (only a DieFace can carry one).
    public Image seal;
    // Optional: a small colored bubble in a corner for each action the face has.
    public GameObject attackPip;
    public GameObject defensePip;
    public GameObject magicPip;

    private static readonly Color Ivory = new Color32(0xE9, 0xDC, 0xC0, 0xFF);

    public void Set(DieFace face, int value, string valueSuffix = "", bool dimmed = false){
        Set(face != null ? face.definition : null, value, valueSuffix, dimmed);
        ShowSeal(face != null && face.enchantment != null ? face.enchantment.icon : null);
    }

    public void Set(DieFaceSO face, int value, string valueSuffix = "", bool dimmed = false){
        FaceTag? first = null;
        if(face != null && !face.plainTile){
            foreach(FaceTag tag in face.tags){
                if(tag == FaceTag.Physical) continue;
                first = tag == FaceTag.Lifesteal ? FaceTag.Bleed : tag;
                break;
            }
        }
        Color familyColor = first != null ? FamilyColor(first.Value) : Ivory;
        rim.color = RarityColor(face != null ? face.rarity : Rarity.Common);
        band.color = familyColor;
        center.color = first != null ? Color.Lerp(Ivory, familyColor, 0.22f) : Ivory;
        emblem.sprite = face != null ? face.emblem : null;
        emblem.enabled = emblem.sprite != null;
        if(valueText != null) valueText.text = face != null && face.hasNoValue ? valueSuffix : value + valueSuffix;
        if(overlay != null) overlay.enabled = dimmed;
        ShowPip(attackPip, HasAction(face, ActionType.Attack));
        ShowPip(defensePip, HasAction(face, ActionType.Defense));
        ShowPip(magicPip, HasAction(face, ActionType.Magic));
        ShowSeal(null);
    }

    // The face's own actions (an enchantment's Magic shows as its seal).
    private static bool HasAction(DieFaceSO face, ActionType action){
        if(face == null) return false;
        foreach(ActionEffect effect in face.effects){
            if(effect.actionType == action && effect.effectLogic != null) return true;
        }
        return false;
    }

    private static void ShowPip(GameObject pip, bool show){
        if(pip != null) pip.SetActive(show);
    }

    // The seal takes the Magic corner: an enchanted face has no Magic of its own.
    private void ShowSeal(Sprite sprite){
        if(seal == null) return;
        seal.sprite = sprite;
        seal.enabled = sprite != null;
        if(sprite != null) ShowPip(magicPip, false);
    }

    public static Color FamilyColor(FaceTag family){
        switch(family){
            case FaceTag.Bleed: return new Color32(0x7A, 0x20, 0x22, 0xFF);
            case FaceTag.Burn: return new Color32(0xB0, 0x54, 0x1E, 0xFF);
            case FaceTag.Poison: return new Color32(0x3A, 0x68, 0x28, 0xFF);
            case FaceTag.Ritual: return new Color32(0x58, 0x30, 0x78, 0xFF);
            case FaceTag.Rage: return new Color32(0x78, 0x34, 0x24, 0xFF);
            case FaceTag.Block: return new Color32(0x3A, 0x54, 0x6E, 0xFF);
            case FaceTag.Hammer: return new Color32(0x4E, 0x4C, 0x4A, 0xFF);
            case FaceTag.Fortune: return new Color32(0xA8, 0x80, 0x28, 0xFF);
            default: return new Color32(0xC4, 0xB2, 0x92, 0xFF);
        }
    }

    public static Color RarityColor(Rarity rarity){
        switch(rarity){
            case Rarity.Rare: return new Color32(0xC8, 0xCE, 0xD6, 0xFF);
            case Rarity.Epic: return new Color32(0xE2, 0xB2, 0x3C, 0xFF);
            default: return new Color32(0x9C, 0x6B, 0x3C, 0xFF);
        }
    }
}
