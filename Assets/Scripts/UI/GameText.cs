using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;

// Shared player-facing text: keyword colors and the glossary shown in tooltips.
// One place for these rules keeps every screen consistent (see GDD "Status Effects").
public static class GameText
{
    private struct Keyword
    {
        public string word;
        public string color;
        public string definition;
    }

    private static readonly Keyword[] Keywords = {
        new Keyword{ word = "FireShield", color = "#FFC857", definition = "Attackers gain Burn equal to its stacks. Lasts until its owner's next turn." },
        new Keyword{ word = "Fortify", color = "#7FB3E6", definition = "Absorbs damage after Block. Never resets." },
        new Keyword{ word = "Burn", color = "#FF9A3C", definition = "Fuel: deals no damage on its own until it is ignited. Loses 2 stacks at the end of its owner's turn (not the turn it was applied)." },
        new Keyword{ word = "ignite", color = "#FF9A3C", definition = "Deal damage equal to the target's Burn. The Burn stays." },
        new Keyword{ word = "Poison", color = "#8BD45E", definition = "At the end of its owner's turn: lose HP equal to the stacks, then lose 1 stack. Ignores Block and Fortify." },
        new Keyword{ word = "Rage", color = "#FF5A5A", definition = "Every hit deals +1 damage per stack (each hit of a multi-hit). Lasts the whole battle." },
        new Keyword{ word = "Ritual", color = "#B388FF", definition = "Every status applied to the opponent gets +1 per stack. Lasts the whole battle." },
        new Keyword{ word = "Bleed", color = "#E0435A", definition = "At the end of its owner's turn: lose 1 HP, then lose 1 stack. Stacks are how many turns it lasts. Ignores Block and Fortify." },
        new Keyword{ word = "block", color = "#B8C4CC", definition = "Absorbs damage. Resets at the start of its owner's turn." },
        new Keyword{ word = "damage", color = "#FF7A6B", definition = null },
        new Keyword{ word = "heal", color = "#6EE7A0", definition = null },
    };

    public static string KeywordColor(string word){
        foreach(Keyword keyword in Keywords){
            if(string.Equals(keyword.word, word, System.StringComparison.OrdinalIgnoreCase)) return keyword.color;
        }
        return "#FFFFFF";
    }

    // Wraps every keyword in its color (case-insensitive, whole words, original casing kept).
    // Only use on plain text; text that already contains rich text tags may get nested colors.
    public static string Colorize(string text){
        if(string.IsNullOrEmpty(text)) return text;
        foreach(Keyword keyword in Keywords){
            text = Regex.Replace(text, @"\b(" + keyword.word + @"\w*)\b",
                match => "<color=" + keyword.color + ">" + match.Value + "</color>", RegexOptions.IgnoreCase);
        }
        return text;
    }

    // Definitions of every keyword that appears in the text, for tooltips.
    public static string Glossary(string text){
        if(string.IsNullOrEmpty(text)) return "";
        StringBuilder builder = new StringBuilder();
        HashSet<string> added = new HashSet<string>();
        foreach(Keyword keyword in Keywords){
            if(keyword.definition == null || added.Contains(keyword.word)) continue;
            if(Regex.IsMatch(text, @"\b" + keyword.word + @"\b", RegexOptions.IgnoreCase)){
                added.Add(keyword.word);
                builder.Append("\n<size=85%><color=" + keyword.color + "><b>" + Capitalize(keyword.word) + ":</b></color> " + keyword.definition + "</size>");
            }
        }
        return builder.ToString();
    }

    // Tooltip text for a face definition (e.g. a Market offer): name, rarity, value, colored description, keywords.
    public static string FaceTooltip(DieFaceSO face, int value){
        return "<b>" + face.faceName + "</b>  <size=80%>" + face.rarity + " · Value " + value + " (cap " + face.softCap + ")</size>\n"
            + Colorize(face.description)
            + Glossary(face.description);
    }

    // Tooltip text for a face on the die, including its attached modules.
    public static string FaceTooltip(DieFace face){
        string description = face.FullDescription;
        return "<b>" + face.DisplayName + "</b>  <size=80%>" + face.definition.rarity + " · Value " + face.value + " (cap " + face.definition.softCap + ")</size>\n"
            + Colorize(description)
            + Glossary(description);
    }

    private static string Capitalize(string word){
        return char.ToUpper(word[0]) + word.Substring(1);
    }
}
