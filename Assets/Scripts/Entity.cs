using UnityEngine;

public class Entity : MonoBehaviour
{
    public string entityName;
    public int maxHP;
    public int currentHP;
    public int currentBlock;

    public void TakeDamage(int damageAmount){
        if(currentBlock == damageAmount){
            currentBlock = 0;
        }
        else if(currentBlock > damageAmount){
            currentBlock = currentBlock - damageAmount;
        }
        else if(currentBlock < damageAmount){
            int damageTaken = damageAmount - currentBlock;
            currentBlock = 0;
            currentHP = currentHP - damageTaken;
        }

        if(currentHP <= 0){
            Debug.Log(entityName + " öldü.");
            Destroy(gameObject);
        }
    }

    public void GainBlock(int amount){
        currentBlock = currentBlock + amount;
    }

    public void ResetBlock(){
        currentBlock = 0;
    }
}
