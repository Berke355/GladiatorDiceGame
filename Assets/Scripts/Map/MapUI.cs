using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

// The map screen (see GDD "Map"): draws the act's map on a parchment, floor by floor from left to right up to the boss,
// and sends a click on a reachable node to RunManager.EnterNode. Rests are played here: heal, or upgrade one face. The
// player can plan a route with a pencil and an eraser (MapDrawing).
public class MapUI : MonoBehaviour
{
    public TextMeshProUGUI titleText;
    public TextMeshProUGUI hintText;
    public TextMeshProUGUI infoText;
    // Nodes and lines are copied from these inactive templates into mapArea, centered on it.
    public RectTransform mapArea;
    public MapNodeUI nodeTemplate;
    public Image lineTemplate;
    // One picture per MapNodeType, in enum order; the boss node shows the boss itself.
    public Sprite[] typeIcons;
    // The drawing tools' pictures: Pencil, Eraser, Clear.
    public Sprite[] toolIcons;
    // A red ink circle drawn around the node the player stands on.
    public Sprite inkCircle;
    // Floors run left to right, lanes top to bottom.
    public float laneSpacing = 210f;
    public float floorSpacing = 290f;
    public float bossSize = 190f;

    [Header("Rest")]
    public GameObject restPanel;
    public TextMeshProUGUI restBody;
    public Button restHealButton;
    public TextMeshProUGUI restHealText;
    public Button restUpgradeButton;
    // The die's faces to pick from for an upgrade, copied from the template.
    public RectTransform restFaceRow;
    public Button restFaceTemplate;
    public Button restBackButton;

    [Header("Elite reward")]
    // A free item after an elite: one option per button (each with children named Icon, Name and Text), or skip.
    public GameObject rewardPanel;
    public Button[] rewardOptions;
    public Button rewardSkipButton;

    private RunData run;
    private readonly List<MapNodeUI> nodes = new List<MapNodeUI>();
    private readonly List<GameObject> drawn = new List<GameObject>();
    private readonly List<GameObject> faceButtons = new List<GameObject>();
    private List<int> reachable = new List<int>();

    void Start(){
        RunManager manager = RunManager.Instance;
        run = manager.run;
        if(!manager.UsesMap){
            manager.GoToNextBattle();
            return;
        }
        // Played directly from the Editor before a run had a map.
        if(run.map == null) manager.BeginAct(run.act);
        manager.PrepareReachable();
        AudioManager.PlayMusic(MusicTrack.RunStart);
        nodeTemplate.gameObject.SetActive(false);
        lineTemplate.gameObject.SetActive(false);
        if(restFaceTemplate != null) restFaceTemplate.gameObject.SetActive(false);
        restHealButton.onClick.AddListener(OnRestHeal);
        restUpgradeButton.onClick.AddListener(ShowUpgradeChoice);
        restBackButton.onClick.AddListener(ShowRestChoice);
        if(rewardPanel != null){
            for(int i = 0; i < rewardOptions.Length; i++){
                int index = i;
                rewardOptions[i].onClick.AddListener(() => OnRewardTaken(index));
            }
            rewardSkipButton.onClick.AddListener(OnRewardSkipped);
            rewardPanel.SetActive(false);
        }
        Draw();
        // The tools sit in the bottom bar's right end.
        MapDrawing.Build(mapArea, restBackButton, () => restPanel.activeSelf || (rewardPanel != null && rewardPanel.activeSelf), new Vector2(730f, -668f), toolIcons);
        restPanel.SetActive(false);
        if(run.restPending) ShowRestChoice();
        else if(run.eliteRewardChoices.Count > 0) ShowReward();
    }

    void Update(){
        // The nodes the player can go to breathe a little.
        float pulse = 1f + 0.06f * Mathf.Sin(Time.unscaledTime * 4f);
        foreach(int index in reachable){
            if(index < nodes.Count && nodes[index] != null && !run.restPending) nodes[index].transform.localScale = new Vector3(pulse, pulse, 1f);
        }
    }

    private Vector2 PositionOf(MapNode node){
        ActMap map = run.map;
        float left = -floorSpacing * map.floors / 2f;
        // The boss's big portrait stands a little apart from the last floor.
        if(node.type == MapNodeType.Boss) return new Vector2(left + floorSpacing * map.floors + bossSize * 0.25f, 0f);
        return new Vector2(left + floorSpacing * node.floor, -(node.lane - (map.lanes - 1) / 2f) * laneSpacing);
    }

