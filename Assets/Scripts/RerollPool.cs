// The player's rerolls for one battle (see GDD "Rerolls"). Plain C#, owned by BattleFlow and refilled every battle.
// Passed to effects through EffectContext.rerolls, so items and faces can grant rerolls or reward using (or saving) them.
public class RerollPool
{
    public int Left;
    public int UsedThisTurn;
    public int UsedThisBattle;

    public RerollPool(int perBattle){
        Left = perBattle;
    }
}
