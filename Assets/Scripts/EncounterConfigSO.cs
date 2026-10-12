using UnityEngine;
using System.Collections.Generic;

// Every normal enemy that can appear in a run. Each enemy's tier decides which part of the run it belongs to.
[CreateAssetMenu(fileName = "EncounterConfig", menuName = "Dice/Encounter Config")]
public class EncounterConfigSO : ScriptableObject
{
    public List<EnemySO> enemyPool = new List<EnemySO>();
}
