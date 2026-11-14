using System;
using System.Collections.Generic;

// The kinds of map nodes (see GDD "Map"). Serialized as numbers: only add new values at the end.
public enum MapNodeType
{
    Battle,
    Market,
    EnchantingTable,
    Rest,
    Boss,
    Elite,
    Event
}

// One stop on an act's map. Plain data, so the map is saved with the run.
[Serializable]
public class MapNode
{
    // 0 is the first floor; the boss stands on the floor after the last one.
    public int floor;
    // 0..lanes-1, left to right.
    public int lane;
    public MapNodeType type;
    // Indices (into ActMap.nodes) of the nodes this one leads to, on the next floor.
    public List<int> next = new List<int>();
    // A battle node's enemies, picked when the node becomes reachable so the map can show them; members count as met
    // only once the player enters the node.
    public EnemyGroup encounter;
}

// One act's map: its nodes, floor by floor, and the boss on top.
[Serializable]
public class ActMap
{
    // 0, 1, 2: the Minotaur's, Medusa's and the Hydra's act.
    public int act;
    public int floors;
    public int lanes;
    public List<MapNode> nodes = new List<MapNode>();
    public int bossNode = -1;

    public List<int> NodesOnFloor(int floor){
        List<int> found = new List<int>();
        for(int i = 0; i < nodes.Count; i++) if(nodes[i].floor == floor) found.Add(i);
        return found;
    }

    // Where the player may go next: the first floor before the first step, then the current node's next nodes.
    public List<int> Reachable(int current){
        if(current < 0) return NodesOnFloor(0);
        return new List<int>(nodes[current].next);
    }

    // Every route from a first-floor node to the boss, as lists of node indices (small maps: at most a few hundred).
    public List<List<int>> Routes(){
        List<List<int>> routes = new List<List<int>>();
        foreach(int start in NodesOnFloor(0)) Walk(start, new List<int>(), routes);
        return routes;
    }

    private void Walk(int node, List<int> path, List<List<int>> routes){
        path.Add(node);
        if(nodes[node].next.Count == 0) routes.Add(new List<int>(path));
        else foreach(int next in nodes[node].next) Walk(next, path, routes);
        path.RemoveAt(path.Count - 1);
    }
}
