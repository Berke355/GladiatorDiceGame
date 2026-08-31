using UnityEngine;

[CreateAssetMenu(fileName = "NewHealEffect", menuName = "Dice/Effects/Heal Effect")]
public class HealEffectSO : EffectSO
{
    public override void Execute(Entity source, Entity target, int value){
        source.Heal(value);
    }
}
