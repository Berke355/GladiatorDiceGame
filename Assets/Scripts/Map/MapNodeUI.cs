using UnityEngine;
using UnityEngine.UI;
using TMPro;

// One node on the map screen: references to its scene objects, set up by MapUI from a template.
public class MapNodeUI : MonoBehaviour
{
    public Button button;
    // The disc colored by the node's type.
    public Image disc;
    // The type's picture (the boss's own sprite on the boss node).
    public Image icon;
    // The gold ring around the node the player stands on and around the nodes they can go to.
    public Image ring;
    public TextMeshProUGUI label;
    public TooltipTrigger tooltip;
}
