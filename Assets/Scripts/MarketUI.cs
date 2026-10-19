using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Serialization;
using TMPro;
using System.Collections.Generic;

// Draws the Market's state and forwards clicks to it. All rules live in Market.
// Buying is two steps: pick an offer, then pick the die face it replaces (a face offer),
// the die face whose empty slot it fills (an action module) or, with full slots, the item it replaces (equipment).
// After a boss, a reward panel covers the Grand Market until an item is taken or skipped.
public class MarketUI : MonoBehaviour
{
    public TextMeshProUGUI titleText;
    public TextMeshProUGUI runInfoText;
    public TextMeshProUGUI hintText;
    public OfferCardUI[] offerCards;
    // The second row: action modules in the Normal Market, equipment in the Grand Market.
    [FormerlySerializedAs("moduleCards")]
    public OfferCardUI[] extraCards;
    public TextMeshProUGUI extraCardsLabel;
    public DieSlotUI[] dieSlots;
    public EquipmentSlotUI[] equipmentSlots;
    public Button cancelButton;
    public Button rerollButton;
    public TextMeshProUGUI rerollButtonText;
    public Button nextBattleButton;

    [Header("Boss Reward")]
    public GameObject rewardPanel;
    public OfferCardUI[] rewardCards;
    public Button skipRewardButton;

    private Market market;
    private RunData run;
    // Offers waiting for the player to choose a slot; -1 when none. Only one is ever pending.
    private int pendingOffer = -1;
    private int pendingModule = -1;
    private int pendingEquipment = -1;
    private int pendingReward = -1;

    private bool ChoosingDieSlot => pendingOffer >= 0 || pendingModule >= 0;
    private bool ChoosingEquipmentSlot => pendingEquipment >= 0 || pendingReward >= 0;
    private bool Choosing => ChoosingDieSlot || ChoosingEquipmentSlot;

