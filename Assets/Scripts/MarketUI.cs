using UnityEngine;
using System.Collections.Generic;
using UnityEngine.UI;
using TMPro;

// Draws the Market's state and forwards clicks to it. All rules live in Market.
// Buying is two steps: pick an offer, then pick the die face it replaces (a face offer),
// the die face whose empty Magic it fills (an enchantment) or, with full slots, the item it replaces (equipment).
// After a boss, a reward panel covers the Grand Market until an item is taken or skipped.
public class MarketUI : MonoBehaviour
{
    public TextMeshProUGUI titleText;
    public TextMeshProUGUI runInfoText;
    public TextMeshProUGUI hintText;
    public OfferCardUI[] offerCards;
    // The Enchanting Table, in every Market.
    public OfferCardUI[] enchantmentCards;
    // The Enchanting Table is a tab: it swaps the faces and items for the enchantments, with the die net in view.
    public GameObject facesSection;
    public GameObject enchantmentSection;
    public GameObject dieEnchantmentSection;
    public OfferCardUI[] dieEnchantmentCards;
    public Button tabButton;
    public TextMeshProUGUI tabButtonText;
    // The enchantments on the die; clickable only while choosing which one to replace.
    public EquipmentSlotUI[] dieEnchantmentSlots;
    // Equipment for sale; the whole section is hidden when the pool has nothing left to offer.
    public GameObject equipmentSection;
    public OfferCardUI[] equipmentCards;
    public DieSlotUI[] dieSlots;
    // The items the player has. Only the first object is needed in the scene; it is copied as items pile up.
    public EquipmentSlotUI[] equipmentSlots;
    private List<EquipmentSlotUI> equipmentChips;
    public Button cancelButton;
    public Button rerollButton;
    public TextMeshProUGUI rerollButtonText;
    // The next battle (picked when the Market opens), so purchases can be planned; details on hover.
    public TextMeshProUGUI nextBattleText;
    public TooltipTrigger nextBattleTooltip;
    // The Healer: HP for gold, the price rising within the visit.
    public Button healButton;
    public TextMeshProUGUI healButtonText;
    public Button nextBattleButton;

    [Header("Boss Reward")]
    public GameObject rewardPanel;
    public OfferCardUI[] rewardCards;
    public Button skipRewardButton;

    private Market market;
    private RunData run;
    // Offers waiting for the player to choose a slot; -1 when none. Only one is ever pending.
    private int pendingOffer = -1;
    private int pendingEnchantment = -1;
    // The die face picked to move; the next face clicked swaps places with it.
    private int pendingSwap = -1;
    // A die enchantment being bought with every slot full: the next slot clicked is replaced.
    private int pendingDieEnchantment = -1;
    private bool showingTable;

    private bool ChoosingDieSlot => pendingOffer >= 0 || pendingEnchantment >= 0;
    private bool Choosing => ChoosingDieSlot || pendingSwap >= 0 || pendingDieEnchantment >= 0;

