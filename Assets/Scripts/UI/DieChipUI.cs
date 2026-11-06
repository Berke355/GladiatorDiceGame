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
    public Color stoneColor = new Color(0.42f, 0.44f, 0.47f, 0.95f);

    // charge: the face's Charge this battle, shown as "+N" next to its value.
    // stoneTurns: player turns the face stays petrified (0 = not stone).
    public void Set(DieFace face, bool highlighted, int charge = 0, int stoneTurns = 0){
        string value = face.value + (charge > 0 ? " <color=#FFC857>+" + charge + "</color>" : "");
        text.text = "<b>" + face.DisplayName + "</b>\n" + (stoneTurns > 0 ? "STONE " + stoneTurns : value);
        tooltip.text = GameText.FaceTooltip(face)
            + (stoneTurns > 0 ? "\n<color=#A8B0B8>Petrified for " + stoneTurns + (stoneTurns == 1 ? " turn" : " turns") + ": when rolled, it can only gain " + Petrification.StoneBlock + " block.</color>" : "");
        background.color = highlighted ? highlightColor : stoneTurns > 0 ? stoneColor : normalColor;
    }
}
