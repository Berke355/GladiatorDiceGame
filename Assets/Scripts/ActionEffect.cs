using UnityEngine;
using System;

[Serializable]
public struct ActionEffect
{
    public ActionType actionType;
    public EffectSO effectLogic;
    public float multiplier;
}
