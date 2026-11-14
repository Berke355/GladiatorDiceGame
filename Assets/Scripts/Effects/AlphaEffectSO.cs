using UnityEngine;

// Alpha (GDD "Beastmaster", Epic): calls the beast if it isn't out, raises its max HP by the value, then the source
// gains Hound equal to the beast's HP: toughness turned into strength. Brings its own beast, so it works on any die.
[CreateAssetMenu(fileName = "NewAlphaEffect", menuName = "Dice/Effects/Alpha Effect")]
public class AlphaEffectSO : EffectSO
{
    public override void Execute(EffectContext context){
        BeastCompanion beast = context.source != null ? context.source.beast : null;
        if(beast == null) return;
        beast.Summon();
        beast.RaiseMaxHP(context.value);
        context.GainSelf(StatusType.Hound, beast.HP);
    }

    public override string Describe(EffectContext context){
        BeastCompanion beast = context.source != null ? context.source.beast : null;
        int hp = (beast != null && beast.IsOut ? beast.HP : BeastCompanion.BaseMaxHP) + context.value;
        if(beast == null) return "your beast gains " + context.value + " max HP, then gain Hound equal to its HP";
        context.NotePreviewGain(context.source, StatusType.Hound, hp);
        return "your beast gains " + context.value + " max HP, then gain " + hp + " Hound (its HP)";
    }
}
