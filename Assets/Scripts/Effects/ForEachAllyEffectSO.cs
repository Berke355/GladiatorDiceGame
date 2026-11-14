using System.Collections.Generic;
using UnityEngine;

// A support effect for enemies: runs its inner effects with each living ally as the source (and the effect's owner
// too with includeSelf), so effects that work on their source (block, heal, gaining Rage) reach the whole group:
// Howl, Shield Wall, Feed the Beast. Alone, it is just the inner effects on the owner.
[CreateAssetMenu(fileName = "NewForEachAllyEffect", menuName = "Dice/Effects/For Each Ally Effect")]
public class ForEachAllyEffectSO : EffectSO
{
    public bool includeSelf = true;
    public List<EffectSO> effects = new List<EffectSO>();

    public override void Execute(EffectContext context){
        Entity original = context.source;
        foreach(Entity member in Members(context)){
            if(member == null || member.IsDead) continue;
            context.source = member;
            foreach(EffectSO effect in effects){
                if(effect != null) effect.Execute(context);
            }
        }
        context.source = original;
    }

    public override string Describe(EffectContext context){
        List<string> parts = new List<string>();
        foreach(EffectSO effect in effects){
            string description = effect != null ? effect.Describe(context) : "";
            if(description != "") parts.Add(description);
        }
        bool hasAllies = context.allies != null && context.allies.Count > 0;
        string who = includeSelf ? (hasAllies ? "it and every ally: " : "") : "every ally: ";
        return who + string.Join(", ", parts);
    }

    private List<Entity> Members(EffectContext context){
        List<Entity> members = new List<Entity>();
        if(includeSelf) members.Add(context.source);
        if(context.allies != null) members.AddRange(context.allies);
        return members;
    }
}
