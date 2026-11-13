using UnityEngine;
using UnityEngine.UI;
using TMPro;

// One enemy's card in battle, standing on its sprite: the root covers the sprite (clicking it selects the enemy as
// the target), the plate under its feet holds the panel (name, HP, statuses) and the target marker, and the head
// group above it holds the intent and the traits. Copied from a template by UIManager, once per enemy, and placed
// on the sprite every frame (Place).
public class EnemyCardUI : MonoBehaviour
{
    public CombatantPanelUI panel;
    public TextMeshProUGUI intentText;
    public TooltipTrigger intentTooltip;
    public GameObject traitStrip;
    public EquipmentChipUI[] traitChips;
    // Shown on the selected target's card.
    public GameObject targetMarker;
    public Button selectButton;
    // Dims a defeated enemy's card.
    public CanvasGroup canvasGroup;
    // Above the sprite's head: the intent and the traits.
    public RectTransform head;

    public BattleEnemy Enemy { get; private set; }

    public void Bind(BattleEnemy enemy, System.Action onClick){
        Enemy = enemy;
        int count = enemy.brain.Traits != null ? enemy.brain.Traits.Count : 0;
        traitStrip.SetActive(count > 0);
        for(int i = 0; i < traitChips.Length; i++){
            bool has = i < count;
            traitChips[i].gameObject.SetActive(has);
            if(has) traitChips[i].SetTrait(enemy.brain.Traits[i]);
        }
        enemy.traits.TraitTriggered += FlashTrait;
        selectButton.onClick.AddListener(() => onClick());
        Refresh(false);
    }

    void OnDestroy(){
        if(Enemy != null && Enemy.traits != null) Enemy.traits.TraitTriggered -= FlashTrait;
    }

    public void Refresh(bool targeted){
        panel.Refresh(Enemy.entity);
        bool dead = Enemy.IsDead;
        string intent = dead ? "" : Enemy.brain.DescribeCurrentIntent();
        // Over the head: the intent in the card language, plus a waiting trait; the full text is in the tooltip.
        string summary = GameText.IntentSummary(intent);
        if(!dead && Enemy.traits != null && Enemy.traits.HasPending) summary += "\n" + GameText.Iconize(Enemy.traits.PendingDescription) + " next turn";
        // A trait waiting for its next turn (Curl Up) is shown with the intent.
        if(!dead && Enemy.traits != null && Enemy.traits.HasPending) intent += "\n" + Enemy.traits.PendingDescription + " (at the start of its turn)";
        intentText.text = dead ? "<color=#888888>Defeated</color>" : summary;
        // An enemy that joined this turn has no intent yet: no empty box over its head.
        intentTooltip.gameObject.SetActive(dead || intent != "");
        intentTooltip.text = dead ? "" : "<b>" + Enemy.entity.entityName + "'s intent</b>\n" + GameText.Colorize(intent) + GameText.Glossary(intent);
        targetMarker.SetActive(targeted && !dead);
        selectButton.interactable = !dead;
        if(canvasGroup != null) canvasGroup.alpha = dead ? 0.45f : 1f;
    }

    // feet: the sprite's bottom center in the parent's space (anchored at its bottom left); spriteHeight: its height;
    // slotWidth: the width the enemy has in the row. The plate and the head stretch to the card's width; a slot
    // narrower than MinWidth shrinks the whole card instead.
    public void Place(Vector2 feet, float spriteHeight, float slotWidth, bool plateOnTop = false){
        SetPlateOnTop(plateOnTop);
        RectTransform rect = (RectTransform)transform;
        float scale = Mathf.Min(1f, slotWidth / MinWidth);
        rect.anchoredPosition = feet;
        rect.localScale = new Vector3(scale, scale, 1f);
        rect.sizeDelta = new Vector2(Mathf.Max(slotWidth, MinWidth), spriteHeight / scale);
    }

    private const float MinWidth = 240f;

    // A mounted head (the Hydra's) has its body under it, so its plate goes over its head, with the intent above it.
    public RectTransform plate;
    private bool plateOnTop;
    private const float PlateGap = 6f, HeadGap = 12f;

    private void SetPlateOnTop(bool onTop){
        if(plate == null || onTop == plateOnTop) return;
        plateOnTop = onTop;
        plate.anchorMin = new Vector2(0f, onTop ? 1f : 0f);
        plate.anchorMax = new Vector2(1f, onTop ? 1f : 0f);
        plate.pivot = new Vector2(0.5f, onTop ? 0f : 1f);
        plate.anchoredPosition = new Vector2(0f, onTop ? PlateGap : -PlateGap);
        head.anchoredPosition = new Vector2(0f, onTop ? PlateGap + plate.sizeDelta.y + HeadGap : HeadGap);
    }

    private void FlashTrait(int index){
        if(index >= 0 && index < traitChips.Length) traitChips[index].Flash();
    }
}
