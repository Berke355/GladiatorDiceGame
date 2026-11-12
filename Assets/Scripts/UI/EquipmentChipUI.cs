using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections;

// One equipped item in the battle UI, with its tooltip. Flashes when the item triggers,
// so the player can see which item did what.
public class EquipmentChipUI : MonoBehaviour
{
    public Image background;
    public TextMeshProUGUI text;
    public TooltipTrigger tooltip;
    // Optional: the item's picture. An item with one shows only it (the name is in the tooltip); traits show their name.
    public Image icon;
    public Color normalColor = new Color(0.12f, 0.14f, 0.2f, 0.9f);
    public Color flashColor = new Color(0.85f, 0.65f, 0.2f, 0.95f);
    public float flashDuration = 0.6f;

    private Coroutine flashing;

    public void Set(EquipmentSO item){
        text.text = "<b>" + item.equipmentName + "</b>";
        ShowIcon(item.icon);
        tooltip.text = GameText.EquipmentTooltip(item);
        background.color = normalColor;
    }

    // Also used for enemy traits, which show the same way (name, tooltip, flash when they fire).
    public void SetTrait(EnemyTrait trait){
        text.text = "<b>" + trait.traitName + "</b>";
        ShowIcon(null);
        tooltip.text = "<b>" + trait.traitName + "</b>  <size=80%>Trait</size>\n" + GameText.Colorize(trait.description) + GameText.Glossary(trait.description);
        background.color = normalColor;
    }

    private void ShowIcon(Sprite picture){
        bool pictured = icon != null && picture != null;
        if(icon != null){
            icon.sprite = picture;
            icon.gameObject.SetActive(pictured);
        }
        text.gameObject.SetActive(!pictured);
    }

    public void Flash(){
        if(!isActiveAndEnabled) return;
        if(flashing != null) StopCoroutine(flashing);
        flashing = StartCoroutine(FlashRoutine());
    }

    private IEnumerator FlashRoutine(){
        for(float t = 0f; t < flashDuration; t += Time.deltaTime){
            background.color = Color.Lerp(flashColor, normalColor, t / flashDuration);
            yield return null;
        }
        background.color = normalColor;
        flashing = null;
    }
}
