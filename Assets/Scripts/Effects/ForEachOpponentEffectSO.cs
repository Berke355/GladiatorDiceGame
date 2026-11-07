using UnityEngine;
using System.Collections.Generic;

// An area effect: runs its inner effects once on every living opponent, e.g. "deal damage to every enemy"
// (see GDD "Area Effects"). Each enemy is a separate target, so every hit gets Rage and fires hit triggers and
// every status gets Ritual. Against a single enemy it is just the inner effects on the target.
[CreateAssetMenu(fileName = "NewForEachOpponentEffect", menuName = "Dice/Effects/For Each Opponent Effect")]
public class ForEachOpponentEffectSO : EffectSO
{
    public List<EffectSO> effects = new List<EffectSO>();

    public override void Execute(EffectContext context){
        Entity original = context.target;
        // A copy: an enemy that dies on the way is skipped, and nothing joins mid-effect.
        foreach(Entity opponent in new List<Entity>(context.AllOpponents)){
            if(opponent == null || opponent.IsDead) continue;
            context.target = opponent;
            foreach(EffectSO effect in effects){
                if(effect != null) effect.Execute(context);
            }
        }
        context.target = original;
    }

    // The inner effects as shown on the target (Rage and Ritual included), for every enemy.
    public override string Describe(EffectContext context){
        List<string> parts = new List<string>();
        foreach(EffectSO effect in effects){
            string description = effect != null ? effect.Describe(context) : "";
            if(description != "") parts.Add(description);
        }
        return string.Join(", ", parts) + " to every enemy";
    }
}
