using UnityEngine;
using TMPro;
using UnityEngine.UI;
using System.Collections.Generic;

// Battle-scene UI. States tell it what phase the turn is in; entity changes refresh the panels through OnHealthChanged.
public class UIManager : MonoBehaviour
{
    public static UIManager Instance { get; private set; }

    public Entity player;

    [Header("Combatants")]
    public CombatantPanelUI playerPanel;

    [Header("Enemies")]
    // One card per enemy (panel, intent, traits), copied from the inactive template into the row.
    public EnemyCardUI enemyCardTemplate;
    public Transform enemyCardRow;
    private readonly List<EnemyCardUI> enemyCards = new List<EnemyCardUI>();

    [Header("Rolled Face")]
    public TextMeshProUGUI rolledFaceTitle;
    public TextMeshProUGUI rolledFaceText;
    public TooltipTrigger rolledFaceTooltip;

    [Header("Die")]
    public DieChipUI[] dieChips;

    [Header("Boss Die")]
    // Shown only in boss battles: the boss's 6 faces, with the rolled one highlighted.
    public GameObject bossDieStrip;
    public DieChipUI[] bossDieChips;

    // This battle's Charge (set by BattleManager), so the die and the previews show battle values.
    [System.NonSerialized]
    public BattleFlow flow;

    [Header("Equipment")]
    public EquipmentChipUI[] equipmentChips;

    [Header("Buttons")]
    public Button rollButton;
    // Spends one of the battle's rerolls; active only after the die has landed.
    public Button rerollButton;
    public TextMeshProUGUI rerollButtonText;
    // Ends the turn without acting; active only after the die has landed.
    public Button passButton;
    // Indexed by ActionType (0 Attack, 1 Defense, 2 Magic), matching BattleManager.OnActionSelected.
    public Button[] actionButtons;
    public TextMeshProUGUI[] actionButtonTexts;

    [Header("Battle Result")]
    public GameObject battleResultPanel;
    public TextMeshProUGUI battleResultText;
    public TextMeshProUGUI battleResultButtonText;

    void Awake(){
        if (Instance != null && Instance != this){
            Destroy(this);
        }
        else{
            Instance = this;
        }
    }

    void Start(){
        player.OnHealthChanged += UpdatePlayerUI;
        UpdatePlayerUI();
    }

    void OnDestroy(){
        if(Instance == this){
            Instance = null;
        }

        if(player != null){
            player.OnHealthChanged -= UpdatePlayerUI;
        }

        foreach(EnemyCardUI card in enemyCards){
            if(card != null && card.Enemy != null) card.Enemy.entity.OnHealthChanged -= RefreshEnemies;
        }
    }

    // The run's items, one chip each; unused chips are hidden.
    public void ShowEquipment(List<EquipmentSO> equipment){
        // Items have no limit: the first chip is copied as they pile up.
        while(equipmentChips.Length < equipment.Count){
            System.Array.Resize(ref equipmentChips, equipmentChips.Length + 1);
            equipmentChips[equipmentChips.Length - 1] = Instantiate(equipmentChips[0], equipmentChips[0].transform.parent);
        }
        for(int i = 0; i < equipmentChips.Length; i++){
            bool filled = i < equipment.Count;
            equipmentChips[i].gameObject.SetActive(filled);
            if(filled) equipmentChips[i].Set(equipment[i]);
        }
        shownEquipment = equipment;
    }

    // One card per enemy, in turn order; onClick gets the enemy's index (target selection).
    public void ShowEnemies(List<BattleEnemy> enemies, System.Action<int> onClick){
        enemyCardTemplate.gameObject.SetActive(false);
        for(int i = 0; i < enemies.Count; i++){
            AddEnemyCard(enemies[i], i, onClick);
        }
    }

