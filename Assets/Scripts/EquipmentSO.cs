using UnityEngine;
using System;
using System.Collections.Generic;

// Assets store these as numbers: always add new triggers at the end.
public enum TriggerType
{
    BattleStart,
    TurnStart,
    // The die landed. Can require the rolled face to have a tag.
    FaceRolled,
    // An action was chosen, before the face's effects run. Can require the action.
    ActionChosen,
    // Each hit on the enemy (each hit of a multi-hit).
    HitDealt,
    // Each hit that makes the player lose HP (not Poison or Bleed ticks).
    HitTaken,
    // The end of the player's turn, before the player's statuses tick.
    TurnEnd,
    // The enemy died, before the player's HP is saved to the run.
    BattleWon
}

// One reaction of an item: when the trigger fires (and its condition holds), its effects run with the value.
[Serializable]
public class TriggeredEffect
{
    public TriggerType trigger;
    // FaceRolled: only faces with this tag.
    public bool requireFaceTag;
    public FaceTag faceTag;
    // ActionChosen: only this action.
    public bool requireAction;
    public ActionType action;
    // Conditions on the battle state ("if the enemy is bleeding") go in a ConditionalEffectSO here.
    public List<EffectSO> effects = new List<EffectSO>();
    public int value = 1;
}

// A passive item in one of the run's equipment slots (see GDD "Equipment"). Design data only:
// it is never changed at runtime, so the same asset can sit in any run.
[CreateAssetMenu(fileName = "NewEquipment", menuName = "Dice/Equipment")]
public class EquipmentSO : ScriptableObject
{
    public string equipmentName;
    [TextArea]
    public string description;
    public Rarity rarity;
    // Archetype tags, for build-aware offers.
    public List<FaceTag> tags = new List<FaceTag>();
    public List<TriggeredEffect> triggers = new List<TriggeredEffect>();
}
