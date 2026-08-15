using UnityEngine;
using TMPro;

public class UIManager : MonoBehaviour
{
    public Entity player;
    public Entity enemy;

    public TextMeshProUGUI playerText;
    public TextMeshProUGUI enemyText;

    void Start(){
        player.OnHealthChanged += UpdatePlayerUI;
        enemy.OnHealthChanged += UpdateEnemyUI;

        UpdatePlayerUI();
        UpdateEnemyUI();
    }

    private void UpdatePlayerUI(){
        playerText.text = "HP " + player.currentHP + " / " + player.maxHP + "\nBlock: " + player.currentBlock;
    }

    private void UpdateEnemyUI(){
        if(enemy == null || enemy.currentHP <= 0){
            enemyText.text = "ÖLDÜ!";
            return;
        }

        enemyText.text = "HP: " + enemy.currentHP + " / " + enemy.maxHP + "\nBlock: " + enemy.currentBlock; 
    }

    void OnDestroy(){
        if(player != null){
            player.OnHealthChanged -= UpdatePlayerUI;
        }

        if(enemy != null){
            enemy.OnHealthChanged -= UpdateEnemyUI;
        }
    }
}
