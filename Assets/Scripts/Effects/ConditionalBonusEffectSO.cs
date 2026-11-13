using UnityEngine;

// Deals damage (or gains block) equal to the value, plus a printed bonus if a condition holds (Feint, Sharpshooter,
// Spare Blade). The bonus is additive, so these stay Common or Rare. Assets store Condition as a number: append only.
[CreateAssetMenu(fileName = "NewConditionalBonusEffect", menuName = "Dice/Effects/Conditional Bonus Effect")]
public class ConditionalBonusEffectSO : EffectSO
{
    public enum Condition { ActionChanged, NotRerolledThisTurn, FaceEnchanted }
    public Condition condition;
    public int bonus = 2;
    // Gains block instead of dealing damage.
    public bool block;

    public override void Execute(EffectContext context){
        int amount = context.value + (Met(context) ? bonus : 0);
        if(block) context.source.GainBlock(amount);
        else context.Hit(amount);
    }

    public override string Describe(EffectContext context){
        int amount = context.value + (Met(context) ? bonus : 0);
        return block ? amount + " block" : context.PreviewHit(amount) + " damage";
    }

    private bool Met(EffectContext context){
        switch(condition){
            case Condition.ActionChanged: return context.actionChanged;
            case Condition.NotRerolledThisTurn: return context.rerolls != null && context.rerolls.UsedThisTurn == 0;
            default: return context.face != null && context.face.enchantment != null;
        }
    }
}