    void Start(){
        run = RunManager.Instance.run;
        market = new Market(run, RunManager.Instance.economy, RunManager.Instance.IsGrandMarket);
        titleText.text = market.isGrand ? "GRAND MARKET" : "MARKET";
        extraCardsLabel.text = market.isGrand ? "EQUIPMENT" : "ACTION MODULES";

        // Buttons are wired here because each one needs its own index.
        for(int i = 0; i < offerCards.Length; i++){
            int index = i;
            offerCards[i].buyButton.onClick.AddListener(() => OnBuyClicked(index));
        }
        for(int i = 0; i < extraCards.Length; i++){
            int index = i;
            extraCards[i].buyButton.onClick.AddListener(() => OnExtraCardClicked(index));
        }
        for(int i = 0; i < dieSlots.Length; i++){
            int index = i;
            dieSlots[i].slotButton.onClick.AddListener(() => OnDieSlotClicked(index));
            dieSlots[i].upgradeButton.onClick.AddListener(() => OnUpgradeClicked(index));
        }
        for(int i = 0; i < equipmentSlots.Length; i++){
            int index = i;
            equipmentSlots[i].slotButton.onClick.AddListener(() => OnEquipmentSlotClicked(index));
        }
        for(int i = 0; i < rewardCards.Length; i++){
            int index = i;
            rewardCards[i].buyButton.onClick.AddListener(() => OnRewardClicked(index));
        }
        cancelButton.onClick.AddListener(CancelBuy);
        rerollButton.onClick.AddListener(OnRerollClicked);
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

    private void OnExtraCardClicked(int index){
        if(market.isGrand){
            if(!market.CanBuyEquipment(index)) return;
            // With a free slot the item goes straight in; with full slots the player picks the item to replace.
            if(market.HasFreeEquipmentSlot) market.BuyEquipment(index, -1);
            else pendingEquipment = index;
        }
        else if(market.CanBuyModule(index)){
            pendingModule = index;
        }
        Refresh();
    }

    private void OnRewardClicked(int choiceIndex){
        if(market.HasFreeEquipmentSlot) market.ClaimReward(choiceIndex, -1);
        else pendingReward = choiceIndex;
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
        else if(pendingModule >= 0){
            market.BuyModule(pendingModule, dieSlot);
        }
        ClearPending();
        Refresh();
    }

    private void OnEquipmentSlotClicked(int slot){
        if(pendingReward >= 0){
            market.ClaimReward(pendingReward, slot);
        }
        else if(pendingEquipment >= 0){
            market.BuyEquipment(pendingEquipment, slot);
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
        pendingModule = -1;
        pendingEquipment = -1;
        pendingReward = -1;
    }

    // Called by the Next Battle button.
    public void OnNextBattleClicked(){
        RunManager.Instance.GoToNextBattle();
    }

    private void Refresh(){
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
        }

        for(int i = 0; i < extraCards.Length; i++){
            if(market.isGrand) DrawEquipmentOffer(extraCards[i], i);
            else DrawModuleOffer(extraCards[i], i);
        }

        for(int i = 0; i < dieSlots.Length; i++){
            DieSlotUI slot = dieSlots[i];
            bool hasFace = i < run.dieFaces.Count;
            slot.gameObject.SetActive(hasFace);
            if(!hasFace) continue;

            DieFace dieFace = run.dieFaces[i];
            slot.faceText.text = dieFace.DisplayName + "\nValue " + dieFace.value
                + "\n<size=75%>" + ActionSlots(dieFace) + "</size>";
            // A module can only go on a face whose slot for its action is still empty.
            slot.slotButton.interactable = pendingOffer >= 0 || (pendingModule >= 0 && market.CanAttachModule(pendingModule, i));

            // The upgrade button only shows where this face can be upgraded (Normal: Common/Rare, Grand: Epic).
            bool upgradable = market.IsUpgradableHere(i);
            slot.upgradeButton.gameObject.SetActive(upgradable);
            if(upgradable){
                slot.upgradeButtonText.text = "Upgrade  " + market.GetUpgradeCost(i) + "g";
                slot.upgradeButton.interactable = !Choosing && market.CanUpgrade(i);
            }
        }

        // Equipment slots are clickable only while choosing which item to replace.
        for(int i = 0; i < equipmentSlots.Length; i++){
            EquipmentSlotUI slot = equipmentSlots[i];
            bool filled = i < run.equipment.Count;
            slot.text.text = filled ? OfferCardUI.DescribeEquipmentShort(run.equipment[i]) : "<color=#7F8C8D>Empty slot</color>";
            slot.slotButton.interactable = ChoosingEquipmentSlot && filled;
        }

        bool rewardOpen = market.IsRewardPending && pendingReward < 0;
        rewardPanel.SetActive(rewardOpen);
        for(int i = 0; i < rewardCards.Length; i++){
            bool hasChoice = i < market.rewardChoices.Count;
            rewardCards[i].gameObject.SetActive(hasChoice);
            if(!hasChoice) continue;
            rewardCards[i].infoText.text = OfferCardUI.DescribeEquipment(market.rewardChoices[i]);
            rewardCards[i].buyButtonText.text = market.HasFreeEquipmentSlot ? "Take" : "Take (replace)";
        }

        cancelButton.gameObject.SetActive(Choosing);
        // The price rises with every reroll in this visit, so the button always shows the next price.
        rerollButtonText.text = "Reroll  " + market.GetRerollCost() + "g";
        rerollButton.interactable = !Choosing && market.CanReroll();
        // The boss reward must be taken or skipped before leaving.
        nextBattleButton.interactable = !Choosing && !market.IsRewardPending;
        hintText.text = Hint();
    }

    private void DrawModuleOffer(OfferCardUI card, int i){
        bool hasOffer = i < market.moduleOffers.Count;
        card.gameObject.SetActive(hasOffer);
        if(!hasOffer) return;

        ModuleOffer offer = market.moduleOffers[i];
        card.infoText.text = OfferCardUI.DescribeModule(offer.module);
        card.buyButtonText.text = offer.sold ? "SOLD" : (i == pendingModule ? "Selected" : "Buy  " + offer.price + "g");
        card.buyButton.interactable = !Choosing && market.CanBuyModule(i);
    }

    private void DrawEquipmentOffer(OfferCardUI card, int i){
        bool hasOffer = i < market.equipmentOffers.Count;
        card.gameObject.SetActive(hasOffer);
        if(!hasOffer) return;

        EquipmentOffer offer = market.equipmentOffers[i];
        card.infoText.text = OfferCardUI.DescribeEquipment(offer.item);
        card.buyButtonText.text = offer.sold ? "SOLD" : (i == pendingEquipment ? "Selected" : "Buy  " + offer.price + "g");
        card.buyButton.interactable = !Choosing && market.CanBuyEquipment(i);
    }

    private string Hint(){
        if(pendingOffer >= 0) return "Choose a face to replace with " + market.offers[pendingOffer].face.faceName + " (its modules are lost)";
        if(pendingModule >= 0){
            ActionModuleSO module = market.moduleOffers[pendingModule].module;
            return "Choose a face with an empty " + module.action + " slot for " + module.moduleName;
        }
        if(pendingEquipment >= 0) return "Your equipment slots are full. Choose an item to replace with " + market.equipmentOffers[pendingEquipment].item.equipmentName;
        if(pendingReward >= 0) return "Your equipment slots are full. Choose an item to replace with " + market.rewardChoices[pendingReward].equipmentName;
        return market.isGrand ? "Epic faces can be bought and upgraded here." : "Epic faces are upgraded at the Grand Market.";
    }

    // The actions this face has no effect for yet, i.e. where a module can go.
    private static string ActionSlots(DieFace face){
        List<string> empty = new List<string>();
        foreach(ActionType action in new[]{ ActionType.Attack, ActionType.Defense, ActionType.Magic }){
            if(!face.HasAction(action)) empty.Add(action.ToString());
        }
        return empty.Count == 0
            ? "<color=#7F8C8D>No empty slots</color>"
            : "Empty slots: <color=#B07800>" + string.Join(", ", empty.ToArray()) + "</color>";
    }
}
