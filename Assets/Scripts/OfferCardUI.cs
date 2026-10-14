using UnityEngine;
using UnityEngine.UI;
using TMPro;

// References for one offer card (Market faces and modules, starting face choice). The owning UI fills it.
public class OfferCardUI : MonoBehaviour
{
    public TextMeshProUGUI infoText;
    public Button buyButton;
    public TextMeshProUGUI buyButtonText;

    // Card text shared by every screen that offers a face.
    public static string DescribeFace(DieFaceSO face){
        return "<b>" + face.faceName + "</b>\n"
            + RarityLabel(face.rarity) + "\n"
            + "Value " + face.baseValue + "  (cap " + face.softCap + ")\n"
            + "<size=80%>" + GameText.Colorize(face.description) + "</size>";
    }

    // Card text for an action module offer. A module uses the value of the face it is attached to.
    public static string DescribeModule(ActionModuleSO module){
        return "<b>" + module.moduleName + "</b>\n"
            + RarityLabel(module.rarity) + "  ·  " + module.action + " module\n"
            + "<size=80%>" + GameText.Colorize(module.description) + "</size>";
    }

    private static string RarityLabel(Rarity rarity){
        switch(rarity){
            case Rarity.Rare: return "<color=#5DADE2>Rare</color>";
            case Rarity.Epic: return "<color=#BB8FCE>Epic</color>";
            default: return "<color=#D5D8DC>Common</color>";
        }
    }
}
