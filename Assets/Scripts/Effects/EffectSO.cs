using UnityEngine;

public abstract class EffectSO : ScriptableObject
{
    public abstract void Execute(Entity player, Entity enemy, int value);
}
