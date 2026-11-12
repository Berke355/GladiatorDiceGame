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
    // Optional: the item's or enchantment's picture, left of its name.
    public Image icon;
    // Only the picture, the name and rules on hover (the Market's owned items, which can be many).
    public bool iconOnly;

    public void Show(Sprite picture, string caption, string tooltipText){
        if(icon != null){
            icon.sprite = picture;
            icon.gameObject.SetActive(picture != null);
        }
        text.gameObject.SetActive(!(iconOnly && picture != null));
        text.text = caption;
        tooltip.text = tooltipText;
    }
}
