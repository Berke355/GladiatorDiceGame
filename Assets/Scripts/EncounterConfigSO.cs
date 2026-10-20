using UnityEngine;
using System.Collections.Generic;

// Every normal enemy that can appear in a run (each enemy's tier decides which part of the run it belongs to),
// and the boss battles with the bosses that can appear in each.
[CreateAssetMenu(fileName = "EncounterConfig", menuName = "Dice/Encounter Config")]
public class EncounterConfigSO : ScriptableObject
{
    public List<EnemySO> enemyPool = new List<EnemySO>();
    // A slot without bosses (not built yet) keeps using normal enemies of the next tier.
    public List<BossSlot> bossSlots = new List<BossSlot>();

    // The boss for this battle number, picked from its slot's pool; null if it isn't a boss battle (or has no boss yet).
    public BossSO PickBoss(int battleNumber){
        BossSlot slot = bossSlots.Find(s => s.battle == battleNumber);
        if(slot == null || slot.bosses.Count == 0) return null;
        return slot.bosses[Random.Range(0, slot.bosses.Count)];
    }
}
