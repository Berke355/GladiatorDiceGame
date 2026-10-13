using UnityEngine;

[CreateAssetMenu(fileName = "VampireEffect", menuName = "Dice/Effects/Vampire Effect")]
public class VampireEffectSO : EffectSO
{
    public override void Execute(EffectContext context){
        context.target.TakeDamage(context.value, context.source);

        context.source.Heal(context.value);

        Debug.Log(context.source.entityName + " used Vampire strike! Dealt " + context.value + " damage and healed.");
    }

    public override string Describe(int value){
        return value + " damage, heal " + value;
    }
}
