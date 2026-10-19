using System;
using System.Collections.Generic;

// One face on the player's die: a DieFaceSO definition plus this copy's own value and enchantment.
// Upgrades and enchantments change the instance, never the shared asset, so two copies of the same
// face can differ and the asset stays untouched.
[Serializable]
public class DieFace
{
    public DieFaceSO definition;
    public int value;
    // Inscribed at the Enchanting Table into this copy's empty Magic action (see GDD "Enchanting Table"); null if none.
    public EnchantmentSO enchantment;

    public DieFace(DieFaceSO definition){
        this.definition = definition;
        value = definition.baseValue;
    }

    // Every effect this face runs for an action: its own, then its enchantment's (Magic only).
    // Battle, UI and the simulator read effects only through here.
    public List<EffectSO> GetEffects(ActionType action){
        List<EffectSO> result = new List<EffectSO>();
        foreach(ActionEffect effect in definition.effects){
            if(effect.actionType == action && effect.effectLogic != null){
                result.Add(effect.effectLogic);
            }
        }
        if(action == ActionType.Magic && enchantment != null){
            foreach(EffectSO effect in enchantment.effects){
                if(effect != null) result.Add(effect);
            }
        }
        return result;
    }

    public bool HasAction(ActionType action){
        return GetEffects(action).Count > 0;
    }

    // Only a face with an empty Magic action can be enchanted, which also means one enchantment per face.
    public bool CanEnchant => !HasAction(ActionType.Magic);

    public void Enchant(EnchantmentSO newEnchantment){
        if(CanEnchant){
            enchantment = newEnchantment;
        }
    }

    // Enchanted faces show their name in the enchantment color.
    public string DisplayName{
        get{ return enchantment != null ? "<color=#B388FF>" + definition.faceName + "</color>" : definition.faceName; }
    }

    // The face's own description followed by its enchantment's ("Venom Coating: Magic: apply Poison...").
    public string FullDescription{
        get{
            return enchantment != null
                ? definition.description + "\n" + enchantment.enchantmentName + ": " + enchantment.description
                : definition.description;
        }
    }
}
