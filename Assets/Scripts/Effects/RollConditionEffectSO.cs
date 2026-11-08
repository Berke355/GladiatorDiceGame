using UnityEngine;
using System.Collections.Generic;

// Fortune's payoff: runs its inner effects only if the roll meets a condition, e.g. "if this is my second time this
// turn". Without the player's die (enemies) the condition isn't met. Assets store Condition as a number: append only.
[CreateAssetMenu(fileName = "NewRollConditionEffect", menuName = "Dice/Effects/Roll Condition Effect")]
public class RollConditionEffectSO : EffectSO
{
    public enum Condition { FirstTurn, LandedTimesThisTurn, ArrivedBySteer, OppositeHasTag }
    public Condition condition;
    // LandedTimesThisTurn: this face has come up at least this many times this turn.
    public int times = 2;
    // OppositeHasTag: the face opposite this one on the net carries this tag.
    public FaceTag tag;
    public List<EffectSO> effects = new List<EffectSO>();

    public override void Execute(EffectContext context){
        if(!Met(context)) return;
        foreach(EffectSO effect in effects){
            if(effect != null) effect.Execute(context);
        }
    }

    public override string Describe(EffectContext context){
        List<string> parts = new List<string>();
        foreach(EffectSO effect in effects){
            string description = effect != null ? effect.Describe(context) : "";
            if(description != "") parts.Add(description);
        }
        // During a turn (the action previews) a condition already met shows just its result.
        if(context.face != null && context.turnNumber > 0 && Met(context)) return string.Join(", ", parts);
        return "(" + ConditionText() + ": " + string.Join(", ", parts) + ")";
    }

    private string ConditionText(){
        switch(condition){
            case Condition.FirstTurn: return "on the first turn";
            case Condition.LandedTimesThisTurn: return "if this is my " + (times == 2 ? "second" : times == 3 ? "third" : times + "th") + " time this turn";
            case Condition.ArrivedBySteer: return "if I came from a Change, Flip or Tumble";
            default: return "if the opposite face is " + tag;
        }
    }

    private bool Met(EffectContext context){
        switch(condition){
            case Condition.FirstTurn: return context.turnNumber == 1;
            case Condition.LandedTimesThisTurn: return context.faceLandings >= times;
            case Condition.ArrivedBySteer: return context.arrivedBySteer;
            default:
                if(context.die == null || context.face == null) return false;
                int here = context.die.IndexOf(context.face);
                if(here < 0 || here >= DieLayout.Count) return false;
                int opposite = DieLayout.OppositeOf(here);
                return opposite < context.die.Count && context.die[opposite].definition.tags.Contains(tag);
        }
    }
}
