using UnityEngine;

// Deals damage and heals the source by the damage of the hit (Rage included).
[CreateAssetMenu(fileName = "VampireEffect", menuName = "Dice/Effects/Vampire Effect")]
public class VampireEffectSO : EffectSO
{
    public override void Execute(EffectContext context){
        int dealt = context.Hit(context.value);
        context.source.Heal(dealt);

        Debug.Log(context.source.entityName + " used Vampire strike! Dealt " + dealt + " damage and healed.");
    }

    public override string Describe(EffectContext context){
        int damage = context.value + context.RageBonus;
        return damage + " damage, heal " + damage;
    }
}
