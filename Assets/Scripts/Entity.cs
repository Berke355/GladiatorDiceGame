using UnityEngine;
using System;
using System.Collections.Generic;

public enum StatusType {
    None,
    Burn,
    FireShield
}

public class Entity : MonoBehaviour
{
    public string entityName;
    public int maxHP;
    public int currentHP;
    public int currentBlock;

    public event Action OnHealthChanged;

    private Dictionary<StatusType, int> activeStatuses = new Dictionary<StatusType, int>();
    private bool burnAddedThisTurn = false;

    public void ApplyStatus(StatusType type, int amount) {
        if (!activeStatuses.ContainsKey(type)) {
            activeStatuses[type] = 0;
        }
        activeStatuses[type] += amount;
        
        if (type == StatusType.Burn) {
            burnAddedThisTurn = true;
        }

        OnHealthChanged?.Invoke();
    }

    public int GetStatus(StatusType type) {
        if (activeStatuses.ContainsKey(type)) {
            return activeStatuses[type];
        }
        return 0;
    }

    public void ConsumeStatus(StatusType type) {
        if (activeStatuses.ContainsKey(type)) {
            activeStatuses[type] = 0;
            OnHealthChanged?.Invoke();
        }
    }

    public void TakeDamage(int damageAmount, Entity source = null){
        if(currentBlock >= damageAmount){
            currentBlock -= damageAmount;
        }
        else{
            int damageTaken = damageAmount - currentBlock;
            currentBlock = 0;
            currentHP -= damageTaken;
            if(currentHP <= 0){
                currentHP = 0;
                Debug.Log(entityName + " died.");
            }
        }

        if (GetStatus(StatusType.FireShield) > 0 && source != null) {
            source.ApplyStatus(StatusType.Burn, GetStatus(StatusType.FireShield));
        }

        OnHealthChanged?.Invoke();
    }

    public void Heal(int amount){
        currentHP += amount;
        if(currentHP > maxHP){
            currentHP = maxHP;
        }
        OnHealthChanged?.Invoke();
    }

    public void GainBlock(int amount){
        currentBlock += amount;
        OnHealthChanged?.Invoke();
    }

    public void ResetBlock(){
        currentBlock = 0;
        OnHealthChanged?.Invoke();
    }

    public void OnTurnStart() {
        ResetBlock();
        burnAddedThisTurn = false;
        ConsumeStatus(StatusType.FireShield);
    }

    public void OnTurnEnd() {
        int burn = GetStatus(StatusType.Burn);
        if (burn > 0) {
            TakeDamage(burn, null);
            
            if (!burnAddedThisTurn) {
                activeStatuses[StatusType.Burn] -= 2;
                if (activeStatuses[StatusType.Burn] < 0) {
                    activeStatuses[StatusType.Burn] = 0;
                }
            }
            OnHealthChanged?.Invoke();
        }
    }
}
