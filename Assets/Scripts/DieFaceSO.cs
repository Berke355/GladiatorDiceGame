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
    // The Blank face (see GDD "Bench"): what a die place holds after its face moved to the bench. No actions, never
    // upgraded or enchanted; landing on it means a reroll or a pass.
    public bool isBlank;

    // Upgrade cap (GDD "Upgrade Caps"): the highest value a copy can reach by upgrades, rests and die enchantments.
    // 0: baseValue + DefaultUpgradeRoom. Measured per face (RunSimulator.MeasureValueCurve): one below the value at
    // which the face alone carries the starting die through act 3's strong fights. Not shown; the player finds it.
    public int maxValue;
    public const int DefaultUpgradeRoom = 6;
    public int MaxValue => maxValue > 0 ? maxValue : baseValue + DefaultUpgradeRoom;

    public bool HasKeyword(FaceKeyword keyword) => keywords != null && keywords.Contains(keyword);
    public bool Charges => HasKeyword(FaceKeyword.Charge);
    public bool IsConsumable => HasKeyword(FaceKeyword.Consumable);
}
