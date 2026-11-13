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

    // How the enemy looks in battle (from its EnemySO or BossSO); null keeps the scene's placeholder.
    public UnityEngine.Sprite sprite;
    public float spriteHeight = 1f;
    // The world width BattleManager gives the enemy in the row (its sprite or its plate, whichever is wider, after a
    // crowded row shrinks); its card is as wide.
    public float slotWidth;
    // A head growing from another enemy's body (the Hydra's): drawn on that body's picture instead of standing in the row.
    public BattleEnemy mountedOn;
    // Which of the body's mounts the head sits on (-1: none yet). It keeps it, so a head never jumps to another neck.
    public int mountIndex = -1;
    // A dead head has let its mount go: when it grows back it takes a free one.
    public bool releasedMount;

    public BattleEnemy(Entity entity, IEnemyBrain brain, bool endsBattleOnDeath = false){
        this.entity = entity;
        this.brain = brain;
        this.endsBattleOnDeath = endsBattleOnDeath;
    }

    public bool IsDead => entity.IsDead;
}
