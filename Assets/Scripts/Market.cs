using UnityEngine;
using System.Collections.Generic;

public class MarketOffer
{
    public DieFaceSO face;
    public int price;
    public bool sold;
}

public class EnchantmentOffer
{
    public EnchantmentSO enchantment;
    public int price;
    public bool sold;
}

public class EquipmentOffer
{
    public EquipmentSO item;
    public int price;
    public bool sold;
}

// Market rules: generating offers, buying and upgrading. Plain C# with no UI,
// so MarketUI only draws this state and forwards clicks (see GDD "Market").
public class Market
{
    public readonly bool isGrand;
    public readonly List<MarketOffer> offers = new List<MarketOffer>();
    // The Enchanting Table's offers (see GDD "Enchanting Table"): same rules as face offers, Epic only in the Grand Market.
    public readonly List<EnchantmentOffer> enchantmentOffers = new List<EnchantmentOffer>();
    // Equipment for sale (Grand Market only) and the free boss reward choice (see GDD "Equipment").
    public readonly List<EquipmentOffer> equipmentOffers = new List<EquipmentOffer>();
    public readonly List<EquipmentSO> rewardChoices = new List<EquipmentSO>();

    private readonly RunData run;
    private readonly EconomyConfigSO economy;
    // Rerolls bought during this visit; each one costs more than the last (resets next Market).
    private int rerollCount;
    // Faces, enchantments and items from the offers a reroll replaced; avoided by the new offers when the pool allows.
    private readonly List<DieFaceSO> replacedFaces = new List<DieFaceSO>();
    private readonly List<EnchantmentSO> replacedEnchantments = new List<EnchantmentSO>();
    private readonly List<EquipmentSO> replacedEquipment = new List<EquipmentSO>();

    public Market(RunData run, EconomyConfigSO economy, bool isGrand){
        this.run = run;
        this.economy = economy;
        this.isGrand = isGrand;
        if(run.bossRewardPending){
            GenerateRewardChoices();
        }
        GenerateOffers();
        GenerateEnchantmentOffers();
        GenerateEquipmentOffers();
    }

    // ---------- Reroll ----------

    public int GetRerollCost(){
        return economy.GetRerollCost(isGrand, rerollCount);
    }

    public bool CanReroll(){
        return run.gold >= GetRerollCost();
    }

