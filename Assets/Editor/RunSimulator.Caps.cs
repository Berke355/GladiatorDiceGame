using System.Collections.Generic;
using System.Text;
using UnityEditor;
using UnityEngine;

// Upgrade caps (GDD "Upgrade Caps"): how strong a face gets as its value grows, to find where it breaks the game.
// Each test die is the starting die with the face on Rightmost, rolled twice as often (Weighted Die: the concentration
// a player can build, as in the play-test that found Shield breaking), against act 3's strong encounters at battle 10.
public static partial class RunSimulator
{
    public class CapPoint
    {
        public int value;
        public float winRate;
        public float hpLost;
        public float turns;
    }

    // For each face (by name; empty: every face of the Market pool and the starting die), its results from its base value
    // to base + maxExtra. fights per point.
    public static Dictionary<string, List<CapPoint>> MeasureValueCurve(int fights, int maxExtra, params string[] faceNames){
        RunManager config = AssetDatabase.LoadAssetAtPath<RunManager>("Assets/Resources/RunManager.prefab");
        EncounterConfigSO encounters = config.encounters;
        List<EnemySO> pool = BuildEnemyPool(encounters, null, null);
        List<EnemyGroup> groups = BuildGroups(encounters, pool);
        DieEnchantmentSO weighted = AssetDatabase.LoadAssetAtPath<DieEnchantmentSO>("Assets/DieEnchantments/WeightedDie.asset");
        List<DieFaceSO> faces = new List<DieFaceSO>();
        foreach(DieFaceSO face in config.economy.marketPool) if(!faces.Contains(face)) faces.Add(face);
        foreach(DieFaceSO face in config.startingDieFaces) if(!faces.Contains(face)) faces.Add(face);
        if(faceNames != null && faceNames.Length > 0) faces = faces.FindAll(face => System.Array.IndexOf(faceNames, face.faceName) >= 0);

        Dictionary<string, List<CapPoint>> result = new Dictionary<string, List<CapPoint>>();
        GameObject playerObject = new GameObject("SimPlayer"){ hideFlags = HideFlags.HideAndDontSave };
        GameObject[] enemyObjects = new GameObject[EnemyGroup.MaxMembers];
        for(int i = 0; i < enemyObjects.Length; i++) enemyObjects[i] = new GameObject("SimEnemy" + i){ hideFlags = HideFlags.HideAndDontSave };
        bool logWasEnabled = Debug.unityLogger.logEnabled;
        Debug.unityLogger.logEnabled = false;
        try{
            foreach(DieFaceSO face in faces){
                if(face.isBlank || face.hasNoValue) continue;
                List<CapPoint> curve = new List<CapPoint>();
                for(int value = face.baseValue; value <= face.baseValue + maxExtra; value++){
                    int wins = 0, lost = 0, turnSum = 0;
                    for(int f = 0; f < fights; f++){
                        RunData run = new RunData(config.startingDieFaces, config.startingMaxHP);
                        run.dieFaces.Add(new DieFace(config.startingDieFaces[0]));
                        // The face on Rightmost (index 4), rolled twice as often.
                        run.dieFaces[DieLayout.Rightmost] = new DieFace(face){ value = value };
                        if(weighted != null) run.dieEnchantments.Add(weighted);
                        run.startingFaceChosen = true;
                        EnemyGroup encounter = EncounterConfigSO.PickEncounter(pool, groups, EnemyTier.Strong, new List<EnemySO>());
                        int turns;
                        bool won = FightBattle(run, encounter, null, playerObject, enemyObjects, out turns, encounters, 10);
                        if(won) wins++;
                        lost += run.maxHP - Mathf.Max(0, run.currentHP);
                        turnSum += turns;
                    }
                    curve.Add(new CapPoint{ value = value, winRate = (float)wins / fights, hpLost = (float)lost / fights, turns = (float)turnSum / fights });
                }
                result[face.faceName] = curve;
            }
        }
        finally{
            Debug.unityLogger.logEnabled = logWasEnabled;
            Object.DestroyImmediate(playerObject);
            foreach(GameObject each in enemyObjects) Object.DestroyImmediate(each);
        }
        return result;
    }

    public static string CapReport(Dictionary<string, List<CapPoint>> curves){
        StringBuilder text = new StringBuilder();
        foreach(KeyValuePair<string, List<CapPoint>> entry in curves){
            text.Append(entry.Key).Append(':');
            foreach(CapPoint point in entry.Value){
                text.Append(' ').Append(point.value).Append('=').Append(Mathf.RoundToInt(point.winRate * 100)).Append("%/")
                    .Append(point.hpLost.ToString("0")).Append("hp/").Append(point.turns.ToString("0")).Append('t');
            }
            text.Append('\n');
        }
        return text.ToString();
    }
}
