using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections.Generic;

// Draws the Market's state and forwards clicks to it. All rules live in Market.
// Buying is two steps: pick an offer, then pick the die face it replaces.
public class MarketUI : MonoBehaviour
{
    public TextMeshProUGUI titleText;
    public TextMeshProUGUI runInfoText;
    public TextMeshProUGUI hintText;
    public OfferCardUI[] offerCards;
    public DieSlotUI[] dieSlots;
    public Button cancelButton;
    public Button rerollButton;
    public TextMeshProUGUI rerollButtonText;

    private Market market;
    private RunData run;
    // Offer waiting for the player to choose which die face to replace; -1 when none.
    private int pendingOffer = -1;

    void Start(){
        run = RunManager.Instance.run;
        market = new Market(run, RunManager.Instance.economy, RunManager.Instance.IsGrandMarket);
        titleText.text = market.isGrand ? "GRAND MARKET" : "MARKET";

        // Buttons are wired here because each one needs its own index.
        for(int i = 0; i < offerCards.Length; i++){
            int index = i;
            offerCards[i].buyButton.onClick.AddListener(() => OnBuyClicked(index));
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
        Refresh();
    }

    private void OnBuyClicked(int offerIndex){
        if(market.CanBuy(offerIndex)){
            pendingOffer = offerIndex;
            Refresh();
        }
    }

    private void OnDieSlotClicked(int dieSlot){
        if(pendingOffer < 0){
            return;
        }
        market.Buy(pendingOffer, dieSlot);
        pendingOffer = -1;
        Refresh();
    }

    private void OnUpgradeClicked(int dieSlot){
        market.Upgrade(dieSlot);
        Refresh();
    }

    private void CancelBuy(){
        pendingOffer = -1;
        Refresh();
    }

    // Called by the Next Battle button.
    public void OnNextBattleClicked(){
        RunManager.Instance.GoToNextBattle();
    }

    private void Refresh(){
        bool choosingSlot = pendingOffer >= 0;

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

        for(int i = 0; i < dieSlots.Length; i++){
            DieSlotUI slot = dieSlots[i];
            bool hasFace = i < run.dieFaces.Count;
            slot.gameObject.SetActive(hasFace);
            if(!hasFace) continue;

            DieFace dieFace = run.dieFaces[i];
            slot.faceText.text = dieFace.definition.faceName + "\nValue " + dieFace.value + "  (cap " + dieFace.definition.softCap + ")";
            slot.slotButton.interactable = choosingSlot;

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
        hintText.text = choosingSlot
            ? "Choose a face to replace with " + market.offers[pendingOffer].face.faceName
            : (market.isGrand ? "Epic faces can be bought and upgraded here." : "Epic faces are upgraded at the Grand Market.");
    }
}
