using UnityEngine;
using System.Collections.Generic;

public class MarketOffer
{
    public DieFaceSO face;
    public int price;
    public bool sold;
    public bool locked;
}

public class EnchantmentOffer
{
    public EnchantmentSO enchantment;
    public int price;
    public bool sold;
    public bool locked;
}

public class DieEnchantmentOffer
{
    public DieEnchantmentSO enchantment;
    public int price;
    public bool sold;
    public bool locked;
}

public class EquipmentOffer
{
    public EquipmentSO item;
    public int price;
    public bool sold;
    public bool locked;
}

// Market rules: generating offers, buying and upgrading. Plain C# with no UI,
// so MarketUI only draws this state and forwards clicks (see GDD "Market").
public class Market
{
    public readonly bool isGrand;
    public readonly List<MarketOffer> offers = new List<MarketOffer>();
    // The Enchanting Table's offers (see GDD "Enchanting Table"): same rules as face offers, Epic only in the Grand Market.
    public readonly List<EnchantmentOffer> enchantmentOffers = new List<EnchantmentOffer>();
    // The table's enchantments for the die itself (see GDD "Die Enchantments").
    public readonly List<DieEnchantmentOffer> dieEnchantmentOffers = new List<DieEnchantmentOffer>();
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
    private readonly List<DieEnchantmentSO> replacedDieEnchantments = new List<DieEnchantmentSO>();
    private readonly List<EquipmentSO> replacedEquipment = new List<EquipmentSO>();

    public Market(RunData run, EconomyConfigSO economy, bool isGrand){
        this.run = run;
        this.economy = economy;
        this.isGrand = isGrand;
        // Locked offers from earlier visits come first and keep their slots.
        foreach(DieFaceSO face in run.lockedFaces.ToArray()){
            if(offers.Count >= economy.offersPerMarket) break;
            offers.Add(new MarketOffer{ face = face, price = economy.GetBuyPrice(face.rarity), locked = true });
        }
        foreach(EnchantmentSO enchantment in run.lockedEnchantments.ToArray()){
            if(enchantmentOffers.Count >= economy.enchantmentsPerMarket) break;
            enchantmentOffers.Add(new EnchantmentOffer{ enchantment = enchantment, price = economy.GetBuyPrice(enchantment.rarity), locked = true });
        }
        run.lockedDieEnchantments.RemoveAll(enchantment => run.dieEnchantments.Contains(enchantment));
        foreach(DieEnchantmentSO enchantment in run.lockedDieEnchantments.ToArray()){
            if(dieEnchantmentOffers.Count >= economy.dieEnchantmentsPerMarket) break;
            dieEnchantmentOffers.Add(new DieEnchantmentOffer{ enchantment = enchantment, price = economy.GetBuyPrice(enchantment.rarity), locked = true });
        }
        run.lockedEquipment.RemoveAll(item => run.equipment.Contains(item));
        foreach(EquipmentSO item in run.lockedEquipment.ToArray()){
            if(equipmentOffers.Count >= EquipmentOfferCount) break;
            equipmentOffers.Add(new EquipmentOffer{ item = item, price = economy.equipmentPrice, locked = true });
        }
        if(run.bossRewardPending){
            GenerateRewardChoices();
        }
        GenerateOffers();
        GenerateEnchantmentOffers();
        GenerateDieEnchantmentOffers();
        GenerateEquipmentOffers();
    }

    // ---------- Healer ----------

    public int HealsThisVisit { get; private set; }
    public int HealsLeft => economy.maxHealsPerVisit - HealsThisVisit;
    public int HealAmount => economy.healAmount;

    public int GetHealCost(){
        return economy.GetHealCost(HealsThisVisit);
    }

    // Needs gold, a heal left this visit and missing HP.
    public bool CanHeal(){
        return HealsThisVisit < economy.maxHealsPerVisit && run.gold >= GetHealCost() && run.currentHP < run.maxHP;
    }

