using UnityEngine;

[CreateAssetMenu(fileName = "NewDamageEffect", menuName = "Dice/Effects/Damage Effect")]
public class DamageEffectSO : EffectSO
{
    public override void Execute(EffectContext context){
        context.target.TakeDamage(context.value, context.source);
        Debug.Log(context.source.entityName + " attacked! Dealt " + context.value + " damage.");
    }

    public override string Describe(int value){
        return value + " damage";
    }
}
