using UnityEngine;
using UnityEngine.UI;
using TMPro;

// One enemy's card in battle: its panel (name, HP, Block/Fortify, statuses), its intent and its traits.
// Clicking the card selects the enemy as the target; the target's card shows a marker. Copied from a
// template by UIManager, once per enemy.
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
        intentText.text = dead ? "<color=#888888>Defeated</color>" : GameText.Colorize(intent);
        intentTooltip.text = dead ? "" : "<b>" + Enemy.entity.entityName + "'s intent</b>\n" + GameText.Colorize(intent) + GameText.Glossary(intent);
        targetMarker.SetActive(targeted && !dead);
        selectButton.interactable = !dead;
        if(canvasGroup != null) canvasGroup.alpha = dead ? 0.45f : 1f;
    }

    private void FlashTrait(int index){
        if(index >= 0 && index < traitChips.Length) traitChips[index].Flash();
    }
}
