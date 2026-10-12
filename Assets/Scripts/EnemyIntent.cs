using UnityEngine;
using System;
using System.Collections.Generic;

[Serializable]
public struct EnemyIntent{
    public string intentName;
    // Attack intents grow with the enemy's attack bonus (escalation).
    public ActionType intentType;
    // Every effect runs with the same value, like the effects of a die face.
    public List<EffectSO> effects;
    public int value;
    // Relative chance of being picked. 0 counts as 1.
    public int weight;
    // Permanently added to the enemy's attack bonus when this intent runs (e.g. Battle Cry).
    public int attackBonusGain;
}
