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

    public List<DieFace> Die => die;
    public DieFace RolledFace { get; private set; }

    // The running countdown (Stampede): enemy turns left until the hit, its damage, and HP lost from hits since it started.
    public int CountdownTurnsLeft { get; private set; }
    public int CountdownDamage { get; private set; }
    public int StaggerTaken { get; private set; }
    public bool CountdownRunning => CountdownTurnsLeft > 0;
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
        self.HitTaken += OnHitTaken;
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
        return text;
    }

    public void ExecuteCurrentIntent(){
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
        return new EffectContext{ source = self, target = opponent, value = face != null ? face.value : 0, die = die, face = face, targetDie = playerDie };
    }
}
