using UnityEngine;

// Deals value + the source's Block + Fortify as damage. Nothing is consumed (e.g. Shield Bash).
[CreateAssetMenu(fileName = "NewDefenseScaledDamageEffect", menuName = "Dice/Effects/Defense Scaled Damage Effect")]
public class DefenseScaledDamageEffectSO : EffectSO
{
    public override void Execute(EffectContext context)
    {
        int dealt = context.Hit(context.value + Defense(context.source));
        Debug.Log(context.source.entityName + " bashed with its defense and dealt " + dealt + " damage.");
    }

    public override string Describe(EffectContext context){
        if(context.source == null){
            return context.value + " + block + Fortify damage";
        }
        return (context.value + Defense(context.source) + context.RageBonus) + " damage";
    }

    private static int Defense(Entity source){
        return source.currentBlock + source.GetStatus(StatusType.Fortify);
    }
}
