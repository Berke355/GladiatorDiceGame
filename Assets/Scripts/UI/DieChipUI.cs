using UnityEngine;
using UnityEngine.UI;
using TMPro;

// One face of the player's die in the battle strip, with its tooltip. The rolled face is highlighted.
public class DieChipUI : MonoBehaviour
{
    public Image background;
    public TextMeshProUGUI text;
    public TooltipTrigger tooltip;
    public Color normalColor = new Color(0.12f, 0.14f, 0.2f, 0.9f);
    public Color highlightColor = new Color(0.85f, 0.65f, 0.2f, 0.95f);

    // charge: the face's Charge this battle, shown as "+N" next to its value.
    public void Set(DieFace face, bool highlighted, int charge = 0){
        text.text = "<b>" + face.DisplayName + "</b>\n" + face.value + (charge > 0 ? " <color=#FFC857>+" + charge + "</color>" : "");
        tooltip.text = GameText.FaceTooltip(face);
        background.color = highlighted ? highlightColor : normalColor;
    }
}
