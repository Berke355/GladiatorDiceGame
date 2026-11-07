using System;
using System.Collections.Generic;

// When an enemy trait reacts (see GDD "Enemy Traits"). Assets store these as numbers: always add new values at the end.
public enum EnemyTrigger
{
    // Never fires: a rule the enemy's brain handles itself, listed for the UI (e.g. Second Wind).
    Passive,
    // Each hit the player lands on it, blocked or not (Thorns).
    HitByPlayer,
    // Each hit that costs it HP (Enrage, Curl Up).
    LostHPFromHit,
    // The player chose an action, before the face's effects run; can require the action (Challenge, Parry).
    PlayerChoseAction,
    // The player spent a reroll (Momentum).
    PlayerRerolled
}

// An always-on rule of a normal enemy that reacts to the player (inspired by Slay the Spire). Its effects run with the
// enemy as source and the player as target, with this trait's value. Shown next to the enemy, flashing when it fires.
[Serializable]
public class EnemyTrait
{
    public string traitName;
    public string description;
    public EnemyTrigger trigger;
    // PlayerChoseAction: only this action.
    public bool requireAction;
    public ActionType action;
    // Only while the enemy has block (Parry).
    public bool requireBlock;
    // PlayerChoseAction: only when this enemy is the player's target (Parry: you attack it, not another enemy).
    public bool requireTargeted;
    public List<EffectSO> effects = new List<EffectSO>();
    public int value = 1;
    // 0 = no limit (Curl Up fires once per battle).
    public int maxPerBattle;
}
