using UnityEngine;
using TMPro;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;

// Battle-scene UI. States tell it what phase the turn is in; entity changes refresh the panels through OnHealthChanged.
public class UIManager : MonoBehaviour
{
    public static UIManager Instance { get; private set; }

    public Entity player;

    [Header("Combatants")]
    public CombatantPanelUI playerPanel;

    [Header("Enemies")]
    // One card per enemy (plate, intent, traits), copied from the inactive template into the layer and placed on the
    // enemy's sprite every frame. The layer stretches over the whole canvas with its pivot at the bottom left.
    public EnemyCardUI enemyCardTemplate;
    public RectTransform enemyCardRow;
    private readonly List<EnemyCardUI> enemyCards = new List<EnemyCardUI>();

    [Header("Rolled Face")]
    public TextMeshProUGUI rolledFaceTitle;
    public TextMeshProUGUI rolledFaceText;
    public TooltipTrigger rolledFaceTooltip;
    // The rolled face as a big tile next to its name and rules; hidden until the die lands.
    public FaceTileUI rolledFaceTile;

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

    // A band across the middle of the screen that names whose turn it is (optional).
    public CanvasGroup turnBanner;
    public TextMeshProUGUI turnBannerText;

    // HP changes pop as numbers over the heads (optional).
    public FloatingNumbersUI floatingNumbers;

    [Header("Top Bar")]
    public TextMeshProUGUI battleInfoText;
    public TextMeshProUGUI goldText;

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
        if(floatingNumbers != null) floatingNumbers.Track(player);
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

    // Also used for an enemy that joins during the battle (a Hydra head).
    public void AddEnemyCard(BattleEnemy enemy, int index, System.Action<int> onClick){
        EnemyCardUI card = Instantiate(enemyCardTemplate, enemyCardRow);
        card.gameObject.SetActive(true);
        card.name = "EnemyCard " + enemy.entity.entityName;
        card.Bind(enemy, () => onClick(index));
        enemy.entity.OnHealthChanged += RefreshEnemies;
        if(floatingNumbers != null) floatingNumbers.Track(enemy.entity);
        enemyCards.Add(card);
        RefreshEnemies();
    }

    // Sprites move when an enemy joins (the row is arranged again), so the cards follow them every frame.
    void LateUpdate(){
        Camera view = Camera.main;
        if(view == null) return;
        Vector2 canvasSize = enemyCardRow.rect.size;
        foreach(EnemyCardUI card in enemyCards){
            if(card == null || card.Enemy == null) continue;
            SpriteRenderer renderer = card.Enemy.entity.GetComponent<SpriteRenderer>();
            if(renderer == null || renderer.sprite == null) continue;
            // From the sprite's own bounds, so a hidden (dead) enemy keeps its place.
            Bounds local = renderer.sprite.bounds;
            Transform sprite = renderer.transform;
            Vector3 feet = view.WorldToViewportPoint(sprite.TransformPoint(new Vector3(local.center.x, local.min.y, 0f)));
            Vector3 top = view.WorldToViewportPoint(sprite.TransformPoint(new Vector3(local.center.x, local.max.y, 0f)));
            float worldWidth = 2f * view.orthographicSize * view.aspect;
            float slot = card.Enemy.slotWidth > 0f ? card.Enemy.slotWidth : worldWidth * 0.1f;
            card.Place(Vector2.Scale(feet, canvasSize), (top.y - feet.y) * canvasSize.y, slot / worldWidth * canvasSize.x);
        }
    }

