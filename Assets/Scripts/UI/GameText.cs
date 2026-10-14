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
        new Keyword{ word = "Burn", color = "#FF9A3C", definition = "At the end of its owner's turn: lose HP equal to the stacks, then lose 2 stacks. Ignores Block and Fortify." },
        new Keyword{ word = "Poison", color = "#8BD45E", definition = "At the end of its owner's turn: lose HP equal to the stacks. Never decays. Ignores Block and Fortify." },
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

    // Tooltip text for one die face: name, rarity, value, colored description and its keywords.
    public static string FaceTooltip(DieFaceSO face, int value){
        return "<b>" + face.faceName + "</b>  <size=80%>" + face.rarity + " · Value " + value + " (cap " + face.softCap + ")</size>\n"
            + Colorize(face.description)
            + Glossary(face.description);
    }

    private static string Capitalize(string word){
        return char.ToUpper(word[0]) + word.Substring(1);
    }
}
