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
}

// A boss battle in the run and the bosses that can appear in it (one is picked per run).
[System.Serializable]
public class BossSlot
{
    public int battle;
    public List<BossSO> bosses = new List<BossSO>();
}
