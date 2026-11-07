using UnityEngine;
using System.Collections.Generic;

// A boss (see GDD "Bosses"): its own 6-face die of boss-only faces, rolled every turn as its intent.
// Boss faces are normal DieFaceSO assets with one action each, so effects work as for the player.
[CreateAssetMenu(fileName = "NewBoss", menuName = "Dice/Boss")]
public class BossSO : ScriptableObject
{
    public string bossName;
    public int maxHP = 40;
    // The 6 faces of its die; a face listed twice comes up twice as often.
    public List<DieFaceSO> faces = new List<DieFaceSO>();

    [Header("Countdown (Minotaur's Stampede)")]
    // Enemy turns from the countdown face until the hit lands.
    public int countdownTurns = 3;
    // HP the boss must lose from hits during the countdown to cancel it (stagger).
    public int staggerDamage = 12;
    // Rolled instead of the countdown face while a countdown is already running (e.g. Gore).
    public DieFaceSO whileCountingDown;

    [Header("Heads (Hydra)")]
    // Each head is a separate enemy of this kind, fought next to the boss (see GDD "Hydra"). Null = no heads.
    public EnemySO headEnemy;
    public int startingHeads;
    // Living heads at most; a cut head that can't grow back stays dead.
    public int maxHeads = 3;
    // At the start of the boss's turn, every head killed since grows back as two, unless it has one of these (Burn).
    public List<StatusType> regrowthBlockedBy = new List<StatusType>{ StatusType.Burn };
    // At the end of the boss's turn it heals this much per living head, unless it has one of these (Burn, Bleed).
    public int regenPerHead = 1;
    public List<StatusType> regenBlockedBy = new List<StatusType>{ StatusType.Burn, StatusType.Bleed };
}

// A boss battle in the run and the bosses that can appear in it (one is picked per run).
[System.Serializable]
public class BossSlot
{
    public int battle;
    public List<BossSO> bosses = new List<BossSO>();
}