    // A dash and its gap, tiled along each road; DashScale stretches the 8-pixel pattern to 24 canvas units.
    private const float DashScale = 1f / 3f;
    private static Sprite dash;
    private static Sprite DashSprite(){
        if(dash != null) return dash;
        Texture2D texture = new Texture2D(8, 2, TextureFormat.RGBA32, false){ filterMode = FilterMode.Point, wrapMode = TextureWrapMode.Repeat };
        for(int x = 0; x < 8; x++){
            for(int y = 0; y < 2; y++) texture.SetPixel(x, y, x < 5 ? Color.white : new Color(1f, 1f, 1f, 0f));
        }
        texture.Apply();
        dash = Sprite.Create(texture, new Rect(0f, 0f, 8f, 2f), new Vector2(0.5f, 0.5f), 100f, 0, SpriteMeshType.FullRect);
        return dash;
    }

    // Ink on the parchment: the open ways in red, the walked road dark, the rest faint.
    private static readonly Color OpenInk = new Color(0.62f, 0.12f, 0.08f, 1f);
    private static readonly Color WalkedInk = new Color(0.24f, 0.14f, 0.08f, 0.95f);
    private static readonly Color FaintInk = new Color(0.36f, 0.25f, 0.16f, 0.45f);
    private static readonly Color LabelInk = new Color(0.22f, 0.13f, 0.07f, 1f);

    private void Draw(){
        foreach(GameObject each in drawn) Destroy(each);
        drawn.Clear();
        nodes.Clear();
        ActMap map = run.map;
        reachable = run.restPending || run.eliteRewardChoices.Count > 0 ? new List<int>() : map.Reachable(run.mapNode);
        BossSO boss = run.actBoss;
        titleText.text = "ACT " + (run.act + 1) + (boss != null ? ":  " + boss.bossName.ToUpperInvariant() : "");
        hintText.text = run.mapNode < 0 ? "Choose your first fight. Hover a node to see what waits there." : "Choose where to go next.";
        infoText.text = GameText.Icon("Health") + " " + run.currentHP + " / " + run.maxHP + "     " + GameText.Icon("Gold") + " " + run.gold;

        // Lines first, so the nodes cover their ends.
        for(int i = 0; i < map.nodes.Count; i++){
            foreach(int next in map.nodes[i].next){
                Image line = Instantiate(lineTemplate, mapArea);
                line.gameObject.SetActive(true);
                // Roads are dashed ink, like a route drawn on an old map.
                line.sprite = DashSprite();
                line.type = Image.Type.Tiled;
                line.pixelsPerUnitMultiplier = DashScale;
                drawn.Add(line.gameObject);
                Vector2 a = PositionOf(map.nodes[i]), b = PositionOf(map.nodes[next]);
                RectTransform rect = line.rectTransform;
                rect.anchoredPosition = (a + b) / 2f;
                rect.sizeDelta = new Vector2(Vector2.Distance(a, b), rect.sizeDelta.y);
                rect.localRotation = Quaternion.Euler(0f, 0f, Mathf.Atan2(b.y - a.y, b.x - a.x) * Mathf.Rad2Deg);
                bool walked = run.visitedNodes.Contains(i) && run.visitedNodes.Contains(next);
                bool open = i == run.mapNode && reachable.Contains(next);
                line.color = open ? OpenInk : walked ? WalkedInk : FaintInk;
            }
        }
        for(int i = 0; i < map.nodes.Count; i++){
            MapNode node = map.nodes[i];
            MapNodeUI ui = Instantiate(nodeTemplate, mapArea);
            ui.gameObject.SetActive(true);
            drawn.Add(ui.gameObject);
            nodes.Add(ui);
            RectTransform rect = (RectTransform)ui.transform;
            rect.anchoredPosition = PositionOf(node);
            if(node.type == MapNodeType.Boss) rect.sizeDelta = new Vector2(bossSize, bossSize);
            bool visited = run.visitedNodes.Contains(i);
            bool canGo = reachable.Contains(i) || RunManager.Instance.CanReenterShop(i);
            bool current = i == run.mapNode;
            // The picture is the node; its type's color only glows softly behind it (the boss keeps a full disc).
            bool isBoss = node.type == MapNodeType.Boss;
            // No discs or rings behind the pictures (the user: they clash with the parchment); the open nodes breathe.
            ui.disc.enabled = false;
            Sprite picture = isBoss && boss != null && (boss.portrait != null || boss.sprite != null) ? (boss.portrait != null ? boss.portrait : boss.sprite)
                : (int)node.type < typeIcons.Length ? typeIcons[(int)node.type] : null;
            ui.icon.sprite = picture;
            ui.icon.enabled = picture != null;
            ui.icon.preserveAspect = true;
            // A place already passed fades to ink.
            ui.icon.color = visited && !current && !isBoss ? new Color(0.55f, 0.48f, 0.40f, 1f) : Color.white;
            ui.ring.enabled = current && inkCircle != null;
            ui.ring.sprite = inkCircle;
            ui.ring.color = Color.white;
            ui.ring.preserveAspect = true;
            if(current) rect.localScale = new Vector3(1.15f, 1.15f, 1f);
            ui.label.text = node.type == MapNodeType.Boss && boss != null ? boss.bossName : TypeName(node.type);
            ui.label.color = LabelInk;
            // Under the boss portrait's skull, not on it.
            if(isBoss) ui.label.rectTransform.anchoredPosition += new Vector2(0f, -24f);
            CanvasGroup group = ui.GetComponent<CanvasGroup>();
            if(group != null) group.alpha = canGo || current || visited || node.type == MapNodeType.Boss ? 1f : 0.55f;
            // The node's look is drawn here; the button's own tint would wash out the nodes it can't go to.
            ui.button.transition = Selectable.Transition.None;
            ui.button.interactable = canGo;
            int index = i;
            ui.button.onClick.AddListener(() => OnNodeClicked(index));
            ui.tooltip.text = Describe(node);
        }
    }

