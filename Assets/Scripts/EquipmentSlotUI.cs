using UnityEngine;
using UnityEngine.UI;
using TMPro;

// References for one equipment slot in the Market scene. MarketUI fills it.
public class EquipmentSlotUI : MonoBehaviour
{
    // Clicked to choose which item a new one replaces when every slot is full.
    public Button slotButton;
    public TextMeshProUGUI text;
}
