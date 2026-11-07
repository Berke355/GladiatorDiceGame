using UnityEngine;

// Copies the target's stacks of a status onto every other living opponent (Contagion). The copies are applied to
// the opponent, so each gets the source's Ritual. Against a single enemy it does nothing; faces using it pair it
// with an effect that works alone (e.g. apply Poison first).
[CreateAssetMenu(fileName = "NewSpreadStatusEffect", menuName = "Dice/Effects/Spread Status Effect")]
public class SpreadStatusEffectSO : EffectSO
{
    public StatusType status = StatusType.Poison;

    public override void Execute(EffectContext context){
        Entity original = context.target;
        int stacks = original != null ? original.GetStatus(status) : 0;
        if(stacks <= 0) return;
        foreach(Entity opponent in context.AllOpponents){
            if(opponent == original || opponent == null || opponent.IsDead) continue;
            context.target = opponent;
            context.ApplyToTarget(status, stacks);
        }
        context.target = original;
        Debug.Log(context.source.entityName + " spread " + stacks + " " + status + " to the other enemies.");
    }

    public override string Describe(EffectContext context){
        return "copy the target's " + status + " to every other enemy";
    }
}
