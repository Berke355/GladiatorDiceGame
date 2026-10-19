using UnityEngine;
using System;
using System.Collections.Generic;

// Runs the player's equipment when a trigger fires. Plain C#, created for one battle by BattleFlow.
// Hits are reported by Entity events; every other trigger is fired by BattleFlow's turn steps.
public class TriggerSystem
{
    private readonly Entity player;
    private readonly Entity enemy;
    private readonly List<DieFace> die;
    private readonly List<EquipmentSO> equipment;
    private readonly DieCharge charge;
    // The face rolled this turn (null outside the player's turn), passed to effects like a face's own effects.
    public DieFace currentFace;
    // Effects run by a trigger never fire triggers, so items can never loop (e.g. an extra hit firing itself).
    private bool firing;

    // How often each reaction fired this round and this battle, for maxPerRound / maxPerBattle.
    private readonly Dictionary<TriggeredEffect, int> firedThisRound = new Dictionary<TriggeredEffect, int>();
    private readonly Dictionary<TriggeredEffect, int> firedThisBattle = new Dictionary<TriggeredEffect, int>();

    // Raised after an item's effects ran, e.g. for the battle UI to show which item triggered.
    public event Action<EquipmentSO> ItemTriggered;

    private readonly Action<Entity, int> onHitDealt;
    private readonly Action<int> onHitTaken;

    public TriggerSystem(Entity player, Entity enemy, List<DieFace> die, List<EquipmentSO> equipment, DieCharge charge = null){
        this.player = player;
        this.charge = charge;
        this.enemy = enemy;
        this.die = die;
        this.equipment = equipment ?? new List<EquipmentSO>();

        onHitDealt = (target, amount) => Fire(TriggerType.HitDealt);
        onHitTaken = amount => Fire(TriggerType.HitTaken);
        player.HitDealt += onHitDealt;
        player.HitTaken += onHitTaken;
    }

    // Unsubscribes from the player's events; call when the battle ends.
    public void Dispose(){
        player.HitDealt -= onHitDealt;
        player.HitTaken -= onHitTaken;
    }

    // Called at the start of each player turn.
    public void StartRound(){
        firedThisRound.Clear();
    }

    // action is only checked for ActionChosen.
    public void Fire(TriggerType trigger, ActionType action = ActionType.Attack){
        if(firing || equipment.Count == 0){
            return;
        }
        firing = true;
        try{
            foreach(EquipmentSO item in equipment){
                foreach(TriggeredEffect reaction in item.triggers){
                    if(reaction.trigger != trigger || !ConditionHolds(reaction, action) || LimitReached(reaction)) continue;
                    Count(firedThisRound, reaction);
                    Count(firedThisBattle, reaction);

                    EffectContext context = new EffectContext{ source = player, target = enemy, value = reaction.value, die = die, face = currentFace, charge = charge };
                    foreach(EffectSO effect in reaction.effects){
                        effect.Execute(context);
                    }
                    Debug.Log(item.equipmentName + " triggered on " + trigger + ".");
                    ItemTriggered?.Invoke(item);
                }
            }
        }
        finally{
            firing = false;
        }
    }

    private bool LimitReached(TriggeredEffect reaction){
        return (reaction.maxPerRound > 0 && Fired(firedThisRound, reaction) >= reaction.maxPerRound)
            || (reaction.maxPerBattle > 0 && Fired(firedThisBattle, reaction) >= reaction.maxPerBattle);
    }

    private static int Fired(Dictionary<TriggeredEffect, int> counts, TriggeredEffect reaction){
        int count;
        return counts.TryGetValue(reaction, out count) ? count : 0;
    }

    private static void Count(Dictionary<TriggeredEffect, int> counts, TriggeredEffect reaction){
        counts[reaction] = Fired(counts, reaction) + 1;
    }

    private bool ConditionHolds(TriggeredEffect reaction, ActionType action){
        if(reaction.requireFaceTag && (currentFace == null || !currentFace.definition.tags.Contains(reaction.faceTag))){
            return false;
        }
        if(reaction.trigger == TriggerType.ActionChosen && reaction.requireAction && reaction.action != action){
            return false;
        }
        return true;
    }
}
