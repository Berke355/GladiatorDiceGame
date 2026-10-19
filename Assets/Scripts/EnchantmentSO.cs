using UnityEngine;
using UnityEngine.Serialization;
using System.Collections.Generic;

// A Magic action inscribed at the Market's Enchanting Table onto a die face whose Magic is empty
// (see GDD "Enchanting Table"). Its effects run with the face's value, so upgrading the face upgrades it too.
[CreateAssetMenu(fileName = "NewEnchantment", menuName = "Dice/Enchantment")]
public class EnchantmentSO : ScriptableObject
{
    [FormerlySerializedAs("moduleName")]
    public string enchantmentName;
    [TextArea]
    public string description;
    public Rarity rarity;
    public List<FaceTag> tags = new List<FaceTag>();
    // Run as the face's Magic action.
    public List<EffectSO> effects = new List<EffectSO>();
}
