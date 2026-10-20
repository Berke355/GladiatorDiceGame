using System.Collections.Generic;

// What the battle flow, the UI and the simulator need from whatever controls the enemy:
// the intent brain of normal enemies (EnemyBrain) or the dice brain of bosses (BossBrain).
public interface IEnemyBrain
{
    // Start of the player's turn: pick (or roll) what the enemy will do, so it can be shown.
    void ChooseNextIntent();
    string DescribeCurrentIntent();
    void ExecuteCurrentIntent();
    // After the enemy's turn (escalation, countdowns).
    void OnTurnEnd();
    // Damage the enemy is about to deal this turn, with its Rage (0 if it isn't attacking). Used by bots to block big hits.
    int IncomingDamage { get; }
    // The boss's die and the face it rolled this turn; null for normal enemies.
    List<DieFace> Die { get; }
    DieFace RolledFace { get; }
}
