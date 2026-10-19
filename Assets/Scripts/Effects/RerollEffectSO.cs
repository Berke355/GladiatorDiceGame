using UnityEngine;

// Rerolls the die and lets the source act again (e.g. Steady Hand). The effect only raises a request;
// BattleFlow and the battle states run the reroll, since effects don't control the turn.
[CreateAssetMenu(fileName = "NewRerollEffect", menuName = "Dice/Effects/Reroll Effect")]
public class RerollEffectSO : EffectSO
{
    public override void Execute(EffectContext context){
        context.rerollRequested = true;
    }

    public override string Describe(EffectContext context){
        return "reroll and act again";
    }
}
