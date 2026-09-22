using UnityEngine;

[CreateAssetMenu(fileName = "VampireEffect", menuName = "Dice/Effects/Vampire Effect")]
public class VampireEffectSO : EffectSO
{
    public override void Execute(Entity source, Entity target, int value){
        target.TakeDamage(value);

        source.Heal(value);

        Debug.Log(source.entityName + " used Vampire strike! Dealt " + value + " damage and healed.");
    }
}