    // Pays for a fresh set of offers, all of them replaced (sold ones included).
    public bool Reroll(){
        if(!CanReroll()){
            return false;
        }
        run.gold -= GetRerollCost();
        rerollCount++;

        replacedFaces.Clear();
        foreach(MarketOffer offer in offers){
            replacedFaces.Add(offer.face);
        }
        replacedEnchantments.Clear();
        foreach(EnchantmentOffer offer in enchantmentOffers){
            replacedEnchantments.Add(offer.enchantment);
        }
        replacedEquipment.Clear();
        foreach(EquipmentOffer offer in equipmentOffers){
            replacedEquipment.Add(offer.item);
        }
        offers.Clear();
        enchantmentOffers.Clear();
        equipmentOffers.Clear();
        GenerateOffers();
        GenerateEnchantmentOffers();
        GenerateEquipmentOffers();
        return true;
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

    // rarity == null means any rarity. A face is offered at most once per set of offers,
    // and faces a reroll just replaced are skipped unless nothing else is left.
    private List<DieFaceSO> GetUnofferedFaces(Rarity? rarity){
        List<DieFaceSO> fresh = new List<DieFaceSO>();
        List<DieFaceSO> repeated = new List<DieFaceSO>();
        foreach(DieFaceSO face in economy.marketPool){
            if(rarity.HasValue && face.rarity != rarity.Value) continue;
            if(offers.Exists(offer => offer.face == face)) continue;
            if(replacedFaces.Contains(face)) repeated.Add(face);
            else fresh.Add(face);
        }
        return fresh.Count > 0 ? fresh : repeated;
    }

    private bool SharesTagWithDie(DieFaceSO face){
        return SharesTagWithDie(face.tags);
    }

    private bool SharesTagWithDie(List<FaceTag> tags){
        foreach(DieFace dieFace in run.dieFaces){
            foreach(FaceTag tag in tags){
                if(dieFace.definition.tags.Contains(tag)){
                    return true;
                }
            }
        }
        return false;
    }

    // ---------- Enchanting Table ----------

    // Same rules as face offers: rarity weights (Epic only in the Grand Market), build-aware tags,
    // no repeats in a set, and enchantments a reroll just replaced are avoided when possible.
    private void GenerateEnchantmentOffers(){
        for(int i = 0; i < economy.enchantmentsPerMarket; i++){
            Rarity rarity = isGrand ? Rarity.Epic : RollRarity();
            List<EnchantmentSO> candidates = GetUnofferedEnchantments(rarity);
            if(candidates.Count == 0 && !isGrand) candidates = GetUnofferedEnchantments(null);
            if(candidates.Count == 0) break;

            if(Random.value < economy.tagMatchChance){
                List<EnchantmentSO> matching = candidates.FindAll(enchantment => SharesTagWithDie(enchantment.tags));
                if(matching.Count > 0) candidates = matching;
            }
            EnchantmentSO picked = candidates[Random.Range(0, candidates.Count)];
            enchantmentOffers.Add(new EnchantmentOffer{ enchantment = picked, price = economy.GetBuyPrice(picked.rarity) });
        }
    }

    private List<EnchantmentSO> GetUnofferedEnchantments(Rarity? rarity){
        List<EnchantmentSO> fresh = new List<EnchantmentSO>();
        List<EnchantmentSO> repeated = new List<EnchantmentSO>();
        foreach(EnchantmentSO enchantment in economy.enchantmentPool){
            if(rarity.HasValue && enchantment.rarity != rarity.Value) continue;
            if(enchantmentOffers.Exists(offer => offer.enchantment == enchantment)) continue;
            if(replacedEnchantments.Contains(enchantment)) repeated.Add(enchantment);
            else fresh.Add(enchantment);
        }
        return fresh.Count > 0 ? fresh : repeated;
    }

    // An enchantment can be bought only if some die face has an empty Magic action.
    public bool CanBuyEnchantment(int offerIndex){
        EnchantmentOffer offer = enchantmentOffers[offerIndex];
        return !offer.sold && run.gold >= offer.price && run.dieFaces.Exists(face => face.CanEnchant);
    }

    public bool CanEnchant(int dieSlot){
        return run.dieFaces[dieSlot].CanEnchant;
    }

    // Inscribes the bought enchantment into the Magic action of the die face in dieSlot.
    public bool BuyEnchantment(int offerIndex, int dieSlot){
        if(!CanBuyEnchantment(offerIndex) || !CanEnchant(dieSlot)){
            return false;
        }
        EnchantmentOffer offer = enchantmentOffers[offerIndex];
        run.gold -= offer.price;
        run.dieFaces[dieSlot].Enchant(offer.enchantment);
        offer.sold = true;
        return true;
    }

    // ---------- Equipment ----------

    // The Grand Market sells a few items; the boss reward choice is never offered for sale at the same time.
    private void GenerateEquipmentOffers(){
        if(!isGrand){
            return;
        }
        for(int i = 0; i < economy.equipmentPerGrandMarket; i++){
            EquipmentSO item = PickEquipment();
            if(item == null) break;
            equipmentOffers.Add(new EquipmentOffer{ item = item, price = economy.equipmentPrice });
        }
    }

    private void GenerateRewardChoices(){
        for(int i = 0; i < economy.bossRewardChoices; i++){
            EquipmentSO item = PickEquipment();
            if(item == null) break;
            rewardChoices.Add(item);
        }
    }

    // Items the player owns or that are already on show are skipped; items a reroll just replaced
    // only come back if nothing else is left. Most of the time, items sharing a tag with the die are preferred.
    private EquipmentSO PickEquipment(){
        List<EquipmentSO> fresh = new List<EquipmentSO>();
        List<EquipmentSO> repeated = new List<EquipmentSO>();
        foreach(EquipmentSO item in economy.equipmentPool){
            if(run.equipment.Contains(item) || rewardChoices.Contains(item) || equipmentOffers.Exists(offer => offer.item == item)) continue;
            if(replacedEquipment.Contains(item)) repeated.Add(item);
            else fresh.Add(item);
        }
        List<EquipmentSO> candidates = fresh.Count > 0 ? fresh : repeated;
        if(candidates.Count == 0){
            return null;
        }
        if(Random.value < economy.tagMatchChance){
            List<EquipmentSO> matching = candidates.FindAll(item => SharesTagWithDie(item.tags));
            if(matching.Count > 0) candidates = matching;
        }
        return candidates[Random.Range(0, candidates.Count)];
    }

    public bool IsRewardPending => run.bossRewardPending && rewardChoices.Count > 0;
    public bool HasFreeEquipmentSlot => run.equipment.Count < RunData.EquipmentSlots;

    public bool CanBuyEquipment(int offerIndex){
        EquipmentOffer offer = equipmentOffers[offerIndex];
        return !offer.sold && run.gold >= offer.price && !run.equipment.Contains(offer.item);
    }

    // replaceSlot -1 puts the item in a free slot; otherwise it replaces the item in that slot.
    public bool BuyEquipment(int offerIndex, int replaceSlot){
        if(!CanBuyEquipment(offerIndex) || !CanEquip(replaceSlot)){
            return false;
        }
        EquipmentOffer offer = equipmentOffers[offerIndex];
        run.gold -= offer.price;
        Equip(offer.item, replaceSlot);
        offer.sold = true;
        return true;
    }

    // Takes the free boss reward (same slot rule as buying). The reward is gone once taken or skipped.
    public bool ClaimReward(int choiceIndex, int replaceSlot){
        if(!IsRewardPending || !CanEquip(replaceSlot)){
            return false;
        }
        Equip(rewardChoices[choiceIndex], replaceSlot);
        EndReward();
        return true;
    }

    public void SkipReward(){
        EndReward();
    }

    private void EndReward(){
        run.bossRewardPending = false;
        rewardChoices.Clear();
    }

    private bool CanEquip(int replaceSlot){
        return replaceSlot < 0 ? HasFreeEquipmentSlot : replaceSlot < run.equipment.Count;
    }

    private void Equip(EquipmentSO item, int replaceSlot){
        if(replaceSlot < 0) run.equipment.Add(item);
        else run.equipment[replaceSlot] = item;
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
