using UnityEngine;
using System.Collections.Generic;

// The sounds of a combatant (GDD "Sound"): a hit that costs HP, a hit that Block or Fortify stops, Block gained,
// healing and death. It only listens to the entity's events, like the floating numbers, so the rules never call it.
// Poison and Bleed ticks are silent on purpose: they come in a row at every turn's end and the numbers show them.
public class CombatSounds
{
    private readonly Entity entity;
    private readonly bool isPlayer;
    private int lastHP;
    private int lastBlock;

    private CombatSounds(Entity entity, bool isPlayer){
        this.entity = entity;
        this.isPlayer = isPlayer;
        lastHP = entity.currentHP;
        lastBlock = entity.currentBlock;
        entity.HitTaken += OnHitTaken;
        entity.HitAbsorbed += OnHitAbsorbed;
        entity.OnHealthChanged += OnHealthChanged;
    }

    // Watched entities, so one is never watched twice (an enemy card can be added again).
    private static readonly HashSet<Entity> watched = new HashSet<Entity>();

    public static void Watch(Entity entity, bool isPlayer){
        watched.RemoveWhere(e => e == null);
        if(entity == null || !watched.Add(entity)) return;
        new CombatSounds(entity, isPlayer);
    }

    private void OnHitTaken(int hpLost){
        AudioManager.Play(isPlayer ? Sfx.HitPlayer : Sfx.HitEnemy);
    }

    private void OnHitAbsorbed(int amount){
        AudioManager.Play(Sfx.Blocked);
    }

    private void OnHealthChanged(){
        if(entity.currentHP > lastHP && lastHP > 0) AudioManager.Play(Sfx.Heal);
        if(entity.currentHP <= 0 && lastHP > 0) AudioManager.Play(Sfx.Death);
        // Block lost to a hit sounds as Blocked; only a gain sounds here (the turn-start reset is silent).
        if(entity.currentBlock > lastBlock) AudioManager.Play(Sfx.GainBlock);
        lastHP = entity.currentHP;
        lastBlock = entity.currentBlock;
    }
}
