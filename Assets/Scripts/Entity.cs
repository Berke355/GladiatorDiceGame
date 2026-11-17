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
    Ritual,
    // Debuffs enemies put on the player (stacks are turns, -1 at the owner's turn end):
    // every hit taken deals +1, every hit dealt deals -1, can't reroll.
    Vulnerable,
    Weak,
    Entangled,
    // Permanent for the battle: added to every Rage, Fortify and FireShield the owner gains on itself (Ritual's mirror).
    Encourage,
    // Deflect (see GDD "Deflect"): a pool that hits with an attacker spend first, until it runs out; what it stops
    // fills the attacker's posture. Cleared at the start of the owner's turn; if no hit came, the owner loses posture.
    Deflect,
    // Retired (Vigil, before the Deflect rework): kept so the numbers after it stay the same.
    RetiredVigil,
    // Perfect Counter: until the owner's next turn, a hit Deflect stops completely breaks the attacker's posture at once.
    PerfectGuard,
    // Beastmaster (see GDD "Beastmaster"): the owner's beast. At the end of the owner's turn it hits the target once
    // for the stacks. Permanent for the battle; 0 means no beast.
    Hound,
    // A debuff (stacks are turns, -1 at the owner's turn end): its owner gains no Block (Fortify still works).
    Exposed,
    // Phoenix Feather: HP that would drop to 0 stops at 1 and one stack is spent (any HP loss: hits, ticks, costs).
    Undying,
    // Bronze plates (the Bronze Colossus): a hit that gets through Block and Fortify deals only 1 damage and breaks
    // one plate. Ticks and HP costs go past them.
    Plating,
    // Armor (Lorica Segmentata): every hit and every Poison or Bleed tick its owner takes is this much smaller. It lasts
    // the battle and isn't spent. HP costs ignore it.
    Armor
}

public class Entity : MonoBehaviour
{
    public string entityName;
    public int maxHP;
    public int currentHP;
    public int currentBlock;

    public event Action OnHealthChanged;
    // Vulnerable makes a hit half again as big (rounded up), so it rewards big hits where Rage rewards many hits
    // (GDD "Status Effects").
    public static int VulnerableExtra(int damage){
        return (damage + 1) / 2;
    }
    // A Riposte that met a hit this round (no Off-Balance at the owner's turn start).
    private bool deflectUsed;

    // Posture (GDD "Deflect"): enemies fill theirs with the damage the player's Deflect stops; the player fills theirs
    // with timing mistakes (a Deflect no hit came into), one pip each. Full: it breaks (PostureBroken), which stuns.
    // MaxPosture 0: no posture. The battle sets both up.
    public int Posture { get; private set; }
    public int MaxPosture;
    // A whiffed Deflect costs this entity a pip of posture (the player).
    public bool whiffCostsPosture;
    // Turns its owner skips (a broken posture); the posture is whole again when the last one is spent.
    public int StunTurns { get; private set; }
    public event Action PostureBroken;
    // A hit with an attacker that Deflect reduced, and by how much (Old Scars).
    public event Action<Entity, int> Deflected;
    // Any status gained on this entity (Wolf's Arm watches Deflect).
    public event Action<StatusType, int> StatusGained;

    public void AddPosture(int amount){
        if(MaxPosture <= 0 || amount <= 0 || StunTurns > 0 || IsDead) return;
        Posture = Math.Min(MaxPosture, Posture + amount);
        OnHealthChanged?.Invoke();
        if(Posture >= MaxPosture) PostureBroken?.Invoke();
    }

    public void RecoverPosture(int amount){
        if(amount <= 0 || Posture <= 0) return;
        Posture = Math.Max(0, Posture - amount);
        OnHealthChanged?.Invoke();
    }

    public void ResetPosture(){
        Posture = 0;
        StunTurns = 0;
        deflectUsed = false;
    }

    public void Stun(int turns){
        StunTurns = Math.Max(StunTurns, turns);
        OnHealthChanged?.Invoke();
    }