    // Heals healAmount (up to max HP) for the current price; the next heal costs more.
    public bool Heal(){
        if(!CanHeal()) return false;
        run.gold -= GetHealCost();
        HealsThisVisit++;
        run.currentHP = System.Math.Min(run.maxHP, run.currentHP + economy.healAmount);
        return true;
    }

    // ---------- Reroll ----------

    public int GetRerollCost(){
        return economy.GetRerollCost(isGrand, rerollCount);
    }

    public bool CanReroll(){
        return run.gold >= GetRerollCost();
    }

    // Pays for a fresh set of offers: everything is replaced (sold ones included) except locked offers.
    public bool Reroll(){
        if(!CanReroll()){
            return false;
        }
        run.gold -= GetRerollCost();
        rerollCount++;

        replacedFaces.Clear();
        foreach(MarketOffer offer in offers){
            if(!offer.locked) replacedFaces.Add(offer.face);
        }
        replacedEnchantments.Clear();
        foreach(EnchantmentOffer offer in enchantmentOffers){
            if(!offer.locked) replacedEnchantments.Add(offer.enchantment);
        }
        replacedDieEnchantments.Clear();
        foreach(DieEnchantmentOffer offer in dieEnchantmentOffers){
            if(!offer.locked) replacedDieEnchantments.Add(offer.enchantment);
        }
        replacedEquipment.Clear();
        foreach(EquipmentOffer offer in equipmentOffers){
            if(!offer.locked) replacedEquipment.Add(offer.item);
        }
        offers.RemoveAll(offer => !offer.locked);
        enchantmentOffers.RemoveAll(offer => !offer.locked);
        dieEnchantmentOffers.RemoveAll(offer => !offer.locked);
        equipmentOffers.RemoveAll(offer => !offer.locked);
        GenerateOffers();
        GenerateEnchantmentOffers();
        GenerateDieEnchantmentOffers();
        GenerateEquipmentOffers();
        return true;
    }

    // ---------- Offers ----------

