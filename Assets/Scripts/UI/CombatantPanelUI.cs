using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections.Generic;

// One combatant's plate in battle: name, HP bar and a row of chips (Block, Fortify, then every other status) with tooltips.
public class CombatantPanelUI : MonoBehaviour
{
    public TextMeshProUGUI nameText;
    public Image hpFill;
    public TextMeshProUGUI hpText;
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

        RefreshStatuses(entity);
    }

    // Block and Fortify come first, then every other active status. A defeated combatant shows none.
    private void RefreshStatuses(Entity entity){
        int index = 0;
        if(!entity.IsDead){
            if(entity.currentBlock > 0) SetChip(index++, "Block", entity.currentBlock);
            int fortify = entity.GetStatus(StatusType.Fortify);
            if(fortify > 0) SetChip(index++, "Fortify", fortify);
            foreach(KeyValuePair<StatusType, int> status in entity.Statuses){
                if(status.Value <= 0 || status.Key == StatusType.Fortify || status.Key == StatusType.None) continue;
                SetChip(index++, status.Key.ToString(), status.Value);
            }
        }
        for(int i = index; i < chips.Count; i++){
            chips[i].gameObject.SetActive(false);
        }
    }

    private void SetChip(int index, string keyword, int amount){
        if(index >= chips.Count){
            TooltipTrigger chip = Instantiate(statusChipTemplate, statusRow);
            chips.Add(chip);
        }
        TooltipTrigger current = chips[index];
        current.gameObject.SetActive(true);
        string label = keyword + " " + amount;
        current.GetComponentInChildren<TextMeshProUGUI>().text = GameText.Colorize(label);
        current.text = "<b>" + label + "</b>" + GameText.Glossary(keyword);
    }
}
