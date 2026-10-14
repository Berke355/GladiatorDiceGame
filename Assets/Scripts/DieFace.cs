using System;
using System.Collections.Generic;

// One face on the player's die: a DieFaceSO definition plus this copy's own value and modules.
// Upgrades and modules change the instance, never the shared asset, so two copies of the same
// face can differ and the asset stays untouched.
[Serializable]
public class DieFace
{
    public DieFaceSO definition;
    public int value;
    // Action modules attached to this copy's empty action slots (see GDD "Action Modules").
    public List<ActionModuleSO> modules = new List<ActionModuleSO>();

    public DieFace(DieFaceSO definition){
        this.definition = definition;
        value = definition.baseValue;
    }

    // Every effect this face runs for an action: its own, then the attached module's.
    // Battle, UI and the simulator read effects only through here.
    public List<EffectSO> GetEffects(ActionType action){
        List<EffectSO> result = new List<EffectSO>();
        foreach(ActionEffect effect in definition.effects){
            if(effect.actionType == action && effect.effectLogic != null){
                result.Add(effect.effectLogic);
            }
        }
        foreach(ActionModuleSO module in modules){
            if(module.action != action) continue;
            foreach(EffectSO effect in module.effects){
                if(effect != null) result.Add(effect);
            }
        }
        return result;
    }

    public bool HasAction(ActionType action){
        return GetEffects(action).Count > 0;
    }

    // Modules only go into empty slots.
    public bool CanAttach(ActionModuleSO module){
        return !HasAction(module.action);
    }

    public void Attach(ActionModuleSO module){
        if(CanAttach(module)){
            modules.Add(module);
        }
    }

    public string DisplayName{
        get{ return definition.faceName + (modules.Count > 0 ? " +" + modules.Count : ""); }
    }

    // The face's own description followed by its modules' ("Venom Coating: Magic: apply Poison...").
    public string FullDescription{
        get{
            string text = definition.description;
            foreach(ActionModuleSO module in modules){
                text += "\n" + module.moduleName + ": " + module.description;
            }
            return text;
        }
    }
}
