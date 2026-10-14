using UnityEngine;

// Deals value + the source's Block + Fortify as damage. Nothing is consumed (e.g. Shield Bash).
[CreateAssetMenu(fileName = "NewDefenseScaledDamageEffect", menuName = "Dice/Effects/Defense Scaled Damage Effect")]
public class DefenseScaledDamageEffectSO : EffectSO
{
    public override void Execute(EffectContext context)
    {
        int defense = context.source.currentBlock + context.source.GetStatus(StatusType.Fortify);
        int totalDamage = context.value + defense;

        context.target.TakeDamage(totalDamage, context.source);
        Debug.Log(context.source.entityName + " bashed with " + defense + " defense and dealt " + totalDamage + " damage.");
    }

    public override string Describe(int value){
        return value + " + block + Fortify damage";
    }
}
