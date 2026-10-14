using UnityEngine;
using System.Collections.Generic;

// One action bought in the Market and attached to an empty action slot of a die face (see GDD "Action Modules").
// Its effects run with the host face's value, so upgrading the face upgrades the module too.
[CreateAssetMenu(fileName = "NewActionModule", menuName = "Dice/Action Module")]
public class ActionModuleSO : ScriptableObject
{
    public string moduleName;
    [TextArea]
    public string description;
    public Rarity rarity;
    public List<FaceTag> tags = new List<FaceTag>();
    // The slot this module fills.
    public ActionType action;
    public List<EffectSO> effects = new List<EffectSO>();
}
