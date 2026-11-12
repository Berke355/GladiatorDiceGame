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

    // ---- Card language: numbers on screen, words on hover (GDD "Visual Style").
    // Icons live in the TMP sprite asset Resources/Sprite Assets/Icons (Assets/Art/Icons packed by name).
    private static readonly string[] IconNames = {
        "Damage", "Block", "Heal", "Health", "Fortify", "Burn", "Poison", "Bleed", "Rage", "Ritual", "Encourage",
        "FireShield", "Vulnerable", "Weak", "Entangled", "Consumable", "Charge", "Reroll", "Gold", "Threat", "Change",
        "Magic", "Petrified", "Target"
    };
    // Statuses that show as an icon when they stand alone ("consume Bleed").
    private const string StatusWords = "Fortify|Burn|Poison|Bleed|Rage|Ritual|Encourage|FireShield|Vulnerable|Weak|Entangled";

    // The inline icon for a status, a keyword or an icon name; the word itself if there is no icon.
    public static string Icon(string name){
        int index = IconIndex(name);
        return index < 0 ? name : "<sprite=\"Icons\" name=\"" + IconNames[index] + "\">";
    }

    private static int IconIndex(string name){
        if(string.Equals(name, "damage", System.StringComparison.OrdinalIgnoreCase)) name = "Damage";
        else if(string.Equals(name, "block", System.StringComparison.OrdinalIgnoreCase)) name = "Block";
        else if(string.Equals(name, "heal", System.StringComparison.OrdinalIgnoreCase)) name = "Heal";
        else if(name.StartsWith("reroll", System.StringComparison.OrdinalIgnoreCase)) name = "Reroll";
        else if(name == "HP") name = "Health";
        return System.Array.IndexOf(IconNames, name);
    }

    // Each icon is first a single private-use character, so a later pattern never matches inside a sprite tag.
    private static string Mark(string name){
        int index = IconIndex(name);
        return index < 0 ? name : ((char)(0xE000 + index)).ToString();
    }

    // Turns an effect description into the card language: "5 damage, 4 Bleed" → [Damage]5, [Bleed]4;
    // "gain 2 Rage" → +[Rage]2; "2 x 3 damage" → 2×[Damage]3. Words with no icon stay words.
    public static string Iconize(string text){
        if(string.IsNullOrEmpty(text)) return text;
        string s = text;
        s = Regex.Replace(s, @"(\d+) x (\d+) damage", m => m.Groups[1].Value + "×" + Mark("Damage") + m.Groups[2].Value);
        s = Regex.Replace(s, @"(\d+) \+ rerolls (left|used) (damage|block)", m => Mark(m.Groups[3].Value) + m.Groups[1].Value + " + " + Mark("Reroll") + " " + m.Groups[2].Value);
        s = Regex.Replace(s, @"(\d+) x rerolls used damage", m => Mark("Damage") + m.Groups[1].Value + " × " + Mark("Reroll") + " used");
        s = Regex.Replace(s, @"(\d+) \+ (" + StatusWords + @") damage", m => Mark("Damage") + m.Groups[1].Value + " + " + Mark(m.Groups[2].Value));
        s = Regex.Replace(s, @"\bgain (\d+) (" + StatusWords + @"|rerolls?)\b", m => "+" + Mark(m.Groups[2].Value) + m.Groups[1].Value);
        s = Regex.Replace(s, @"\bheal (\d+)", m => Mark("Heal") + m.Groups[1].Value, RegexOptions.IgnoreCase);
        s = Regex.Replace(s, @"\blose (\d+) HP\b", m => "−" + Mark("Health") + m.Groups[1].Value);
        s = Regex.Replace(s, @"(\d+) (damage|block)\b", m => Mark(m.Groups[2].Value) + m.Groups[1].Value, RegexOptions.IgnoreCase);
        s = Regex.Replace(s, @"(\d+) (" + StatusWords + @"|Charge)\b", m => Mark(m.Groups[2].Value) + m.Groups[1].Value);
        s = Regex.Replace(s, @"\b(" + StatusWords + @") x(\d+)", m => Mark(m.Groups[1].Value) + "×" + m.Groups[2].Value);
        s = Regex.Replace(s, @"\b(" + StatusWords + @"|Consumable)\b", m => Mark(m.Groups[1].Value));
        StringBuilder result = new StringBuilder();
        foreach(char c in s){
            int index = c - 0xE000;
            if(index >= 0 && index < IconNames.Length) result.Append("<sprite=\"Icons\" name=\"" + IconNames[index] + "\">");
            else result.Append(c);
        }
        return result.ToString();
    }

    // What an enemy shows over its head: the first line of its intent without the intent's name, in the card
    // language ("Shield Wall: 2 block" → [Block]2). The full text stays in the tooltip.
    public static string IntentSummary(string intent){
        if(string.IsNullOrEmpty(intent)) return intent;
        string line = intent.Split('\n')[0];
        int colon = line.IndexOf(": ", System.StringComparison.Ordinal);
        if(colon > 0 && colon < 32) line = line.Substring(colon + 2);
        return Iconize(line);
    }

    // A face's action as the head of its line: the action's icon and its name in its color (Attack red, Defense blue,
    // Magic purple), so the three actions, a face's main parts, stand out.
    public static string ActionLabel(ActionType action){
        string icon = action == ActionType.Attack ? "Damage" : action == ActionType.Defense ? "Block" : "Magic";
        string color = action == ActionType.Attack ? "#FF7A6B" : action == ActionType.Defense ? "#8FC1EA" : "#C39BE0";
        return Icon(icon) + "<size=80%><font=\"Cinzel SDF\"><b><color=" + color + ">" + action.ToString().ToUpperInvariant() + "</color></b></font></size>";
    }

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
