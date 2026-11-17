using System.Collections.Generic;
using UnityEngine;

// The Mimic's intents (GDD "Elite Enemies", the user's design): an elite with its own intents, each one copying the
// player. It reads the player's die through the enemy's context (targetDie, targetCharge) and the player's last action
// (targetLastFace / targetLastAction / targetLastValue).
// A copied face plays its own effects with the Mimic as source; a face whose effects need the player's own systems
// (EffectSO.WorksForEnemies: rerolls, the beast, reading an intent...) is played as a plain hit of its value instead
// ("if I can't steal the face, I steal its value").
[CreateAssetMenu(fileName = "NewMimicEffect", menuName = "Dice/Effects/Mimic Effect")]
public class MimicEffectSO : EffectSO
{
    public enum Mode
    {
        // Play the player's highest-value face, with its value (Attack, else Magic, else Defense).
        PlayHighestFace,
        // Play the face and action the player played last, with the value it had; the intent's value without one.
        RepeatLastAction,
        // Gain every status the player has, good and bad alike (3 Rage and 5 Fortify on the player: the same on it).
        CopyStatuses,
        // Damage equal to the player's highest face value.
        DamageEqualHighest,
        // Block equal to the player's highest face value.
        BlockEqualHighest
    }
    public Mode mode;

    // Statuses the Mimic never copies: the player's stance and beast, which only work for the player.
    private static readonly StatusType[] NotCopied = { StatusType.Deflect, StatusType.PerfectGuard, StatusType.Hound, StatusType.RetiredVigil };

    public override void Execute(EffectContext context){
        DieFace face;
        ActionType action;
        int value;
        switch(mode){
            case Mode.CopyStatuses:
                foreach(KeyValuePair<StatusType, int> status in Copied(context)) context.source.ApplyStatus(status.Key, status.Value);
                return;
            case Mode.DamageEqualHighest:
                context.Hit(HighestValue(context));
                return;
            case Mode.BlockEqualHighest:
                context.source.GainBlock(HighestValue(context));
                return;
        }
        if(!Copy(context, out face, out action, out value)){
            context.Hit(context.value);
            return;
        }
        if(!Portable(face, action)){
            context.Hit(value);
            return;
        }
        EffectContext played = PlayedContext(context, value);
        foreach(EffectSO effect in face.GetEffects(action)){
            if(effect != null && !context.target.IsDead && !context.source.IsDead) effect.Execute(played);
        }
    }

    public override string Describe(EffectContext context){
        DieFace face;
        ActionType action;
        int value;
        switch(mode){
            case Mode.CopyStatuses:
                List<string> parts = new List<string>();
                foreach(KeyValuePair<StatusType, int> status in Copied(context)) parts.Add(status.Value + " " + GameText.StatusName(status.Key));
                return parts.Count > 0 ? "copies your statuses: " + string.Join(", ", parts) : "copies your statuses (you have none)";
            case Mode.DamageEqualHighest:
                return context.PreviewHit(HighestValue(context)) + " damage (your highest face's value)";
            case Mode.BlockEqualHighest:
                return HighestValue(context) + " block (your highest face's value)";
        }
        if(!Copy(context, out face, out action, out value)) return context.PreviewHit(context.value) + " damage";
        string name = face.DisplayName;
        if(!Portable(face, action)) return context.PreviewHit(value) + " damage (copies your " + name + "'s value)";
        EffectContext played = PlayedContext(context, value);
        List<string> lines = new List<string>();
        foreach(EffectSO effect in face.GetEffects(action)){
            string line = effect != null ? effect.Describe(played) : "";
            if(line != "") lines.Add(line);
        }
        return "copies your " + name + " (" + action + "): " + string.Join(", ", lines);
    }

    // The face, action and value this intent copies; false when there is none (no die, nothing played yet).
    private bool Copy(EffectContext context, out DieFace face, out ActionType action, out int value){
        face = null;
        action = ActionType.Attack;
        value = 0;
        if(mode == Mode.RepeatLastAction){
            if(context.targetLastFace == null || context.targetLastFace.IsBlank) return false;
            face = context.targetLastFace;
            action = context.targetLastAction;
            value = context.targetLastValue;
            return face.HasAction(action);
        }
        face = Highest(context);
        if(face == null) return false;
        action = face.HasAction(ActionType.Attack) ? ActionType.Attack : face.HasAction(ActionType.Magic) ? ActionType.Magic : ActionType.Defense;
        value = ValueOf(context, face);
        return face.HasAction(action);
    }

    private static bool Portable(DieFace face, ActionType action){
        foreach(EffectSO effect in face.GetEffects(action)){
            if(effect != null && !effect.WorksForEnemies) return false;
        }
        return true;
    }

    // The copy plays as the Mimic, against the player, with the copied value; no die of its own.
    private static EffectContext PlayedContext(EffectContext context, int value){
        return new EffectContext{ source = context.source, target = context.target, value = value, allies = context.allies, targetRerolls = context.targetRerolls };
    }

    private static DieFace Highest(EffectContext context){
        if(context.targetDie == null) return null;
        DieFace best = null;
        foreach(DieFace face in context.targetDie){
            if(face == null || face.IsBlank) continue;
            if(best == null || ValueOf(context, face) > ValueOf(context, best)) best = face;
        }
        return best;
    }

    private static int ValueOf(EffectContext context, DieFace face){
        return context.targetCharge != null ? context.targetCharge.ValueOf(face) : face.value;
    }

    private int HighestValue(EffectContext context){
        DieFace face = Highest(context);
        return face != null ? ValueOf(context, face) : context.value;
    }

    private static List<KeyValuePair<StatusType, int>> Copied(EffectContext context){
        List<KeyValuePair<StatusType, int>> copied = new List<KeyValuePair<StatusType, int>>();
        if(context.target == null) return copied;
        foreach(KeyValuePair<StatusType, int> status in context.target.Statuses){
            if(status.Value <= 0 || status.Key == StatusType.None || System.Array.IndexOf(NotCopied, status.Key) >= 0) continue;
            copied.Add(status);
        }
        return copied;
    }
}
