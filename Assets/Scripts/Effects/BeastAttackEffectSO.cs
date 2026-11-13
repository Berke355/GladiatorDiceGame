using UnityEngine;

// The beast attacks at once, value times (Sic 'Em): each bite hits for the source's Hound, like its bite at the end
// of the turn. Multiplicative (value × Hound), so it belongs on Epics.
[CreateAssetMenu(fileName = "NewBeastAttackEffect", menuName = "Dice/Effects/Beast Attack Effect")]
public class BeastAttackEffectSO : EffectSO
{
    public override void Execute(EffectContext context){
        for(int i = 0; i < context.value && !context.target.IsDead; i++){
            Beast.Bite(context);
        }
    }

    public override string Describe(EffectContext context){
        int bites = context.value;
        string times = bites == 1 ? "once" : bites + " times";
        if(context.source == null) return "your beast bites " + times;
        int hound = context.Stacks(context.source, StatusType.Hound);
        if(hound <= 0) return "your beast bites " + times + " (no beast yet)";
        return (bites == 1 ? "" : bites + " x ") + context.PreviewHit(hound) + " damage (beast)";
    }
}
