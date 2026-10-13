using UnityEngine;

// Run start screen: shows the starting face offers and adds the picked one to the die (see GDD "Run Start").
public class RunStartUI : MonoBehaviour
{
    public OfferCardUI[] offerCards;

    void Start(){
        RunData run = RunManager.Instance.run;
        for(int i = 0; i < offerCards.Length; i++){
            OfferCardUI card = offerCards[i];
            bool hasOffer = i < run.startingFaceOffers.Count;
            card.gameObject.SetActive(hasOffer);
            if(!hasOffer) continue;

            DieFaceSO face = run.startingFaceOffers[i];
            card.infoText.text = OfferCardUI.DescribeFace(face);
            card.buyButtonText.text = "Choose";
            // Each button needs its own face, so it is wired here instead of in the Inspector.
            card.buyButton.onClick.AddListener(() => OnFaceChosen(face));
        }
    }

    private void OnFaceChosen(DieFaceSO face){
        RunManager.Instance.ChooseStartingFace(face);
        RunManager.Instance.GoToNextBattle();
    }
}