    public static string TypeName(MapNodeType type){
        switch(type){
            case MapNodeType.EnchantingTable: return "Enchanting Table";
            default: return type.ToString();
        }
    }

    private string Describe(MapNode node){
        RunManager manager = RunManager.Instance;
        int battle = run.battlesWon + 1;
        switch(node.type){
            case MapNodeType.Elite:
                if(node.encounter != null && node.encounter.members.Count == 1 && node.encounter.members[0].tier == EnemyTier.Elite){
                    EnemySO elite = node.encounter.members[0];
                    return "<b>Elite: " + elite.enemyName + "</b>  " + elite.maxHP + " HP\n" + GameText.Colorize(elite.eliteMechanic)
                        + "\n\nHarder than a battle. Win it for " + manager.economy.eliteGoldBonus + " extra gold and a free item.\n\n" + EncounterInfo.Details(node.encounter, battle);
                }
                goto case MapNodeType.Battle;
            case MapNodeType.Battle:
                if(node.encounter != null && node.encounter.members.Count > 0)
                    return EncounterInfo.Summary(node.encounter, battle) + "\n\n" + EncounterInfo.Details(node.encounter, battle);
                return "<b>" + TypeName(node.type) + "</b>\nA fight with " + manager.ActTier + " enemies. Win it for gold.";
            case MapNodeType.Boss:
                return run.actBoss != null ? "<b>Boss</b>\n" + EncounterInfo.BossDetails(run.actBoss) + "\n\nWinning heals you and opens the Grand Market." : "<b>Boss</b>";
            case MapNodeType.Market:
                return "<b>Market</b>\nBuy faces and items, upgrade faces, use the bench.";
            case MapNodeType.EnchantingTable:
                return "<b>Enchanting Table</b>\nInscribe enchantments onto faces and onto the die.";
            case MapNodeType.Rest:
                return "<b>Rest</b>\nHeal " + manager.RestHealAmount + " HP, or upgrade one face for free.";
            default:
                return "<b>" + TypeName(node.type) + "</b>";
        }
    }

    private void OnNodeClicked(int index){
        if(!RunManager.Instance.CanEnter(index)) return;
        AudioManager.Play(Sfx.UIClick);
        RunManager.Instance.EnterNode(index);
        // Fights and shops load their own screen; a rest stays here.
        if(run.restPending){
            Draw();
            ShowRestChoice();
        }
    }

    // ---------- Elite reward ----------

