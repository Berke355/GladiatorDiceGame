using UnityEngine;
using UnityEngine.UI;
using TMPro;

// References for one face card (Market offers, starting face choice). The owning UI fills it.
public class OfferCardUI : MonoBehaviour
{
    public TextMeshProUGUI infoText;
    public Button buyButton;
    public TextMeshProUGUI buyButtonText;

    // Card text shared by every screen that offers a face.
    public static string DescribeFace(DieFaceSO face){
        return "<b>" + face.faceName + "</b>\n"
            + RarityLabel(face.rarity) + "\n"
            + "Value " + face.baseValue + "  (max " + face.maxValue + ")\n"
            + "<size=80%>" + face.description + "</size>";
    }

    private static string RarityLabel(Rarity rarity){
        switch(rarity){
            case Rarity.Rare: return "<color=#5DADE2>Rare</color>";
            case Rarity.Epic: return "<color=#BB8FCE>Epic</color>";
            default: return "<color=#D5D8DC>Common</color>";
        }
    }
}
