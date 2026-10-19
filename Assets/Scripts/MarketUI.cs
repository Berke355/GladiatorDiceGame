using UnityEngine;
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
    // Equipment for sale, Grand Market only; the whole section is hidden in the Normal Market.
    public GameObject equipmentSection;
    public OfferCardUI[] equipmentCards;
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
    private int pendingEnchantment = -1;
    private int pendingEquipment = -1;
    private int pendingReward = -1;

    private bool ChoosingDieSlot => pendingOffer >= 0 || pendingEnchantment >= 0;
    private bool ChoosingEquipmentSlot => pendingEquipment >= 0 || pendingReward >= 0;
    private bool Choosing => ChoosingDieSlot || ChoosingEquipmentSlot;

    void Start(){
        run = RunManager.Instance.run;
        market = new Market(run, RunManager.Instance.economy, RunManager.Instance.IsGrandMarket);
        titleText.text = market.isGrand ? "GRAND MARKET" : "MARKET";
        equipmentSection.SetActive(market.isGrand);

        // Buttons are wired here because each one needs its own index.
        for(int i = 0; i < offerCards.Length; i++){
            int index = i;
            offerCards[i].buyButton.onClick.AddListener(() => OnBuyClicked(index));
        }
        for(int i = 0; i < enchantmentCards.Length; i++){
            int index = i;
            enchantmentCards[i].buyButton.onClick.AddListener(() => OnEnchantmentClicked(index));
        }
        for(int i = 0; i < equipmentCards.Length; i++){
            int index = i;
            equipmentCards[i].buyButton.onClick.AddListener(() => OnEquipmentClicked(index));
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

    private void OnEnchantmentClicked(int offerIndex){
        if(market.CanBuyEnchantment(offerIndex)){
            pendingEnchantment = offerIndex;
            Refresh();
        }
    }

    // With a free slot the item goes straight in; with full slots the player picks the item to replace.
    private void OnEquipmentClicked(int offerIndex){
        if(!market.CanBuyEquipment(offerIndex)) return;
        if(market.HasFreeEquipmentSlot) market.BuyEquipment(offerIndex, -1);
        else pendingEquipment = offerIndex;
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
        else if(pendingEnchantment >= 0){
            market.BuyEnchantment(pendingEnchantment, dieSlot);
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
        pendingEnchantment = -1;
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

        for(int i = 0; i < enchantmentCards.Length; i++){
            OfferCardUI card = enchantmentCards[i];
            bool hasOffer = i < market.enchantmentOffers.Count;
            card.gameObject.SetActive(hasOffer);
            if(!hasOffer) continue;

            EnchantmentOffer offer = market.enchantmentOffers[i];
            card.infoText.text = OfferCardUI.DescribeEnchantment(offer.enchantment);
            card.buyButtonText.text = offer.sold ? "SOLD" : (i == pendingEnchantment ? "Selected" : "Buy  " + offer.price + "g");
            card.buyButton.interactable = !Choosing && market.CanBuyEnchantment(i);
        }

        for(int i = 0; i < equipmentCards.Length; i++){
            OfferCardUI card = equipmentCards[i];
            bool hasOffer = i < market.equipmentOffers.Count;
            card.gameObject.SetActive(hasOffer);
            if(!hasOffer) continue;

            EquipmentOffer offer = market.equipmentOffers[i];
            card.infoText.text = OfferCardUI.DescribeEquipment(offer.item);
            card.buyButtonText.text = offer.sold ? "SOLD" : (i == pendingEquipment ? "Selected" : "Buy  " + offer.price + "g");
            card.buyButton.interactable = !Choosing && market.CanBuyEquipment(i);
        }

        for(int i = 0; i < dieSlots.Length; i++){
            DieSlotUI slot = dieSlots[i];
            bool hasFace = i < run.dieFaces.Count;
            slot.gameObject.SetActive(hasFace);
            if(!hasFace) continue;

            DieFace dieFace = run.dieFaces[i];
            slot.faceText.text = dieFace.DisplayName + "\nValue " + dieFace.value + "\n<size=75%>" + MagicStatus(dieFace) + "</size>";
            // An enchantment can only go on a face whose Magic action is still empty.
            slot.slotButton.interactable = pendingOffer >= 0 || (pendingEnchantment >= 0 && market.CanEnchant(i));

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

    private string Hint(){
        if(pendingOffer >= 0) return "Choose a face to replace with " + market.offers[pendingOffer].face.faceName + " (its enchantment is lost)";
        if(pendingEnchantment >= 0) return "Choose a face with an empty Magic action to enchant with " + market.enchantmentOffers[pendingEnchantment].enchantment.enchantmentName;
        if(pendingEquipment >= 0) return "Your equipment slots are full. Choose an item to replace with " + market.equipmentOffers[pendingEquipment].item.equipmentName;
        if(pendingReward >= 0) return "Your equipment slots are full. Choose an item to replace with " + market.rewardChoices[pendingReward].equipmentName;
        return market.isGrand ? "Epic faces can be bought and upgraded here." : "Epic faces are upgraded at the Grand Market.";
    }

    // Whether the face can take an enchantment, or which one it has.
    private static string MagicStatus(DieFace face){
        if(face.enchantment != null) return "<color=#B388FF>Enchanted: " + face.enchantment.enchantmentName + "</color>";
        return face.CanEnchant ? "<color=#B07800>Can be enchanted</color>" : "<color=#4A5562>Has its own Magic</color>";
    }
}