    private void ShowReward(){
        if(rewardPanel == null) return;
        rewardPanel.SetActive(true);
        rewardPanel.transform.SetAsLastSibling();
        if(TooltipUI.Instance != null) TooltipUI.Instance.transform.SetAsLastSibling();
        for(int i = 0; i < rewardOptions.Length; i++){
            bool has = i < run.eliteRewardChoices.Count;
            rewardOptions[i].gameObject.SetActive(has);
            if(!has) continue;
            EquipmentSO item = run.eliteRewardChoices[i];
            Transform option = rewardOptions[i].transform;
            Image icon = option.Find("Icon") != null ? option.Find("Icon").GetComponent<Image>() : null;
            if(icon != null){
                icon.sprite = item.icon;
                icon.enabled = item.icon != null;
                icon.preserveAspect = true;
            }
            TextMeshProUGUI name = option.Find("Name") != null ? option.Find("Name").GetComponent<TextMeshProUGUI>() : null;
            if(name != null) name.text = item.equipmentName;
            TextMeshProUGUI text = option.Find("Text") != null ? option.Find("Text").GetComponent<TextMeshProUGUI>() : null;
            if(text != null) text.text = "<color=#C9A46A>" + item.rarity + "</color>\n" + GameText.Colorize(item.description);
        }
    }

    private void OnRewardTaken(int index){
        EliteReward.Claim(run, index);
        AudioManager.Play(Sfx.Buy);
        rewardPanel.SetActive(false);
        Draw();
    }

    private void OnRewardSkipped(){
        EliteReward.Skip(run);
        rewardPanel.SetActive(false);
        Draw();
    }

    // ---------- Rest ----------

    private void ShowRestChoice(){
        restPanel.SetActive(true);
        // The rest panel must draw over the map.
        restPanel.transform.SetAsLastSibling();
        if(TooltipUI.Instance != null) TooltipUI.Instance.transform.SetAsLastSibling();
        ClearFaceButtons();
        restFaceRow.gameObject.SetActive(false);
        restBackButton.gameObject.SetActive(false);
        restHealButton.gameObject.SetActive(true);
        restUpgradeButton.gameObject.SetActive(true);
        int heal = Mathf.Min(RunManager.Instance.RestHealAmount, run.maxHP - run.currentHP);
        restBody.text = "Rest by the fire: heal, or work on one face of your die.\n" + GameText.Icon("Health") + " " + run.currentHP + " / " + run.maxHP;
        restHealText.text = "Heal " + heal + " HP";
        restHealButton.interactable = heal > 0;
    }

    private void ShowUpgradeChoice(){
        restHealButton.gameObject.SetActive(false);
        restUpgradeButton.gameObject.SetActive(false);
        restBackButton.gameObject.SetActive(true);
        restFaceRow.gameObject.SetActive(true);
        restBody.text = "Pick a face: its value goes up by 1.";
        ClearFaceButtons();
        for(int i = 0; i < run.dieFaces.Count; i++){
            DieFace face = run.dieFaces[i];
            Button button = Instantiate(restFaceTemplate, restFaceRow);
            button.gameObject.SetActive(true);
            faceButtons.Add(button.gameObject);
            FaceTileUI tile = button.GetComponentInChildren<FaceTileUI>();
            if(tile != null) tile.Set(face, face.value);
            // The caption under the tile (the tile has its own value text).
            Transform captionObject = button.transform.Find("Caption");
            TextMeshProUGUI caption = captionObject != null ? captionObject.GetComponent<TextMeshProUGUI>() : null;
            if(caption != null) caption.text = face.IsBlank ? "Blank" : face.DisplayName + "\n" + face.value + " → " + (face.value + 1);
            TooltipTrigger tooltip = button.GetComponent<TooltipTrigger>();
            if(tooltip != null) tooltip.text = face.IsBlank ? "A blank face can't be upgraded." : GameText.FaceRules(face.FullDescription);
            button.interactable = RunManager.Instance.CanRestUpgrade(i);
            int slot = i;
            button.onClick.AddListener(() => OnRestUpgrade(slot));
        }
    }

    private void ClearFaceButtons(){
        foreach(GameObject each in faceButtons) Destroy(each);
        faceButtons.Clear();
    }

    private void OnRestHeal(){
        RunManager.Instance.RestHeal();
        AudioManager.Play(Sfx.Heal);
        FinishRest();
    }

    private void OnRestUpgrade(int slot){
        RunManager.Instance.RestUpgrade(slot);
        AudioManager.Play(Sfx.Upgrade);
        FinishRest();
    }

    private void FinishRest(){
        RunManager.Instance.FinishRest();
        ClearFaceButtons();
        restPanel.SetActive(false);
        Draw();
    }
}
