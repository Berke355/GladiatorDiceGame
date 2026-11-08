using UnityEngine;
using UnityEngine.UI;
using TMPro;

// References for one equipment slot in the Market scene. MarketUI fills it.
public class EquipmentSlotUI : MonoBehaviour
{
    public Button slotButton;
    public TextMeshProUGUI text;
    // The item's description on hover.
    public TooltipTrigger tooltip;
}
