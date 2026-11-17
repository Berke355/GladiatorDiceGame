using UnityEngine;

// Pack Bond (GDD "Beastmaster"): a hit for the value, and the source's beast heals half of the damage dealt.
[CreateAssetMenu(fileName = "NewPackBondEffect", menuName = "Dice/Effects/Pack Bond Effect")]
public class PackBondEffectSO : EffectSO
{
    public override void Execute(EffectContext context){
        int dealt = context.Hit(context.value);
        BeastCompanion beast = context.source != null ? context.source.beast : null;
        if(beast != null) beast.Heal(dealt / 2);
    }

    public override string Describe(EffectContext context){
        int damage = context.PreviewHit(context.value);
        return damage + " damage, heal your beast half of it (" + damage / 2 + ")";
    }

    // Needs the player's own systems (see EffectSO.WorksForEnemies).
    public override bool WorksForEnemies => false;
}
