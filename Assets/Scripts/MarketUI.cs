using UnityEngine;
using TMPro;

// Placeholder market: shows the run's state and moves on to the next battle.
public class MarketUI : MonoBehaviour
{
    public TextMeshProUGUI runInfoText;

    void Start(){
        RunData run = RunManager.Instance.run;
        runInfoText.text = "Gold: " + run.gold
            + "\nHP: " + run.currentHP + " / " + run.maxHP
            + "\nBattles won: " + run.battlesWon;
    }

    // Called by the Next Battle button.
    public void OnNextBattleClicked(){
        RunManager.Instance.GoToNextBattle();
    }
}
