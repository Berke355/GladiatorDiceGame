using UnityEngine;

// Raises value of the source's minions in front of it (the Necromancer's Raise Dead), up to its maxMinions alive.
[CreateAssetMenu(fileName = "NewSummonMinionEffect", menuName = "Dice/Effects/Summon Minion Effect")]
public class SummonMinionEffectSO : EffectSO
{
    public override void Execute(EffectContext context){
        if(context.summon == null) return;
        for(int i = 0; i < Mathf.Max(1, context.value); i++){
            if(!context.summon()) break;
        }
    }

    public override string Describe(EffectContext context){
        int count = Mathf.Max(1, context.value);
        return "raise " + (count == 1 ? "a skeleton" : count + " skeletons");
    }
}