    // Fills the face offers up to offersPerMarket (locked offers already hold their slots).
    private void GenerateOffers(){
        while(offers.Count < economy.offersPerMarket){
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

        // Build-aware offers: most of the time, prefer faces of the build's tags.
        if(Random.value < economy.tagMatchChance){
            List<DieFaceSO> matching = MatchingBuild(candidates, face => face.tags);
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

    // The candidates that fit the build: one build tag, picked by weight, when the die has build faces; otherwise any
    // candidate sharing a tag with the die (also the behaviour with weightByBuildTags off).
    private List<T> MatchingBuild<T>(List<T> candidates, System.Func<T, List<FaceTag>> tagsOf){
        if(economy.weightByBuildTags){
            FaceTag? tag = PickBuildTag();
            if(tag.HasValue){
                List<T> withTag = candidates.FindAll(candidate => tagsOf(candidate).Contains(tag.Value));
                if(withTag.Count > 0) return withTag;
            }
        }
        return candidates.FindAll(candidate => SharesTagWithDie(tagsOf(candidate)));
    }

    // How many faces carry each tag beyond the starting die: 2 Fortune faces count 2 Fortune; a third Sword counts
    // its Physical and Bleed, the two starting Swords don't.
    public Dictionary<FaceTag, int> BuildTagCounts(){
        Dictionary<FaceTag, int> counts = new Dictionary<FaceTag, int>();
        foreach(DieFace dieFace in run.dieFaces){
            foreach(FaceTag tag in dieFace.definition.tags){
                counts[tag] = (counts.ContainsKey(tag) ? counts[tag] : 0) + 1;
            }
        }
        if(run.startingDie != null){
            foreach(DieFaceSO starter in run.startingDie){
                foreach(FaceTag tag in starter.tags){
                    if(counts.ContainsKey(tag)) counts[tag]--;
                }
            }
        }
        return counts;
    }

    private FaceTag? PickBuildTag(){
        Dictionary<FaceTag, int> counts = BuildTagCounts();
        int total = 0;
        foreach(KeyValuePair<FaceTag, int> entry in counts) if(entry.Value > 0) total += entry.Value;
        if(total == 0) return null;
        int roll = Random.Range(0, total);
        foreach(KeyValuePair<FaceTag, int> entry in counts){
            if(entry.Value <= 0) continue;
            roll -= entry.Value;
            if(roll < 0) return entry.Key;
        }
        return null;
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
        while(enchantmentOffers.Count < economy.enchantmentsPerMarket){
            Rarity rarity = isGrand ? Rarity.Epic : RollRarity();
            List<EnchantmentSO> candidates = GetUnofferedEnchantments(rarity);
            if(candidates.Count == 0 && !isGrand) candidates = GetUnofferedEnchantments(null);
            if(candidates.Count == 0) break;

            if(Random.value < economy.tagMatchChance){
                List<EnchantmentSO> matching = MatchingBuild(candidates, enchantment => enchantment.tags);
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

    // Same rules as face offers (Epic only in the Grand Market); what the die already has isn't offered.
    private void GenerateDieEnchantmentOffers(){
        while(dieEnchantmentOffers.Count < economy.dieEnchantmentsPerMarket){
            Rarity rarity = isGrand ? Rarity.Epic : RollRarity();
            List<DieEnchantmentSO> candidates = GetUnofferedDieEnchantments(rarity);
            if(candidates.Count == 0 && !isGrand) candidates = GetUnofferedDieEnchantments(null);
            if(candidates.Count == 0) break;
            DieEnchantmentSO picked = candidates[Random.Range(0, candidates.Count)];
            dieEnchantmentOffers.Add(new DieEnchantmentOffer{ enchantment = picked, price = economy.GetBuyPrice(picked.rarity) });
        }
    }

    private List<DieEnchantmentSO> GetUnofferedDieEnchantments(Rarity? rarity){
        List<DieEnchantmentSO> fresh = new List<DieEnchantmentSO>();
        List<DieEnchantmentSO> repeated = new List<DieEnchantmentSO>();
        foreach(DieEnchantmentSO enchantment in economy.dieEnchantmentPool){
            if(rarity.HasValue && enchantment.rarity != rarity.Value) continue;
            if(run.dieEnchantments.Contains(enchantment)) continue;
            if(dieEnchantmentOffers.Exists(offer => offer.enchantment == enchantment)) continue;
            if(replacedDieEnchantments.Contains(enchantment)) repeated.Add(enchantment);
            else fresh.Add(enchantment);
        }
        return fresh.Count > 0 ? fresh : repeated;
    }

    public bool HasFreeDieEnchantmentSlot => run.dieEnchantments.Count < RunData.DieEnchantmentSlots;

    public bool CanBuyDieEnchantment(int offerIndex){
        DieEnchantmentOffer offer = dieEnchantmentOffers[offerIndex];
        return !offer.sold && run.gold >= offer.price;
    }

    // Inscribes the enchantment on the die. replaceSlot -1 needs a free slot; otherwise it replaces that one.
    public bool BuyDieEnchantment(int offerIndex, int replaceSlot){
        if(!CanBuyDieEnchantment(offerIndex)) return false;
        if(replaceSlot < 0 && !HasFreeDieEnchantmentSlot) return false;
        if(replaceSlot >= run.dieEnchantments.Count) return false;
        DieEnchantmentOffer offer = dieEnchantmentOffers[offerIndex];
        run.gold -= offer.price;
        if(replaceSlot >= 0) run.dieEnchantments[replaceSlot] = offer.enchantment;
        else run.dieEnchantments.Add(offer.enchantment);
        offer.sold = true;
        if(offer.locked){ offer.locked = false; run.lockedDieEnchantments.Remove(offer.enchantment); }
        return true;
    }

    public bool CanToggleDieEnchantmentLock(int i){ return CanToggle(dieEnchantmentOffers[i].sold, dieEnchantmentOffers[i].locked); }

    public void ToggleDieEnchantmentLock(int i){
        DieEnchantmentOffer offer = dieEnchantmentOffers[i];
        if(!CanToggleDieEnchantmentLock(i)) return;
        offer.locked = Toggle(offer.locked, run.lockedDieEnchantments, offer.enchantment);
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
        if(offer.locked){ offer.locked = false; run.lockedEnchantments.Remove(offer.enchantment); }
        return true;
    }

    // ---------- Equipment ----------

    private int EquipmentOfferCount => isGrand ? economy.equipmentPerGrandMarket : economy.equipmentPerMarket;

    // Every Market sells items, the Grand Market more; the boss reward choice is never offered for sale at the same time.
    private void GenerateEquipmentOffers(){
        while(equipmentOffers.Count < EquipmentOfferCount){
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
        if(offer.locked){ offer.locked = false; run.lockedEquipment.Remove(offer.item); }
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
        if(offer.locked){ offer.locked = false; run.lockedFaces.Remove(offer.face); }
        return true;
    }

    // ---------- Upgrading ----------

    // The Normal Market upgrades Common and Rare faces; the Grand Market upgrades every face, Epic ones included.
    public bool IsUpgradableHere(int dieSlot){
        return isGrand || run.dieFaces[dieSlot].definition.rarity != Rarity.Epic;
    }

    // ---------- Locking ----------

    public int GetLockCost(){
        return isGrand ? economy.grandLockCost : economy.lockCost;
    }

    // Locking costs gold; unlocking is free (the gold isn't refunded). Sold offers can't be locked.
    public bool CanToggleFaceLock(int i){ return CanToggle(offers[i].sold, offers[i].locked); }
    public bool CanToggleEnchantmentLock(int i){ return CanToggle(enchantmentOffers[i].sold, enchantmentOffers[i].locked); }
    public bool CanToggleEquipmentLock(int i){ return CanToggle(equipmentOffers[i].sold, equipmentOffers[i].locked); }

    public void ToggleFaceLock(int i){
        MarketOffer offer = offers[i];
        if(!CanToggleFaceLock(i)) return;
        offer.locked = Toggle(offer.locked, run.lockedFaces, offer.face);
    }

    public void ToggleEnchantmentLock(int i){
        EnchantmentOffer offer = enchantmentOffers[i];
        if(!CanToggleEnchantmentLock(i)) return;
        offer.locked = Toggle(offer.locked, run.lockedEnchantments, offer.enchantment);
    }

    public void ToggleEquipmentLock(int i){
        EquipmentOffer offer = equipmentOffers[i];
        if(!CanToggleEquipmentLock(i)) return;
        offer.locked = Toggle(offer.locked, run.lockedEquipment, offer.item);
    }

    private bool CanToggle(bool sold, bool locked){
        return !sold && (locked || run.gold >= GetLockCost());
    }

    // Returns the new locked state and keeps the run's locked list in step.
    private bool Toggle<T>(bool locked, List<T> runList, T thing){
        if(locked){
            runList.Remove(thing);
            return false;
        }
        run.gold -= GetLockCost();
        runList.Add(thing);
        return true;
    }

    public int GetUpgradeCost(int dieSlot){
        return economy.GetUpgradeCost(run.dieFaces[dieSlot]);
    }

    public bool CanUpgrade(int dieSlot){
        return IsUpgradableHere(dieSlot) && run.gold >= GetUpgradeCost(dieSlot);
    }

    // Swaps two faces' places on the net (see GDD "Die Layout"). Free and unlimited: the arrangement is a puzzle, not a
    // cost. The face instances move, so their upgrades and enchantments go with them.
    public bool SwapFaces(int a, int b){
        if(a == b || a < 0 || b < 0 || a >= run.dieFaces.Count || b >= run.dieFaces.Count) return false;
        DieFace moved = run.dieFaces[a];
        run.dieFaces[a] = run.dieFaces[b];
        run.dieFaces[b] = moved;
        return true;
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