    void Start(){
        equipmentChips = new List<EquipmentSlotUI>(equipmentSlots);
        run = RunManager.Instance.run;
        market = new Market(run, RunManager.Instance.economy, RunManager.Instance.IsGrandMarket);
        titleText.text = market.isGrand ? "GRAND MARKET" : "MARKET";
        // Usually picked already by GoToMarket; also covers playing the Market scene directly.
        RunManager.Instance.PickNextBattle();
        ShowNextBattle();

        // Buttons are wired here because each one needs its own index.
        for(int i = 0; i < offerCards.Length; i++){
            int index = i;
            offerCards[i].buyButton.onClick.AddListener(() => OnBuyClicked(index));
            offerCards[i].lockButton.onClick.AddListener(() => { market.ToggleFaceLock(index); Refresh(); });
        }
        for(int i = 0; i < enchantmentCards.Length; i++){
            int index = i;
            enchantmentCards[i].buyButton.onClick.AddListener(() => OnEnchantmentClicked(index));
            enchantmentCards[i].lockButton.onClick.AddListener(() => { market.ToggleEnchantmentLock(index); Refresh(); });
        }
        for(int i = 0; i < dieEnchantmentCards.Length; i++){
            int index = i;
            dieEnchantmentCards[i].buyButton.onClick.AddListener(() => OnDieEnchantmentClicked(index));
            dieEnchantmentCards[i].lockButton.onClick.AddListener(() => { market.ToggleDieEnchantmentLock(index); Refresh(); });
        }
        for(int i = 0; i < dieEnchantmentSlots.Length; i++){
            int index = i;
            dieEnchantmentSlots[i].slotButton.onClick.AddListener(() => OnDieEnchantmentSlotClicked(index));
        }
        tabButton.onClick.AddListener(() => { showingTable = !showingTable; ClearPending(); Refresh(); });
        for(int i = 0; i < equipmentCards.Length; i++){
            int index = i;
            equipmentCards[i].buyButton.onClick.AddListener(() => OnEquipmentClicked(index));
            equipmentCards[i].lockButton.onClick.AddListener(() => { market.ToggleEquipmentLock(index); Refresh(); });
        }
        for(int i = 0; i < dieSlots.Length; i++){
            int index = i;
            dieSlots[i].slotButton.onClick.AddListener(() => OnDieSlotClicked(index));
            dieSlots[i].upgradeButton.onClick.AddListener(() => OnUpgradeClicked(index));
        }
        for(int i = 0; i < rewardCards.Length; i++){
            int index = i;
            rewardCards[i].buyButton.onClick.AddListener(() => OnRewardClicked(index));
        }
        cancelButton.onClick.AddListener(CancelBuy);
        rerollButton.onClick.AddListener(OnRerollClicked);
        if(healButton != null) healButton.onClick.AddListener(() => { if(market.Heal()) Refresh(); });
        skipRewardButton.onClick.AddListener(OnSkipRewardClicked);

        Refresh();
    }

    private void OnRerollClicked(){
        market.Reroll();
        ClearPending();
        Refresh();
    }

    private void OnBuyClicked(int offerIndex){
        if(market.CanBuy(offerIndex)){
            pendingOffer = offerIndex;
            Refresh();
        }
    }

    private void OnEnchantmentClicked(int offerIndex){
        if(market.CanBuyEnchantment(offerIndex)){
            pendingEnchantment = offerIndex;
            Refresh();
        }
    }

    // With a free slot it goes straight on; with both full the player picks the one to replace.
    private void OnDieEnchantmentClicked(int offerIndex){
        if(!market.CanBuyDieEnchantment(offerIndex)) return;
        if(market.HasFreeDieEnchantmentSlot) market.BuyDieEnchantment(offerIndex, -1);
        else pendingDieEnchantment = offerIndex;
        Refresh();
    }

    private void OnDieEnchantmentSlotClicked(int slot){
        if(pendingDieEnchantment >= 0) market.BuyDieEnchantment(pendingDieEnchantment, slot);
        ClearPending();
        Refresh();
    }

    private void OnEquipmentClicked(int offerIndex){
        market.BuyEquipment(offerIndex);
        Refresh();
    }

    private void OnRewardClicked(int choiceIndex){
        market.ClaimReward(choiceIndex);
        Refresh();
    }

    private void OnSkipRewardClicked(){
        market.SkipReward();
        Refresh();
    }

    private void OnDieSlotClicked(int dieSlot){
        if(pendingOffer >= 0){
            market.Buy(pendingOffer, dieSlot);
        }
        else if(pendingEnchantment >= 0){
            market.BuyEnchantment(pendingEnchantment, dieSlot);
        }
        else if(pendingSwap < 0){
            // Nothing is being bought: the click picks this face to move.
            pendingSwap = dieSlot;
            Refresh();
            return;
        }
        else{
            market.SwapFaces(pendingSwap, dieSlot);
        }
        ClearPending();
        Refresh();
    }

    private void OnUpgradeClicked(int dieSlot){
        market.Upgrade(dieSlot);
        Refresh();
    }

    // Cancelling a reward slot choice brings the reward panel back; the reward itself stays.
    private void CancelBuy(){
        ClearPending();
        Refresh();
    }

