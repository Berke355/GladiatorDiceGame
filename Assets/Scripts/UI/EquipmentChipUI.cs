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
    public Color normalColor = new Color(0.12f, 0.14f, 0.2f, 0.9f);
    public Color flashColor = new Color(0.85f, 0.65f, 0.2f, 0.95f);
    public float flashDuration = 0.6f;

    private Coroutine flashing;

    public void Set(EquipmentSO item){
        text.text = "<b>" + item.equipmentName + "</b>";
        tooltip.text = GameText.EquipmentTooltip(item);
        background.color = normalColor;
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
