using System.Collections.Generic;
using UnityEngine;

// An elite's reward (see GDD "Elite Enemies"): a free pick of one item among a few the player doesn't own, Rare or Epic
// when there are enough, leaning toward the build's tags like the Market's offers. Plain C#.
public static class EliteReward
{
    public static List<EquipmentSO> Pick(EconomyConfigSO economy, RunData run, int count){
        List<EquipmentSO> unowned = economy.equipmentPool.FindAll(item => item != null && !run.equipment.Contains(item));
        List<EquipmentSO> rare = unowned.FindAll(item => item.rarity != Rarity.Common);
        List<EquipmentSO> pool = rare.Count >= count ? rare : unowned;
        List<EquipmentSO> picked = new List<EquipmentSO>();
        for(int i = 0; i < count && pool.Count > 0; i++){
            List<EquipmentSO> candidates = pool;
            if(Random.value < economy.tagMatchChance){
                List<EquipmentSO> matching = pool.FindAll(item => SharesTag(item, run));
                if(matching.Count > 0) candidates = matching;
            }
            EquipmentSO item = candidates[Random.Range(0, candidates.Count)];
            picked.Add(item);
            pool = pool.FindAll(other => other != item);
        }
        return picked;
    }

    private static bool SharesTag(EquipmentSO item, RunData run){
        foreach(DieFace face in run.dieFaces){
            foreach(FaceTag tag in face.definition.tags){
                if(tag != FaceTag.Physical && item.tags.Contains(tag)) return true;
            }
        }
        return false;
    }

    public static void Claim(RunData run, int index){
        if(index < 0 || index >= run.eliteRewardChoices.Count) return;
        run.equipment.Add(run.eliteRewardChoices[index]);
        run.eliteRewardChoices.Clear();
    }

    public static void Skip(RunData run){
        run.eliteRewardChoices.Clear();
    }
}
