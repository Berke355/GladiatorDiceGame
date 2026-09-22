using UnityEngine;

[CreateAssetMenu(fileName = "NewDamageEffect", menuName = "Dice/Effects/Damage Effect")]
public class DamageEffectSO : EffectSO
{
    public override void Execute(Entity source, Entity target, int value){
        target.TakeDamage(value);
        Debug.Log(source.entityName + " attacked! Dealt " + value + " damage.");
    }
}
