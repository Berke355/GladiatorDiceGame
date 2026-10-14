using UnityEngine;
using UnityEngine.Serialization;
using System.Collections.Generic;

[CreateAssetMenu(fileName = "NewDieFace", menuName = "Dice/Die Face")]
public class DieFaceSO : ScriptableObject
{
    public string faceName;
    public string description;
    public Rarity rarity;
    public List<FaceTag> tags = new List<FaceTag>();

    // The value a new copy of this face starts with. Upgrades raise a DieFace's value, never this.
    public int baseValue;
    // Soft cap: upgrades past this value are still possible but get exponentially more expensive
    // (see EconomyConfigSO.GetUpgradeCost). Renamed from maxValue; existing assets keep their data.
    [FormerlySerializedAs("maxValue")]
    public int softCap;

    public List<ActionEffect> effects = new List<ActionEffect>();

    void OnValidate(){
        if(softCap < baseValue){
            softCap = baseValue;
        }
    }
}
