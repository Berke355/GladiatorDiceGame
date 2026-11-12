using UnityEngine;
using System.Collections.Generic;

[CreateAssetMenu(fileName = "NewDieFace", menuName = "Dice/Die Face")]
public class DieFaceSO : ScriptableObject
{
    public string faceName;
    public string description;
    public Rarity rarity;
    public List<FaceTag> tags = new List<FaceTag>();
    // The face's picture on its tile (GDD "Visual Style", Face tiles); the tile's colors come from tags and rarity.
    public Sprite emblem;
    // The starting faces (Sword, Shield, Health Potion) keep a plain ivory tile instead of their family's color.
    public bool plainTile;

    // The value a new copy of this face starts with. Upgrades raise a DieFace's value, never this.
    // There is no upper limit: anyone who can pay can keep upgrading (see EconomyConfigSO.GetUpgradeCost).
    public int baseValue;

    public List<ActionEffect> effects = new List<ActionEffect>();
    // Rules on the whole face, which affect every action on it (see GDD "Face Keywords").
    public List<FaceKeyword> keywords = new List<FaceKeyword>();

    // A special face whose value isn't a number (none exist yet). Rules that compare values treat it as lowest,
    // e.g. it counts as below any threshold (see PunishChoiceEffectSO).
    public bool hasNoValue;

    public bool HasKeyword(FaceKeyword keyword) => keywords != null && keywords.Contains(keyword);
    public bool Charges => HasKeyword(FaceKeyword.Charge);
    public bool IsConsumable => HasKeyword(FaceKeyword.Consumable);
}
