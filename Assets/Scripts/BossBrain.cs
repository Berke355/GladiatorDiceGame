using UnityEngine;
using System;
using System.Collections.Generic;

// The dice brain of a boss (see GDD "Boss Dice"): at the start of the player's turn it rolls its die,
// and the rolled face is its intent. Plain C#, created for one battle. Boss mechanics that outlive a turn
// (the Stampede countdown, the Hydra's heads) live here, never in RunData.
public class BossBrain : IEnemyBrain, IDisposable
{
    public readonly BossSO definition;
    private readonly Entity self;
    private readonly Entity opponent;
    private readonly List<DieFace> playerDie;
    private readonly List<DieFace> die = new List<DieFace>();
    // The battle, for the player's stone faces and Charge and for spawning heads (set by BattleFlow.Bind).
    private BattleFlow flow;

    public List<DieFace> Die => die;
    public List<EnemyTrait> Traits => new List<EnemyTrait>();
    public int ThreatAgainst(DieFace face, int battleValue, ActionType action){ return 0; }
    public DieFace RolledFace { get; private set; }

    // The running countdown (Stampede): enemy turns left until the hit, its damage, and HP lost from hits since it started.
    public int CountdownTurnsLeft { get; private set; }
    public int CountdownDamage { get; private set; }
    public int StaggerTaken { get; private set; }
    public bool CountdownRunning => CountdownTurnsLeft > 0;
    // The player's face this turn's intent aims at (Medusa's Gaze), locked when the die is rolled so the
    // telegraphed target is the one hit, even if the player's values change before the enemy acts.
    private DieFace lockedTarget;
    // The countdown started this turn, so this turn's end doesn't count down yet.
    private bool countdownJustStarted;

    // Heads (Hydra): every head ever grown (a separate enemy, dead ones included), and the dead heads already
    // counted as cut (grown back or cauterized), so each death is counted once.
    private readonly List<BattleEnemy> heads = new List<BattleEnemy>();
    private readonly HashSet<BattleEnemy> settledCuts = new HashSet<BattleEnemy>();
    public bool HasHeads => definition.headEnemy != null;
    public int LivingHeads => heads.FindAll(head => !head.IsDead).Count;
    // Heads killed since the boss's last turn; at its next turn start each grows back as two (or is cauterized).
    public int CutHeads => heads.FindAll(head => head.IsDead && !settledCuts.Contains(head)).Count;
    public bool RegrowthBlocked => Blocked(definition.regrowthBlockedBy);
    public bool RegenBlocked => Blocked(definition.regenBlockedBy);

    public BossBrain(BossSO boss, Entity self, Entity opponent, List<DieFace> playerDie){
        definition = boss;
        this.self = self;
        this.opponent = opponent;
        this.playerDie = playerDie;
        foreach(DieFaceSO face in boss.faces){
            die.Add(new DieFace(face));
        }
        self.HitTaken += OnHitTaken;
    }

    // The heads join the battle here, behind the boss.
    public void Bind(BattleFlow flow){
        this.flow = flow;
        if(!HasHeads) return;
        for(int i = 0; i < definition.startingHeads; i++){
            GrowHead();
        }
    }

    public void Dispose(){
        self.HitTaken -= OnHitTaken;
    }

    public void ChooseNextIntent(){
        if(die.Count == 0) return;
        RolledFace = die[UnityEngine.Random.Range(0, die.Count)];
        // A second countdown face while one is running counts as the fallback face (a Gore).
        if(CountdownRunning && StartsCountdown(RolledFace) && definition.whileCountingDown != null){
            RolledFace = die.Find(face => face.definition == definition.whileCountingDown) ?? new DieFace(definition.whileCountingDown);
        }
        lockedTarget = null;
        if(RolledFace.definition.effects.Exists(effect => effect.effectLogic is PetrifyHighestFaceEffectSO)){
            lockedTarget = PetrifyHighestFaceEffectSO.Target(CreateContext(RolledFace));
        }
        Debug.Log(definition.bossName + " rolled " + RolledFace.definition.faceName + ": " + DescribeCurrentIntent());
    }

    // The rolled face and its numbers, plus the countdown or the heads.
    public string DescribeCurrentIntent(){
        string text = RolledFace != null ? RolledFace.definition.faceName + ": " + DescribeFace(RolledFace) : "";
        if(CountdownRunning){
            int left = Mathf.Max(0, definition.staggerDamage - StaggerTaken);
            text += "\nStampede: " + (CountdownDamage + self.GetStatus(StatusType.Rage)) + " damage "
                + (CountdownTurnsLeft == 1 ? "at the end of this turn" : "in " + CountdownTurnsLeft + " turns")
                + " (stagger: " + left + " more damage)";
        }
        if(HasHeads){
            int cut = CutHeads;
            text += "\nHeads: " + LivingHeads
                + (cut > 0 ? " | " + cut + " cut: " + (RegrowthBlocked ? "cauterized by Burn" : "will grow back as " + 2 * cut) : "")
                + " | regenerates " + (RegenBlocked ? "0 (blocked)" : (definition.regenPerHead * LivingHeads).ToString());
        }
        return text;
    }

