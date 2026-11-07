// One enemy in a battle: its entity, the brain that picks and runs its intents, and its traits.
// A battle holds a list of them (see GDD "Multiple Enemies"); each one has its own intent, traits and statuses.
// Plain C#, created for one battle.
public class BattleEnemy
{
    public readonly Entity entity;
    public readonly IEnemyBrain brain;
    // Created by BattleFlow once the brain is bound.
    public EnemyTraitSystem traits { get; internal set; }

    public BattleEnemy(Entity entity, IEnemyBrain brain){
        this.entity = entity;
        this.brain = brain;
    }

    public bool IsDead => entity.IsDead;
}
