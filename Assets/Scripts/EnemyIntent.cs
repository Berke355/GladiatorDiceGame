using UnityEngine;
using System;

[Serializable]
public struct EnemyIntent{
    public string intentName;
    public ActionType intentType;
    public EffectSO effectLogic;
    public int value;
}
