using UnityEngine;

public abstract class EffectSO : ScriptableObject
{
    public abstract void Execute(EffectContext context);
}
