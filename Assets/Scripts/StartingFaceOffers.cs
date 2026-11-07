using UnityEngine;
using System.Collections.Generic;

// Generates the starting face choice (see GDD "Run Start"): mostly Common, rarely Rare or Epic,
// each offer from a different archetype, never a face that is already on the starting die.
// Also the paid reroll of that choice, shared by RunManager and the simulator.
public static class StartingFaceOffers
{
    // avoid: faces shown before (a reroll), skipped while the pool allows.
    public static List<DieFaceSO> Generate(EconomyConfigSO economy, List<DieFaceSO> startingDie, List<DieFaceSO> avoid = null){
        List<DieFaceSO> offers = new List<DieFaceSO>();
        List<DieFaceSO> none = new List<DieFaceSO>();
        for(int i = 0; i < economy.startingFaceOfferCount; i++){
            Rarity rarity = RollRarity(economy);
            // Fall back to any rarity, then allow a shared archetype, then shown faces, so the screen always fills up.
            DieFaceSO face = Pick(economy, startingDie, offers, avoid, rarity, true)
                ?? Pick(economy, startingDie, offers, avoid, null, true)
                ?? Pick(economy, startingDie, offers, avoid, null, false)
                ?? Pick(economy, startingDie, offers, none, null, false);
            if(face == null) break;
            offers.Add(face);
        }
        return offers;
    }

    // The price of the next reroll: the Normal Market's rising reroll price.
    public static int RerollCost(EconomyConfigSO economy, RunData run){
        return economy.GetRerollCost(false, run.startingOfferRerolls);
    }

    public static bool CanReroll(EconomyConfigSO economy, RunData run){
        return !run.startingFaceChosen && run.gold >= RerollCost(economy, run);
    }

    // Pays for a fresh set of three offers that avoids the ones shown.
    public static bool Reroll(EconomyConfigSO economy, RunData run, List<DieFaceSO> startingDie){
        if(!CanReroll(economy, run)) return false;
        run.gold -= RerollCost(economy, run);
        run.startingOfferRerolls++;
        run.startingFaceOffers = Generate(economy, startingDie, run.startingFaceOffers);
        return true;
    }

    private static DieFaceSO Pick(EconomyConfigSO economy, List<DieFaceSO> startingDie, List<DieFaceSO> offers, List<DieFaceSO> avoid, Rarity? rarity, bool distinctArchetype){
        List<DieFaceSO> candidates = economy.marketPool.FindAll(face =>
            !startingDie.Contains(face)
            && !offers.Contains(face)
            && (avoid == null || !avoid.Contains(face))
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
