using UnityEngine;
using System;
using System.Collections.Generic;

// A normal battle against several enemies at once (see GDD "Multiple Enemies"). Each member has less HP
// than when it fights alone, since the group shares the work of one enemy.
[Serializable]
public class EnemyGroup
{
    public string groupName;
    public EnemyTier tier;
    // In turn order: the first member is the front. At most MaxMembers.
    public List<EnemySO> members = new List<EnemySO>();
    // Each member's HP is its maxHP times this, rounded (at least 1). A lone enemy uses 1.
    public float hpMultiplier = 0.6f;

    public const int MaxMembers = 3;

    public int MemberHP(EnemySO member){
        return Mathf.Max(1, Mathf.RoundToInt(member.maxHP * hpMultiplier));
    }

    // A lone enemy as an encounter.
    public static EnemyGroup Alone(EnemySO enemy){
        return new EnemyGroup{ groupName = enemy.enemyName, tier = enemy.tier, members = new List<EnemySO>{ enemy }, hpMultiplier = 1f };
    }

    public string DisplayName => string.IsNullOrEmpty(groupName) ? (members.Count > 0 ? members[0].enemyName : "") : groupName;
}

// Every normal enemy and enemy group that can appear in a run (each one's tier decides which part of the run
// it belongs to), and the boss battles with the bosses that can appear in each.
[CreateAssetMenu(fileName = "EncounterConfig", menuName = "Dice/Encounter Config")]
public class EncounterConfigSO : ScriptableObject
{
    public List<EnemySO> enemyPool = new List<EnemySO>();
    public List<EnemyGroup> groups = new List<EnemyGroup>();
    // A slot without bosses (not built yet) keeps using normal enemies of the next tier.
    public List<BossSlot> bossSlots = new List<BossSlot>();

    // The boss for this battle number, picked from its slot's pool; null if it isn't a boss battle (or has no boss yet).
    public BossSO PickBoss(int battleNumber){
        BossSlot slot = bossSlots.Find(s => s.battle == battleNumber);
        if(slot == null || slot.bosses.Count == 0) return null;
        return slot.bosses[UnityEngine.Random.Range(0, slot.bosses.Count)];
    }

    // Picks a normal encounter of this tier: a lone enemy or a group, every candidate equally likely, and adds
    // its members to met. Nothing already met is picked (a group needs all its members unmet) until the tier
    // runs out, then repeats are allowed. Shared by RunManager and the simulator (which passes scaled copies).
    public static EnemyGroup PickEncounter(List<EnemySO> enemies, List<EnemyGroup> groups, EnemyTier tier, List<EnemySO> met){
        List<EnemyGroup> candidates = Candidates(enemies, groups, tier, met);
        if(candidates.Count == 0){
            candidates = Candidates(enemies, groups, tier, null);
        }
        EnemyGroup picked = candidates[UnityEngine.Random.Range(0, candidates.Count)];
        foreach(EnemySO member in picked.members){
            if(!met.Contains(member)) met.Add(member);
        }
        return picked;
    }

    private static List<EnemyGroup> Candidates(List<EnemySO> enemies, List<EnemyGroup> groups, EnemyTier tier, List<EnemySO> met){
        List<EnemyGroup> candidates = new List<EnemyGroup>();
        foreach(EnemySO enemy in enemies){
            if(enemy.tier == tier && (met == null || !met.Contains(enemy))) candidates.Add(EnemyGroup.Alone(enemy));
        }
        if(groups != null){
            foreach(EnemyGroup group in groups){
                if(group.tier != tier || group.members.Count == 0) continue;
                if(met != null && group.members.Exists(member => met.Contains(member))) continue;
                candidates.Add(group);
            }
        }
        return candidates;
    }
}
