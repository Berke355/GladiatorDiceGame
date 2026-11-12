using UnityEngine;
using System.Collections.Generic;

// Assets store these as numbers: add new tiers at the end.
public enum EnemyTier
{
    Weak,
    Medium,
    Strong,
    // Only used to label boss battles (e.g. in the simulator's statistics); bosses are BossSO assets.
    Boss
}

// A normal enemy definition (see GDD "Enemy Roster").
[CreateAssetMenu(fileName = "NewEnemy", menuName = "Dice/Enemy")]
public class EnemySO : ScriptableObject
{
    public string enemyName;
    public EnemyTier tier;
    public int maxHP;
    // The battle sprite: facing left, pivot at the feet. spriteHeight is its height in human heights (1 = a gladiator).
    public Sprite sprite;
    public float spriteHeight = 1f;
    // Rage gained after each of the enemy's turns (e.g. Essedarius gathering speed).
    public int escalationPerTurn;
    public List<EnemyIntent> intents = new List<EnemyIntent>();
    // Always-on rules that react to the player (see GDD "Enemy Traits").
    public List<EnemyTrait> traits = new List<EnemyTrait>();
    // Pack tactics: when an earlier pack member already attacks this turn, this one picks something else if it can
    // (two wolves never bite at once, so a group's damage comes in turns instead of all at once).
    public bool packTactics;
    // The pack keeps its tactics for this many turns (0: the whole battle); after that a long fight lets them all
    // attack at once, so dragging it out is punished.
    public int packTacticsTurns;
    // Second Wind: below half HP the enemy switches to these intents for the rest of the battle (empty = never).
    public List<EnemyIntent> secondWindIntents = new List<EnemyIntent>();
}
