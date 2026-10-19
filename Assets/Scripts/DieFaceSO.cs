using UnityEngine;
using System.Collections.Generic;

[CreateAssetMenu(fileName = "NewDieFace", menuName = "Dice/Die Face")]
public class DieFaceSO : ScriptableObject
{
    public string faceName;
    public string description;
    public Rarity rarity;
    public List<FaceTag> tags = new List<FaceTag>();

    // The value a new copy of this face starts with. Upgrades raise a DieFace's value, never this.
    // There is no upper limit: anyone who can pay can keep upgrading (see EconomyConfigSO.GetUpgradeCost).
    public int baseValue;

    public List<ActionEffect> effects = new List<ActionEffect>();

    // Charge rule (Hammer): every turn this face isn't the one you act with, it gains +1 value for the battle.
    public bool charges;
}
