using UnityEngine;
using UnityEngine.UI;
using TMPro;

// One face of the player's die in the battle strip, with its tooltip. The rolled face is highlighted.
public class DieChipUI : MonoBehaviour
{
    public Image background;
    public TextMeshProUGUI text;
    public TooltipTrigger tooltip;
    // Clickable only while the player chooses the next face.
    public Button button;
    public Color normalColor = new Color(0.12f, 0.14f, 0.2f, 0.9f);
    public Color highlightColor = new Color(0.85f, 0.65f, 0.2f, 0.95f);
    public Color stoneColor = new Color(0.42f, 0.44f, 0.47f, 0.95f);

    // charge: the face's Charge this battle, shown as "+N" next to its value.
    // stoneTurns: player turns the face stays petrified (0 = not stone).
    // used: a Consumable action of the face was used this battle (it is Change now).
    // layoutNote: the face's place on the net, added to the tooltip (see DieLayout.Describe).
    public void Set(DieFace face, bool highlighted, int charge = 0, int stoneTurns = 0, bool used = false, string layoutNote = ""){
        string value = face.value + (charge > 0 ? " <color=#FFC857>+" + charge + "</color>" : "");
        if(used) value = "<color=#9AA5B1>used: Change</color>";
        text.text = "<b>" + face.DisplayName + "</b>\n" + (stoneTurns > 0 ? "STONE " + stoneTurns : value);
        tooltip.text = GameText.FaceTooltip(face)
            + (stoneTurns > 0 ? "\n<color=#A8B0B8>Petrified for " + stoneTurns + (stoneTurns == 1 ? " turn" : " turns") + ": when rolled, it can only gain " + Petrification.StoneBlock + " block.</color>" : "")
            + (used ? "\n<color=#9AA5B1>Used this battle: its Consumable action is Change now.</color>" : "")
            + (layoutNote != "" ? "\n<color=#9AA5B1>" + layoutNote + "</color>" : "");
        background.color = highlighted ? highlightColor : stoneTurns > 0 ? stoneColor : normalColor;
    }
}
