using System.Collections.Generic;
using UnityEngine;

// The Mimic (an Elite, GDD "Elite Enemies"): it copies the player's die at the start of the battle (faces, values and
// face enchantments) and fights with it. At the start of the player's turn it rolls its copy, and the rolled face is
// its intent, with the action it prefers: Attack, else Magic, else Defense. The player's own effects run with the
// Mimic as source, which works because every effect is source/target-agnostic. Plain C#, one per battle.
public class MimicBrain : IEnemyBrain
{
    public readonly EnemySO definition;
    private readonly Entity self;
    private readonly Entity opponent;
    private readonly List<DieFace> die = new List<DieFace>();
    private BattleFlow flow;
    private ActionType action;

    public List<DieFace> Die => die;
    public DieFace RolledFace { get; private set; }
    public List<EnemyTrait> Traits => definition.traits;
    public int ThreatAgainst(DieFace face, int battleValue, ActionType chosen){ return 0; }

    public MimicBrain(EnemySO definition, Entity self, Entity opponent, List<DieFace> playerDie){
        this.definition = definition;
        this.self = self;
        this.opponent = opponent;
        self.maxHPLossPerRound = 0;
        if(playerDie == null) return;
        // A snapshot: the player's upgrades later in the run don't change this battle's copy.
        foreach(DieFace face in playerDie){
            die.Add(new DieFace(face.definition){ value = face.value, enchantment = face.enchantment });
        }
    }

    public void Bind(BattleFlow flow){ this.flow = flow; }

    // The action the Mimic takes with a face: Attack if it has one, else Magic, else Defense. Blank faces do nothing.
    public static ActionType Preferred(DieFace face){
        if(face.HasAction(ActionType.Attack)) return ActionType.Attack;
        if(face.HasAction(ActionType.Magic)) return ActionType.Magic;
        return ActionType.Defense;
    }

    public void ChooseNextIntent(){
        if(die.Count == 0) return;
        RolledFace = die[Random.Range(0, die.Count)];
        action = Preferred(RolledFace);
        Debug.Log(definition.enemyName + " rolled " + RolledFace.definition.faceName + ": " + DescribeCurrentIntent());
    }

    public string DescribeCurrentIntent(){
        if(RolledFace == null) return "";
        if(RolledFace.IsBlank) return RolledFace.DisplayName + ": nothing";
        EffectContext context = CreateContext();
        List<string> parts = new List<string>();
        foreach(EffectSO effect in RolledFace.GetEffects(action)){
            string description = effect != null ? effect.Describe(context) : "";
            if(description != "") parts.Add(description);
        }
        return RolledFace.DisplayName + " (" + action + "): " + string.Join(", ", parts);
    }

    public void ExecuteCurrentIntent(){
        if(RolledFace == null || RolledFace.IsBlank) return;
        EffectContext context = CreateContext();
        foreach(EffectSO effect in RolledFace.GetEffects(action)){
            if(effect != null && !opponent.IsDead && !self.IsDead) effect.Execute(context);
        }
    }

    public void OnTurnEnd(){ }

    public bool IntendsAttack => RolledFace != null && !RolledFace.IsBlank && action == ActionType.Attack;

    // An estimate for the bots: one hit of the face's value with Rage.
    public int IncomingDamage => IntendsAttack ? RolledFace.value + self.GetStatus(StatusType.Rage) : 0;

    // Provoke: its weakest attack face.
    public bool ProvokeToWeakestAttack(){
        if(RolledFace == null || IntendsAttack) return false;
        DieFace weakest = null;
        foreach(DieFace face in die){
            if(face.IsBlank || !face.HasAction(ActionType.Attack)) continue;
            if(weakest == null || face.value < weakest.value) weakest = face;
        }
        if(weakest == null) return false;
        RolledFace = weakest;
        action = ActionType.Attack;
        return true;
    }

    private EffectContext CreateContext(){
        List<Entity> allies = new List<Entity>();
        if(flow != null){
            foreach(BattleEnemy enemy in flow.enemies) if(!enemy.IsDead && enemy.entity != self) allies.Add(enemy.entity);
        }
        return new EffectContext{
            source = self, target = opponent, value = RolledFace != null ? RolledFace.value : 0,
            die = die, face = RolledFace, allies = allies,
            targetRerolls = flow != null ? flow.rerolls : null
        };
    }
}
