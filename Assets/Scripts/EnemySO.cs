using UnityEngine;
using System.Collections.Generic;

// Assets store these as numbers: add new tiers at the end.
public enum EnemyTier
{
    Weak,
    Medium,
    Strong,
    // Only used to label boss battles (e.g. in the simulator's statistics); bosses are BossSO assets.
    Boss,
    // Elite enemies: the map's Elite nodes (see GDD "Elite Enemies"), one act each (eliteAct).
    Elite
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

    [Header("Elite")]
    // The act whose Elite nodes this enemy can be (0, 1, 2), with tier Elite; -1: not an elite.
    public int eliteAct = -1;
    // Its mechanic in one sentence, shown on the map before the player chooses the node.
    [TextArea] public string eliteMechanic;
    // The battle ends when it dies, even if its minions live (the Necromancer).
    public bool endsBattleOnDeath;
    // Minions it raises (Raise Dead, SummonMinionEffectSO): which enemy, how many at the start, at most how many alive.
    public EnemySO minion;
    public int startingMinions;
    public int maxMinions = 3;
    // While one of its minions lives, it can't be targeted (the Wall of Bones). Area effects still reach it.
    public bool guardedByMinions;
    // Bronze plates (Plating): how many at the start, and how many come back at the end of each of its turns (never
    // more than the starting count).
    public int startingPlates;
    public int platesPerTurn;
}