    // Also used for an enemy that joins during the battle (a Hydra head). Past 3 cards, all cards shrink to fit the row.
    public void AddEnemyCard(BattleEnemy enemy, int index, System.Action<int> onClick){
        EnemyCardUI card = Instantiate(enemyCardTemplate, enemyCardRow);
        card.gameObject.SetActive(true);
        card.name = "EnemyCard " + enemy.entity.entityName;
        card.Bind(enemy, () => onClick(index));
        enemy.entity.OnHealthChanged += RefreshEnemies;
        enemyCards.Add(card);

        HorizontalLayoutGroup row = enemyCardRow.GetComponent<HorizontalLayoutGroup>();
        if(row != null){
            row.childScaleWidth = true;
            row.childScaleHeight = true;
        }
        float scale = enemyCards.Count > MaxFullSizeCards ? (float)MaxFullSizeCards / enemyCards.Count : 1f;
        foreach(EnemyCardUI each in enemyCards){
            each.transform.localScale = new Vector3(scale, scale, 1f);
        }
        RefreshEnemies();
    }

    private const int MaxFullSizeCards = 3;

    // Every card's panel, intent (numbers change with Rage, Ritual and statuses) and the target marker.
    public void RefreshEnemies(){
        BattleEnemy target = flow != null ? flow.Target : null;
        foreach(EnemyCardUI card in enemyCards){
            card.Refresh(card.Enemy == target);
        }
    }

    // Called when an item triggers.
    public void FlashEquipment(EquipmentSO item){
        if(shownEquipment == null) return;
        int index = shownEquipment.IndexOf(item);
        if(index >= 0 && index < equipmentChips.Length) equipmentChips[index].Flash();
    }

    private List<EquipmentSO> shownEquipment;

    private void UpdatePlayerUI(){
        playerPanel.Refresh(player);
    }

    // Each face on the net reports its position when clicked (choosing the next face).
    public void WireDieChips(System.Action<int> onClick){
        for(int i = 0; i < dieChips.Length; i++){
            int position = i;
            if(dieChips[i].button != null) dieChips[i].button.onClick.AddListener(() => onClick(position));
        }
    }

    private void SetDieChipsClickable(bool clickable){
        foreach(DieChipUI chip in dieChips){
            if(chip.button != null) chip.button.interactable = clickable;
        }
    }

    // An action lets the player choose the next face: only the faces on the net are active.
    public void ShowChooseFace(List<DieFace> die){
        rolledFaceTitle.text = "Choose a face";
        rolledFaceText.text = "Click a face on your die: it comes up next.";
        rolledFaceTooltip.text = "";
        UpdateDieChips(die, null);
        RefreshEnemies();
        rollButton.interactable = false;
        UpdateRerollButton(false);
        if(passButton != null) passButton.interactable = false;
        for(int i = 0; i < actionButtons.Length; i++){
            actionButtons[i].interactable = false;
            actionButtonTexts[i].text = "<b>" + ActionLabel((ActionType)i) + "</b>";
        }
        SetDieChipsClickable(true);
    }

    // Start of the player's turn: only the roll button is active.
    public void ShowRollPhase(List<DieFace> die){
        rolledFaceTitle.text = "Roll the die";
        rolledFaceText.text = "";
        rolledFaceTooltip.text = "";
        UpdateDieChips(die, null);

        RefreshEnemies();
        SetDieChipsClickable(false);
        rollButton.interactable = true;
        UpdateRerollButton(false);
        if(passButton != null) passButton.interactable = false;
        for(int i = 0; i < actionButtons.Length; i++){
            actionButtons[i].interactable = false;
            actionButtonTexts[i].text = "<b>" + ActionLabel((ActionType)i) + "</b>";
        }
    }

    // After the roll: show the face and what each action would do; actions the face lacks stay disabled.
    public void ShowRolledFace(DieFace face, List<DieFace> die){
        bool stone = flow != null && flow.petrification.IsStone(face);
        rolledFaceTitle.text = face.DisplayName + "  <size=70%>" + (stone ? "<color=#A8B0B8>Petrified</color>" : "Value " + ValueOf(face)) + "</size>";
        rolledFaceText.text = GameText.Colorize(face.FullDescription);
        rolledFaceTooltip.text = GameText.FaceTooltip(face);
        UpdateDieChips(die, face);

        RefreshEnemies();
        SetDieChipsClickable(false);
        rollButton.interactable = false;
        UpdateRerollButton(flow != null && flow.CanReroll);
        if(passButton != null) passButton.interactable = true;
        for(int i = 0; i < actionButtons.Length; i++){
            string preview = DescribeAction(face, die, (ActionType)i);
            actionButtons[i].interactable = preview != "" && (flow == null || flow.CanAct(face, (ActionType)i));
            actionButtonTexts[i].text = "<b>" + ActionLabel((ActionType)i) + "</b>"
                + (preview != "" ? "\n<size=75%>" + GameText.Colorize(preview) + "</size>" : "");
        }
    }

