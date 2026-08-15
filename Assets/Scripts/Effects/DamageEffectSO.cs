using UnityEngine;

[CreateAssetMenu(fileName = "NewDamageEffect", menuName = "Dice/Effects/Damage Effect")]
public class DamageEffectSO : EffectSO
{
    public override void Execute(Entity player, Entity enemy, int value){
        enemy.TakeDamage(value);
        Debug.Log(player.entityName + " saldırdı! Düşmana " + value + " hasar verdi.");
    }
}
