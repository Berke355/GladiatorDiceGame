using UnityEngine;

[CreateAssetMenu(fileName = "VampireEffect", menuName = "Dice/Effects/Vampire Effect")]
public class VampireEffectSO : EffectSO
{
    public override void Execute(Entity player, Entity enemy, int value){
        enemy.TakeDamage(value);

        player.Heal(value);

        Debug.Log(player.entityName + " Vampir vuruşu yaptı! " + value + " hasar verdi ve canını doldurdu.");
    }
}
