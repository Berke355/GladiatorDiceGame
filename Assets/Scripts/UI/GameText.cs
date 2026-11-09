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
        new Keyword{ word = "Poison", color = "#8BD45E", definition = "At the end of its owner's turn: lose HP equal to the stacks, then the stacks halve (rounded down). Ignores Block and Fortify." },
        new Keyword{ word = "Rage", color = "#FF5A5A", definition = "Every hit deals +1 damage per stack (each hit of a multi-hit). Lasts the whole battle." },
        new Keyword{ word = "Ritual", color = "#B388FF", definition = "Each time you apply Burn, Poison or Bleed, apply 1 more for each Ritual you have. Lasts the whole battle." },
        new Keyword{ word = "Encourage", color = "#FFD27F", definition = "Every Rage, Fortify and FireShield you gain gets +1 per stack (not Block). Lasts the whole battle." },
        new Keyword{ word = "Bleed", color = "#E0435A", definition = "At the end of its owner's turn: lose 1 HP, then lose 1 stack. Stacks are how many turns it lasts. Ignores Block and Fortify." },
        new Keyword{ word = "Vulnerable", color = "#E5A35C", definition = "Every hit its owner takes deals +1 damage. Loses 1 stack at the end of its owner's turn." },
        new Keyword{ word = "Weak", color = "#9AA5B1", definition = "Every hit its owner deals deals 1 less damage. Loses 1 stack at the end of its owner's turn." },
        new Keyword{ word = "Entangled", color = "#7FA88B", definition = "Its owner can't reroll. Loses 1 stack at the end of its owner's turn." },
        new Keyword{ word = "Consumable", color = "#E8C170", definition = "This face can be used once per battle, whichever action you choose. Afterwards every action on it is Change, usable once per turn." },
        new Keyword{ word = "Change", color = "#E8C170", definition = "Roll the die again and land on a face other than this one: your turn goes on and the enemy doesn't act. Not a reroll: it spends no rerolls." },
        new Keyword{ word = "Flip", color = "#E8C170", definition = "Roll the die again and land on the face opposite this one: your turn goes on. Not a reroll." },
        new Keyword{ word = "Tumble", color = "#E8C170", definition = "Roll the die again and land on a neighboring or named face: your turn goes on. Not a reroll." },
        new Keyword{ word = "petrify", color = "#A8B0B8", definition = "A petrified face is stone for the player's next 2 turns: when rolled, its only action is Defense: gain 2 block. Its other actions and its enchantment don't work." },
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

    // Wraps every keyword in its color (case-insensitive, whole words, original casing kept). Keywords with a definition
    // are also wrapped in a TMP link, so KeywordLinkTooltip can show the definition when the pointer is over the word.
    // One pass over the text, so a link added for one keyword is never matched again by another.
    // Only use on plain text; text that already contains rich text tags may get nested colors.
    public static string Colorize(string text){
        if(string.IsNullOrEmpty(text)) return text;
        string[] words = new string[Keywords.Length];
        for(int i = 0; i < Keywords.Length; i++) words[i] = Keywords[i].word + @"\w*";
        return Regex.Replace(text, @"\b(" + string.Join("|", words) + @")\b", match => {
            Keyword keyword = FindKeyword(match.Value);
            string colored = "<color=" + keyword.color + ">" + match.Value + "</color>";
            return keyword.definition != null ? "<link=\"" + keyword.word + "\">" + colored + "</link>" : colored;
        }, RegexOptions.IgnoreCase);
    }

    // The tooltip for one keyword (a TMP link id from Colorize); empty if it has no definition.
    public static string KeywordTooltip(string word){
        Keyword keyword = FindKeyword(word);
        if(keyword.definition == null) return "";
        return "<color=" + keyword.color + "><b>" + Capitalize(keyword.word) + ":</b></color> " + keyword.definition;
    }

    // The keyword a matched word starts with (e.g. "bleeding" -> Bleed).
    private static Keyword FindKeyword(string matched){
        foreach(Keyword keyword in Keywords){
            if(matched.StartsWith(keyword.word, System.StringComparison.OrdinalIgnoreCase)) return keyword;
        }
        return new Keyword{ word = matched, color = "#FFFFFF", definition = null };
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
        return "<b>" + face.faceName + "</b>  <size=80%>" + face.rarity + " · Value " + value + "</size>\n"
            + Colorize(face.description)
            + Glossary(face.description);
    }

    // Tooltip text for a face on the die, including its enchantment.
    public static string FaceTooltip(DieFace face){
        string description = face.FullDescription;
        return "<b>" + face.DisplayName + "</b>  <size=80%>" + face.definition.rarity + " · Value " + face.value + "</size>\n"
            + Colorize(description)
            + Glossary(description);
    }

    // Tooltip text for an equipped item: name, rarity, colored description, keywords.
    public static string EquipmentTooltip(EquipmentSO item){
        return "<b>" + item.equipmentName + "</b>  <size=80%>" + item.rarity + " · Equipment</size>\n"
            + Colorize(item.description)
            + Glossary(item.description);
    }

    private static string Capitalize(string word){
        return char.ToUpper(word[0]) + word.Substring(1);
    }
}
