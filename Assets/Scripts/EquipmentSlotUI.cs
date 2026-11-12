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

    public void Show(Sprite picture, string caption, string tooltipText){
        if(icon != null){
            icon.sprite = picture;
            icon.gameObject.SetActive(picture != null);
        }
        text.text = caption;
        tooltip.text = tooltipText;
    }
}