    // Spends one stunned turn; returns true if the owner skips this one. The last one makes the posture whole again.
    public bool SpendStunTurn(){
        if(StunTurns <= 0) return false;
        StunTurns--;
        if(StunTurns == 0) Posture = 0;
        OnHealthChanged?.Invoke();
        return true;
    }
    // The owner's beast in the current battle (Beastmaster), set by BattleFlow; null for enemies.
    [NonSerialized] public BeastCompanion beast;
    // Raised on the attacker for each hit it deals (target, damage before Block), e.g. for equipment triggers.
    public event Action<Entity, int> HitDealt;
    // Raised when a hit makes this entity lose HP (HP lost). Damage over time and HP costs don't count.
    public event Action<int> HitTaken;
    // Raised when Block or Fortify absorbed some of a hit (the amount absorbed). For the UI only: nothing in the rules
    // listens to it.
    public event Action<int> HitAbsorbed;
    // Raised for every hit with an attacker, even one Block absorbed completely (the attacker), e.g. Wrath counts them.
    public event Action<Entity> Struck;
    // Raised when Riposte, Vigil or Reversal dealt damage back to an attacker (the damage), e.g. Grudge counts them.
    public event Action<int> Retaliated;
    // Raised when this entity's beast (Hound) bites (the bitten entity), for the beast's picture.
    public event Action<Entity> BeastBit;
    public void RaiseBeastBit(Entity bitten){ BeastBit?.Invoke(bitten); }

    private Dictionary<StatusType, int> activeStatuses = new Dictionary<StatusType, int>();
    // Burn applied since this entity's last Burn tick does not decay on the next tick,
    // even if it was applied during the opponent's turn.
    private bool burnAddedSinceLastTick = false;