    // runComplete: the final boss was beaten, which ends the run.
    public void ShowBattleResult(bool playerWon, int goldEarned, bool runComplete = false){
        battleResultPanel.SetActive(true);
        battleResultText.text = runComplete
            ? "RUN COMPLETE\n<size=50%>The arena is yours</size>"
            : playerWon ? "VICTORY\n<size=50%>+" + goldEarned + " Gold</size>" : "DEFEAT";
        battleResultButtonText.text = playerWon && !runComplete ? "Continue" : "New Run";
    }

    // die is null for normal enemies, which hides the strip.
    public void ShowBossDie(List<DieFace> die, DieFace rolled){
        bossDieStrip.SetActive(die != null);
        if(die == null) return;
        for(int i = 0; i < bossDieChips.Length; i++){
            bool hasFace = i < die.Count;
            bossDieChips[i].gameObject.SetActive(hasFace);
            if(hasFace) bossDieChips[i].Set(die[i], die[i] == rolled);
        }
    }

    // ToUpperInvariant, not ToUpper: on a Turkish system "Magic".ToUpper() becomes "MAGİC".
    private static string ActionLabel(ActionType action){
        return action.ToString().ToUpperInvariant();
    }

    // Short text of every effect the face runs for this action with the real numbers (Rage, Ritual, stacks),
    // e.g. "5 damage, 4 Bleed". Empty if the face has no effect for this action.
    private string DescribeAction(DieFace face, List<DieFace> die, ActionType action){
        if(flow != null && flow.petrification.IsStone(face)){
            return action == ActionType.Defense ? Petrification.StoneBlock + " block (petrified)" : "";
        }
        if(flow != null && flow.IsChange(face, action)){
            return flow.ChangesThisTurn < BattleFlow.MaxChangesPerTurn ? "Change: roll another face" : "";
        }
        EffectContext context = new EffectContext{ source = player, target = flow != null ? flow.Target.entity : null, opponents = flow != null ? flow.LivingEnemyEntities() : null, value = ValueOf(face), die = die, face = face, charge = flow != null ? flow.charge : null, rerolls = flow != null ? flow.rerolls : null, faceLandings = flow != null ? flow.LandingsThisTurn(face) : 0, arrivedBySteer = flow != null && flow.ArrivedBySteer, turnNumber = flow != null ? flow.TurnNumber : 0, actionChanged = flow != null && flow.WouldChangeAction(action) };
        List<string> parts = new List<string>();
        foreach(EffectSO effect in face.GetEffects(action)){
            string description = effect.Describe(context);
            parts.Add(description != "" ? description : effect.name);
        }
        string text = string.Join(", ", parts);
        return text != "" && (flow != null ? flow.IsConsumableFace(face) : face.definition.IsConsumable) ? text + " (Consumable)" : text;
    }

    private void UpdateRerollButton(bool canReroll){
        if(rerollButton == null) return;
        rerollButton.interactable = canReroll;
        rerollButtonText.text = "Reroll (" + (flow != null ? flow.rerolls.Left : 0) + ")";
    }

    private int ValueOf(DieFace face){
        return flow != null ? flow.charge.ValueOf(face) : face.value;
    }

    private void UpdateDieChips(List<DieFace> die, DieFace rolled){
        for(int i = 0; i < dieChips.Length; i++){
            bool hasFace = i < die.Count;
            dieChips[i].gameObject.SetActive(hasFace);
            if(hasFace){
                dieChips[i].Set(die[i], die[i] == rolled, flow != null ? flow.charge.ValueOf(die[i]) - die[i].value : 0, flow != null ? flow.petrification.TurnsLeft(die[i]) : 0, flow != null && flow.consumables.IsUsed(die[i]), DieLayout.Describe(die, i));
            }
        }
    }
}