    // The top bar: which battle this is and the run's gold.
    public void ShowBattleInfo(int battle, int lastBattle, int gold, bool boss){
        if(battleInfoText != null) battleInfoText.text = "BATTLE " + battle + " / " + lastBattle + (boss ? "  <color=#E2B23C>BOSS</color>" : "");
        if(goldText != null) goldText.text = GameText.Icon("Gold") + " " + gold;
    }

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
        actionPhase = false;
        rolledFaceTitle.text = "Choose a face";
        rolledFaceText.text = "Click a face on your die: it comes up next.";
        rolledFaceTooltip.text = "";
        ShowRolledTile(null);
        UpdateDieChips(die, null);
        RefreshEnemies();
        rollButton.interactable = false;
        rollButton.gameObject.SetActive(false);
        UpdateRerollButton(false);
        if(passButton != null) passButton.interactable = false;
        for(int i = 0; i < actionButtons.Length; i++){
            actionButtons[i].interactable = false;
            actionButtonTexts[i].text = ActionTitle((ActionType)i);
        }
        SetDieChipsClickable(true);
    }

    // The enemies act: every button waits until the player's next turn.
    public void ShowEnemyTurn(){
        actionPhase = false;
        rolledFaceTitle.text = "Enemy turn";
        rolledFaceText.text = "";
        rolledFaceTooltip.text = "";
        rollButton.interactable = false;
        rollButton.gameObject.SetActive(false);
        UpdateRerollButton(false);
        if(passButton != null) passButton.interactable = false;
        for(int i = 0; i < actionButtons.Length; i++) actionButtons[i].interactable = false;
        SetDieChipsClickable(false);
    }

    // Start of the player's turn: only the roll button is active.
    public void ShowRollPhase(List<DieFace> die){
        actionPhase = false;
        rolledFaceTitle.text = "Roll the die";
        rolledFaceText.text = "";
        rolledFaceTooltip.text = "";
        ShowRolledTile(null);
        UpdateDieChips(die, null);

        RefreshEnemies();
        SetDieChipsClickable(false);
        rollButton.interactable = true;
        rollButton.gameObject.SetActive(true);
        UpdateRerollButton(false);
        if(passButton != null) passButton.interactable = false;
        for(int i = 0; i < actionButtons.Length; i++){
            actionButtons[i].interactable = false;
            actionButtonTexts[i].text = ActionTitle((ActionType)i);
        }
    }

    // After the roll: show the face and what each action would do; actions the face lacks stay disabled.
    // landed: the die was just rolled (a roll, a reroll, Change/Flip/Tumble), so the tray plays the roll; a refresh
    // (a new target) doesn't.
    public void ShowRolledFace(DieFace face, List<DieFace> die, bool landed = false){
        bool stone = flow != null && flow.petrification.IsStone(face);
        rolledFaceTitle.text = face.DisplayName + "  <size=70%>" + (stone ? "<color=#A8B0B8>Petrified</color>" : "Value " + ValueOf(face)) + "</size>";
        // The tray shows the face's full rules, one action per line; keywords show their definitions on hover, so the
        // tray itself has no tooltip.
        rolledFaceText.text = stone ? "<color=#A8B0B8>Petrified: its only action is Defense.</color>" : GameText.FaceRules(face.FullDescription);
        rolledFaceTooltip.text = "";
        if(landed) PlayRoll(face, die, stone);
        else ShowRolledTile(face, stone);
        UpdateDieChips(die, face);

        RefreshEnemies();
        SetDieChipsClickable(false);
        rollButton.interactable = false;
        // The tray holds the die now; Roll comes back at the next roll phase.
        rollButton.gameObject.SetActive(false);
        actionPhase = true;
        shownFace = face;
        shownDie = die;
        // While the die tumbles nothing can be chosen; the buttons open when it lands (RollAnimation's end).
        bool tumbling = rollAnimation != null;
        UpdateRerollButton(!tumbling && flow != null && flow.CanReroll);
        if(passButton != null) passButton.interactable = !tumbling;
        for(int i = 0; i < actionButtons.Length; i++){
            string preview = DescribeAction(face, die, (ActionType)i);
            actionButtons[i].interactable = !tumbling && preview != "" && (flow == null || flow.CanAct(face, (ActionType)i));
            actionButtonTexts[i].text = ActionTitle((ActionType)i)
                + (preview != "" ? "\n<size=85%>" + GameText.Colorize(preview) + "</size>" : "");
        }
    }

    // runComplete: the final boss was beaten, which ends the run.
    public void ShowBattleResult(bool playerWon, int goldEarned, bool runComplete = false){
        actionPhase = false;
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

    // The button's title line in the title font (the rest of the button is in the number font).
    private static string ActionTitle(ActionType action){
        return "<font=\"Cinzel SDF\"><b>" + ActionLabel(action) + "</b></font>";
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
        rerollButtonText.text = GameText.Icon("Reroll") + " Reroll " + (flow != null ? flow.rerolls.Left : 0);
    }

    private void ShowRolledTile(DieFace face, bool stone = false){
        if(rolledFaceTile == null || rollAnimation != null) return;
        rolledFaceTile.gameObject.SetActive(face != null);
        if(face != null) rolledFaceTile.Set(face, ValueOf(face), "", stone);
    }

    // The dice tray: the tile tumbles through the die's faces, then lands on the rolled one at a slight angle.
    // Only the picture moves; the rules already happened.
    private Coroutine rollAnimation;
    private const float RestAngle = -6f;

    private void PlayRoll(DieFace face, List<DieFace> die, bool stone){
        if(rolledFaceTile == null) return;
        if(rollAnimation != null) StopCoroutine(rollAnimation);
        rollAnimation = null;
        rolledFaceTile.gameObject.SetActive(true);
        rollAnimation = StartCoroutine(RollAnimation(face, die, stone));
    }

    private IEnumerator RollAnimation(DieFace face, List<DieFace> die, bool stone){
        RectTransform tile = (RectTransform)rolledFaceTile.transform;
        const int Tumbles = 7;
        for(int i = 0; i < Tumbles; i++){
            DieFace shown = die[Random.Range(0, die.Count)];
            rolledFaceTile.Set(shown, ValueOf(shown));
            tile.localRotation = Quaternion.Euler(0f, 0f, Random.Range(-28f, 28f));
            float size = 0.8f + 0.15f * i / Tumbles;
            tile.localScale = new Vector3(size, size, 1f);
            yield return new WaitForSecondsRealtime(0.04f + 0.012f * i);
        }
        rolledFaceTile.Set(face, ValueOf(face), "", stone);
        tile.localRotation = Quaternion.Euler(0f, 0f, RestAngle);
        tile.localScale = new Vector3(1.1f, 1.1f, 1f);
        yield return new WaitForSecondsRealtime(0.07f);
        tile.localScale = Vector3.one;
        rollAnimation = null;
        // The die has landed: open the buttons, unless the phase moved on meanwhile (the battle ended).
        if(actionPhase && shownFace != null) ShowRolledFace(shownFace, shownDie);
    }

    // Whether the action buttons belong to the current phase, and the face they show (for RollAnimation's end).
    private bool actionPhase;
    private DieFace shownFace;
    private List<DieFace> shownDie;

    // True while the die tumbles in the tray (BattleManager ignores actions meanwhile).
    public bool IsRolling => rollAnimation != null;

    // "YOUR TURN" / "ENEMY TURN" across the middle of the screen: it pops in, holds and fades.
    public const float BannerDuration = 0.9f;
    private Coroutine bannerAnimation;

    public void ShowTurnBanner(string text){
        if(turnBanner == null) return;
        if(bannerAnimation != null) StopCoroutine(bannerAnimation);
        turnBannerText.text = text;
        bannerAnimation = StartCoroutine(BannerAnimation());
    }

    private IEnumerator BannerAnimation(){
        turnBanner.gameObject.SetActive(true);
        RectTransform rect = (RectTransform)turnBanner.transform;
        float time = 0f;
        while(time < BannerDuration){
            time += Time.deltaTime;
            float t = time / BannerDuration;
            turnBanner.alpha = t < 0.15f ? t / 0.15f : t > 0.7f ? 1f - (t - 0.7f) / 0.3f : 1f;
            float scale = t < 0.15f ? Mathf.Lerp(1.25f, 1f, t / 0.15f) : 1f;
            rect.localScale = new Vector3(1f, scale, 1f);
            yield return null;
        }
        turnBanner.alpha = 0f;
        turnBanner.gameObject.SetActive(false);
        bannerAnimation = null;
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
