using UnityEngine;
using System.Collections.Generic;

public class MarketOffer
{
    public DieFaceSO face;
    public int price;
    public bool sold;
}

// Market rules: generating offers, buying and upgrading. Plain C# with no UI,
// so MarketUI only draws this state and forwards clicks (see GDD "Market").
public class Market
{
    public readonly bool isGrand;
    public readonly List<MarketOffer> offers = new List<MarketOffer>();

    private readonly RunData run;
    private readonly EconomyConfigSO economy;

    public Market(RunData run, EconomyConfigSO economy, bool isGrand){
        this.run = run;
        this.economy = economy;
        this.isGrand = isGrand;
        GenerateOffers();
    }

    // ---------- Offers ----------

    private void GenerateOffers(){
        for(int i = 0; i < economy.offersPerMarket; i++){
            DieFaceSO face = PickOfferFace();
            if(face == null){
                // The pool ran out of faces for this market (e.g. fewer Epic faces than offers).
                break;
            }
            offers.Add(new MarketOffer{ face = face, price = economy.GetBuyPrice(face.rarity) });
        }
    }

    private DieFaceSO PickOfferFace(){
        Rarity rarity = isGrand ? Rarity.Epic : RollRarity();
        List<DieFaceSO> candidates = GetUnofferedFaces(rarity);

        // A normal market falls back to any rarity if the rolled one has nothing left.
        if(candidates.Count == 0 && !isGrand){
            candidates = GetUnofferedFaces(null);
        }
        if(candidates.Count == 0){
            return null;
        }

        // Build-aware offers: most of the time, prefer faces that share a tag with the die.
        if(Random.value < economy.tagMatchChance){
            List<DieFaceSO> matching = candidates.FindAll(SharesTagWithDie);
            if(matching.Count > 0){
                candidates = matching;
            }
        }

        return candidates[Random.Range(0, candidates.Count)];
    }

    private Rarity RollRarity(){
        int roll = Random.Range(0, economy.commonWeight + economy.rareWeight + economy.epicWeight);
        if(roll < economy.commonWeight) return Rarity.Common;
        if(roll < economy.commonWeight + economy.rareWeight) return Rarity.Rare;
        return Rarity.Epic;
    }

    // rarity == null means any rarity. A face is offered at most once per visit.
    private List<DieFaceSO> GetUnofferedFaces(Rarity? rarity){
        List<DieFaceSO> result = new List<DieFaceSO>();
        foreach(DieFaceSO face in economy.marketPool){
            if(rarity.HasValue && face.rarity != rarity.Value) continue;
            if(offers.Exists(offer => offer.face == face)) continue;
            result.Add(face);
        }
        return result;
    }

    private bool SharesTagWithDie(DieFaceSO face){
        foreach(DieFace dieFace in run.dieFaces){
            foreach(FaceTag tag in face.tags){
                if(dieFace.definition.tags.Contains(tag)){
                    return true;
                }
            }
        }
        return false;
    }

    // ---------- Buying ----------

    public bool CanBuy(int offerIndex){
        MarketOffer offer = offers[offerIndex];
        return !offer.sold && run.gold >= offer.price;
    }

    // The bought face replaces the die face in dieSlot.
    public bool Buy(int offerIndex, int dieSlot){
        if(!CanBuy(offerIndex)){
            return false;
        }
        MarketOffer offer = offers[offerIndex];
        run.gold -= offer.price;
        run.dieFaces[dieSlot] = new DieFace(offer.face);
        offer.sold = true;
        return true;
    }

    // ---------- Upgrading ----------

    // The Normal Market upgrades Common and Rare faces; the Grand Market upgrades Epic faces.
    public bool IsUpgradableHere(int dieSlot){
        bool isEpic = run.dieFaces[dieSlot].definition.rarity == Rarity.Epic;
        return isGrand == isEpic;
    }

    // Upgrades are unlimited; past the soft cap they just cost much more.
    public bool IsAboveSoftCap(int dieSlot){
        return economy.IsAboveSoftCap(run.dieFaces[dieSlot]);
    }

    public int GetUpgradeCost(int dieSlot){
        return economy.GetUpgradeCost(run.dieFaces[dieSlot]);
    }

    public bool CanUpgrade(int dieSlot){
        return IsUpgradableHere(dieSlot) && run.gold >= GetUpgradeCost(dieSlot);
    }

    public bool Upgrade(int dieSlot){
        if(!CanUpgrade(dieSlot)){
            return false;
        }
        run.gold -= GetUpgradeCost(dieSlot);
        run.dieFaces[dieSlot].value++;
        return true;
    }
}
