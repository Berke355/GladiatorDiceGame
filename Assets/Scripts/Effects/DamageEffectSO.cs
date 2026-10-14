using UnityEngine;

[CreateAssetMenu(fileName = "NewDamageEffect", menuName = "Dice/Effects/Damage Effect")]
public class DamageEffectSO : EffectSO
{
    public override void Execute(EffectContext context){
        int dealt = context.Hit(context.value);
        Debug.Log(context.source.entityName + " attacked! Dealt " + dealt + " damage.");
    }

    public override string Describe(EffectContext context){
        return (context.value + context.RageBonus) + " damage";
    }
}
