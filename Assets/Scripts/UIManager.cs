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
    public GameObject battleResultPanel;
    public TextMeshProUGUI battleResultText;
    public TextMeshProUGUI battleResultButtonText;

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

        string statusText = BuildStatusText(player);

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

        string statusText = BuildStatusText(enemy);

        enemyText.text = "HP: " + enemy.currentHP + " / " + enemy.maxHP + "\nBlock: " + enemy.currentBlock; 
        if (statusText != "") enemyText.text += "\nStatus: " + statusText;
    }

    // Lists every active status. New statuses must be added here to show up.
    private static string BuildStatusText(Entity entity){
        string statusText = "";
        int burn = entity.GetStatus(StatusType.Burn);
        if (burn > 0) statusText += "Burn (" + burn + ") ";
        int poison = entity.GetStatus(StatusType.Poison);
        if (poison > 0) statusText += "Poison (" + poison + ") ";
        int fireShield = entity.GetStatus(StatusType.FireShield);
        if (fireShield > 0) statusText += "FireShield (" + fireShield + ") ";
        return statusText;
    }

    void OnDestroy(){
        if(Instance == this){
            Instance = null;
        }

        if(player != null){
            player.OnHealthChanged -= UpdatePlayerUI;
        }

        if(enemy != null){
            enemy.OnHealthChanged -= UpdateEnemyUI;
        }
    }

    public void UpdateDiceText(DieFace rolledFace){
        if(rolledFace != null){
            diceInfoText.text = "Face: " + rolledFace.definition.faceName + "\nValue: " + rolledFace.value + "\n" + rolledFace.definition.description;
        }
    }

    public void UpdateEnemyIntentText(EnemyIntent intent){
        if(enemyIntentText != null){
            enemyIntentText.text = "Intent:\n" + intent.intentName + " (" + intent.value + ")";
        }
    }

    public void ShowBattleResult(bool playerWon, int goldEarned){
        if(battleResultPanel != null){
            battleResultPanel.SetActive(true);
        }
        if(battleResultText != null){
            battleResultText.text = playerWon
                ? "VICTORY\n<size=50%>+" + goldEarned + " Gold</size>"
                : "DEFEAT";
        }
        if(battleResultButtonText != null){
            battleResultButtonText.text = playerWon ? "Continue" : "New Run";
        }
    }

}
