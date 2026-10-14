using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections.Generic;

// Draws the Market's state and forwards clicks to it. All rules live in Market.
// Buying is two steps: pick an offer, then pick the die face it replaces (a face offer)
// or the die face whose empty slot it fills (an action module).
public class MarketUI : MonoBehaviour
{
    public TextMeshProUGUI titleText;
    public TextMeshProUGUI runInfoText;
    public TextMeshProUGUI hintText;
    public OfferCardUI[] offerCards;
    public OfferCardUI[] moduleCards;
    public DieSlotUI[] dieSlots;
    public Button cancelButton;
    public Button rerollButton;
    public TextMeshProUGUI rerollButtonText;

    private Market market;
    private RunData run;
    // Offer waiting for the player to choose which die face to replace; -1 when none.
    private int pendingOffer = -1;
    // Module offer waiting for the player to choose which die face gets it; -1 when none.
    private int pendingModule = -1;

    void Start(){
        run = RunManager.Instance.run;
        market = new Market(run, RunManager.Instance.economy, RunManager.Instance.IsGrandMarket);
        titleText.text = market.isGrand ? "GRAND MARKET" : "MARKET";

        // Buttons are wired here because each one needs its own index.
        for(int i = 0; i < offerCards.Length; i++){
            int index = i;
            offerCards[i].buyButton.onClick.AddListener(() => OnBuyClicked(index));
        }
        for(int i = 0; i < moduleCards.Length; i++){
            int index = i;
            moduleCards[i].buyButton.onClick.AddListener(() => OnBuyModuleClicked(index));
        }
        for(int i = 0; i < dieSlots.Length; i++){
            int index = i;
            dieSlots[i].slotButton.onClick.AddListener(() => OnDieSlotClicked(index));
            dieSlots[i].upgradeButton.onClick.AddListener(() => OnUpgradeClicked(index));
        }
        cancelButton.onClick.AddListener(CancelBuy);
        rerollButton.onClick.AddListener(OnRerollClicked);

        Refresh();
    }

    private void OnRerollClicked(){
        market.Reroll();
        pendingOffer = -1;
        pendingModule = -1;
        Refresh();
    }

    private void OnBuyClicked(int offerIndex){
        if(market.CanBuy(offerIndex)){
            pendingOffer = offerIndex;
            Refresh();
        }
    }

    private void OnBuyModuleClicked(int offerIndex){
        if(market.CanBuyModule(offerIndex)){
            pendingModule = offerIndex;
            Refresh();
        }
    }

    private void OnDieSlotClicked(int dieSlot){
        if(pendingOffer >= 0){
            market.Buy(pendingOffer, dieSlot);
        }
        else if(pendingModule >= 0){
            market.BuyModule(pendingModule, dieSlot);
        }
        pendingOffer = -1;
        pendingModule = -1;
        Refresh();
    }

    private void OnUpgradeClicked(int dieSlot){
        market.Upgrade(dieSlot);
        Refresh();
    }

    private void CancelBuy(){
        pendingOffer = -1;
        pendingModule = -1;
        Refresh();
    }

    // Called by the Next Battle button.
    public void OnNextBattleClicked(){
        RunManager.Instance.GoToNextBattle();
    }

    private void Refresh(){
        bool choosingSlot = pendingOffer >= 0 || pendingModule >= 0;

        runInfoText.text = "Gold: " + run.gold + "     HP: " + run.currentHP + " / " + run.maxHP + "     Battles won: " + run.battlesWon;

        for(int i = 0; i < offerCards.Length; i++){
            OfferCardUI card = offerCards[i];
            bool hasOffer = i < market.offers.Count;
            card.gameObject.SetActive(hasOffer);
            if(!hasOffer) continue;

            MarketOffer offer = market.offers[i];
            DieFaceSO face = offer.face;
            card.infoText.text = OfferCardUI.DescribeFace(face);

            card.buyButtonText.text = offer.sold ? "SOLD" : (i == pendingOffer ? "Selected" : "Buy  " + offer.price + "g");
            card.buyButton.interactable = !choosingSlot && market.CanBuy(i);
        }

        // The Grand Market sells no modules, so its module cards stay hidden.
        for(int i = 0; i < moduleCards.Length; i++){
            OfferCardUI card = moduleCards[i];
            bool hasOffer = i < market.moduleOffers.Count;
            card.gameObject.SetActive(hasOffer);
            if(!hasOffer) continue;

            ModuleOffer offer = market.moduleOffers[i];
            card.infoText.text = OfferCardUI.DescribeModule(offer.module);
            card.buyButtonText.text = offer.sold ? "SOLD" : (i == pendingModule ? "Selected" : "Buy  " + offer.price + "g");
            card.buyButton.interactable = !choosingSlot && market.CanBuyModule(i);
        }

        for(int i = 0; i < dieSlots.Length; i++){
            DieSlotUI slot = dieSlots[i];
            bool hasFace = i < run.dieFaces.Count;
            slot.gameObject.SetActive(hasFace);
            if(!hasFace) continue;

            DieFace dieFace = run.dieFaces[i];
            slot.faceText.text = dieFace.DisplayName + "\nValue " + dieFace.value + "  (cap " + dieFace.definition.softCap + ")"
                + "\n<size=75%>" + ActionSlots(dieFace) + "</size>";
            // A module can only go on a face whose slot for its action is still empty.
            slot.slotButton.interactable = pendingOffer >= 0 || (pendingModule >= 0 && market.CanAttachModule(pendingModule, i));

            // The upgrade button only shows where this face can be upgraded (Normal: Common/Rare, Grand: Epic).
            bool upgradable = market.IsUpgradableHere(i);
            slot.upgradeButton.gameObject.SetActive(upgradable);
            if(upgradable){
                // Past the soft cap the price jumps, so the button says so.
                slot.upgradeButtonText.text = (market.IsAboveSoftCap(i) ? "Break cap  " : "Upgrade  ") + market.GetUpgradeCost(i) + "g";
                slot.upgradeButton.interactable = !choosingSlot && market.CanUpgrade(i);
            }
        }

        cancelButton.gameObject.SetActive(choosingSlot);
        // The price rises with every reroll in this visit, so the button always shows the next price.
        rerollButtonText.text = "Reroll  " + market.GetRerollCost() + "g";
        rerollButton.interactable = !choosingSlot && market.CanReroll();
        hintText.text = pendingOffer >= 0
            ? "Choose a face to replace with " + market.offers[pendingOffer].face.faceName + " (its modules are lost)"
            : pendingModule >= 0
            ? "Choose a face with an empty " + market.moduleOffers[pendingModule].module.action + " slot for " + market.moduleOffers[pendingModule].module.moduleName
            : (market.isGrand ? "Epic faces can be bought and upgraded here." : "Epic faces are upgraded at the Grand Market.");
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
