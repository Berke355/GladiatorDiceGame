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
    // Optional: the shields on the HP bar, after the HP in the order they absorb a hit from the outside in
    // (HP | Fortify | Block), so the bar shows how much a hit must get through.
    public Image fortifyFill;
    public Image blockFill;
    // Optional: the HP the owner's statuses will take at the end of its turn, marked inside the HP from its end
    // (Poison first, then Bleed, as Entity.OnTurnEnd takes them; Entity.NextTicks).
    public Image poisonTickFill;
    public Image bleedTickFill;
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
        RefreshShields(entity, ratio);
        RefreshTicks(entity);

        RefreshStatuses(entity);
    }

    // Fortify then Block follow the HP on the bar, each as wide as its share of max HP. When the bar runs out (HP near
    // full), the shields slide back over the HP's end (drawn semi-transparent), so they always show.
    private void RefreshShields(Entity entity, float hpRatio){
        if(fortifyFill == null || blockFill == null) return;
        float max = Mathf.Max(1, entity.maxHP);
        float fortify = entity.IsDead ? 0f : entity.GetStatus(StatusType.Fortify) / max;
        float block = entity.IsDead ? 0f : entity.currentBlock / max;
        float total = Mathf.Min(1f, fortify + block);
        float start = Mathf.Min(hpRatio, 1f - total);
        PlaceSegment(fortifyFill, start, Mathf.Min(1f, start + fortify));
        PlaceSegment(blockFill, Mathf.Min(1f, start + fortify), Mathf.Min(1f, start + fortify + block));
    }

    // Ticks skip Block and Fortify, so they come out of the HP itself: Poison at the HP's very end (it is taken first),
    // Bleed just before it.
    private void RefreshTicks(Entity entity){
        if(poisonTickFill == null || bleedTickFill == null) return;
        int poison = 0, bleed = 0;
        if(!entity.IsDead) entity.NextTicks(out poison, out bleed);
        float max = Mathf.Max(1, entity.maxHP);
        float end = Mathf.Clamp01(entity.currentHP / max);
        float afterPoison = Mathf.Clamp01((entity.currentHP - poison) / max);
        float afterBleed = Mathf.Clamp01((entity.currentHP - poison - bleed) / max);
        PlaceSegment(poisonTickFill, afterPoison, end);
        PlaceSegment(bleedTickFill, afterBleed, afterPoison);
    }

    private static void PlaceSegment(Image segment, float from, float to){
        segment.enabled = to > from;
        segment.rectTransform.anchorMin = new Vector2(from, 0f);
        segment.rectTransform.anchorMax = new Vector2(to, 1f);
        segment.rectTransform.offsetMin = Vector2.zero;
        segment.rectTransform.offsetMax = Vector2.zero;
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
        // The chip is the icon and the number; the tooltip keeps the name and the rule.
        current.GetComponentInChildren<TextMeshProUGUI>().text = GameText.Icon(keyword) + amount;
        current.text = "<b>" + label + "</b>" + GameText.Glossary(keyword);
    }
}
