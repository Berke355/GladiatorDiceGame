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
        RefreshPosture(entity);
    }

    // Posture (GDD "Deflect"), shown only when the die has a Deflect face (BattleManager sets it from BattleFlow): a
    // thin bar under the HP bar that fills from its middle out, red when close to breaking. Enemies and the player look
    // the same; the player's bar is cut into one step per timing mistake (ticks on both sides of the middle), so the
    // mistakes still allowed read at a glance.
    public static bool ShowPosture;
    private RectTransform postureRow;
    private Image postureFill;
    private readonly List<Image> ticks = new List<Image>();
    private int tickedFor = -1;
    private TextMeshProUGUI stunText;
    private static readonly Color PostureColor = new Color(0.91f, 0.71f, 0.29f, 1f);
    private static readonly Color PostureDanger = new Color(0.86f, 0.25f, 0.18f, 1f);
    private static readonly Color TickColor = new Color(0.08f, 0.06f, 0.05f, 1f);
    private const float PostureHeight = 12f;
    private const float TickWidth = 4f;

    private void RefreshPosture(Entity entity){
        bool show = ShowPosture && entity.MaxPosture > 0 && !entity.IsDead;
        if(!show){
            if(postureRow != null) postureRow.gameObject.SetActive(false);
            return;
        }
        if(postureRow == null) BuildPosture();
        postureRow.gameObject.SetActive(true);
        float ratio = Mathf.Clamp01((float)entity.Posture / entity.MaxPosture);
        postureFill.rectTransform.anchorMin = new Vector2(0.5f - ratio / 2f, 0f);
        postureFill.rectTransform.anchorMax = new Vector2(0.5f + ratio / 2f, 1f);
        bool lastStep = entity.whiffCostsPosture ? entity.MaxPosture - entity.Posture <= 1 : ratio > 0.75f;
        postureFill.color = lastStep ? PostureDanger : Color.Lerp(PostureColor, PostureDanger, ratio * ratio * 0.6f);
        if(entity.whiffCostsPosture && tickedFor != entity.MaxPosture) PlaceTicks(entity.MaxPosture);
        if(stunText != null){
            stunText.gameObject.SetActive(entity.StunTurns > 0);
            stunText.text = "STUNNED " + entity.StunTurns;
        }
    }

    // The steps of the player's bar: k mistakes fill k/max of it around the middle, so a step ends at 0.5 ± k/(2·max).
    private void PlaceTicks(int max){
        tickedFor = max;
        foreach(Image tick in ticks) Destroy(tick.gameObject);
        ticks.Clear();
        for(int k = 1; k < max; k++){
            foreach(int side in new[]{ -1, 1 }){
                GameObject mark = new GameObject("Tick", typeof(RectTransform), typeof(CanvasRenderer), typeof(Image));
                mark.transform.SetParent(postureRow, false);
                RectTransform rect = (RectTransform)mark.transform;
                float x = 0.5f + side * k / (2f * max);
                rect.anchorMin = new Vector2(x, 0f);
                rect.anchorMax = new Vector2(x, 1f);
                rect.sizeDelta = new Vector2(TickWidth, 0f);
                Image image = mark.GetComponent<Image>();
                image.color = TickColor;
                image.raycastTarget = false;
                ticks.Add(image);
            }
        }
        if(stunText != null) stunText.transform.SetAsLastSibling();
    }

    private void BuildPosture(){
        RectTransform bar = (RectTransform)hpFill.rectTransform.parent;
        GameObject row = new GameObject("Posture", typeof(RectTransform));
        row.transform.SetParent(bar, false);
        postureRow = (RectTransform)row.transform;
        postureRow.anchorMin = new Vector2(0f, 0f);
        postureRow.anchorMax = new Vector2(1f, 0f);
        postureRow.pivot = new Vector2(0.5f, 1f);
        postureRow.sizeDelta = new Vector2(0f, PostureHeight);
        postureRow.anchoredPosition = new Vector2(0f, -4f);
        Image back = row.AddComponent<Image>();
        back.color = new Color(0.08f, 0.06f, 0.05f, 0.85f);
        back.raycastTarget = false;
        GameObject fill = new GameObject("Fill", typeof(RectTransform), typeof(CanvasRenderer), typeof(Image));
        fill.transform.SetParent(row.transform, false);
        postureFill = fill.GetComponent<Image>();
        postureFill.raycastTarget = false;
        postureFill.rectTransform.offsetMin = postureFill.rectTransform.offsetMax = Vector2.zero;
        // STUNNED under the posture while it lasts.
        GameObject label = new GameObject("Stun", typeof(RectTransform));
        label.transform.SetParent(row.transform, false);
        stunText = label.AddComponent<TextMeshProUGUI>();
        stunText.font = hpText.font;
        stunText.fontSize = 30f;
        stunText.fontStyle = FontStyles.Bold;
        stunText.alignment = TextAlignmentOptions.Center;
        stunText.color = PostureColor;
        stunText.raycastTarget = false;
        RectTransform labelRect = stunText.rectTransform;
        labelRect.anchorMin = new Vector2(0f, 0f);
        labelRect.anchorMax = new Vector2(1f, 0f);
        labelRect.pivot = new Vector2(0.5f, 1f);
        labelRect.sizeDelta = new Vector2(0f, 34f);
        labelRect.anchoredPosition = new Vector2(0f, -2f);
        label.SetActive(false);
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
                SetChip(index++, GameText.StatusName(status.Key), status.Value);
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
        string icon = GameText.Icon(keyword);
        current.GetComponentInChildren<TextMeshProUGUI>().text = icon + (icon == keyword ? " " : "") + amount;
        current.text = "<b>" + label + "</b>" + GameText.Glossary(keyword);
    }
}
