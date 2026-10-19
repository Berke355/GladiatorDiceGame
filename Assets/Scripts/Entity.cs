using UnityEngine;
using System;
using System.Collections.Generic;

// Assets store these as numbers: always add new statuses at the end.
public enum StatusType {
    None,
    Burn,
    FireShield,
    Poison,
    // Block that does not reset at turn start. Absorbs damage after Block.
    Fortify,
    // Stacks are duration: 1 damage per tick, then -1.
    Bleed,
    // Permanent for the battle: added to every hit the owner deals (each hit of a multi-hit).
    Rage,
    // Permanent for the battle: added to every status the owner applies to its opponent.
    Ritual
}

public class Entity : MonoBehaviour
{
    public string entityName;
    public int maxHP;
    public int currentHP;
    public int currentBlock;

    public event Action OnHealthChanged;
    // Raised on the attacker for each hit it deals (target, damage before Block), e.g. for equipment triggers.
    public event Action<Entity, int> HitDealt;
    // Raised when a hit makes this entity lose HP (HP lost). Damage over time and HP costs don't count.
    public event Action<int> HitTaken;

    private Dictionary<StatusType, int> activeStatuses = new Dictionary<StatusType, int>();
    // Burn applied since this entity's last Burn tick does not decay on the next tick,
    // even if it was applied during the opponent's turn.
    private bool burnAddedSinceLastTick = false;

    public bool IsDead => currentHP <= 0;

    // Read-only view of the statuses, for the UI.
    public IEnumerable<KeyValuePair<StatusType, int>> Statuses => activeStatuses;

    public void ApplyStatus(StatusType type, int amount) {
        if (!activeStatuses.ContainsKey(type)) {
            activeStatuses[type] = 0;
        }
        activeStatuses[type] += amount;

        if (type == StatusType.Burn) {
            burnAddedSinceLastTick = true;
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

    // Damage is absorbed by Block first, then Fortify, and only the rest reaches HP.
    public void TakeDamage(int damageAmount, Entity source = null){
        int remaining = damageAmount;

        int blocked = Math.Min(currentBlock, remaining);
        currentBlock -= blocked;
        remaining -= blocked;

        int fortified = Math.Min(GetStatus(StatusType.Fortify), remaining);
        if(fortified > 0){
            activeStatuses[StatusType.Fortify] -= fortified;
            remaining -= fortified;
        }

        if(remaining > 0){
            LoseHP(remaining);
        }

        if (GetStatus(StatusType.FireShield) > 0 && source != null) {
            source.ApplyStatus(StatusType.Burn, GetStatus(StatusType.FireShield));
        }

        OnHealthChanged?.Invoke();

        if(remaining > 0){
            HitTaken?.Invoke(remaining);
        }
        if(source != null){
            source.HitDealt?.Invoke(this, damageAmount);
        }
    }

    // A cost paid in HP (e.g. Bloodrage): ignores Block and Fortify and triggers nothing.
    public void PayHP(int amount){
        LoseHP(amount);
        OnHealthChanged?.Invoke();
    }

    // Direct HP loss that ignores block (used by damage over time).
    private void LoseHP(int amount){
        currentHP -= amount;
        if(currentHP <= 0){
            currentHP = 0;
            Debug.Log(entityName + " died.");
        }
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
        ConsumeStatus(StatusType.FireShield);
    }

    public void OnTurnEnd() {
        // Burn deals no damage on its own: it is fuel that ignitions (IgniteEffectSO) and Detonate turn
        // into damage. It decays by 2, except right after Burn was added.
        int burn = GetStatus(StatusType.Burn);
        if (burn > 0 && !burnAddedSinceLastTick) {
            activeStatuses[StatusType.Burn] = Math.Max(0, burn - 2);
            OnHealthChanged?.Invoke();
        }
        burnAddedSinceLastTick = false;

        // Poison deals its stacks as damage, bypassing block, then halves (rounded down, so 1 stack runs out).
        // Halving keeps one application's total damage about 2x its stacks; losing only 1 made it grow with the square.
        int poison = GetStatus(StatusType.Poison);
        if (poison > 0) {
            LoseHP(poison);
            activeStatuses[StatusType.Poison] = poison / 2;
            OnHealthChanged?.Invoke();
        }

        // Bleed always deals 1 and bypasses block; its stacks count down the remaining turns.
        if (GetStatus(StatusType.Bleed) > 0) {
            LoseHP(1);
            activeStatuses[StatusType.Bleed] -= 1;
            OnHealthChanged?.Invoke();
        }
    }
}
