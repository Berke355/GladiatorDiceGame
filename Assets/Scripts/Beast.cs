using System;

// The Beastmaster's beast (GDD "Beastmaster"): the source's Hound stacks are its strength. A bite is the source's own
// hit (through EffectContext.Hit), so Rage, Weak, the target's Vulnerable and the source's on-hit items all count.
// Used at the end of the player's turn (BattleFlow.BeastBite, unless the player only defended) and by faces that
// make it attack at once (Sic 'Em).
public static class Beast
{
    public static int Bite(EffectContext context){
        if(context.source == null || context.target == null || context.target.IsDead) return 0;
        if(context.source.beast != null && !context.source.beast.IsOut) return 0;
        int hound = context.source.GetStatus(StatusType.Hound);
        if(hound <= 0) return 0;
        int dealt = context.Hit(hound);
        context.source.RaiseBeastBit(context.target);
        return dealt;
    }
}

// The beast's life in one battle (GDD "Beastmaster"): Hound is its strength, HP its toughness. The first Hound summons
// it with MaxHP; every hit that costs its owner HP costs it 1 HP; at 0 it dies and Hound is gone. A new Hound summons
// a fresh beast. Owned by BattleFlow, reached by effects through Entity.beast. Plain C#: the simulator runs it too.
public class BeastCompanion
{
    public const int BaseMaxHP = 4;

    private readonly Entity owner;
    // Max HP from items (Studded Harness), on top of BaseMaxHP for every beast summoned this battle.
    private readonly int itemMaxHP;
    // Loyal to the End: once per battle, a killing blow leaves the beast at 1 HP.
    private bool lastStandLeft;

    public int HP { get; private set; }
    public int MaxHP { get; private set; }
    public bool IsOut => HP > 0;
    // Heel!: the beast takes no damage until its owner's next turn.
    public bool Sheltered { get; private set; }
    // How many beasts died this battle (for tests and the simulator's report).
    public int Deaths { get; private set; }

    // For the picture: a hurt (HP lost), a death, a summon.
    public event Action<int> Hurt;
    public event Action Died;
    public event Action Summoned;

    public BeastCompanion(Entity owner, int itemMaxHP, bool lastStand){
        // An entity used for several battles (the simulator) keeps only the newest beast.
        if(owner.beast != null) owner.beast.Detach();
        this.owner = owner;
        this.itemMaxHP = itemMaxHP;
        lastStandLeft = lastStand;
        owner.beast = this;
        owner.HitTaken += OnHitTaken;
        owner.OnHealthChanged += CheckSummon;
        CheckSummon();
    }

    public void Detach(){
        owner.HitTaken -= OnHitTaken;
        owner.OnHealthChanged -= CheckSummon;
        if(owner.beast == this) owner.beast = null;
    }

    // The first Hound (or a new one after a death) calls the beast.
    private void CheckSummon(){
        if(!IsOut && !owner.IsDead && owner.GetStatus(StatusType.Hound) > 0) Summon();
    }

    // A fresh beast at full HP. Effects may call it without Hound (Alpha).
    public void Summon(){
        if(IsOut) return;
        MaxHP = BaseMaxHP + itemMaxHP;
        HP = MaxHP;
        Sheltered = false;
        Summoned?.Invoke();
    }

    private void OnHitTaken(int hpLost){
        if(!IsOut || Sheltered) return;
        Damage(1);
    }

    public void Damage(int amount){
        if(!IsOut || amount <= 0) return;
        HP -= amount;
        if(HP <= 0 && lastStandLeft){
            lastStandLeft = false;
            HP = 1;
        }
        Hurt?.Invoke(amount);
        if(HP > 0) return;
        HP = 0;
        Deaths++;
        Sheltered = false;
        // Its strength goes with it; the next Hound calls a new beast.
        owner.ReduceStatus(StatusType.Hound, owner.GetStatus(StatusType.Hound));
        Died?.Invoke();
    }

    // Heals up to MaxHP; returns the HP healed.
    public int Heal(int amount){
        if(!IsOut || amount <= 0) return 0;
        int healed = Math.Min(amount, MaxHP - HP);
        HP += healed;
        return healed;
    }

    // Raises this beast's max HP and heals as much (Leather Barding). A new beast starts from the base again.
    public void RaiseMaxHP(int amount){
        if(!IsOut || amount <= 0) return;
        MaxHP += amount;
        HP += amount;
    }

    public void Shelter(){
        if(IsOut) Sheltered = true;
    }

    // The owner's turn starts: Heel! ends.
    public void OnOwnerTurnStart(){
        Sheltered = false;
    }
}
