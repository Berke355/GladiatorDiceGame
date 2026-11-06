using UnityEngine;
using System;
using System.Collections.Generic;

// The dice brain of a boss (see GDD "Boss Dice"): at the start of the player's turn it rolls its die,
// and the rolled face is its intent. Plain C#, created for one battle. Boss mechanics that outlive a turn
// (the Stampede countdown) live here, never in RunData.
public class BossBrain : IEnemyBrain, IDisposable
{
    public readonly BossSO definition;
    private readonly Entity self;
    private readonly Entity opponent;
    private readonly List<DieFace> playerDie;
    private readonly List<DieFace> die = new List<DieFace>();
    // The battle, for the player's stone faces and Charge (set by BattleFlow.Bind).
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
    // Heads (Hydra): current heads, and heads cut since the boss's last turn (they regrow at its next turn start).
    public int Heads { get; private set; }
    public int CutHeads { get; private set; }
    public bool HasHeads => definition.startingHeads > 0;
    // The countdown started this turn, so this turn's end doesn't count down yet.
    private bool countdownJustStarted;

    public BossBrain(BossSO boss, Entity self, Entity opponent, List<DieFace> playerDie){
        definition = boss;
        this.self = self;
        this.opponent = opponent;
        this.playerDie = playerDie;
        foreach(DieFaceSO face in boss.faces){
            die.Add(new DieFace(face));
        }
        Heads = boss.startingHeads;
        self.HitTaken += OnHitTaken;
    }

    public void Bind(BattleFlow flow){
        this.flow = flow;
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

    // The rolled face and its numbers, plus the countdown if one is running.
    public string DescribeCurrentIntent(){
        string text = RolledFace != null ? RolledFace.definition.faceName + ": " + DescribeFace(RolledFace) : "";
        if(CountdownRunning){
            int left = Mathf.Max(0, definition.staggerDamage - StaggerTaken);
            text += "\nStampede: " + (CountdownDamage + self.GetStatus(StatusType.Rage)) + " damage "
                + (CountdownTurnsLeft == 1 ? "at the end of this turn" : "in " + CountdownTurnsLeft + " turns")
                + " (stagger: " + left + " more damage)";
        }
        if(HasHeads){
            text += "\nHeads: " + Heads + (CutHeads > 0 ? " (" + CutHeads + " cut: " + (Blocked(definition.regrowthBlockedBy) ? "cauterized by Burn" : "will grow back as " + 2 * CutHeads) + ")" : "")
                + " | a hit of " + definition.cutDamage + "+ cuts a head | regenerates " + definition.regenPerHead * HeadsAfterRegrowth + (Blocked(definition.regenBlockedBy) ? " (blocked)" : "");
        }
        return text;
    }

    // Heads once cut heads regrow at the boss's next turn start (or are cauterized by Burn).
    public int HeadsAfterRegrowth => Blocked(definition.regrowthBlockedBy) ? Heads : Mathf.Min(definition.maxHeads, Heads + 2 * CutHeads);

    private bool Blocked(List<StatusType> statuses){
        return statuses.Exists(status => self.GetStatus(status) > 0);
    }

    public void ExecuteCurrentIntent(){
        // Heads: cut heads grow back as two at the start of the boss's turn, unless Burn cauterized them.
        if(HasHeads && CutHeads > 0){
            Heads = HeadsAfterRegrowth;
            CutHeads = 0;
        }
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

    // The countdown ticks at the end of each enemy turn after the one that started it, and the hit lands on the last.
    public void OnTurnEnd(){
        // Heads: regenerate per head, unless Burn or Bleed keeps the wounds open.
        if(HasHeads && !Blocked(definition.regenBlockedBy)){
            self.Heal(definition.regenPerHead * Heads);
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
                // Bite: one hit per head (heads regrow before it acts), each with Rage.
                int hits = 1;
                foreach(EffectSO effect in RolledFace.GetEffects(ActionType.Attack)){
                    if(effect is PerHeadHitEffectSO) hits = Mathf.Max(1, HeadsAfterRegrowth);
                }
                damage += hits * (RolledFace.value + self.GetStatus(StatusType.Rage));
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

    // A big hit cuts a head (never the last one). Hits during a countdown stagger the boss; enough of them cancel it.
    private void OnHitTaken(int hpLost){
        if(HasHeads && hpLost >= definition.cutDamage && Heads > 1){
            Heads--;
            CutHeads++;
            Debug.Log(definition.bossName + " lost a head (" + Heads + " left).");
        }
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
            targetFace = lockedTarget,
            heads = HasHeads ? HeadsAfterRegrowth : 0
        };
    }
}
