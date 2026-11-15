using UnityEngine;
using System;
using System.Collections.Generic;

[Serializable]
public struct EnemyIntent{
    public string intentName;
    public ActionType intentType;
    // Every effect runs with the same value, like the effects of a die face.
    public List<EffectSO> effects;
    public int value;
    // Relative chance of being picked. 0 counts as 1.
    public int weight;
    // Only picked while the player has this status (None: always), e.g. Ignite waits for Burn to ignite.
    public StatusType requiresTargetStatus;
    // Only picked while the enemy has room for another minion (EnemySO.maxMinions), e.g. Raise Dead.
    public bool requiresMinionRoom;
    // Turns after it was picked before it can be picked again (0: no wait), e.g. Raise Dead.
    public int cooldown;
}