    private void ClearPending(){
        pendingOffer = -1;
        pendingEnchantment = -1;
        pendingSwap = -1;
        pendingDieEnchantment = -1;
    }

    // Called by the Next Battle button.
    public void OnNextBattleClicked(){
        RunManager.Instance.GoToNextBattle();
    }

    private void ShowNextBattle(){
        if(nextBattleText == null) return;
        int battle = run.battlesWon + 1;
        string summary;
        System.Text.StringBuilder details = new System.Text.StringBuilder();
        if(run.nextBoss != null){
            BossSO boss = run.nextBoss;
            summary = "<b>" + boss.bossName + "</b>  <size=80%>(boss)</size>";
            details.Append("<b>" + boss.bossName + "</b>  " + boss.maxHP + " HP\nIts die: ");
            List<string> faces = new List<string>();
            foreach(DieFaceSO face in boss.faces) faces.Add(face.faceName);
            details.Append(string.Join(", ", faces));
        }
        else if(run.nextEncounter != null){
            EnemyGroup encounter = run.nextEncounter;
            int rage = RunManager.Instance.encounters.ScalingFor(battle).startingRage;
            summary = "<b>" + encounter.DisplayName + "</b>  <size=80%>(" + encounter.tier + (rage > 0 ? ", starts with " + rage + " Rage" : "") + ")</size>";
            List<string> names = BattleManager.MemberNames(encounter);
            for(int i = 0; i < encounter.members.Count; i++){
                EnemySO member = encounter.members[i];
                if(i > 0) details.Append("\n\n");
                details.Append("<b>" + names[i] + "</b>  " + RunManager.Instance.encounters.ScaledHP(encounter, member, battle) + " HP");
                List<string> intents = new List<string>();
                foreach(EnemyIntent intent in member.intents) intents.Add(intent.intentName);
                details.Append("\nIntents: " + string.Join(", ", intents));
                foreach(EnemyTrait trait in member.traits){
                    if(trait.trigger == EnemyTrigger.Passive && trait.effects.Count == 0 && member.secondWindIntents.Count == 0) continue;
                    details.Append("\n" + trait.traitName + ": " + GameText.Colorize(trait.description));
                }
            }
        }
        else{
            nextBattleText.text = "";
            return;
        }
        nextBattleText.text = "Next battle (" + battle + "): " + summary;
        if(nextBattleTooltip != null) nextBattleTooltip.text = details.ToString();
    }

