using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections.Generic;

// One combatant's panel in battle: name, HP bar, Block/Fortify and a row of status chips with tooltips.
public class CombatantPanelUI : MonoBehaviour
{
    public TextMeshProUGUI nameText;
    public Image hpFill;
    public TextMeshProUGUI hpText;
    public TextMeshProUGUI defenseText;
    public TooltipTrigger defenseTooltip;
    public Transform statusRow;
    // Inactive chip that is copied once per active status.
    public TooltipTrigger statusChipTemplate;

    private readonly List<TooltipTrigger> chips = new List<TooltipTrigger>();

    public void Refresh(Entity entity){
        nameText.text = entity.entityName;
        // The fill is stretched to the bar; its right edge follows the HP ratio (no sprite needed).
        float ratio = entity.maxHP > 0 ? Mathf.Clamp01((float)entity.currentHP / entity.maxHP) : 0f;
        hpFill.rectTransform.anchorMax = new Vector2(ratio, 1f);
        hpText.text = entity.IsDead ? "DEFEATED" : entity.currentHP + " / " + entity.maxHP;

        int fortify = entity.GetStatus(StatusType.Fortify);
        string defense = "Block " + entity.currentBlock + "     Fortify " + fortify;
        defenseText.text = GameText.Colorize(defense);
        defenseTooltip.text = "<b>Defense</b>" + GameText.Glossary(defense);

        RefreshStatuses(entity);
    }

    // Fortify is shown with Block above; every other active status gets a chip.
    private void RefreshStatuses(Entity entity){
        int index = 0;
        foreach(KeyValuePair<StatusType, int> status in entity.Statuses){
            if(status.Value <= 0 || status.Key == StatusType.Fortify || status.Key == StatusType.None) continue;

            if(index >= chips.Count){
                TooltipTrigger chip = Instantiate(statusChipTemplate, statusRow);
                chips.Add(chip);
            }
            TooltipTrigger current = chips[index];
            current.gameObject.SetActive(true);
            string label = status.Key + " " + status.Value;
            current.GetComponentInChildren<TextMeshProUGUI>().text = GameText.Colorize(label);
            current.text = "<b>" + label + "</b>" + GameText.Glossary(status.Key.ToString());
            index++;
        }
        for(int i = index; i < chips.Count; i++){
            chips[i].gameObject.SetActive(false);
        }
    }
}
