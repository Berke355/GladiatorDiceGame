using UnityEngine;

// Lex Talionis (GDD "Deflect"): if an enemy intends to attack this turn, the face's value rises to all the damage
// coming this turn (Rage included), so its Deflect keyword, given after its effects with that value, meets every hit.
// Nothing changes if no attack is coming: the stance then whiffs like any other.
[CreateAssetMenu(fileName = "NewRiposteFromIncomingEffect", menuName = "Dice/Effects/Riposte From Incoming Effect")]
public class RiposteFromIncomingEffectSO : EffectSO
{
    public override void Execute(EffectContext context){
        context.value = Mathf.Max(context.value, context.incomingDamage);
    }

    // The preview raises the value too, so the Deflect the keyword adds previews the real number.
    public override string Describe(EffectContext context){
        if(context.incomingDamage <= context.value) return context.incomingDamage > 0 ? "my value already meets the attack" : "(no attack coming: my value stays)";
        context.value = context.incomingDamage;
        return "my value becomes " + context.incomingDamage + " (the damage coming)";
    }
}
