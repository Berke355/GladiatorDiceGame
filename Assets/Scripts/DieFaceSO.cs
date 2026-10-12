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
    public int baseValue;
    // Upgrade cap for this face.
    public int maxValue;

    public List<ActionEffect> effects = new List<ActionEffect>();

    void OnValidate(){
        if(maxValue < baseValue){
            maxValue = baseValue;
        }
    }
}
