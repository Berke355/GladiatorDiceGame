using UnityEngine;
using System;
using System.Collections.Generic;

// Runs a normal enemy's traits (see GDD "Enemy Traits"): the equipment trigger idea, from the enemy's side.
// Plain C#, created for one battle by BattleFlow. Hits come from Entity events; actions and rerolls from BattleFlow.
public class EnemyTraitSystem
{
    private readonly Entity enemy;
    private readonly Entity player;
    private readonly List<EnemyTrait> traits;
    private readonly Dictionary<EnemyTrait, int> firedThisBattle = new Dictionary<EnemyTrait, int>();
    // Effects run by a trait never fire traits (e.g. Thorns damage can't trigger another reaction).
    private bool firing;

    // Raised with the trait's index after its effects ran, for the battle UI.
    public event Action<int> TraitTriggered;

    private readonly Action<Entity, int> onPlayerHit;
    private readonly Action<int> onEnemyLostHP;

    public EnemyTraitSystem(Entity enemy, Entity player, List<EnemyTrait> traits){
        this.enemy = enemy;
        this.player = player;
        this.traits = traits ?? new List<EnemyTrait>();
        onPlayerHit = (target, amount) => { if(target == enemy) Fire(EnemyTrigger.HitByPlayer); };
        onEnemyLostHP = amount => Fire(EnemyTrigger.LostHPFromHit);
        player.HitDealt += onPlayerHit;
        enemy.HitTaken += onEnemyLostHP;
    }

    public void Dispose(){
        player.HitDealt -= onPlayerHit;
        enemy.HitTaken -= onEnemyLostHP;
    }

    // targeted: this enemy is the player's target.
    public void PlayerChoseAction(ActionType action, bool targeted = true){
        Fire(EnemyTrigger.PlayerChoseAction, action, targeted);
    }

    public void PlayerRerolled(){
        Fire(EnemyTrigger.PlayerRerolled);
    }

    private void Fire(EnemyTrigger trigger, ActionType action = ActionType.Attack, bool targeted = true){
        if(firing || traits.Count == 0 || enemy.IsDead) return;
        firing = true;
        try{
            for(int i = 0; i < traits.Count; i++){
                EnemyTrait trait = traits[i];
                if(trait.trigger != trigger) continue;
                if(trigger == EnemyTrigger.PlayerChoseAction && trait.requireAction && trait.action != action) continue;
                if(trait.requireBlock && enemy.currentBlock <= 0) continue;
                if(trait.requireTargeted && !targeted) continue;
                int fired;
                firedThisBattle.TryGetValue(trait, out fired);
                if(trait.maxPerBattle > 0 && fired >= trait.maxPerBattle) continue;
                firedThisBattle[trait] = fired + 1;

                EffectContext context = new EffectContext{ source = enemy, target = player, value = trait.value };
                foreach(EffectSO effect in trait.effects){
                    if(effect != null) effect.Execute(context);
                }
                Debug.Log(enemy.entityName + "'s " + trait.traitName + " triggered.");
                TraitTriggered?.Invoke(i);
            }
        }
        finally{
            firing = false;
        }
    }
}
