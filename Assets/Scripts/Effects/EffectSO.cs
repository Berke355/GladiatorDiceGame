using UnityEngine;

public abstract class EffectSO : ScriptableObject
{
    public abstract void Execute(Entity source, Entity target, int value);
}
