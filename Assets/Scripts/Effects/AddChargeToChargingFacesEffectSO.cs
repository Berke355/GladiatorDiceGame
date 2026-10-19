using UnityEngine;

// Adds value as Charge to every other face on the source's die that has the Charge rule, and the triggering
// face keeps its own Charge (e.g. Resonance). This Charge ignores the cap. Does nothing without a die.
[CreateAssetMenu(fileName = "NewAddChargeToChargingFacesEffect", menuName = "Dice/Effects/Add Charge To Charging Faces Effect")]
public class AddChargeToChargingFacesEffectSO : EffectSO
{
    public override void Execute(EffectContext context){
        if(context.charge == null || context.die == null) return;
        context.keepCharge = true;
        int charged = 0;
        foreach(DieFace face in context.die){
            if(face == context.face || !face.definition.charges) continue;
            context.charge.Add(face, context.value, false);
            charged++;
        }
        Debug.Log(context.source.entityName + " charged " + charged + " faces by " + context.value + ".");
    }

    public override string Describe(EffectContext context){
        if(context.die == null){
            return context.value + " Charge to your Charge faces";
        }
        int count = context.die.FindAll(face => face != context.face && face.definition.charges).Count;
        return context.value + " Charge to " + count + (count == 1 ? " face" : " faces") + ", keep my Charge";
    }
}
