using UnityEngine;

// Gain 1 Rage for each hit the source took on the enemies' last turn, at most the value (Wrath). Through GainSelf,
// so Encourage adds to it.
[CreateAssetMenu(fileName = "NewRagePerHitTakenEffect", menuName = "Dice/Effects/Rage Per Hit Taken Effect")]
public class RagePerHitTakenEffectSO : EffectSO
{
    public override void Execute(EffectContext context){
        int hits = Hits(context);
        if(hits <= 0) return;
        int gained = context.GainSelf(StatusType.Rage, hits);
        Debug.Log(context.source.entityName + " gained " + gained + " Rage from the hits it took.");
    }

    public override string Describe(EffectContext context){
        int hits = Hits(context);
        if(hits <= 0) return "gain 0 Rage";
        int amount = hits + context.EncourageBonus;
        context.NotePreviewGain(context.source, StatusType.Rage, amount);
        return "gain " + amount + " Rage";
    }

    private static int Hits(EffectContext context){
        return Mathf.Min(context.hitsTakenLastEnemyTurn, context.value);
    }
}
