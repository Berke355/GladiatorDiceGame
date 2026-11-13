// The Beastmaster's beast (GDD "Beastmaster"): the source's Hound stacks are its strength. A bite is the source's own
// hit (through EffectContext.Hit), so Rage, Weak, the target's Vulnerable and the source's on-hit items all count.
// Used at the end of the player's turn (BattleFlow.EndPlayerTurn, unless the player only defended) and by faces that
// make it attack at once (Sic 'Em).
public static class Beast
{
    public static int Bite(EffectContext context){
        if(context.source == null || context.target == null || context.target.IsDead) return 0;
        int hound = context.source.GetStatus(StatusType.Hound);
        if(hound <= 0) return 0;
        int dealt = context.Hit(hound);
        context.source.RaiseBeastBit(context.target);
        return dealt;
    }
}