    private bool Blocked(List<StatusType> statuses){
        return statuses.Exists(status => self.GetStatus(status) > 0);
    }

    public void ExecuteCurrentIntent(){
        if(HasHeads) RegrowHeads();
        if(RolledFace == null) return;
        EffectContext context = CreateContext(RolledFace);
        foreach(EffectSO effect in RolledFace.GetEffects(ActionOf(RolledFace))){
            effect.Execute(context);
        }
        if(context.countdownDamage > 0 && !CountdownRunning){
            CountdownTurnsLeft = definition.countdownTurns;
            CountdownDamage = context.countdownDamage;
            StaggerTaken = 0;
            countdownJustStarted = true;
        }
    }

    // Start of the boss's turn: every head cut since its last turn grows back as two (up to maxHeads living),
    // unless Burn cauterized the wounds. Grown heads act from the next turn.
    private void RegrowHeads(){
        List<BattleEnemy> cut = heads.FindAll(head => head.IsDead && !settledCuts.Contains(head));
        if(cut.Count == 0) return;
        foreach(BattleEnemy head in cut) settledCuts.Add(head);
        if(RegrowthBlocked){
            Debug.Log(definition.bossName + "'s cut necks are cauterized.");
            return;
        }
        for(int i = 0; i < 2 * cut.Count; i++){
            if(!GrowHead()) break;
        }
    }

    // Brings a dead head back (the oldest first) or adds a new one, unless maxHeads are already alive.
    private bool GrowHead(){
        if(LivingHeads >= definition.maxHeads) return false;
        BattleEnemy dead = heads.Find(head => head.IsDead);
        if(dead != null){
            dead.entity.Revive();
            EnemyBrain brain = dead.brain as EnemyBrain;
            if(brain != null) brain.ClearIntent();
            settledCuts.Remove(dead);
            return true;
        }
        if(flow == null) return false;
        BattleEnemy head = flow.SpawnEnemy(definition.headEnemy, definition.headEnemy.enemyName + " " + (heads.Count + 1));
        if(head == null) return false;
        heads.Add(head);
        return true;
    }

    // The countdown ticks at the end of each enemy turn after the one that started it, and the hit lands on the last.
    public void OnTurnEnd(){
        // Heads: regenerate per living head, unless Burn or Bleed keeps the wounds open.
        if(HasHeads && !RegenBlocked){
            self.Heal(definition.regenPerHead * LivingHeads);
        }
        if(!CountdownRunning) return;
        if(countdownJustStarted){
            countdownJustStarted = false;
            return;
        }
        CountdownTurnsLeft--;
        if(CountdownTurnsLeft == 0){
            int dealt = CreateContext(null).Hit(CountdownDamage);
            Debug.Log(definition.bossName + "'s Stampede hit for " + dealt + ".");
        }
    }

    public int IncomingDamage{
        get{
            int damage = 0;
            if(RolledFace != null && ActionOf(RolledFace) == ActionType.Attack){
                damage += RolledFace.value + self.GetStatus(StatusType.Rage);
                // Coil: the bonus for the player's stone faces.
                EffectContext context = CreateContext(RolledFace);
                foreach(EffectSO effect in RolledFace.GetEffects(ActionType.Attack)){
                    PetrifiedFacesDamageEffectSO coil = effect as PetrifiedFacesDamageEffectSO;
                    if(coil != null) damage += coil.BaseDamage(context) - RolledFace.value;
                }
            }
            if(CountdownTurnsLeft == 1 && !countdownJustStarted){
                damage += CountdownDamage + self.GetStatus(StatusType.Rage);
            }
            return damage;
        }
    }

    // Hits during a countdown stagger the boss; enough of them cancel it.
    private void OnHitTaken(int hpLost){
        if(!CountdownRunning) return;
        StaggerTaken += hpLost;
        if(StaggerTaken >= definition.staggerDamage){
            Debug.Log(definition.bossName + " was staggered: the Stampede is cancelled.");
            CountdownTurnsLeft = 0;
            countdownJustStarted = false;
        }
    }

    private string DescribeFace(DieFace face){
        EffectContext context = CreateContext(face);
        List<string> parts = new List<string>();
        foreach(EffectSO effect in face.GetEffects(ActionOf(face))){
            string description = effect.Describe(context);
            if(description != "") parts.Add(description);
        }
        return string.Join(", ", parts);
    }

    // Boss faces have one action each.
    private static ActionType ActionOf(DieFace face){
        return face.definition.effects.Count > 0 ? face.definition.effects[0].actionType : ActionType.Attack;
    }

    private static bool StartsCountdown(DieFace face){
        return face.definition.effects.Exists(effect => effect.effectLogic is StartCountdownEffectSO);
    }

    private EffectContext CreateContext(DieFace face){
        return new EffectContext{
            source = self, target = opponent, value = face != null ? face.value : 0, die = die, face = face,
            targetDie = playerDie,
            targetPetrification = flow != null ? flow.petrification : null,
            targetCharge = flow != null ? flow.charge : null,
            targetFace = lockedTarget
        };
    }
}
