using UnityEngine;

// A telegraphed threat on the player's choice this turn (e.g. Essedarius's Run Down and Lash): when the enemy acts,
// it hits for my value only if the player's last action met the condition. Faces without a numeric value count as
// below any threshold. Without the player's last action in the context (e.g. on a face), it does nothing.
[CreateAssetMenu(fileName = "NewPunishChoiceEffect", menuName = "Dice/Effects/Punish Choice Effect")]
public class PunishChoiceEffectSO : EffectSO
{
    public enum Condition { FaceValueBelow, ChoseAction }
    public Condition condition;
    // FaceValueBelow: the battle value (value plus Charge) the face must reach to be safe.
    public int threshold = 3;
    // ChoseAction: the punished action.
    public ActionType action;

    public override void Execute(EffectContext context){
        if(context.targetLastFace == null) return;
        if(!Triggers(context.targetLastFace, context.targetLastValue, context.targetLastAction)) return;
        int dealt = context.Hit(context.value);
        Debug.Log(context.source.entityName + " punished the player's choice for " + dealt + " damage.");
    }

    public override string Describe(EffectContext context){
        string damage = (context.value + context.RageBonus) + " damage";
        return condition == Condition.FaceValueBelow
            ? damage + " if you act with a face below " + threshold
            : damage + " if you choose " + action;
    }

    // Whether acting with this face (at this battle value) and this action sets the threat off.
    public bool Triggers(DieFace face, int battleValue, ActionType chosen){
        if(condition == Condition.ChoseAction) return chosen == action;
        return face.definition.hasNoValue || battleValue < threshold;
    }
}
