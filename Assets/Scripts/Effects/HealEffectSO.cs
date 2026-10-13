using UnityEngine;

[CreateAssetMenu(fileName = "NewHealEffect", menuName = "Dice/Effects/Heal Effect")]
public class HealEffectSO : EffectSO
{
    public override void Execute(EffectContext context){
        context.source.Heal(context.value);
    }

    public override string Describe(int value){
        return "heal " + value;
    }
}
