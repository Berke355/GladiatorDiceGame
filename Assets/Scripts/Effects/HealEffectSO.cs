using UnityEngine;

[CreateAssetMenu(fileName = "NewHealEffect", menuName = "Dice/Effects/Heal Effect")]
public class HealEffectSO : EffectSO
{
    public override void Execute(Entity player, Entity enemy, int value){
        player.Heal(value);
    }
}
