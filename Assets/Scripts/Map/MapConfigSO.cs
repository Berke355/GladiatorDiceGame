using System;
using System.Collections.Generic;
using UnityEngine;

// The shape of every act's map and how its node types are drawn (see GDD "Map"). Design data only.
[CreateAssetMenu(menuName = "Dice/Map Config")]
public class MapConfigSO : ScriptableObject
{
    [Header("Shape")]
    // Floors of nodes before the boss.
    public int floors = 6;
    public int lanes = 4;
    // Paths drawn from the first floor to the top; nodes are where they pass, so more paths mean more nodes and choices.
    public int paths = 4;

    [Header("Node Types")]
    // Floors in between draw their types by these weights; the first floor is all battles, the last all rests.
    public List<NodeWeight> weights = new List<NodeWeight>{
        new NodeWeight{ type = MapNodeType.Battle, weight = 50 },
        new NodeWeight{ type = MapNodeType.Market, weight = 20 },
        new NodeWeight{ type = MapNodeType.EnchantingTable, weight = 15 },
        new NodeWeight{ type = MapNodeType.Rest, weight = 15 },
    };
    // A rest on the second floor would come before the player has lost anything.
    public int firstRestFloor = 2;
    // Every route from the first floor to the boss must have at least this many battles and Markets.
    public int minBattlesPerRoute = 2;
    public int minMarketsPerRoute = 1;
    // A new map is drawn until every rule holds (always within a few tries in tests).
    public int maxTries = 200;
}

[Serializable]
public struct NodeWeight
{
    public MapNodeType type;
    public int weight;
}
