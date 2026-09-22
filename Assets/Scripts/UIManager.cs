using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class UIManager : MonoBehaviour
{
    public static UIManager Instance { get; private set; }

    public Entity player;
    public Entity enemy;

    public TextMeshProUGUI diceInfoText;
    public TextMeshProUGUI playerText;
    public TextMeshProUGUI enemyText;
    public TextMeshProUGUI enemyIntentText;
    public Slider playerHealthBar;
    public Slider enemyHealthBar;

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
        enemy.OnHealthChanged += UpdateEnemyUI;

        UpdatePlayerUI();
        UpdateEnemyUI();
    }

    private void UpdatePlayerUI(){
        if(playerHealthBar != null){
            playerHealthBar.maxValue = player.maxHP;
            playerHealthBar.value = player.currentHP;
        }

        string statusText = "";
        int burn = player.GetStatus(StatusType.Burn);
        if (burn > 0) statusText += "Burn (" + burn + ") ";
        int fireShield = player.GetStatus(StatusType.FireShield);
        if (fireShield > 0) statusText += "FireShield (" + fireShield + ") ";

        playerText.text = "HP " + player.currentHP + " / " + player.maxHP + "\nBlock: " + player.currentBlock;
        if (statusText != "") playerText.text += "\nStatus: " + statusText;
    }

    private void UpdateEnemyUI(){
        if(enemyHealthBar != null){
            enemyHealthBar.maxValue = enemy.maxHP;
            enemyHealthBar.value = enemy.currentHP;
        }

        if(enemy == null || enemy.currentHP <= 0){
            enemyText.text = "DEAD!";
            return;
        }

        string statusText = "";
        int burn = enemy.GetStatus(StatusType.Burn);
        if (burn > 0) statusText += "Burn (" + burn + ") ";
        int fireShield = enemy.GetStatus(StatusType.FireShield);
        if (fireShield > 0) statusText += "FireShield (" + fireShield + ") ";

        enemyText.text = "HP: " + enemy.currentHP + " / " + enemy.maxHP + "\nBlock: " + enemy.currentBlock; 
        if (statusText != "") enemyText.text += "\nStatus: " + statusText;
    }

    void OnDestroy(){
        if(player != null){
            player.OnHealthChanged -= UpdatePlayerUI;
        }

        if(enemy != null){
            enemy.OnHealthChanged -= UpdateEnemyUI;
        }
    }

    public void UpdateDiceText(DieFaceSO rolledFace){
        if(rolledFace != null){
            diceInfoText.text = "Face: " + rolledFace.faceName + "\nValue: " + rolledFace.baseValue + "\n" + rolledFace.description;
        }
    }

    public void UpdateEnemyIntentText(EnemyIntent intent){
        if(enemyIntentText != null){
            enemyIntentText.text = "Intent:\n" + intent.intentName + " (" + intent.value + ")";
        }
    }

}
