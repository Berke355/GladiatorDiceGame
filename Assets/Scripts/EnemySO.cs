using UnityEngine;
using System.Collections.Generic;

// Assets store these as numbers: add new tiers at the end.
public enum EnemyTier
{
    Weak,
    Medium,
    Strong
}

// A normal enemy definition (see GDD "Enemy Roster").
[CreateAssetMenu(fileName = "NewEnemy", menuName = "Dice/Enemy")]
public class EnemySO : ScriptableObject
{
    public string enemyName;
    public EnemyTier tier;
    public int maxHP;
    // Rage gained after each of the enemy's turns (e.g. Essedarius gathering speed).
    public int escalationPerTurn;
    public List<EnemyIntent> intents = new List<EnemyIntent>();
}
