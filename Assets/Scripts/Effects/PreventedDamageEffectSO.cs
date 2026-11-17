using UnityEngine;

// The Shield build's siege payoffs (GDD "Shield"): a hit grown from the damage the source's Block and Fortify absorbed.
// Unyielding: my value plus what was absorbed on the enemies' last turn. Siege Breaker: what was absorbed this battle
// times my value (multiplicative, an Epic), and the count starts again.
[CreateAssetMenu(fileName = "NewPreventedDamageEffect", menuName = "Dice/Effects/Prevented Damage Effect")]
public class PreventedDamageEffectSO : EffectSO
{
    // Off: the enemies' last turn, added to the value. On: the whole battle, times the value, then spent.
    public bool wholeBattle;

    public override void Execute(EffectContext context){
        context.Hit(Damage(context));
        if(wholeBattle && context.spendPrevented != null) context.spendPrevented();
    }

    public override string Describe(EffectContext context){
        int damage = context.PreviewHit(Damage(context));
        return wholeBattle
            ? damage + " damage (" + context.preventedThisBattle + " prevented this battle x " + context.value + "), the count starts again"
            : damage + " damage (" + context.preventedLastEnemyTurn + " prevented last turn)";
    }

    private int Damage(EffectContext context){
        return wholeBattle ? context.preventedThisBattle * context.value : context.value + context.preventedLastEnemyTurn;
    }

    // Needs the player's own systems (see EffectSO.WorksForEnemies).
    public override bool WorksForEnemies => false;
}
