using System;
using System.Collections.Generic;

// Draws an act's map (see GDD "Map"): paths walk up from the first floor, one lane at most to either side and never
// crossing another path; nodes are where they pass. Then every node gets a type, and the map is drawn again until every
// route keeps the rules. Plain C#: test it without a scene.
public static class MapGenerator
{
    public static ActMap Generate(MapConfigSO config, int act, Random rng){
        ActMap map = null;
        for(int attempt = 0; attempt < Math.Max(1, config.maxTries); attempt++){
            map = Shape(config, act, rng);
            AssignTypes(map, config, rng);
            if(Problems(map, config).Count == 0) return map;
        }
        UnityEngine.Debug.LogWarning("MapGenerator: no map kept every rule in " + config.maxTries + " tries; using the last one.");
        return map;
    }

    private static ActMap Shape(MapConfigSO config, int act, Random rng){
        ActMap map = new ActMap{ act = act, floors = config.floors, lanes = config.lanes };
        int[,] at = new int[config.floors, config.lanes];
        for(int f = 0; f < config.floors; f++) for(int l = 0; l < config.lanes; l++) at[f, l] = -1;
        int firstStart = -1;
        for(int p = 0; p < config.paths; p++){
            int lane = rng.Next(config.lanes);
            // The first two paths start apart, so the very first step is already a choice.
            if(p == 1) while(lane == firstStart && config.lanes > 1) lane = rng.Next(config.lanes);
            if(p == 0) firstStart = lane;
            for(int floor = 0; floor < config.floors; floor++){
                int here = NodeAt(map, at, floor, lane);
                if(floor == config.floors - 1) break;
                int nextLane = StepLane(map, at, floor, lane, config.lanes, rng);
                int there = NodeAt(map, at, floor + 1, nextLane);
                if(!map.nodes[here].next.Contains(there)) map.nodes[here].next.Add(there);
                lane = nextLane;
            }
        }
        map.nodes.Add(new MapNode{ floor = config.floors, lane = (config.lanes - 1) / 2, type = MapNodeType.Boss });
        map.bossNode = map.nodes.Count - 1;
        foreach(int last in map.NodesOnFloor(config.floors - 1)) map.nodes[last].next.Add(map.bossNode);
        // Next nodes left to right, so a screen and a route list read the same way.
        foreach(MapNode node in map.nodes) node.next.Sort((a, b) => map.nodes[a].lane.CompareTo(map.nodes[b].lane));
        return map;
    }

    private static int NodeAt(ActMap map, int[,] at, int floor, int lane){
        if(at[floor, lane] < 0){
            map.nodes.Add(new MapNode{ floor = floor, lane = lane });
            at[floor, lane] = map.nodes.Count - 1;
        }
        return at[floor, lane];
    }

    // One step up: the same lane or a neighbour, never crossing a path already drawn between these two floors.
    private static int StepLane(ActMap map, int[,] at, int floor, int lane, int lanes, Random rng){
        List<int> options = new List<int>{ lane - 1, lane, lane + 1 };
        for(int i = options.Count - 1; i > 0; i--){
            int j = rng.Next(i + 1);
            int swap = options[i]; options[i] = options[j]; options[j] = swap;
        }
        foreach(int to in options){
            if(to < 0 || to >= lanes) continue;
            // Going right crosses an edge from the right neighbour down to this lane, and the other way round.
            if(to == lane + 1 && HasEdge(map, at, floor, lane + 1, lane)) continue;
            if(to == lane - 1 && HasEdge(map, at, floor, lane - 1, lane)) continue;
            return to;
        }
        return lane;
    }

    private static bool HasEdge(ActMap map, int[,] at, int floor, int fromLane, int toLane){
        int from = at[floor, fromLane], to = at[floor + 1, toLane];
        return from >= 0 && to >= 0 && map.nodes[from].next.Contains(to);
    }

    private static void AssignTypes(ActMap map, MapConfigSO config, Random rng){
        int total = 0;
        foreach(NodeWeight each in config.weights) total += Math.Max(0, each.weight);
        for(int floor = 0; floor < config.floors; floor++){
            foreach(int index in map.NodesOnFloor(floor)){
                MapNode node = map.nodes[index];
                if(floor == 0){ node.type = MapNodeType.Battle; continue; }
                if(floor == config.floors - 1){ node.type = MapNodeType.Rest; continue; }
                List<int> parents = Parents(map, index);
                MapNodeType pick = MapNodeType.Battle;
                for(int tries = 0; tries < 20; tries++){
                    pick = Draw(config, total, rng);
                    if(Fits(map, config, index, pick, parents)) break;
                    pick = MapNodeType.Battle;
                }
                node.type = pick;
            }
        }
    }

    private static MapNodeType Draw(MapConfigSO config, int total, Random rng){
        if(total <= 0) return MapNodeType.Battle;
        int roll = rng.Next(total);
        foreach(NodeWeight each in config.weights){
            roll -= Math.Max(0, each.weight);
            if(roll < 0) return each.type;
        }
        return MapNodeType.Battle;
    }

    // A non-battle node never follows one of its own type, never comes right before the rests of the last floor if it
    // is a rest, and siblings (nodes sharing a parent) offer different stops.
    private static bool Fits(ActMap map, MapConfigSO config, int index, MapNodeType type, List<int> parents){
        if(type == MapNodeType.Battle) return true;
        MapNode node = map.nodes[index];
        if(type == MapNodeType.Rest && (node.floor < config.firstRestFloor || node.floor == config.floors - 2)) return false;
        foreach(int parent in parents){
            if(map.nodes[parent].type == type) return false;
            foreach(int sibling in map.nodes[parent].next){
                if(sibling != index && map.nodes[sibling].floor == node.floor && map.nodes[sibling].type == type && sibling < index) return false;
            }
        }
        return true;
    }

    private static List<int> Parents(ActMap map, int index){
        List<int> parents = new List<int>();
        for(int i = 0; i < map.nodes.Count; i++) if(map.nodes[i].next.Contains(index)) parents.Add(i);
        return parents;
    }

    // Every broken rule, for the generator's retries and for tests. Empty: the map is good.
    public static List<string> Problems(ActMap map, MapConfigSO config){
        List<string> problems = new List<string>();
        if(map.NodesOnFloor(0).Count < 2) problems.Add("fewer than two first-floor nodes");
        for(int i = 0; i < map.nodes.Count; i++){
            if(i != map.bossNode && map.nodes[i].next.Count == 0) problems.Add("dead end at node " + i);
            if(map.nodes[i].floor > 0 && Parents(map, i).Count == 0) problems.Add("unreachable node " + i);
        }
        foreach(List<int> route in map.Routes()){
            int battles = 0, markets = 0;
            for(int i = 0; i < route.Count; i++){
                MapNodeType type = map.nodes[route[i]].type;
                if(type == MapNodeType.Battle || type == MapNodeType.Elite) battles++;
                if(type == MapNodeType.Market) markets++;
                if(i > 0 && type != MapNodeType.Battle && type == map.nodes[route[i - 1]].type) problems.Add("two " + type + " in a row");
            }
            if(battles < config.minBattlesPerRoute) problems.Add("a route with " + battles + " battles");
            if(markets < config.minMarketsPerRoute) problems.Add("a route without a Market");
        }
        return problems;
    }
}
