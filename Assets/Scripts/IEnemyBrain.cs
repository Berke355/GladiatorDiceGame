using System.Collections.Generic;

// What the battle flow, the UI and the simulator need from whatever controls the enemy:
// the intent brain of normal enemies (EnemyBrain) or the dice brain of bosses (BossBrain).
public interface IEnemyBrain
{
    // Called once by BattleFlow, so boss effects can reach the battle's player-side state (stone faces, Charge).
    void Bind(BattleFlow flow);
    // Start of the player's turn: pick (or roll) what the enemy will do, so it can be shown.
    void ChooseNextIntent();
    string DescribeCurrentIntent();
    void ExecuteCurrentIntent();
    // After the enemy's turn (escalation, countdowns).
    void OnTurnEnd();
    // Damage the enemy is about to deal this turn, with its Rage (0 if it isn't attacking). Used by bots to block big hits.
    int IncomingDamage { get; }
    // Whether the current intent is an attack (threats included), e.g. for Bait and Provoke.
    bool IntendsAttack { get; }
    // Provoke: if the current intent isn't an attack, switch to the enemy's weakest attack and return true.
    bool ProvokeToWeakestAttack();
    // Damage the current intent's threats would deal if the player acted with this face and action (0 if none).
    int ThreatAgainst(DieFace face, int battleValue, ActionType action);
    // The enemy's traits (empty for bosses, whose mechanics live in their brain).
    List<EnemyTrait> Traits { get; }
    // The boss's die and the face it rolled this turn; null for normal enemies.
    List<DieFace> Die { get; }
    DieFace RolledFace { get; }
}
