using System.Collections.Generic;

// Player-facing text about a coming fight (enemies with HP, intents and traits; a boss with its die), shared by the
// map's node tooltips and the Market's next battle line.
public static class EncounterInfo
{
    public static string Summary(EnemyGroup encounter, int battle){
        int rage = RunManager.Instance.encounters.ScalingFor(battle).startingRage;
        return "<b>" + encounter.DisplayName + "</b>  <size=80%>(" + encounter.tier + (rage > 0 ? ", starts with " + rage + " Rage" : "") + ")</size>";
    }

    public static string Details(EnemyGroup encounter, int battle){
        System.Text.StringBuilder details = new System.Text.StringBuilder();
        List<string> names = BattleManager.MemberNames(encounter);
        for(int i = 0; i < encounter.members.Count; i++){
            EnemySO member = encounter.members[i];
            if(i > 0) details.Append("\n\n");
            details.Append("<b>" + names[i] + "</b>  " + RunManager.Instance.encounters.ScaledHP(encounter, member, battle) + " HP");
            List<string> intents = new List<string>();
            foreach(EnemyIntent intent in member.intents) intents.Add(intent.intentName);
            details.Append("\nIntents: " + string.Join(", ", intents));
            foreach(EnemyTrait trait in member.traits){
                if(trait.trigger == EnemyTrigger.Passive && trait.effects.Count == 0 && member.secondWindIntents.Count == 0) continue;
                details.Append("\n" + trait.traitName + ": " + GameText.Colorize(trait.description));
            }
        }
        return details.ToString();
    }

    public static string BossDetails(BossSO boss){
        List<string> faces = new List<string>();
        foreach(DieFaceSO face in boss.faces) faces.Add(face.faceName);
        return "<b>" + boss.bossName + "</b>  " + boss.maxHP + " HP\nIts die: " + string.Join(", ", faces);
    }
}
