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

    public void Set(DieFace face, bool highlighted){
        text.text = "<b>" + face.definition.faceName + "</b>\n" + face.value;
        tooltip.text = GameText.FaceTooltip(face.definition, face.value);
        background.color = highlighted ? highlightColor : normalColor;
    }
}
