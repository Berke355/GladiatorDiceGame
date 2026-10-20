using UnityEngine;
using TMPro;
using UnityEngine.UI;
using System.Collections.Generic;

// Battle-scene UI. States tell it what phase the turn is in; entity changes refresh the panels through OnHealthChanged.
public class UIManager : MonoBehaviour
{
    public static UIManager Instance { get; private set; }

    public Entity player;
    public Entity enemy;

    [Header("Combatants")]
    public CombatantPanelUI playerPanel;
    public CombatantPanelUI enemyPanel;

    [Header("Enemy Intent")]
    public TextMeshProUGUI intentText;
    public TooltipTrigger intentTooltip;

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
    public DieCharge charge;

    [Header("Equipment")]
    public EquipmentChipUI[] equipmentChips;

    [Header("Buttons")]
    public Button rollButton;
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
        enemy.OnHealthChanged += UpdateEnemyUI;

        UpdatePlayerUI();
        UpdateEnemyUI();
    }

    void OnDestroy(){
        if(Instance == this){
            Instance = null;
        }

        if(player != null){
            player.OnHealthChanged -= UpdatePlayerUI;
        }

        if(enemy != null){
            enemy.OnHealthChanged -= UpdateEnemyUI;
        }
    }

    // The run's items, one chip each; unused chips are hidden.
    public void ShowEquipment(List<EquipmentSO> equipment){
        for(int i = 0; i < equipmentChips.Length; i++){
            bool filled = i < equipment.Count;
            equipmentChips[i].gameObject.SetActive(filled);
            if(filled) equipmentChips[i].Set(equipment[i]);
        }
        shownEquipment = equipment;
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

    private void UpdateEnemyUI(){
        enemyPanel.Refresh(enemy);
    }

    // Start of the player's turn: only the roll button is active.
    public void ShowRollPhase(List<DieFace> die){
        rolledFaceTitle.text = "Roll the die";
        rolledFaceText.text = "";
        rolledFaceTooltip.text = "";
        UpdateDieChips(die, null);

        rollButton.interactable = true;
        for(int i = 0; i < actionButtons.Length; i++){
            actionButtons[i].interactable = false;
            actionButtonTexts[i].text = "<b>" + ActionLabel((ActionType)i) + "</b>";
        }
    }

    // After the roll: show the face and what each action would do; actions the face lacks stay disabled.
    public void ShowRolledFace(DieFace face, List<DieFace> die){
        rolledFaceTitle.text = face.DisplayName + "  <size=70%>Value " + ValueOf(face) + "</size>";
        rolledFaceText.text = GameText.Colorize(face.FullDescription);
        rolledFaceTooltip.text = GameText.FaceTooltip(face);
        UpdateDieChips(die, face);

        rollButton.interactable = false;
        for(int i = 0; i < actionButtons.Length; i++){
            string preview = DescribeAction(face, die, (ActionType)i);
            actionButtons[i].interactable = preview != "";
            actionButtonTexts[i].text = "<b>" + ActionLabel((ActionType)i) + "</b>"
                + (preview != "" ? "\n<size=75%>" + GameText.Colorize(preview) + "</size>" : "");
        }
    }

    public void UpdateEnemyIntentText(string intentDescription){
        intentText.text = GameText.Colorize(intentDescription);
        intentTooltip.text = "<b>Enemy intent</b>\n" + GameText.Colorize(intentDescription) + GameText.Glossary(intentDescription);
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
        EffectContext context = new EffectContext{ source = player, target = enemy, value = ValueOf(face), die = die, face = face, charge = charge };
        List<string> parts = new List<string>();
        foreach(EffectSO effect in face.GetEffects(action)){
            string description = effect.Describe(context);
            parts.Add(description != "" ? description : effect.name);
        }
        return string.Join(", ", parts);
    }

    private int ValueOf(DieFace face){
        return charge != null ? charge.ValueOf(face) : face.value;
    }

    private void UpdateDieChips(List<DieFace> die, DieFace rolled){
        for(int i = 0; i < dieChips.Length; i++){
            bool hasFace = i < die.Count;
            dieChips[i].gameObject.SetActive(hasFace);
            if(hasFace){
                dieChips[i].Set(die[i], die[i] == rolled, charge != null ? charge.Get(die[i]) : 0);
            }
        }
    }
}