    private void Refresh(){
        titleText.text = showingTable ? "ENCHANTING TABLE" : market.isGrand ? "GRAND MARKET" : "MARKET";
        tabButtonText.text = showingTable ? "Back to Market" : "Enchanting Table";
        facesSection.SetActive(!showingTable);
        equipmentSection.SetActive(!showingTable && market.equipmentOffers.Count > 0);
        enchantmentSection.SetActive(showingTable);
        dieEnchantmentSection.SetActive(showingTable);
        for(int i = 0; i < dieEnchantmentCards.Length; i++){
            OfferCardUI card = dieEnchantmentCards[i];
            bool hasOffer = i < market.dieEnchantmentOffers.Count;
            card.gameObject.SetActive(hasOffer);
            if(!hasOffer) continue;
            DieEnchantmentOffer offer = market.dieEnchantmentOffers[i];
            card.infoText.text = OfferCardUI.DescribeDieEnchantment(offer.enchantment);
            card.buyButtonText.text = offer.sold ? "SOLD" : (i == pendingDieEnchantment ? "Selected" : "Buy  " + offer.price + "g");
            card.buyButton.interactable = !Choosing && market.CanBuyDieEnchantment(i);
            DrawLock(card, offer.sold, offer.locked, market.CanToggleDieEnchantmentLock(i));
        }
        for(int i = 0; i < dieEnchantmentSlots.Length; i++){
            EquipmentSlotUI slot = dieEnchantmentSlots[i];
            bool filled = i < run.dieEnchantments.Count;
            slot.text.text = filled ? "<b>" + run.dieEnchantments[i].enchantmentName + "</b>" : "<color=#7F8C8D>No die enchantment</color>";
            slot.tooltip.text = filled ? "<b>" + run.dieEnchantments[i].enchantmentName + "</b>\n" + GameText.Colorize(run.dieEnchantments[i].description) : "";
            slot.slotButton.interactable = pendingDieEnchantment >= 0 && filled;
        }

        runInfoText.text = "Gold: " + run.gold + "     HP: " + run.currentHP + " / " + run.maxHP + "     Battles won: " + run.battlesWon;

        for(int i = 0; i < offerCards.Length; i++){
            OfferCardUI card = offerCards[i];
            bool hasOffer = i < market.offers.Count;
            card.gameObject.SetActive(hasOffer);
            if(!hasOffer) continue;

            MarketOffer offer = market.offers[i];
            card.infoText.text = OfferCardUI.DescribeFace(offer.face);
            card.buyButtonText.text = offer.sold ? "SOLD" : (i == pendingOffer ? "Selected" : "Buy  " + offer.price + "g");
            card.buyButton.interactable = !Choosing && market.CanBuy(i);
            DrawLock(card, offer.sold, offer.locked, market.CanToggleFaceLock(i));
        }

        for(int i = 0; i < enchantmentCards.Length; i++){
            OfferCardUI card = enchantmentCards[i];
            bool hasOffer = i < market.enchantmentOffers.Count;
            card.gameObject.SetActive(hasOffer);
            if(!hasOffer) continue;

            EnchantmentOffer offer = market.enchantmentOffers[i];
            card.infoText.text = OfferCardUI.DescribeEnchantment(offer.enchantment);
            card.buyButtonText.text = offer.sold ? "SOLD" : (i == pendingEnchantment ? "Selected" : "Buy  " + offer.price + "g");
            card.buyButton.interactable = !Choosing && market.CanBuyEnchantment(i);
            DrawLock(card, offer.sold, offer.locked, market.CanToggleEnchantmentLock(i));
        }

        for(int i = 0; i < equipmentCards.Length; i++){
            OfferCardUI card = equipmentCards[i];
            bool hasOffer = i < market.equipmentOffers.Count;
            card.gameObject.SetActive(hasOffer);
            if(!hasOffer) continue;

            EquipmentOffer offer = market.equipmentOffers[i];
            card.infoText.text = OfferCardUI.DescribeEquipment(offer.item);
            card.buyButtonText.text = offer.sold ? "SOLD" : "Buy  " + offer.price + "g";
            card.buyButton.interactable = !Choosing && market.CanBuyEquipment(i);
            DrawLock(card, offer.sold, offer.locked, market.CanToggleEquipmentLock(i));
        }

        for(int i = 0; i < dieSlots.Length; i++){
            DieSlotUI slot = dieSlots[i];
            bool hasFace = i < run.dieFaces.Count;
            slot.gameObject.SetActive(hasFace);
            if(!hasFace) continue;

            DieFace dieFace = run.dieFaces[i];
            string moving = i == pendingSwap ? "<color=#B8860B>Moving: </color>" : "";
            int bonus = DieEnchantmentSO.BonusAt(run.dieEnchantments, run.dieFaces, i);
            string bonusText = bonus > 0 ? " <color=#B8860B>+" + bonus + "</color>" : bonus < 0 ? " <color=#C0392B>" + bonus + "</color>" : "";
            int weight = DieEnchantmentSO.WeightAt(run.dieEnchantments, i);
            string weightText = weight == 1 ? "" : weight == 0 ? " | never rolled" : " | x" + weight + " often";
            slot.faceText.text = moving + "<b>" + dieFace.DisplayName + "</b>  " + dieFace.value + bonusText + "\n<size=70%>" + DieLayout.Name(i) + weightText + " | " + MagicStatus(dieFace) + "</size>";
            slot.tooltip.text = GameText.FaceTooltip(dieFace) + "\n<color=#9AA5B1>" + DieLayout.Describe(run.dieFaces, i) + "</color>";
            // An enchantment can only go on a face whose Magic action is still empty. With nothing being bought,
            // a click picks the face to move, and the next click swaps.
            slot.slotButton.interactable = pendingOffer >= 0 || (pendingEnchantment >= 0 && market.CanEnchant(i)) || pendingSwap >= 0 || !Choosing;

            // The upgrade button only shows where this face can be upgraded (Normal: Common/Rare, Grand: Epic).
            bool upgradable = market.IsUpgradableHere(i);
            slot.upgradeButton.gameObject.SetActive(upgradable);
            if(upgradable){
                slot.upgradeButtonText.text = "Upgrade\n" + market.GetUpgradeCost(i) + "g";
                slot.upgradeButton.interactable = !Choosing && market.CanUpgrade(i);
            }
        }

        // Equipment slots are clickable only while choosing which item to replace.
        // One chip per item, copied as items pile up; the description is on hover.
        while(equipmentChips.Count < run.equipment.Count){
            equipmentChips.Add(Instantiate(equipmentChips[0], equipmentChips[0].transform.parent));
        }
        for(int i = 0; i < equipmentChips.Count; i++){
            EquipmentSlotUI chip = equipmentChips[i];
            bool filled = i < run.equipment.Count;
            chip.gameObject.SetActive(filled);
            if(!filled) continue;
            chip.text.text = "<b>" + run.equipment[i].equipmentName + "</b>";
            chip.tooltip.text = GameText.EquipmentTooltip(run.equipment[i]);
        }

        bool rewardOpen = market.IsRewardPending;
        rewardPanel.SetActive(rewardOpen);
        for(int i = 0; i < rewardCards.Length; i++){
            bool hasChoice = i < market.rewardChoices.Count;
            rewardCards[i].gameObject.SetActive(hasChoice);
            if(!hasChoice) continue;
            rewardCards[i].infoText.text = OfferCardUI.DescribeEquipment(market.rewardChoices[i]);
            rewardCards[i].buyButtonText.text = "Take";
        }

        cancelButton.gameObject.SetActive(Choosing);
        // The price rises with every reroll in this visit, so the button always shows the next price.
        rerollButtonText.text = "Reroll  " + market.GetRerollCost() + "g";
        rerollButton.interactable = !Choosing && market.CanReroll();
        if(healButton != null){
            healButtonText.text = market.HealsLeft > 0 ? "Heal " + market.HealAmount + " HP  " + market.GetHealCost() + "g" : "Healer: done";
            healButton.interactable = !Choosing && market.CanHeal();
        }
        // The boss reward must be taken or skipped before leaving.
        nextBattleButton.interactable = !Choosing && !market.IsRewardPending;
        hintText.text = Hint();
    }

