using UnityEngine;
using System;

public class Entity : MonoBehaviour
{
    public string entityName;
    public int maxHP;
    public int currentHP;
    public int currentBlock;

    public event Action OnHealthChanged;

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

        OnHealthChanged?.Invoke();
    }

    public void GainBlock(int amount){
        currentBlock = currentBlock + amount;

        OnHealthChanged?.Invoke();
    }

    public void ResetBlock(){
        currentBlock = 0;

        OnHealthChanged?.Invoke();
    }

    public void Heal(int amount){
        if(amount >= (maxHP - currentHP)){
            currentHP = maxHP;
        }
        else if(amount < (maxHP - currentHP)){
            currentHP = currentHP + amount;
        }

        OnHealthChanged?.Invoke();
    }
}
