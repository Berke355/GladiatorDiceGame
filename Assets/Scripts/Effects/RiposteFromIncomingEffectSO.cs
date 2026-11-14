using UnityEngine;

// Lex Talionis (see GDD "Provocator"): if the target enemy intends to attack, the source gains Riposte equal to the
// damage of one of its hits (its Rage included), so every hit it lands comes back in full. Through GainSelf, so
// Encourage adds to it. Nothing if the enemy isn't attacking.
[CreateAssetMenu(fileName = "NewRiposteFromIncomingEffect", menuName = "Dice/Effects/Riposte From Incoming Effect")]
public class RiposteFromIncomingEffectSO : EffectSO
{
    public override void Execute(EffectContext context){
        int hit = HitDamage(context);
        if(hit <= 0) return;
        context.GainSelf(StatusType.Riposte, hit);
    }

    public override string Describe(EffectContext context){
        if(context.targetBrain == null) return "if the enemy intends to attack: gain Riposte equal to its hit";
        int hit = HitDamage(context);
        if(hit <= 0) return "(the enemy doesn't attack: no Riposte)";
        int amount = hit + context.EncourageBonus;
        context.NotePreviewGain(context.source, StatusType.Riposte, amount);
        return "gain " + amount + " Riposte";
    }

    // The damage of one of the target's hits this turn, as its intent shows it.
    private static int HitDamage(EffectContext context){
        if(context.targetBrain == null || !context.targetBrain.IntendsAttack) return 0;
        return Mathf.Max(0, context.targetBrain.IncomingDamage);
    }
}