    private string Hint(){
        if(pendingOffer >= 0) return "Choose a face to replace with " + market.offers[pendingOffer].face.faceName + " (its enchantment is lost)";
        if(pendingEnchantment >= 0) return "Choose a face with an empty Magic action to enchant with " + market.enchantmentOffers[pendingEnchantment].enchantment.enchantmentName;
        if(pendingDieEnchantment >= 0) return "Your die has 2 enchantments. Choose one to replace with " + market.dieEnchantmentOffers[pendingDieEnchantment].enchantment.enchantmentName;
        if(pendingSwap >= 0) return "Choose where to move " + run.dieFaces[pendingSwap].DisplayName + ": it swaps places with the face you click";
        return (market.isGrand ? "Every face can be upgraded here, Epic ones included." : "Epic faces are upgraded at the Grand Market.") + " Click a die face to move it (free).";
    }

    // A locked offer survives rerolls and waits in the next Market; unlocking is free, locking costs gold.
    private void DrawLock(OfferCardUI card, bool sold, bool locked, bool canToggle){
        card.lockButton.gameObject.SetActive(!sold);
        card.lockButtonText.text = locked ? "Locked" : "Lock " + market.GetLockCost() + "g";
        card.lockButton.interactable = !Choosing && canToggle;
    }

    // Whether the face can take an enchantment, or which one it has.
    private static string MagicStatus(DieFace face){
        if(face.enchantment != null) return "<color=#B388FF>Enchanted: " + face.enchantment.enchantmentName + "</color>";
        return face.CanEnchant ? "<color=#B07800>Can be enchanted</color>" : "<color=#4A5562>Has its own Magic</color>";
    }
}
