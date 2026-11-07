// One enemy in a battle: its entity, the brain that picks and runs its intents, and its traits.
// A battle holds a list of them (see GDD "Multiple Enemies"); each one has its own intent, traits and statuses.
// Plain C#, created for one battle.
public class BattleEnemy
{
    public readonly Entity entity;
    public readonly IEnemyBrain brain;
    // Created by BattleFlow when the enemy joins the battle.
    public EnemyTraitSystem traits { get; internal set; }

    // The battle ends when this enemy dies, even if others still live (a boss, e.g. the Hydra's body with its heads).
    public readonly bool endsBattleOnDeath;

    public BattleEnemy(Entity entity, IEnemyBrain brain, bool endsBattleOnDeath = false){
        this.entity = entity;
        this.brain = brain;
        this.endsBattleOnDeath = endsBattleOnDeath;
    }

    public bool IsDead => entity.IsDead;
}