    public bool IsDead => currentHP <= 0;
    // Damage the last hit lost to Block and Fortify (0: the whole hit got through), e.g. for Curl Up.
    public int LastHitAbsorbed { get; private set; }
    // The most HP this entity can lose in a round, the player's turn plus the enemies' after it (the Hydra's Scales).
    // 0: no limit. It holds for every HP loss, so a strong build can't end the fight in a few hits.
    public int maxHPLossPerRound;
    private int hpLostThisRound;
    public int HPLossLeftThisRound => maxHPLossPerRound > 0 ? Math.Max(0, maxHPLossPerRound - hpLostThisRound) : int.MaxValue;
    // Who landed the last hit (null for a hit without a source).
    public Entity LastAttacker { get; private set; }

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
        if(amount > 0) StatusGained?.Invoke(type, amount);
    }

    // Takes stacks away without counting as an application (a cleanse): Burn removed this way still decays on time.
    public void ReduceStatus(StatusType type, int amount) {
        int stacks = GetStatus(type);
        if (stacks <= 0 || amount <= 0) return;
        activeStatuses[type] = Math.Max(0, stacks - amount);
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
        LastAttacker = source;
        // Armor makes every hit smaller before anything else takes it.
        if(GetStatus(StatusType.Armor) > 0) damageAmount = Math.Max(0, damageAmount - GetStatus(StatusType.Armor));
        if(damageAmount > 0 && GetStatus(StatusType.Vulnerable) > 0){
            damageAmount += VulnerableExtra(damageAmount);
        }
        int remaining = damageAmount;
        // Deflect (GDD "Deflect"): the pool takes hits with an attacker first, until it runs out.
        int deflected = 0;
        if(source != null && remaining > 0 && GetStatus(StatusType.Deflect) > 0){
            deflected = Math.Min(GetStatus(StatusType.Deflect), remaining);
            remaining -= deflected;
            activeStatuses[StatusType.Deflect] -= deflected;
            deflectUsed = true;
        }

        int blocked = Math.Min(currentBlock, remaining);
        currentBlock -= blocked;
        remaining -= blocked;

        int fortified = Math.Min(GetStatus(StatusType.Fortify), remaining);
        if(fortified > 0){
            activeStatuses[StatusType.Fortify] -= fortified;
            remaining -= fortified;
        }
        // Plates take the hit: only 1 gets through, and one plate breaks.
        int plated = 0;
        if(remaining > 0 && GetStatus(StatusType.Plating) > 0){
            plated = remaining - 1;
            remaining = 1;
            activeStatuses[StatusType.Plating] -= 1;
        }
        LastHitAbsorbed = blocked + fortified + plated;

        if(remaining > 0){
            LoseHP(remaining);
        }

        if (GetStatus(StatusType.FireShield) > 0 && source != null) {
            source.ApplyStatus(StatusType.Burn, GetStatus(StatusType.FireShield));
        }

        // What Deflect stopped fills the attacker's posture; with Perfect Guard, a hit stopped completely by Deflect
        // breaks it at once.
        if (source != null && deflected > 0) {
            bool perfect = GetStatus(StatusType.PerfectGuard) > 0 && deflected == damageAmount;
            source.AddPosture(perfect ? source.MaxPosture : deflected);
            Retaliated?.Invoke(deflected);
            Deflected?.Invoke(source, deflected);
        }

        OnHealthChanged?.Invoke();

        if(LastHitAbsorbed > 0){
            HitAbsorbed?.Invoke(LastHitAbsorbed);
        }
        if(remaining > 0){
            HitTaken?.Invoke(remaining);
        }
        if(source != null){
            Struck?.Invoke(source);
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
        amount = Math.Min(amount, HPLossLeftThisRound);
        hpLostThisRound += amount;
        currentHP -= amount;
        if(currentHP <= 0 && GetStatus(StatusType.Undying) > 0){
            activeStatuses[StatusType.Undying] -= 1;
            currentHP = 1;
            Debug.Log(entityName + " refused to die.");
        }
        if(currentHP <= 0){
            currentHP = 0;
            Debug.Log(entityName + " died.");
        }
    }

    public void StartRound(){
        hpLostThisRound = 0;
    }

    // A dead entity can't be healed back (only Revive brings it back).
    public void Heal(int amount){
        if(IsDead) return;
        currentHP += amount;
        if(currentHP > maxHP){
            currentHP = maxHP;
        }
        OnHealthChanged?.Invoke();
    }

    // Back to full HP with no Block and no statuses (a Hydra head growing back).
    public void Revive(){
        currentHP = maxHP;
        currentBlock = 0;
        activeStatuses.Clear();
        burnAddedSinceLastTick = false;
        OnHealthChanged?.Invoke();
    }

    public void GainBlock(int amount){
        if(GetStatus(StatusType.Exposed) > 0) amount = 0;
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
        // A Deflect no hit came into is a timing mistake: it costs a pip of posture (GDD "Deflect").
        if(whiffCostsPosture && GetStatus(StatusType.Deflect) > 0 && !deflectUsed) AddPosture(1);
        deflectUsed = false;
        ConsumeStatus(StatusType.Deflect);
        ConsumeStatus(StatusType.PerfectGuard);
    }

    // What OnTurnEnd will take from HP, for the HP bar's preview (kept next to the rules it mirrors): Poison's stacks,
    // then 1 for Bleed, never more than the HP left or the round's cap (Scales).
    public void NextTicks(out int poison, out int bleed){
        int left = Math.Max(0, Math.Min(currentHP, HPLossLeftThisRound));
        poison = Math.Min(GetStatus(StatusType.Poison), left);
        left -= poison;
        bleed = GetStatus(StatusType.Bleed) > 0 ? Math.Min(1, left) : 0;
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
            LoseHP(Math.Max(0, poison - GetStatus(StatusType.Armor)));
            activeStatuses[StatusType.Poison] = poison / 2;
            OnHealthChanged?.Invoke();
        }

        // Bleed always deals 1 and bypasses block; its stacks count down the remaining turns.
        if (GetStatus(StatusType.Bleed) > 0) {
            LoseHP(Math.Max(0, 1 - GetStatus(StatusType.Armor)));
            activeStatuses[StatusType.Bleed] -= 1;
            OnHealthChanged?.Invoke();
        }

        // Debuffs count down their turns.
        foreach (StatusType debuff in new[]{ StatusType.Vulnerable, StatusType.Weak, StatusType.Entangled, StatusType.Exposed }) {
            if (GetStatus(debuff) > 0) {
                activeStatuses[debuff] -= 1;
                OnHealthChanged?.Invoke();
            }
        }
    }
}
