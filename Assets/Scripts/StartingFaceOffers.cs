using UnityEngine;
using System.Collections.Generic;

// Generates the starting face choice (see GDD "Run Start"): mostly Common, rarely Rare or Epic,
// each offer from a different archetype, never a face that is already on the starting die.
public static class StartingFaceOffers
{
    public static List<DieFaceSO> Generate(EconomyConfigSO economy, List<DieFaceSO> startingDie){
        List<DieFaceSO> offers = new List<DieFaceSO>();
        for(int i = 0; i < economy.startingFaceOfferCount; i++){
            DieFaceSO face = Pick(economy, startingDie, offers, RollRarity(economy), true);
            // Fall back to any rarity, then allow a shared archetype, so the screen always fills up.
            if(face == null) face = Pick(economy, startingDie, offers, null, true);
            if(face == null) face = Pick(economy, startingDie, offers, null, false);
            if(face == null) break;
            offers.Add(face);
        }
        return offers;
    }

    private static DieFaceSO Pick(EconomyConfigSO economy, List<DieFaceSO> startingDie, List<DieFaceSO> offers, Rarity? rarity, bool distinctArchetype){
        List<DieFaceSO> candidates = economy.marketPool.FindAll(face =>
            !startingDie.Contains(face)
            && !offers.Contains(face)
            && (!rarity.HasValue || face.rarity == rarity.Value)
            && (!distinctArchetype || !SharesTag(face, offers)));
        return candidates.Count > 0 ? candidates[Random.Range(0, candidates.Count)] : null;
    }

    private static bool SharesTag(DieFaceSO face, List<DieFaceSO> others){
        foreach(DieFaceSO other in others){
            foreach(FaceTag tag in face.tags){
                if(other.tags.Contains(tag)) return true;
            }
        }
        return false;
    }

    private static Rarity RollRarity(EconomyConfigSO economy){
        int roll = Random.Range(0, economy.startingCommonWeight + economy.startingRareWeight + economy.startingEpicWeight);
        if(roll < economy.startingCommonWeight) return Rarity.Common;
        if(roll < economy.startingCommonWeight + economy.startingRareWeight) return Rarity.Rare;
        return Rarity.Epic;
    }
}
