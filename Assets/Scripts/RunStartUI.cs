using UnityEngine;
using UnityEngine.UI;
using TMPro;

// Run start screen: shows the starting face offers and adds the picked one to the die (see GDD "Run Start").
// The offers can be rerolled for gold from the run's starting gold.
public class RunStartUI : MonoBehaviour
{
    public OfferCardUI[] offerCards;
    public Button rerollButton;
    public TextMeshProUGUI rerollButtonText;
    public TextMeshProUGUI goldText;
    // A test shortcut to the final boss (RunManager.JumpToFinalBoss), shown only in the Editor and development builds.
    public Button bossTestButton;

    void Start(){
        // Each button needs its own index, so they are wired here instead of in the Inspector.
        for(int i = 0; i < offerCards.Length; i++){
            int index = i;
            offerCards[i].buyButton.onClick.AddListener(() => OnFaceChosen(index));
        }
        if(rerollButton != null) rerollButton.onClick.AddListener(OnReroll);
        if(bossTestButton != null){
            bossTestButton.gameObject.SetActive(Application.isEditor || Debug.isDebugBuild);
            bossTestButton.onClick.AddListener(() => RunManager.Instance.JumpToFinalBoss());
        }
        AudioManager.PlayMusic(MusicTrack.RunStart);
        Refresh();
    }

    private void Refresh(){
        RunData run = RunManager.Instance.run;
        for(int i = 0; i < offerCards.Length; i++){
            OfferCardUI card = offerCards[i];
            bool hasOffer = i < run.startingFaceOffers.Count;
            card.gameObject.SetActive(hasOffer);
            if(!hasOffer) continue;
            card.ShowFace(run.startingFaceOffers[i]);
            card.buyButtonText.text = "Choose";
        }
        if(goldText != null) goldText.text = "Gold: " + run.gold + "  <size=70%>(unspent gold stays for the run)</size>";
        if(rerollButton != null){
            int cost = RunManager.Instance.StartingRerollCost;
            rerollButton.interactable = run.gold >= cost;
            rerollButtonText.text = "Reroll (" + cost + " gold)";
        }
    }

    private void OnReroll(){
        if(RunManager.Instance.RerollStartingFaces()){ AudioManager.Play(Sfx.Reroll); Refresh(); }
    }

    private void OnFaceChosen(int index){
        RunData run = RunManager.Instance.run;
        if(index >= run.startingFaceOffers.Count) return;
        AudioManager.Play(Sfx.Buy);
        RunManager.Instance.ChooseStartingFace(run.startingFaceOffers[index]);
        RunManager.Instance.BeginRun();
    }
}
