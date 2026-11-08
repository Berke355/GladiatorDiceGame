using UnityEngine;

// How an action steers the next roll (see GDD "Die Layout" and "Rerolls": the Fortune redesign).
// Assets store this as a number: add new values at the end.
public enum RollSteer { None, AnyOther, Opposite, Neighbor, Position, Chosen }

// Fortune's engine: after this action the die rolls again in the same turn, onto a steered face. BattleFlow reads the
// request after the action's effects ran, and the actions-per-turn limit can end the chain. Change is any other face,
// Flip the opposite face, Tumble a neighbor or a named position, and Chosen lets the player pick. Enemies ignore it.
[CreateAssetMenu(fileName = "NewSteerRollEffect", menuName = "Dice/Effects/Steer Roll Effect")]
public class SteerRollEffectSO : EffectSO
{
    public RollSteer steer = RollSteer.AnyOther;
    // Position: the face's place on the net (see DieLayout).
    public int position;

    public override void Execute(EffectContext context){
        context.steer = steer;
        context.steerPosition = position;
    }

    public override string Describe(EffectContext context){
        switch(steer){
            case RollSteer.AnyOther: return "Change: roll another face";
            case RollSteer.Opposite: return "Flip: roll the opposite face";
            case RollSteer.Neighbor: return "Tumble: roll a neighboring face";
            case RollSteer.Position: return "Tumble: roll the " + DieLayout.Name(position) + " face";
            case RollSteer.Chosen: return "choose the next face";
            default: return "";
        }
    }
}
