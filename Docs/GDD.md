# Game Design Document

## High Concept

### Elevator Pitch

A turn-based gladiator game where you improve your dice instead of your character.

### Game Vision

The player fights through a series of gladiator battles using a customizable six-sided die as their primary weapon. Instead of leveling up a character, players replace and upgrade the faces of their die, creating unique builds and strategies through different face combinations, equipment, and status effects.

Every decision should make the player think:

> "Which die face should I replace to make my build stronger?"

## Design Pillars

### 1. Build Crafting Comes First

The core progression of the game is not leveling up a character, but continuously improving and reshaping a six-sided die. Every new face should encourage players to think about new synergies and long-term build strategies.

---

### 2. Meaningful Decisions Every Turn

Combat should never become a repetitive sequence of choosing the same action. Players should constantly evaluate their available actions, current build, enemy state, and possible outcomes before making a decision.

---

### 3. Reward Creative Synergies

The most satisfying moments should come from discovering powerful combinations between die faces, equipment, spells, and status effects. Players should feel rewarded for building clever synergies rather than simply increasing damage numbers.

---

### 4. High Risk, High Reward

Powerful effects should often come with meaningful drawbacks. Players are encouraged to take calculated risks in exchange for the possibility of creating exceptionally powerful builds.

---

### 5. Easy to Learn, Hard to Master

The basic rules should be understandable within minutes, while mastering build optimization, resource management, and combat decisions should provide long-term depth.

## Core Gameplay Loop

The game revolves around a simple but highly replayable gameplay loop that constantly encourages players to improve their die and experiment with new builds.

```text
Battle
    ↓
Earn Gold
    ↓
Visit Preparation Area (Market)
    ↓
Choose Whether to Spend Gold
    ↓
Buy Die Faces, Equipment, or Spells
    ↓
Modify Your Die
    ↓
Prepare Your Build
    ↓
Next Battle
```

### Battle

Players fight enemies in turn-based gladiator battles. During combat, every turn presents meaningful decisions through different available actions (such as attacking, defending, or casting spells). The outcome of these actions is determined by rolling the customized die.

### Earn Gold

Winning battles rewards the player with gold. Gold is the primary resource used to improve future runs.

### Preparation Area (Market)

After every battle, players enter a preparation phase.

This is not simply a shop, but a strategic decision space where players can improve their current build before the next fight.

The market offers only a limited number of randomly selected options each visit.

Players may:

- Buy new die faces.
- Buy equipment.
- Learn new spells.
- Skip the market entirely and save their gold for later.

Players are never forced to spend their resources.

### Modify the Die

Newly purchased die faces can replace existing faces on the six-sided die.

Choosing which face to replace is one of the most important strategic decisions in the game.

### Repeat

Players continue alternating between battles and preparation phases until they either defeat the final boss or lose the run.

## Dice System Philosophy

The die is the core of the player's progression. Instead of improving a traditional RPG character, players continuously reshape and optimize their six-sided die throughout a run.

> **You don't build a hero. You build a die.**

Each die face is more than a number. It represents a strategic choice that can interact with other faces, equipment, spells, and status effects to create powerful synergies.

The goal is not to collect the highest numbers, but to discover the strongest combinations.

### Design Principles

- Every die face should have a purpose.
- Synergies are more valuable than raw numbers.
- Powerful builds should emerge from smart decisions, not pure luck.
- Different runs should naturally encourage different builds.
- Players should constantly evaluate whether a new die face fits their current strategy.

The strongest moments in the game should come from discovering unexpected combinations and watching a carefully crafted build come together.

### Die Face Structure

Every die face is defined by a common set of properties.

- Name
- Rarity
- Price
- Action Type
- Value
- Primary Effect
- Secondary Effect (optional)
- Tags
- Description

### Die Layout

The die is a real cube laid out as a net, and where a face sits matters. This gives "You build a die" a layer a deck can't have (inspired by how Slice & Dice names the faces of a die's net).

- **The net is a sideways cross,** always shown in battle and in the Market:

```
        [Top]
[Left] [Middle] [Right] [Rightmost]
        [Bottom]
```

- **Named positions:** faces can refer to Top, Left, Middle, Right, Rightmost and Bottom.
- **Cube relations follow from the net:** opposite faces are Left ↔ Right, Middle ↔ Rightmost and Top ↔ Bottom, and every face has four neighbors. Faces can refer to the opposite face or a neighbor as well as to a named position.
- **Placement is free in the Market:** click a die face, then another, and they swap places, at no cost. A face keeps its upgrades and enchantment when it moves. A bought face takes the place of the face it replaces, and the starting face chosen at the run start takes Bottom. The decision is not a tax on placing a face but the arrangement puzzle itself: each face has one opposite and the net has one Rightmost, so two combos that want the same spot force a choice, and a new face can change the whole arrangement. Content that cares about positions gives the puzzle its depth.
- **In battle the layout is fixed;** changing it mid-battle is a Fortune ability (e.g. a face that swaps two faces for the battle).
- **Built so far (DevLog Day 69):** the net, the positions and opposites, the swap, and the net on screen. No face reads the layout yet; that comes with Fortune's stages 2 and 3.
- **Other builds barely notice the layout;** for Fortune it is vital, and future faces and mechanics will keep giving it more meaning.

### Die Enchantments

Every build should care where its faces sit, not only Fortune (play-test). Die enchantments are sold at the Enchanting Table and inscribed on the die itself, not on a face (inspired by items in Slice & Dice that change certain positions).

- **Bound to the position:** a die enchantment stays where it is when faces move or are replaced. That creates the puzzle: which face goes on the +2 spot, and which on the -1?
- **The minus side is the real decision:** faces that don't depend on their value go there (Turnabout used for its Flip, Lucky Buckler for its reroll, a drunk potion).
- **Value Is the Effect holds:** like Charge, it is part of a face's battle value. The die shows "Sword 2 +2", and effects and previews use 4. A battle value never goes below 0. Upgrade prices use the face's own value.
- **Medusa's gaze takes the highest battle value,** so the face on the bonus spot is her target.
- **At most 2 at a time;** a new one replaces one of them. They can't be removed otherwise. Few means each one is a heavy decision.
- **Prices:** the same as other offers (Common 10, Rare 25, Epic 45). The table offers 2 per Market, under the same rules as faces (Grand Market: Epic only), and they can be locked.

| Enchantment | Rarity | Effect |
|---|---|---|
| **Honed Edge** | Common | Rightmost +2, Left -1 |
| **Balance** | Common | Top +2, Bottom -2 |
| **Heavy Core** | Rare | Middle +3, Top and Bottom -1 |
| **Twin Fangs** | Rare | Left and Right +1, Middle -2 |
| **Kindred** | Epic | Every face that shares a tag with its opposite face gets +1 |
| **Weighted Die** | Epic | Rightmost comes up twice as often. It used to stop Left from coming up, which erased the die's weakest face (the user's play-test). |
| **Keystone** | Common | Middle +2, Right -1 |
| **Right Hand** | Common | Right +2, Top -1 |
| **Outer Ring** | Rare | Top and Bottom +1, Middle -1 |
| **Low Guard** | Rare | Bottom +3, Left and Right -1 |
| **Burnout** | Rare | Right +3, and the face on Right is Consumable (the user's idea) |
| **Center of Gravity** | Epic | Middle comes up twice as often |
| **Twin Weights** | Epic | Left and Right come up twice as often |
| **Twin Soul** | Epic | Top's value x2, Bottom's value 0 (the user's idea; the x2 needs a price, or stacking wins again) |

- **Why so many Epics:** the Grand Market offers 2 of them; with only Kindred and Weighted Die it always offered Weighted Die, which with Honed Edge made the same combo in every run (the user's play-test).

- **Simulation (DevLog Day 74, an enchantment free from the start; Opportunist / Rage / Burn, 350 runs):**

| Enchantment | Opportunist | Rage | Burn |
|---|---|---|---|
| None | 15% | 29% | 9% |
| Honed Edge | 21% | 35% | 13% |
| Balance +1 / -1 | 14% | 37% | 14% |
| Balance +2 / -2 | 17% | 33% | 8% |
| Heavy Core | 21% | 32% | 15% |
| Twin Fangs | 19% | 34% | 12% |
| Kindred | 33% | 55% | 19% |
| Weighted Die | 26% | 46% | 40% |

  - Common and Rare ones are a small gain, and the Epics are strong, as their rarity should be. Balance's two versions are the same within noise; +2 / -2 makes the placement a heavier decision.
  - **Gold is their real cost:** bots that bought them before upgrades did much worse (Rage 34% to 18%, Shield 25% to 13%). Now bots buy Commons and Rares only with 30 gold left over, and overall rates are about as before.
- **The Enchanting Table is a tab in the Market:** a button switches the offers between the Market (faces, items) and the table (face enchantments, die enchantments), with the die net always in view. The map will later make it a node of its own.

### Value Is the Effect

The number printed on a face is exactly what its effects use. There are no hidden multipliers: a face showing 4 never secretly deals 8. Players should be able to trust the big number on a face at a glance. Faces differ through which actions and effects they support, not through hidden scaling.

### Rerolls

Every battle gives the player a small pool of **rerolls** (2 for now), refilled at the start of each battle.

- **When:** after the die lands, before choosing an action, the player may spend a reroll to roll the die again. A reroll can be spent on any turn, and several can be spent on the same turn.
- **The new face must be used:** there is no going back to the face before the reroll. That is where the risk comes from: a reroll can land on something worse.
- **Only the player's die is rerolled:** the enemy's intent stays the same, so the player rerolls to answer a known threat (e.g. hunting for a Defense face when a big hit is coming).
- **Not a new turn:** Block doesn't reset and turn-start items don't fire. FaceRolled items fire again for the new face.
- **Why (Pillar 2, Meaningful Decisions Every Turn):** before rerolls, a turn was one obvious choice between the rolled face's one or two actions. Now every turn asks "keep this face, or spend a reroll and have none left when I need it?"
- **Fortune content** (tag `Fortune`, so the Market offers it to a player building around rerolls):

| Content | Kind | Rarity | Value | Effect |
|---|---|---|---|---|
| **Patient Blade** | Face | Common | 3 | Attack: deal my value + 2 per reroll you have left. Defense: gain block equal to my value. |
| **Lucky Buckler** | Face | Common | 2 | Attack: deal damage equal to my value. Defense: gain block equal to my value and 1 reroll. |
| **Gambler's Strike** | Face | Rare | 3 | Attack: deal my value; if you rerolled this turn, hit again for my value. |
| **Fate's Edge** | Face | Epic | 2 | Attack: deal my value × the rerolls you used this battle (at least my value). |
| **Gambit** | Face | Common | 2 | Attack: deal my value + 2 per reroll you used this battle. Defense: gain block equal to my value. |
| **Hedge** | Face | Common | 2 | Defense: gain block equal to my value + 1 per reroll you have left. Attack: deal damage equal to my value. |
| **Loaded Die** | Item | Epic | — | At the start of each battle: gain 1 reroll. |
| **Lucky Charm** | Enchantment | Rare | — | Magic: gain 1 reroll. |

- **Save or spend:** Patient Blade rewards keeping rerolls, while Gambler's Strike and Fate's Edge reward spending them, so the same die pulls both ways.
- **How Fortune grows during a fight:** Lucky Buckler's Defense fills the reroll pool, and Patient Blade hits harder for every reroll kept (the "Common that grows during a fight" rule). With only Patient Blade as a Common, the Fortune bot cleared 1–3% of runs; with Lucky Buckler and bots that defend with it and keep their rerolls, about 8–15%. Fortune dies most at the Minotaur (slow start, like Burn) and at Medusa (she petrifies the grown Patient Blade).
- **The Fortune pass (DevLog Day 61):** Fortune fell from 8–15% (Day 48) to 2–4%. The enemy traits hit it hardest (Momentum punishes rerolls, Entangle forbids them), and it gained the least from the new economy. Its damage came from one Common (Patient Blade, 6.4 damage per roll in Medium battles, about 2 damage per turn overall): Lucky Buckler defends for a reroll, and the faces that reward spending rerolls were Rare and Epic, which bots almost never found (0.2 and 0.1 per run at battle 8). Raising Patient Blade or Lucky Buckler by 1 only reached 5%. The fix is content, not numbers:
  - **Gambit** is the Common rung of spending, the additive little brother of Fate's Edge (Common adds, Epic multiplies, like Detonate and Exsanguinate).
  - **Hedge** is Patient Blade's defensive twin: saving rerolls also keeps you alive through slow starts (the Minotaur).
  - Both work alone (2 damage or 2 block at the start of a battle) and bring "save or spend" down to Common.
  - **The new faces alone weren't enough** (still 2%): Fortune got further (48% reached battle 8, was 37%) but still couldn't kill. 37% of its rolls only defend (Lucky Buckler for a reroll, Hedge for block), so it took little damage (1 HP per turn in Strong battles) but dealt only 2.7 per turn and fights lasted 15 turns. Its damage didn't scale: one point per reroll is small next to Rage, Ritual or stacking statuses.
  - **+2 per reroll** for both damage payoffs (Patient Blade: per reroll left; Gambit: per reroll used): Gambit alone 6%, Patient Blade alone 9%, both **14–15%**. The other builds stay the same within noise (Burn 12%, Poison 19%, Rage 12%, Bleed 12%) after two faces joined the pool.
- **The Fortune redesign (planned, DevLog Day 67).** The user's play-test: Fortune never lives up to its name. It only gains rerolls and saves them to raise damage and block, which makes it a weaker copy of Fortify + Rage, and its defensive faces (Lucky Buckler, Hedge) leave Sword as the only real attack.
  - **New identity:** Fortune manipulates its own luck, deciding which face comes up and when, using the die's layout (see "Die Layout"). Gaining rerolls stays, but only as a small part.
  - **Two sides, like Burn's fuel and ignition:**
    - **Engines** steer the next roll. Change (any other face), Flip (the opposite face), Tumble (a neighbor or a named position such as Rightmost), choose a face (Epic), effects that also Change, and gaining rerolls.
    - **Payoffs** reward the manipulation: "on the first turn", "if this is my second time this turn", "if I came from a Change or Flip", "if the face opposite me is X".
  - **The user's examples:**
    - An Epic engine: "Gain block equal to my value and choose a face of your die; Change: the chosen face comes up."
    - An Epic payoff: "Attack: if this is my second time this turn, …; Magic: Change."
  - **Several actions in a turn:** an effect that also Changes lets a turn hold more than one action. The limit is **3 actions per turn**; the game tracks how many times each face came up this turn.
  - **Built so far (DevLog Day 70):** roll steering (Change, Flip, Tumble, a chosen face), landing counts, the action limit and the payoff conditions. No face uses them yet; that is stage 3.
  - **Two kinds of Change:** the Change a used Consumable action becomes stays once per turn (it must not thin the die). Fortune's Change actions are their own face actions, held in check by rarity and the actions-per-turn limit. Change is strong, so its faces are Rare or Epic.
  - **The first set (built, DevLog Day 71):**

| Face | Side | Rarity | Value | Effect |
|---|---|---|---|---|
| **Turnabout** | Engine | Common | 2 | Attack: deal my value. Magic: Flip. |
| **Opening Gambit** | Payoff | Common | 3 | Attack: deal my value; on the first turn, hit again. Defense: gain block equal to my value. |
| **Ricochet** | Engine | Rare | 2 | Attack: deal my value, then Flip. |
| **Sidestep** | Engine | Rare | 2 | Defense: gain block equal to my value, then the Rightmost face comes up. |
| **Mirror Blade** | Payoff | Rare | 3 | Attack: deal my value; if I came from a Change, Flip or Tumble, hit again. |
| **Fate's Hand** | Engine | Epic | 2 | Defense: gain block equal to my value and choose a face of your die; it comes up. |
| **Encore** | Payoff | Epic | 3 | Attack: deal my value; if this is my second time this turn, hit 2 more times (3 times my value, each hit with Rage). Magic: Flip. |

    - **Example combo:** Encore opposite Ricochet. Encore's Magic Flips (action 1), Ricochet hits and Flips (action 2), Encore comes up a second time and hits big (action 3). It fits the limit of 3 actions exactly, and all of its power comes from where the faces sit.
    - **Sidestep on Rightmost lands on itself** and repeats until the 3-action limit (6 block at value 2). This is allowed: a small discovery the arrangement puzzle can reward. Watch it in play-tests.
    - **Simulation (DevLog Day 72, bots that arrange simply and play simple combos; a floor, since bots plan poorly):** the Fortune bot clears 14% with or without arranging, because it rarely finds the Rare and Epic pieces (Encore and Ricochet are 1–2% of its rolls). With the die built from the start (500 runs): Fortune 43%, only the Encore + Ricochet pair 42%, Burn 48%, Hammer 46%. So its ceiling already matches the other builds. Real play decides the rest.
    - **Encore's Magic is Flip, not Change:** with Change, the second landing is mostly luck; with Flip, it is the arrangement puzzle.
    - Change's power means its faces are Rare or Epic. Turnabout's plain Flip, with no effect in the same action, is the Common entry point.
  - **Existing faces:** Lucky Buckler stays as a small engine piece (gaining rerolls). Patient Blade, Gambit and Hedge stay for now; the ones the new set makes unnecessary are removed after it is played.
  - **Harder to play,** so Fortune will be a build unlocked through meta progression (Roadmap Phase D). Its pieces are also useful alone in other builds.
  - **Bots will plan combos poorly,** so Fortune's simulated rates are a floor and play-tests decide its balance.
  - **Stages:**
    1. The die layout: net, positions and opposites, shown in battle and the Market, with free swapping.
    2. Roll steering, per-turn landing counts and the actions-per-turn limit.
    3. Content: engines and payoffs.
    4. Bots, simulation and play-tests.
- **Steady Hand stays as it is:** it rerolls without spending the pool, so it is still its own thing.

### Face Keywords

**Rules on a whole face, which affect every action on it** (the user's idea). Charge was already one in practice.

| Keyword | Effect |
|---|---|
| **Consumable** | The face can be used once per battle; afterwards every action on it is Change |
| **Charge** | Every turn the face isn't the one you act with, it gains +1 value for the battle |

- **Planned, after a play-test:** Growing (+1 value this battle per use), Heavy (+2 value, can't be rerolled) and Brittle (+3 value, -1 per use), sold at the Enchanting Table, one per face. "?" events can offer them once the map exists. Consumable stays printed only at first, since adding it to a weak face could erase that face from the die.

### Consumable and Change

- **Consumable** (a Face Keyword, see below): the face can be used once per battle, whichever action is chosen. It used to be a keyword on one action, which let an enchantment use a high-value Consumable face without limit (the user's play-test: Throwing Knife with Whirlwind). Each copy of a face on the die has its own use.
- **Change** (a keyword on an action): roll the die again and land on **any face other than this one**. The turn continues (the enemy doesn't act), and the new face must be used. **Change can be used once per turn.**
- **Every action of a used Consumable face becomes Change for the rest of the battle.** The player never sits with a dead roll: a drunk Health Potion turns into a way to the rest of the die. This follows the rule that the player always has something to do.
- **Change is an action, not a reroll:**
  - It doesn't spend the reroll pool and doesn't count as a reroll for Fortune effects ("if you rerolled this turn", Patient Blade) or enemy traits (Essedarius's Momentum).
  - Entangled doesn't stop it: Entangled forbids rerolls, and blocking Change would bring the dead roll back.
  - Items that fire when the die lands fire again, as with Steady Hand.
  - Threats on the player's choice (Essedarius's Run Down) look at the action that ends the turn, not at Change.
- **Once per turn, a guaranteed other face (DevLog Day 65):** Change first rolled the whole die (a used potion could land on itself) and could be used up to 5 times a turn. The guarantee removes the frustrating repeat; the once-per-turn limit keeps Change from thinning the die, so future Consumable faces (even a Consumable build) don't become too strong. If a reroll then lands on a Change face again, that Change is spent for the turn, and the player passes.
- **Pass:** a button that ends the turn without acting. The rolled face counts as unused (a Hammer keeps its Charge and gains 1 as usual), no items, traits or threats fire for an action, and threats on the player's choice don't trigger (passing gives up the turn). It also lets a player keep an unused potion when HP is full.
- **Die thinning is accepted as a strategy for now:** once used, a Consumable face works like a face removed from the die for the rest of the battle (like Slay the Spire's Exhaust). A die full of Consumables is a build path: use them, then play a die made only of build faces. This competes with Fortune, so Consumable content stays small at first (only Health Potion). If it proves too strong, the fix is a limit on Change per battle or making it spend a reroll.
- **Simulation (DevLog Day 58, 6 focused bots, 800 runs):** the old potion (heal 3 every time) cleared Burn 15%, Poison 19%, Shield 17%, Rage 8%, Bleed 6%, Hammer 4%. Consumable 4 with Change: 19%, 25%, 16%, 10%, 8%, 4%. Consumable 3 with Change: 16%, 16%, 19%, 6%, 7%, 3%, about the same as the old potion. So Change's die thinning costs little; the extra point of healing is most of the gain. The value stays 4 (one strong drink), and the difficulty and gold pass will balance the total.

## Rarity & Economy

### Goals

- **Powerful faces are rare.** Faces whose effect grows fast with their value are hard to find, expensive to buy and expensive to upgrade.
- **Rare does not mean luck-based.** Most runs should be able to find the key Epic face of their build.
- **Gold is a strategic decision.** Players choose between saving gold for a powerful late-game face and spending it now to keep up the tempo.

### Rarity Tiers

A face's rarity is defined by **how strongly its value scales its effect**, not by its raw power. When designing a new face, its formula tells you its rarity.

| Rarity | Scaling | +1 Value gives | Current faces |
|---|---|---|---|
| **Common** | Linear: effect = value | +1 to one effect | Sword, Shield, Ember |
| **Rare** | Conditional or multi-effect: value plus something accumulated, or several effects at once | +1 on top of an accumulated or conditional bonus, or +1 to several effects | Bite, Detonate, Flame Ward |
| **Epic** | Multiplicative: effect = value × something | + "something" (can be large) | Fuel, Cauterize |

> Rule of thumb: if the effect is multiplied by the value, the face is Epic.

### Run Structure

- A run is about **10–11 battles**.
- A **boss battle** happens every 3–5 battles. The **final boss** ends the run.
- Example layout: battles 1–3 → **boss (4)** → battles 5–7 → **boss (8)** → battles 9–10 → **final boss (11)**.
- After every battle except the final boss, the player visits the Market. After a boss, it is a **Grand Market** instead.
- **Player HP:** 50 max. HP carries over between battles.
- **Healing after bosses:** winning a boss battle restores 30% of max HP. Without it, runs would end from slow HP loss instead of from how good the build is. It was 50%, but in play-tests that made boss fights feel safe ("my HP will come back anyway"), so HP stays a resource to manage through the boss.
- **Gold after bosses:** winning a boss battle pays a 30 gold bonus on top of the normal reward, so the player can always afford one Epic at the Grand Market: a boss victory alone pays 50 gold and an Epic costs 45.

### Run Start

Every run starts from the same five neutral faces plus one face the player picks.

| Face | Count | Rarity | Value | Actions |
|---|---|---|---|---|
| **Sword** | 2 | Common | 2 | Attack: deal damage equal to my value and apply 2 Bleed. Defense: gain block equal to my value. |
| **Shield** | 2 | Common | 2 | Attack: deal damage equal to my value. Defense: gain Fortify equal to my value. |
| **Health Potion** | 1 | Common | 4 | Magic (Consumable): heal equal to my value. After it is used, the action becomes Change. No Attack or Defense. |
| **Starting face** | 1 | Chosen | — | Picked by the player at the start of the run |

- **Sword and Shield mirror each other:** Sword is stronger on offense (its attack also bleeds) and weaker on defense (temporary Block). Shield is stronger on defense (permanent Fortify) and weaker on offense. If one of them proves too strong, it will be rebalanced.
- **Sword's Bleed is a fixed 2**, not its value, so upgrading Sword only raises its damage. With +1 value improving a single effect, Sword stays Common. It was 1, but Bleed applied on your turn ticks at the end of the enemy's turn, so 1 Bleed was gone before your next turn and "if the enemy is bleeding" faces never saw it.
- **Health Potion** does one strong thing once per battle (see "Consumable and Change"). It was a repeatable heal of 3; as a Consumable it heals 4, and once drunk it becomes a Change face, so rolling it later is never a dead roll.

**Choosing the starting face:**

- At the start of a run, the player is offered **3 random faces** and picks one for the sixth slot of the die.
- Rarity weights: **Common 85%, Rare 13%, Epic 2%**.
- The three offers come from **different archetypes** (different tags), so the choice is a real decision about the build's first direction.
- Faces already on the starting die (Sword, Shield, Health Potion) are never offered.
- Every run starts different from the first moment, and the game's core question ("which face do I want?") is asked right away.

**Starting gold and the starting reroll:**

- **Every run starts with 20 gold** (one victory's worth).
- **The run start screen has a Reroll button** with the Market's rising price (5, 10, 15... gold). It replaces all three offers and avoids showing the same faces again when the pool allows.
- **Why gold instead of free rerolls:** one of three random faces decided too much of a run, which limited the player's freedom (play-test). A free reroll would be a decision-free gift that everyone always uses; gold spent here is missing in the first Market, so "a better starting face, or an upgrade in the first Market?" is a real Save vs. Tempo decision.
- Unspent gold stays for the run.
- **Simulation (DevLog Day 57):** most of the gain comes from the gold itself (an early upgrade in the first Market); the reroll adds a little on top (Burn: 7% without either, 10% with the gold, 13% with gold and reroll). Bot clear rates rose from 0–14% to 0–27% (Poison 9% → 19%, Ritual + Poison 14% → 27%), above the Phase A target. The Healer will soak up some of the gold, and the difficulty and gold pass will bring the numbers back.

**Future:**

- **Die reroll** (in battle, not the Market reroll below): once per roll, the player may reroll the die. A bad roll can be escaped once, and if the reroll is also bad, that's the game's share of luck.

### Market

Each visit offers a limited, random selection. The player is never forced to spend.

- **Normal Market:** 3 face offers and the Enchanting Table's 3 enchantments, with rarity weights Common 70%, Rare 25%, Epic 5%. Epic faces can appear, but rarely.
- **Grand Market (after a boss):** 3 face offers and 3 enchantments, **all Epic**, plus 2 equipment items. The Grand Market is the station for Epic faces. Because Grand Markets come at known points (after bosses), saving gold for one is a plannable strategy, not a gamble.
- A face is offered at most once per set of offers.
- **Build-aware offers:** offers are weighted toward the tags already on the player's die (around 60%, not 100%). A Burn die is more likely to be offered Burn faces such as Fuel, while some offers still come from outside the build so players keep discovering new faces.
  - **Known problem (play-test, DevLog Day 67):** the starting die's own tags (Sword: Physical + Bleed, Shield: Block) count too, so the matching pool is huge and a build's share is small. With one Patient Blade, 21% of offers were Fortune; with one Ember, only 11% were Burn.
  - **Tried: weight offers by the build's tags** (the die's tag counts minus the starting die's; `EconomyConfigSO.weightByBuildTags`, **off** for now). It lifts those shares to 41% and 47%, but builds then come together much faster, unevenly: Burn 9% → 43% (38% even at a 35% match chance), Poison 17% → 32%, Hammer 10% → 20%, while Rage 20% → 13% and Fortune 10% → 8%. Burn's faces are very strong once assembled; it only looked balanced because they were hard to find.
  - **Decision: wait until content is plentiful.** With few builds and faces today, this rule raises the chance of seeing your build's faces more than it should. It will be tuned as content grows, following "Every Run Is Its Own Build".

Market services:

- **Buy a face:** the new face replaces one face of the die, chosen by the player.
- **Upgrade a face:** raise one die face's value by 1. There is no limit: anyone who can pay can keep upgrading. The Normal Market upgrades Common and Rare faces; **the Grand Market upgrades every face**, and Epic faces can only be upgraded there.
- **Reroll the offers:** pay gold for a new set of offers, faces, enchantments and items alike (sold offers are replaced too). Locked offers stay. The new set avoids the faces it replaced when the pool allows.
- **Lock an offer** (2 gold, 5 in the Grand Market): a locked face, enchantment or item survives rerolls and waits in the next Market that sells that kind (items only in a Grand Market), so the player can save up for it. Unlocking is free, without a refund; buying it releases the lock. The price was 5 and 15, but locking is how a player saves up for an offer, so a high price worked against saving, above all for Epics (play-test).
- **The next battle is shown** under the die slots ("Next battle (5): Tiro Pair (Weak)", or the boss), with each enemy's HP, intents and traits on hover. The battle is picked when the Market opens. Until the map exists, this is how the player sees what is coming and plans purchases.
- **Hover help:** every die face and item shows its full description on hover, and every highlighted keyword (Ritual, Bleed, Poison...) shows its definition when the pointer is over the word.
- **Enchant a face:** inscribe an enchantment into the empty Magic action of one die face (see Enchanting Table).
- **Healer:** heal **10 HP for 20 gold, once per visit**, in every Market (Normal and Grand). Since boss healing dropped to 30%, HP is a real resource (play-test); buying it back competes with upgrades and rerolls, so it adds a decision instead of removing the pressure. The map's rest nodes will take over or join this later (Roadmap Phase B). The price was 40 after the first simulation, but in play-tests 40 felt far too expensive; 20 is measured together with the steeper early battles.
  - **Tuned by simulation (DevLog Day 59, 6 focused bots that heal below half HP, 800 runs):**

| Healer | Burn | Poison | Shield | Rage | Bleed | Hammer |
|---|---|---|---|---|---|---|
| None | 17% | 23% | 21% | 11% | 11% | 4% |
| 10 HP for 15, then 25 gold (2 per visit, the first plan) | 35% | 62% | 19% | 52% | 48% | 26% |
| 10 HP for 15 gold, 1 per visit | 37% | 62% | 23% | 50% | 49% | 29% |
| 6 HP for 20 gold, 1 per visit | 25% | 36% | 19% | 28% | 27% | 11% |
| 5 HP for 20 gold, 1 per visit | 24% | 35% | 16% | 24% | 27% | 11% |
| **10 HP for 40 gold, 1 per visit** | **20%** | **27%** | **18%** | **13%** | **13%** | **5%** |

  - HP is the tightest limit in the game: cheap healing over 9 Markets undid the pressure of the 30% boss heal, most of all for long attrition builds (Rage, Bleed). The heal amount mattered more than the per-visit limit. At 40 gold (two victories) a heal competes with an upgrade or two rerolls; players will use it better than bots (e.g. right before a boss), so it stays on the expensive side.

**Market reroll:**

- **Why:** as the face pool grows, each archetype's faces appear less often. Rerolls let players hunt for their build's faces, and they give gold a third use besides buying and upgrading: "spend on a reroll, or keep the gold for an upgrade?"
- **Paid, not limited:** a limited number of free rerolls would only matter for a few decisions per run and would be gone once spent. A price makes the decision come back every visit.
- **The price rises within a visit and resets at the next Market**, so plenty of gold can't buy endless fishing for an Epic:

| Market | 1st reroll | 2nd | 3rd |
|---|---|---|---|
| Normal | 5 gold | 10 | 15 |
| Grand | 15 gold | 30 | 45 |

- **The Grand Market reroll is much more expensive:** it only offers Epics, so hunting for one specific Epic should cost most of a battle's reward per try.
- Simulated bots that rerolled whenever nothing fit their build did worse, because the gold went to rerolls instead of upgrades. Bots that kept at least 40 gold after a reroll did slightly better than bots that never rerolled. The tension is real.

### Enchanting Table

You don't only build the die, you build its faces. Every Market has an **Enchanting Table**: pay gold and inscribe an **enchantment** onto a die face. An enchantment is a **Magic action** that the face didn't have.

- **Example:** Venom Coating (Common), "Magic: apply Poison equal to the face's value". Inscribed onto a Shield (Attack: damage, Defense: Fortify, Magic: empty), the Shield gains "Magic: apply Poison equal to my value".
- **Magic only:** enchantments always go into the Magic action, so the rule is a single question: is the face's Magic empty? Faces that already have a Magic action (Health Potion, Fury, Plague, Anvil...) are already magical and can't be enchanted. Most weapon faces (Sword, Ember, Venom, Twin Axes, the Hammers...) can, which fits the theme of enchanting a weapon. One enchantment per face keeps faces readable.
- **Uses the face's value:** an enchantment's effects use the value of the face it is on (its battle value, so Hammer Charge counts), so Value Is the Effect still holds and **upgrading the face upgrades its enchantment too**.
- **Permanent:** enchantments can't be removed for now. Buying a face into a slot replaces the old face together with its enchantment.
- **A niche combo tool:** most players won't enchant in most runs. An enchantment shines on a face with a very high value, letting it spend that value on another action (e.g. Poison on a Hammer's Magic, or Dark Rite on a charged Hammer, which buffs the rest of the battle in one roll). It is judged with combo tests, not by average clear rates.
- **An option, not an extra effect:** one roll still means one action. An enchantment adds a new choice to a face, so on a low-value face it changes little, and on a high-value face it changes a lot.
- **Same logic as face offers:** the table shows **3 enchantments** in every Market. The Normal Market uses the face rarity weights (Common 70%, Rare 25%, Epic 5%); the **Grand Market shows only Epic enchantments**. Build-aware tags and rerolls work exactly as for faces, but enchantments are cheaper (Common 5, Rare 10, Epic 20), so they stay an easy-to-reach option (the user's play-tests: they were rarely worth buying).
- **Rarity** follows the same scaling rule as faces (linear Common, conditional or multi-effect Rare, multiplicative Epic). Permanent self-buffs scaled by the value (Rage, Ritual) are Epic: on a high-value face they multiply the rest of the battle.
- **Technical:** the enchantment is stored on the `DieFace` instance, not on the `DieFaceSO` asset (the Definition / Instance split from Day 15), so only that copy of the face changes.

| Enchantment | Rarity | Magic action | Build |
|---|---|---|---|
| **Venom Coating** | Common | Apply Poison equal to my value | Poison |
| **Kindling** | Common | Apply Burn equal to my value | Burn |
| **Rend** | Common | Apply Bleed equal to my value | Bleed |
| **Ward** | Common | Gain block equal to my value | Shield / general (was Iron Skin, a Defense module) |
| **Ignition** | Rare | Ignite the enemy's Burn | Burn (was an Attack module) |
| **War Cry** | Epic | Gain Rage equal to my value | Berserker (was Rare: permanent Rage on a high-value face is as strong as Dark Rite) |
| **Dark Rite** | Epic | Gain Ritual equal to my value | Ritualist: on a charged Hammer, one roll boosts every later status |
| **Whirlwind** | Epic | Hit twice for my value | Rage / Hammer: Rage counts on both hits |
| **Resonance** | Epic | Add my value as Charge to every other face with the Charge rule, past the +5 cap; this face keeps its Charge | Hammer: fills every Hammer at once, scales with how many are on the die |
| **Sweep** | Epic | Deal my value to every enemy | Any high-value face: a group answer (see "Area Effects") |

### Prices

Prices are placeholders, to be tuned through play-testing.

- **Gold income:** 20 gold per victory, plus a 30 gold bonus for boss victories.
- **Buy price by rarity:** Common 10, Rare 25, Epic 45. A boss victory always pays for one Epic, but it leaves little for upgrades, so taking it is still a real choice. Common and Rare faces are cheap enough to keep the build growing.
- **Upgrade price:** rarity multiplier × current value, so each upgrade costs more than the last.

| Rarity | Multiplier | 2 → 3 | 3 → 4 |
|---|---|---|---|
| Common | ×3 | 6 | 9 |
| Rare | ×5 | 10 | 15 |
| Epic | ×20 | 40 | 60 |

- **No cap:** upgrades are unlimited and never jump in price, because game-breaking runs are part of the power fantasy. The only brake is the price itself, which grows with the value (an Ember at 8 costs 24 gold to raise to 9). An earlier soft cap (2×, 4×, 8×... per level past a cap) was removed: it only mattered to players with a lot of gold, and removing it moved the simulated builds by 0–8 points.

### Save vs. Tempo

The decision only works if both paths are viable.

- **Spending now** makes the die stronger in the next battles, so the player wins faster and loses less HP.
- **Saving** keeps gold for an Epic face and its upgrades at the next Grand Market. Since **HP carries over between battles**, fighting with a weaker die costs HP that the player has to survive without.
- No interest mechanic for now. If play-testing shows that saving is never worth it, rewarding saved gold (e.g. interest) is the first thing to revisit.

### Open Questions


## Equipment

Passive items that react to what happens in battle. Equipment makes the die stronger; it never replaces it.

### Design Rules

- **Equipment talks to the die.** Most triggers are die events ("when you roll a Poison face", "when you choose Defense"), so equipment rewards building the die instead of building a hero ("You don't build a hero. You build a die.").
- **No limit: items pile up during a run, like Slay the Spire's relics** (the user's play-test: three slots were too few). Many small items talking to each other are a big part of what makes runs different.
- **Small, fixed numbers.** Equipment builds synergies, not raw power. Value Is the Effect still holds: the printed number is what happens.
- **No upgrades.** Upgrading stays the die's job.

### Triggers

| Trigger | When | Condition |
|---|---|---|
| **BattleStart** | once, before the first turn | — |
| **TurnStart** | at the start of your turn (after your Block resets) | — |
| **FaceRolled** | when your die lands | optional: the face has a tag |
| **ActionChosen** | when you choose an action, before the face's effects | optional: Attack, Defense or Magic |
| **HitDealt** | each time you hit the enemy (each hit of a multi-hit) | — |
| **HitTaken** | each time a hit makes you lose HP (not Poison or Bleed ticks) | — |
| **TurnEnd** | at the end of your turn, before your statuses tick | — |
| **BattleWon** | when the enemy dies, before your HP is saved to the run | — |

- **Effects are the existing `EffectSO` assets**, run with the item's value (source: you, target: the enemy, face: the rolled face when there is one). Rage, Ritual and FireShield apply to them like to any face.
- **Conditions on the battle state** ("if the enemy is bleeding") use `ConditionalEffectSO`, so triggers only need the face-tag and action conditions.
- **Limits:** a reaction can be capped per round (your turn plus the enemy's turn after it) and per battle, e.g. "the first 2 times each battle".
- **No chains:** an effect run by a trigger does not fire triggers. Barbed Hook's extra hit does not fire Smoldering Gauntlet or Barbed Hook again, so items can never loop.
- **One place for the turn steps:** the battle states and the balance simulator call the same turn steps, so every trigger fires at the same point in both.
- **Future triggers:** "when you apply a status", "when the enemy dies to a tick", die manipulation ("reroll once per battle").

### Getting Equipment

- **Boss reward:** after a boss, choose 1 of 3 items for free.
- **Every Market sells items, priced by rarity (Common 10, Rare 15, Epic 25):** 1 in a Normal Market, 2 in the Grand Market. A locked item waits for the very next Market (the user's play-test: it used to wait 4 battles for the next Grand Market). Slay the Spire's shops sell relics the same way.
- With no slot limit, an item's only competition is what else the gold could buy.

### First Set

| Item | Build | Trigger | Effect |
|---|---|---|---|
| **Plague Mask** | Poison | FaceRolled (Poison face), first per battle | Apply 1 Poison |
| **Smoldering Gauntlet** | Burn | HitDealt | Apply 1 Burn |
| **Barbed Hook** | Bleed | HitDealt, first 4 per battle | If the enemy is bleeding, hit again for 1 |
| **Tower Helm** | Shield | ActionChosen (Defense), up to 3 per battle | Gain 1 Fortify |
| **Berserker Torc** | Rage | HitTaken, first per battle | Gain 1 Rage |
| **Bone Fetish** | Ritual | BattleStart | Gain 1 Ritual |
| **Laurel Wreath** | General | BattleWon | Heal 2 |
| **Iron Bracers** | General | BattleStart | Gain 3 Fortify |
| **Censer** | Poison | TurnEnd, first 2 per battle | Apply 1 Poison to every enemy (see "Area Effects") |

### Second Set

Each item changes how you play or what you decide, not just a number. These also fill empty spots: Hammer and Consumable had no item, and a patchwork die had nothing of its own.

| Item | For | Trigger | Effect |
|---|---|---|---|
| **Whetstone** | Hammer | BattleStart | Charge faces start with 1 Charge |
| **Alchemist's Belt** | Consumable | Always | Consumable faces work 2 times per battle before they become Change |
| **Lucky Charm** | Fortune | FaceRolled (a steer brought it), first per round | Gain 2 block |
| **Spiked Greaves** | General | HitTaken, up to 3 per battle | Deal 1 damage to the attacker |
| **Ambusher's Knife** | General | HitDealt, first per battle | Hit again for 3 |
| **Blood Pact** | Rage | BattleStart | Lose 3 HP (never your last) and gain 1 Rage |
| **Motley Cloak** | Patchwork die | TurnStart, first 3 per battle | Gain 1 block for every 3 different faces on your die |
| **Balance Scales** | Patchwork die | ActionChosen (not your last action) | Gain 1 block |
| **Compass** | Die layout | FaceRolled (Rightmost), first per round | Gain 2 block |
| **Merchant's Purse** | Economy | Always | Every victory pays 5 more gold |

- **Motley Cloak counts faces, not tags:** tags aren't shown to the player, while duplicate faces can be seen at a glance.
- **Compass makes the net matter outside Fortune,** and **Merchant's Purse is a timing decision:** it pays for itself only when bought early.

### Third Set

These fill the last empty spots: Burn and Ritual had almost nothing, and enchantments and long fights had nothing. Gold is the limit on how many items a run gets (the user's play-test), which makes rarity prices a real dial.

| Item | Rarity | For | Trigger | Effect |
|---|---|---|---|---|
| **Tinder Box** | Common | Burn | BattleStart | Apply 2 Burn |
| **Witch's Knot** | Common | Statuses, Ritual | BattleStart | Apply 1 Poison and 1 Bleed |
| **Rabbit's Foot** | Common | Fortune | Rerolled, first per round | Gain 1 block |
| **Drill Banner** | Common | Patchwork die | BattleStart | Your lowest-value face gains +1 value this battle |
| **Second Blade** | Rare | Rage, attacks | HitDealt, first per round, 2 per battle | Hit again for 1 |
| **Scribe's Quill** | Rare | Face enchantments | Always | Faces with an enchantment have +1 value |
| **Hourglass** | Rare | Long fights | Start of turn 5 | Gain 1 Rage |
| **Smelling Salts** | Rare | Survival | HitTaken below half HP, first per battle | Heal 3 |
| **Challenger's Mark** | Epic | Risk | BattleStart | You gain 2 Rage and 1 Ritual, every enemy gains 1 Rage |
| **Haggler's Scale** | Epic | Economy | Always | The first reroll in each Market is free |

- **Merchant's Purse is Epic too.** With gold as the limit on items, it pushes hardest against that limit.

### Balance Notes

- **Items that fire on every hit or every turn need small numbers or a cap.** In the difficulty pass (DevLog Day 60) the two uncapped items carried their builds: Smoldering Gauntlet (3 Burn per hit, on 92% of Burn bots at battle 8) lifted Burn to 22% (12% without it); it now applies 1 Burn per hit (16%). Censer is capped at 3 per battle (see "Area Effects").
- **Bridges through existing rules:** Rage adds to Barbed Hook's extra hit (Bleed + Rage), Ritual adds to Plague Mask's Poison (Ritual + Poison), and Smoldering Gauntlet fires on every hit of a multi-hit (Twin Axes + Burn).
- **Plague Mask scales with the die:** the more Poison faces on the die, the more often it fires.
- **Berserker Torc is the risky one** (Pillar 4): it grows from damage taken, and Rage is permanent. Its numbers need the closest watch.
- **Laurel Wreath** is worth the most in long runs, since HP carries over between battles.
- **Equipment is part of the power curve.** Even the smallest numbers are a lot of power: HP carries over and runs are lost narrowly in the last battles, so a few HP per battle compound (healing 1 per battle lifts the Bleed bot from 22% to 37% of runs cleared). The first set first made the run far too easy (Rage 93%, Bleed 84%), and the items were uneven (alone, +0 to +67 points for their build).
- **Tuning rule:** items pile up, so each one alone should add only a few points (it used to be +15 to +20 for three slots). Items that fire often get limits ("first 2 times each battle") instead of smaller numbers, since numbers can't go below 1. The Medium and Strong tiers were then made stronger (HP and intents ×1.1 and ×1.15), so builds with equipment land back in the target band.

## Status Effects

| Status | On | Effect at the end of its owner's turn | Decay | Notes |
|---|---|---|---|---|
| **Burn** | Target | **Nothing:** Burn is fuel and deals no damage on its own | −2 per tick (not on the tick right after it was applied) | Turned into damage by igniting it (e.g. Ember) or consuming it (Detonate, Cauterize) |
| **Poison** | Target | Loses HP equal to its stacks | Halves per tick (rounded down) | Never consumed |
| **Bleed** | Target | Loses **1** HP | −1 per tick | **Stacks are duration, not damage:** 15 Bleed means 1 damage for 15 turns |
| **FireShield** | Self | — | Cleared at the start of its owner's turn | Attackers gain Burn equal to its stacks |
| **Fortify** | Self | — | Never resets | Absorbs damage after Block |
| **Rage** | Self | — | Lasts the whole battle | Every hit deals +1 damage per stack; each hit of a multi-hit gets it |
| **Ritual** | Self | — | Lasts the whole battle | Every status applied to the opponent gets +1 per stack |
| **Vulnerable** | Target | — | −1 per tick (stacks are turns) | Every hit it takes deals +1 damage |
| **Weak** | Target | — | −1 per tick (stacks are turns) | Every hit it deals deals −1 damage |
| **Entangled** | Target | — | −1 per tick (stacks are turns) | Can't reroll |

- Damage over time (Poison, Bleed) bypasses Block and Fortify.
- **Three distinct roles:** Burn is fuel (stored, then ignited or detonated), Poison is front-loaded attrition (big early ticks that halve), Bleed is duration (always 1, for as many turns as it has stacks).
- **Ignite:** deal damage equal to the target's Burn without consuming it. Detonate consumes Burn for one bigger hit. Small ignitions versus waiting for one big detonation is the Burn build's core decision.
- **Rage and Ritual** are visible statuses, not hidden multipliers, so they follow the Value Is the Effect rule: the face's value stays as printed, and previews show the boosted result (e.g. a Sword with 3 Rage previews "5 damage, 1 Bleed").
- **Ritual only boosts statuses applied to the opponent** (Burn, Poison, Bleed). Boosting self-applied statuses would break the "Fortify comes in small amounts" rule. Multiplying a status (Fuel) is not a new application, so Ritual does not apply to it.
- **Vulnerable, Weak and Entangled** are debuffs enemies put on the player (inspired by Slay the Spire's Vulnerable and Weak). Applied on the enemy's turn, N stacks last the player's next N turns. They make some enemy turns worse for the player and interact with rerolls (e.g. Weak: is it worth rerolling for a bigger hit?).
- **Encourage:** the mirror of Ritual. Encourage N adds N to every buff the owner gains on itself: Rage, Fortify and FireShield. It is permanent for the battle, like Ritual. It doesn't affect Block (the basic defense of every face; boosting it would make Encourage a Shield tool instead of a bridge), Ritual, or Encourage itself (no compounding chains). It bridges Rage and Fortify (Shield + Rage). See "Encourage" under Build Archetypes for its content.
- **Fortify is permanent, so it must come in small amounts.** Enemy intents that grant Fortify are kept at 1. A player's Shield face can grant Fortify equal to its value because it pays for it with weaker attacks than Sword.

## Build Archetypes

A build archetype is a family of faces built around one shared resource or play style. Archetypes give the player directions to grow their die, while hybrids keep runs flexible.

### Standalone Faces, Build Potential

**Every face must be useful on its own. Its build unlocks its real potential.**

- **Floor:** On a die with no other faces from its build, a face still has at least one action that does something useful. A player who buys it without a plan is never punished.
- **Ceiling:** With the rest of its build on the die, the same face becomes much stronger.
- **Test:** imagine the face on a die with no other faces from its build. Does at least one action still do something meaningful? If not, give it a standalone fallback action, or make its effect have a useful minimum (e.g. "value + Fortify × value" instead of "Fortify × value").

Common and Rare faces can pass this test through a fallback action. Epic faces have a higher bar:

### Epics Bring Their Own Fuel

**An Epic must beat a good Sword on any die, the turn it comes up, and explode in its build.**

- **Why:** an Epic that needs a working build only pays off in the lucky run. In the other three kinds of run (see below), the player can feel sorry to see one (the user's play-test). Seeing an Epic should be a turning point in every run, and most of all in the normal run with a patchwork die.
- **How:** the Epic produces the resource it spends, as an effect written on the card, so Value Is the Effect still holds. A complete build then multiplies what the Epic started.
- **The fuel is a fixed amount, never the value:** if a multiplying Epic also makes value-sized fuel, its value counts twice and upgrades grow its damage with the square of the value (an upgraded Exsanguinate hit for 24 in the middle of a run, the user's friend's play-test). Exsanguinate and Fuel apply 2.
- **Evidence:** the Epics that lifted bots most when given for free were the ones that need no fuel: Fate's Hand chooses the next face on any die, and Earthshaker reads the highest face on any die.
- **Test:** give the Epic for free before battle 5 to the bots without a focus (Opportunist and Unplanned buyer). It should lift them too, not only the bot of its archetype.
- **Later:** Epics that reward a patchwork die directly (e.g. "per different tag on the die").

### Pragmatist Faces

**Faces for the patchwork die: they belong to no archetype, and they find a different combo in every run.** They serve the normal run below, where a die is built from what the run offers (the user's idea).

- **Rule:** a Pragmatist face reads a general state that many builds produce ("any status on the enemy", not "Burn").
- **Floor:** it works about as well as a Sword on any die.
- **Ceiling:** its bonus adds, it doesn't multiply. It is never the best face of one combo, and it stays Common or Rare.
- **General tag:** these faces carry `General`, not an archetype tag.
- **Test:** they get used and make different combos, without making one-face stacking dominant again. Bots clearing 5-15% remains the target (Roadmap Phase A).

| Face | Rarity | Value | Effect | Finds combos with |
|---|---|---|---|---|
| **Exploit** | Common | 2 | Attack: my value + the number of status types on the enemy | Any die that applies several statuses |
| **Veteran** | Common | 2 | Attack: my value. Defense: block equal to my value + the number of buff types you have | Rage, Fortify, Encourage, Ritual |
| **Feint** | Common | 2 | Attack: my value. Defense: block equal to my value. +2 if your last action was a different one | Balance Scales, mixed dice |
| **Drill Sergeant** | Common | 2 | Defense: block equal to my value. Magic: every face except me and my opposite gains +1 value this battle | The die layout, high-value faces, Hammer |
| **Throwing Knife** | Common | 5 | Attack (Consumable): my value | Fortune, Alchemist's Belt |
| **Spare Blade** | Common | 3 | Attack: my value, +2 if I carry an enchantment | Action enchantments |
| **Catalyst** | Rare | 2 | Magic: add my value to each of Burn, Poison and Bleed on the enemy. Defense: block equal to my value | Status mixes, Ritual |
| **Mimic** | Rare | 2 | Attack: do what the opposite face does on Attack, with my value (a plain hit if it has no Attack) | Whatever the layout gives |
| **Last Stand** | Rare | 2 | Attack: my value + 1 for every 5 HP you are missing | Faces and items that spend HP |
| **Sharpshooter** | Rare | 2 | Attack: my value, +2 if you didn't reroll this turn | Dice that keep their rerolls |

### Every Run Is Its Own Build

**The aim of a run is not to assemble a perfect build. A player who builds a patchwork die from what the run offers can still finish the game.**

- **Every run is unique:** each run reaches its power through a different combination of faces, because the offers decide what is possible.
- **A complete build is the rare power fantasy:** when luck lets a player collect a build perfectly, the die becomes incredibly strong toward the end of the run. These runs must not happen all the time, so they feel valuable when they do.
- **Build-aware offers set that rate:** they are the dial between the two. As content grows, they are tuned so a patchwork die is the normal run and a complete build is the lucky one.

**The four kinds of run, from worst to best. Each one gives the player a reason to press Play again:**

| Run | What happens | Result | How common | Why play again |
|---|---|---|---|---|
| **Bad decisions** | The player builds from faces that don't combo | Loses | Depends on the player | "So that's not how it works; I'll try other faces." |
| **Bad luck** | The offers never fit together, so the player has to take faces that don't work together | Loses | Rare compared to the others | "I was really unlucky; next run will be better." |
| **The normal run** | Not especially unlucky: some faces combo, and the die can finish the game with effort. Bad choices in battle can still lose it | Finishes, or nearly does | The most common | "Better builds are possible; I'll try." |
| **The lucky build** | Great luck and the right decisions collect a build perfectly or nearly so | Becomes hugely strong | Rare | "I was so strong; I want that again." |

**Measured (DevLog Day 68, 1000 runs per bot):**

- **Bad decisions:** the unplanned buyer clears 2%.
- **The normal run:** the Opportunist, with no fixed build, clears 10%. Bots play battles simply, so people should do better.
- **The lucky build:** the clear rate by build faces at battle 8, among the runs that reach it:

| Faces fitting the build | 1 | 2 | 3 | 4 | 5 | 6 |
|---|---|---|---|---|---|---|
| Burn | 4% | 15% | 41% | 76% | 83% | 67% |
| Poison | 0% | 13% | 25% | 58% | 70% | 67% |
| Rage | 0% | 9% | 20% | 31% | 36% | 27% |
| Hammer | 0% | 1% | 11% | 32% | 31% | 26% |
| Fortune | 0% | 7% | 13% | 26% | 25% | 25% |

  - Burn and Poison have the intended power fantasy, and their complete builds are rare (5–6 faces: Burn 15 of 347 runs, Poison 23 of 606).
  - Rage, Hammer and Fortune stop growing at 4 faces, so a complete build isn't a power fantasy. Rage's complete build isn't rare either (148 of 721 runs have 6 faces).
  - The 5–6 face groups of Burn and Poison are small, so those numbers are rough. Bleed and Shield aren't listed because the starting Swords and Shields count as their build faces.

### Archetype Template

Every archetype follows the same three roles. This keeps archetypes readable for players and makes rarity follow naturally from the Rarity Tiers rule.

| Role | Purpose | Rarity |
|---|---|---|
| **Starter** | Produces the build's resource | Common |
| **Engine / Bridge** | Grows the resource, or connects the build to another one | Rare |
| **Payoff** | Turns the resource into results, multiplicatively | Epic |

#### Every archetype needs a Common that grows during a fight

Builds are made of Commons: the simulator's dice at battle 8 hold mostly Commons, and Rares and Epics show up a few times per run. Archetypes win when their Common gets stronger every turn of a fight (Ember applies Burn and ignites the growing total; Venom's Poison stacks and ticks), and lose when it does the same small thing every turn, because strong enemies gain Rage every turn ("clock, not lock").

- So every archetype gets at least one Common that **turns its accumulated resource into results, additively**: value plus (or at least) the resource. Multiplying by the resource stays Epic.
- These Commons keep a floor (their value), so they still pass the Standalone Faces test.

### Burn (implemented)

Burst damage over a short time. Build Burn up, then cash it out.

- **Resource:** Burn on the enemy. It deals no damage on its own and decays by 2 (not on the tick right after it was applied). Ignitions turn it into damage without consuming it; Detonate consumes it.
- **Identity:** Ignite small and often, or stack it up and detonate? Ember applies Burn and ignites it in one action; Detonate trades all the stacks for one big hit.

| Face | Role | Rarity |
|---|---|---|
| Ember | Starter | Common |
| Detonate | Engine (cash-out) | Rare |
| Flame Ward | Bridge (Burn + Block) | Rare |
| Fuel | Payoff (Magic: apply 2 Burn, multiply it, then consume it all for that much damage) | Epic |
| Cauterize | Payoff (Attack: damage and Burn; Magic: consume Burn to heal), value 2 | Epic |

### Poison (implemented)

Slow and inevitable. Strongest in long fights, which makes it the boss-fighting build.

- **Resource:** Poison on the enemy. It deals its stacks as damage at the end of the poisoned entity's turn, bypasses block, **then halves (rounded down)**.
- **Identity:** You only apply Poison. Unlike Burn, nothing consumes or detonates it, and no face does anything extra with the stacks. The payoff is that it works on its own every turn.
- **Difference from Burn:** Burn is fuel that needs to be ignited. Poison deals damage by itself, front-loaded and quickly fading, so it has to be reapplied.
- **Why it halves:** with "then lose 1", one application of N Poison dealt N + (N − 1) + … + 1 = N(N + 1)/2 damage, so its damage grew with the square of the value (value 4: 10 damage, value 6: 21). That broke the "Common scales linearly" rule and made Poison clear 77% of runs, even with half a Poison die. Halving keeps one application at about twice its stacks (4 → 4 + 2 + 1 = 7, 8 → 15), which brought Poison to about 28%. A "Poison does not decay this turn" Epic could bring back the slow, boss-killing side later.

| Face | Role | Rarity | Value | Actions |
|---|---|---|---|---|
| **Venom** | Starter | Common | 2 | Attack: apply Poison equal to my value. Defense: gain block equal to my value. |
| **Toxic Blade** | Bridge (Poison + Physical) | Rare | 2 | Attack: deal damage and apply Poison equal to my value. |
| **Miasma** | Bridge (Poison + Block) | Rare | 2 | Defense: gain block and apply Poison equal to my value. |
| **Plague** | Payoff | Epic | 2 | Magic: apply Poison equal to my value for each Poison face on your die. Attack: apply Poison equal to my value. |

Plague introduces a new Epic pattern: **die-composition scaling** ("for each X face on your die"). It directly rewards building the die around one archetype, and alone it still applies its value (it counts itself).

### Shield (implemented)

Defense that turns into offense. Stack up protection, then trade it for one big hit.

- **Resource:** **Fortify**, a new status on yourself. Fortify absorbs damage like Block (Block first, then Fortify), but it **does not reset at the start of your turn**. Damage over time (Poison, Bleed) bypasses it like it bypasses Block.
- **Identity:** High risk, high reward. Cashing out Fortify deals heavy damage but leaves you without your stored protection.

| Face | Role | Rarity | Value | Actions |
|---|---|---|---|---|
| **Shield** (reworked) | Starter | Common | 2 | Attack: deal damage equal to my value. Defense: gain Fortify equal to my value. |
| **Spiked Shield** | Starter (grows) | Common | 2 | Attack: deal damage equal to your Fortify, at least my value (nothing is consumed). Defense: gain Fortify equal to my value. |
| **Shield Bash** | Engine | Rare | 2 | Attack: deal damage equal to my value plus your Block and Fortify (nothing is consumed). Defense: gain block equal to my value. |
| **Flame Ward** | Bridge (Burn + Block) | Rare | 2 | (existing face) |
| **Shield Burst** | Payoff | Epic | 2 | Attack: consume all Fortify and deal my value plus Fortify × my value as damage. Defense: gain Fortify equal to my value. |

Reworking Shield into the Shield starter also ends the Sword/Shield duplication: Sword stays the plain Physical face.

- **Spiked Shield vs. Shield Bash:** Spiked Shield's hit is just your Fortify (at least its value), while Shield Bash adds its value and your Block on top. Fortify never resets, so Spiked Shield's hit only grows during a fight.

### Berserker (implemented)

Power that keeps growing. Build Rage, then hit often or hit once with everything.

- **Resource:** Rage on yourself. It lasts the whole battle and adds +1 damage per stack to every hit, so multi-hit faces gain the most.
- **Identity:** Spend early turns building Rage instead of attacking, then cash it out. Bloodrage trades HP for Rage, and since HP carries over between battles, that cost lasts the whole run (Pillar 4).

| Face | Role | Rarity | Value | Actions |
|---|---|---|---|---|
| **Fury** | Starter | Common | 1 | Magic: gain Rage equal to my value. Attack: deal damage equal to my value. |
| **Bloodrage** | Starter (risky) | Common | 2 | Magic: lose HP and gain Rage equal to my value. Attack: deal damage equal to my value. |
| **Frenzy** | Starter (grows) | Common | 2 | Attack: deal damage equal to my value, then gain 1 Rage. |
| **Twin Axes** | Engine | Rare | 1 | Attack: hit twice for my value. |
| **Iron Rage** | Bridge (Rage + Block) | Rare | 2 | Defense: gain Fortify equal to my value and 1 Rage. |
| **Unleash** | Payoff | Epic | 2 | Attack: gain 1 Rage, then deal my value × your Rage as damage. |

- **Frenzy builds Rage without losing a turn:** Fury and Bloodrage trade an attack for Rage; Frenzy attacks and grows every later hit by 1. The Rage comes after the hit, so it never boosts its own hit.
- **Twin Axes hits twice, not three times:** with Rage counted on every hit, three hits would scale like a multiplier and belong in Epic. Two hits keep it a Rare (+1 value improves two hits).
- **Iron Rage** is the bridge to the Shield archetype: defending also builds permanent damage, which shortens Shield's slow fights.

### Ritualist (implemented)

The glue between status builds. Ritual makes every status you apply stronger.

- **Resource:** Ritual on yourself. It lasts the whole battle and adds +1 per stack to every status applied to the opponent (Burn, Poison, Bleed). Self-applied statuses are not boosted.
- **Identity:** Ritual does nothing on its own; it multiplies what other faces apply. It is meant to be combined with Bleed, Poison or Burn faces, which makes it the natural hybrid archetype.

| Face | Role | Rarity | Value | Actions |
|---|---|---|---|---|
| **Incantation** | Starter | Common | 1 | Magic: gain Ritual equal to my value. Defense: gain block equal to my value. |
| **Witch's Curse** | Engine | Rare | 2 | Magic: apply Bleed and Poison equal to my value (two applications, so Ritual counts twice). |
| **Blood Rite** | Bridge (Ritual + Physical) | Rare | 3 | Attack: deal damage and apply Bleed equal to my value. |
| **Dark Communion** | Payoff | Epic | 2 | Magic: apply Poison equal to my value, then gain Ritual equal to my value for each status type (Burn, Poison, Bleed) on the enemy, at least my value. |

- **Dark Communion rewards variety:** the more different statuses the enemy carries, the more Ritual it gives. It is the "reward variety" payoff noted under Hybrid Builds.

### Bleed (implemented)

A mark, not a damage source. Keep the enemy bleeding, and your faces hit harder or heal while it lasts.

- **Resource:** Bleed on the enemy. It always deals 1 damage per tick and loses 1 stack, so its stacks are how many turns the enemy stays marked. The rule stays as it is: Bleed's value is the duration, not the damage.
- **Identity:** faces with "if the enemy is bleeding" effects. Keep Bleed going to keep those effects on, or cash in the remaining duration with Exsanguinate.
- **The faces deal the damage, not Bleed.** Bleed's 1 damage per tick is a token; the build's power comes from building Bleed up and the faces getting stronger from it. So the archetype grows by adding more "if the enemy is bleeding" effects and faces that scale with its stacks (Lacerate), not by making Bleed tick harder.

| Face | Role | Rarity | Value | Actions |
|---|---|---|---|---|
| **Sword** | Starter (starting die) | Common | 2 | Attack: deal damage and apply 2 Bleed. Defense: gain block. |
| **Serrated Blade** | Starter | Common | 4 | Attack: if the enemy is already bleeding, deal damage equal to my value; then apply Bleed equal to my value. Defense: gain block equal to my value. |
| **Lacerate** | Starter (grows) | Common | 2 | Attack: deal damage equal to my value plus the enemy's Bleed (nothing is consumed). Defense: gain block equal to my value. |
| **Open Wound** | Engine | Rare | 2 | Attack: deal damage equal to my value; if the enemy is bleeding, hit again for my value. |
| **Bloodscent** | Bridge (Bleed + Rage) | Rare | 2 | Defense: gain block equal to my value; if the enemy is bleeding, heal equal to my value. |
| **Exsanguinate** | Payoff | Epic | 2 | Attack: apply 2 Bleed, then consume the enemy's Bleed and deal my value × the consumed Bleed as damage (at least my value). |

- **Bleed timing:** Bleed applied on your turn ticks at the end of the enemy's turn, so N Bleed keeps the enemy marked for N − 1 of your turns. Bleed sources therefore apply at least 2 (Sword 2, Serrated Blade 4, Blood Rite 3, Witch's Curse 2); 1 Bleed would never reach your next turn.
- **Lacerate keeps Bleed a mark:** Bleed still deals its 1 per tick; Lacerate hits harder the longer the enemy stays marked. Unlike Exsanguinate it consumes nothing and adds instead of multiplying, which keeps it a Common.
- **Serrated Blade is a conditional Common on purpose:** conditions are normally Rare, but Bleed's whole identity is "if the enemy is bleeding", so its Starter has one. When it only applied Bleed, its rolls dealt no damage (Sword already keeps the mark up, so the extra duration was mostly wasted) and the Bleed bot cleared 0–8% of runs. With the hit and value 4 it clears about 27%. Its own Bleed comes after the check, so on an unmarked enemy it marks first and hits on later rolls.
- **Conditional faces keep a floor:** without Bleed, Open Wound still hits once and Bloodscent still blocks (Standalone Faces rule).
- **Exsanguinate vs. Detonate:** Detonate is additive (value + Burn), Exsanguinate is multiplicative (value × Bleed) and consumes the whole remaining duration. "Keep the mark for the conditional effects, or cash it in now?" is the Bleed build's core decision.
- **The life loop (Bleed + Berserker bridge):** Berserker uses HP as a resource. Bloodrage spends HP for Rage, and Bloodscent heals it back while the enemy bleeds: lose HP, apply strong effects, heal back. Open Wound's second hit also benefits from Rage. Bloodscent's healing is kept as is for now and will be revisited if it proves too strong.
- **"If" effects are one reusable box** (`ConditionalEffectSO`), so future conditions ("if the enemy is poisoned", "if you have Fortify") need no new code.

### Hammer (implemented)

Pour power into one face and set up the turn it lands. Hammer stores its resource on the die itself, the purest form of "You build a die".

- **Resource:** **Charge**, temporary value on a die face for the current battle. A Hammer face gains **+1 Charge every turn it isn't the face you act with**, up to **+5**. When you act with a face, its Charge is used and then resets to 0.
- **Value Is the Effect still holds:** a face's value in battle is its value plus its Charge, the number shown on the die, and its effects use exactly that number.
- **Math:** on a six-sided die, a face comes up about once every six turns, so a value-5 War Hammer usually lands at 8–10. Strong for a Common, but you don't know when it will land, and it can only attack.
- **The cap (+5)** applies to Charge gained every turn and from Anvil; only Resonance can push past it. It keeps "when will it land?" more important than "how big has it grown?"; without it, one roll in a long fight could deal 15+.
- **The cost (Pillar 4):** War Hammer and Earthshaker only have Attack. When a charged Hammer lands, you swing even if the enemy is about to hit hard.
- **Grows in battle, not through upgrades** (the "Common that grows during a fight" rule).
- **Any face can hold Charge** (Anvil adds it to any face), but only faces with the Charge rule gain it every turn.
- **Enchantments:** charged Hammers are the high-value faces enchantments were made for. War Hammer, Maul and Earthshaker have an empty Magic action, and an enchantment on them uses the charged value.

| Face | Role | Rarity | Value | Actions |
|---|---|---|---|---|
| **War Hammer** | Starter | Common | 5 | Attack: deal damage equal to my value. Charge. |
| **Maul** | Starter (safe) | Common | 3 | Attack: deal damage equal to my value. Defense: gain block equal to my value. Charge. |
| **Anvil** | Bridge (Hammer + Block) | Rare | 2 | Defense: gain block equal to my value. Magic: add my value as Charge to your highest-value other face. |
| **Steady Hand** | Engine (die manipulation) | Rare | 1 | Magic: reroll the die and act again. Defense: gain block equal to my value. |
| **Earthshaker** | Payoff | Epic | 1 | Attack: deal damage equal to your highest-value other face's value times my value. Its Charge is kept. |

- **Maul is the safe Hammer:** when it lands charged, you choose between the big swing and a big block. It exists because the first set (War Hammer at value 2 as the only Common) cleared 7% of runs: the Hammer bot lost the most HP per turn of any build (2.0), and a second Attack-only Common didn't help (7%), while a Charge face with Defense doubled it. With Maul and War Hammer at 3, the Hammer bot clears about 32%. Maul's Block tag also gives the Shield archetype a growing face (Shield bot 6% → 20%).
- **The Hammer pass (DevLog Day 62):** Hammer fell from about 32% to 5%. Medusa, the Hydra and the enemy traits came after it, and it grows in battle rather than through upgrades, so the new economy (starting gold, Healer) helped it least. Its damage was fine (4.5 per turn, 70% of Strong battles won); it died at Medusa (56% of its arrivals, the others 39–45%) and at the Hydra (62%).
  - Medusa at 42 HP cut her kill rate for everyone (Hammer 56% → 40%) but Hammer still cleared 5%: it died later instead. It also lifted Ritual + Poison to 28%, so Medusa stays.
  - A higher Charge cap (+7, +8) changed nothing: faces rarely sit unused long enough to reach it.
  - War Hammer and Maul +1 gave 9–12%, but Maul's Block tag lifted Shield too (20%). **War Hammer 3 → 5** gives 10% and leaves Shield (17–18%), Rage and Shield + Rage unchanged. Its cost: it only attacks, and its upgrades cost more (Common: value × 3).
- **Anvil** aims the Charge: it builds up the face you want to land, and bridges to the Shield archetype.
- **Steady Hand** is the control tool: spend a weak roll fishing for the charged Hammer. Rerolling is not a new turn (Block doesn't reset, turn-start items don't fire), and a reroll can land on Steady Hand again.
- **Earthshaker** is the multiplicative payoff: it hits with a charged face's value without using its Charge, so you can strike twice from one build-up.
- **Standalone floor:** War Hammer hits without Charge, Anvil and Steady Hand block, and Earthshaker hits for at least its value.

### Future Archetypes

Ideas to revisit after Poison and Shield.

- **Crowd Favor (cross-build mechanic):** Showy plays (overkill, a turn without block, consecutive attacks) earn Favor from the arena crowd, which turns into bonus gold after the battle. Connects combat to the economy.
- **Die topology:** Face positions on the die matter, e.g. opposite faces (1–6, 2–5, 3–4) or adjacent faces. Turns "which face do I replace?" into "which face, in which slot?". Complex, so it stays a late idea.

### Encourage (implemented)

The crowd's favor: the arena cheers, and every buff you gain on yourself grows. Ritual is a dark rite that boosts what you do to the enemy; Encourage is its bright mirror, boosting what you do for yourself.

- **Rule:** see "Status Effects". Encourage adds to every gain of Rage, Fortify and FireShield, each gain separately, so many small gains (items such as Tower Helm, Berserker Torc, Iron Bracers) profit most, just as Ritual does with Plague Mask.
- **A bridge, not an archetype:** Encourage content carries the **Rage and Block** tags, so the Market offers it to Shield and Rage players and the number of archetypes stays the same.
- **The set mirrors Ritual's, so players recognize the pattern:**

| Name | Kind | Rarity | Value | Effect | Ritual's counterpart |
|---|---|---|---|---|---|
| **Rally** | Face | Common | 1 | Magic: gain Encourage equal to my value. Defense: gain block equal to my value. | Incantation |
| **Battle Hymn** | Face | Rare | 2 | Defense: gain Fortify equal to my value; if you have Encourage, also gain 1 Rage. | (the bridge) |
| **Crowd's Favor** | Face | Epic | 1 | Magic: gain 1 Rage, then gain Encourage equal to my value for each buff type you have (Rage, Fortify, FireShield), at least my value. | Dark Communion |
| **Inspire** | Enchantment | Epic | — | Magic: gain Encourage equal to the face's value. | Dark Rite |

- **Battle Hymn gains two buffs in one action,** so Encourage boosts it twice (with Encourage 2: value + 2 Fortify and 3 Rage). It is the shared face of Shield + Rage. Its Rage needs Encourage, so it is an Encourage payoff, not a free Shield upgrade.
- **Small numbers first:** Rage is permanent and already strong, so Rally starts at value 1.
- **Tuned by simulation (DevLog Day 64):** the first set lifted Shield 18% → 28%, Rage 12% → 23%, Shield + Rage 14% → 26%. Two pieces caused most of it:
  - **Herald's Horn** (an item: 1 Encourage at the start of every battle) doubled Tower Helm, which almost every Shield bot carries. Without it Shield + Rage fell from 26% to 17%. It is out of the set for now (the asset stays for a later, delayed version, e.g. "the first time you defend: gain 1 Encourage").
  - **Battle Hymn** first gave 1 Rage on every Defense with no condition. The Shield bot defends all the time, so its Rage piled up and it was a strictly better Shield face; neither value 1 nor Epic fixed it. Now its Rage needs Encourage.
  - With both changes: Shield 23–24%, Rage 12–14%, Shield + Rage 16–17%, Bleed + Rage 9%.

### Hybrid Builds

Hybrids add variety and give players a way to pivot when they can't find the faces they want, instead of giving up on the run.

- **Bridge faces** carry two archetype tags (e.g. Flame Ward: Burn + Block, Toxic Blade: Poison + Physical) and naturally connect two builds.
- **Die-composition payoffs** can be written for any tag, or even reward variety (e.g. "for each different tag on your die").
- **Tag weighting stays around 60%** so about 40% of offers come from outside the current build and pivoting stays possible.
- **Not every pair has to work (DevLog Day 63).** Ritual is the glue for statuses whose stacks are damage: Poison (more stacks, bigger ticks) and Burn (more fuel for ignitions). Ritual + Poison is the strongest hybrid (about 23%).
  - **Ritual + Bleed is a weak pairing by design.** Bleed's stacks are its duration, not its damage (Bleed deals 1 per tick; the damage comes from the faces). Ritual raised the enemy's Bleed from 4.1 to 6.3 stacks, but tick damage stayed the same (0.97 vs 0.91 per turn). Only Lacerate (Common) and Exsanguinate (Epic) turn stacks into damage, and the Ritual pieces (Incantation, Bone Fetish) take die and item slots from Bleed faces, so the hybrid clears less than plain Bleed (7% vs 12%).
  - Neither a stronger Lacerate (Ritual + Bleed 6%) nor a Rare Exsanguinate (5%) helped. A bridge face or making Ritual raise Bleed's tick damage would blur Bleed's identity, so the pairing stays weak on purpose.
  - A Bleed player's strong hybrid is **Bleed + Rage** instead: Rage adds to every hit of the faces that carry Bleed's damage.

### Open Questions

- How Fortify interacts with FireShield and future Block effects.
- Second payoff faces for Poison and Shield, so each archetype has more than one Epic.

## Enemies & Bosses

The arena hosts two kinds of opponents. **Normal enemies** use telegraphed intents and set the pace of a run. **Bosses** are legendary champions of the Colosseum who fight with their own die.

### Normal Enemies

Normal enemies keep the intent system: at the start of the player's turn, the enemy picks and shows its next action (Slay the Spire style), so the player can plan around it.

- **Data, not code:** each enemy is defined as data (name, HP, list of intents), just like die faces.
- **Pools by run segment:** each part of the run has its own enemy pool, so enemies get stronger as the run goes on.

- **Intent choice:** intents have weights, and the same intent cannot be picked several times in a row. This avoids streaks like healing three turns in a row.
- **Readable intents:** an intent shows what it does, not just a name and a number (e.g. "Shield Slam: 4 damage", "Brace: 1 Fortify"). A name alone is easy to misread.
- **No repeats:** the same enemy never appears twice in one run.

#### Enemy Tiers

Each tier has its own pool. Battles before the first boss draw from the Weak pool, battles between the bosses from the Medium pool, and battles after the second boss from the Strong pool.

| Tier | Battles | Role | Player feeling | HP | Damage |
|---|---|---|---|---|---|
| **Weak** | 1–3 | **Toll** | "Let me get through without losing much HP." | 9–14 | 1–2, small Burn/Poison |
| **Medium** | 5–7 | **First real threat** | "I could lose this run." | 17–20 | Spikes of 4–5 |
| **Strong** | 9–10 | **Build check** | With a good build: "Look how strong I've become!" Without one: the end of the run. | 40–56 | Rising every turn |

- **Weak** enemies use plain swords and shields: flat damage, block, and at most small amounts of Burn or Poison. They don't challenge the player; they make the player want to get through cheaply.
- **Medium** enemies each have **one danger mechanic**. It is readable, but ignoring it costs a lot of HP. This is where players first feel that the run can end.
- **Strong** enemies test the whole build: **"clock, not lock."** They never shut down the player's combos (that's what bosses do). Instead, high HP and rising damage set a clock:
  - A strong build melts them before the clock runs out, and their large HP gives combos room to show big numbers. This is where the game's power fantasy happens, and players start wondering what other broken combos exist.
  - A die that came together by chance loses the race.
- The **final boss** serves the same purpose: it tests the whole run rather than simply being hard. A player without a strong build was lucky to get there; a player with one has earned a satisfying win.

#### Enemy Roster

Enemies are real Roman gladiator types and arena beasts. Weights are shown as (×N). Numbers were tuned with the run simulator (see Balance Targets below) and will be tuned further through play-testing.

**Weak (battles 1–3):**

| Enemy | HP | Intents | Note |
|---|---|---|---|
| **Tiro** (recruit) | 11 | Slash 2 (×3), Guard: block 2 (×2) | The simplest enemy, teaches the basics |
| **Murmillo** (heavy shield gladiator) | 14 | Shield Wall: block 2 (×2), Strike 2 (×2) | A defensive enemy whose block has to be broken through |
| **Thraex** (curved dagger) | 13 | Sica Cut 1 (×2), Envenom: Poison 1 (×2), Guard: block 1 (×1) | Small Poison: the fight costs more HP the longer it lasts |
| **Lampadarius** (torchbearer) | 13 | Torch Swing: Burn 1 (×2), Strike 2 (×2), Guard: block 1 (×1), Ignite: ignite Burn (×1) | Small Burn, ignited now and then |
| **Arena Wolf** | 9 | Bite 2 (×3), Howl: block 2 (×1) | Low HP but hits hard; finish it fast |

**Medium (battles 5–7), each with one danger mechanic:**

| Enemy | HP | Intents | Danger |
|---|---|---|---|
| **Retiarius** (net and trident) | 20 | Jab 2 (×2), Net: block 3 (×1), Trident Thrust 6 (×1) | **Telegraphed spike:** ignore the intent and take 6 |
| **Dimachaerus** (two swords) | 19 | Twin Strike: 2 × 2 damage (×2), Parry: block 2 (×1) | **Multi-hit:** each hit wears down block separately, but each hit also triggers FireShield |
| **Arena Lion** | 22 | Maul 1 (×2), Roar: gain 1 Rage (×1), Feast: 1 damage and heal by the damage (×1) | **Builds Rage:** harmless at first, deadly if the fight drags on |
| **Venefica** (poisoner) | 19 | Poison Dart: Poison 2 (×2), Strike 2 (×1), Smoke: block 2 (×1) | **Poisons the player:** repeated darts keep Poison ticking, so long fights cost HP |
| **Secutor** (pursuer) | 22 | Brace: Fortify 1 (×2), Shield Slam 4 (×2) | **Stacks Fortify:** its protection slowly builds up, so deal damage early |

**Strong (battles 9–10), the build check:**

| Enemy | HP | Intents | Clock |
|---|---|---|---|
| **Essedarius** (chariot fighter) | 48 | Run Down: 7 damage if you act with a face below 5 (×2), Lash: 4 damage if you choose Attack (×1), Wheel Spikes 3 (×1), Rein In: block 3 (×1) | **Threats and Momentum:** its threats push you to reroll, and every reroll gives it Rage |
| **War Elephant** | 64 | Stomp 7 (×1), Trunk Swat 2 (×2), Thick Hide: Fortify 1 (×1) | **Huge HP pool** with a telegraphed crushing blow, a big target for combos |
| **Hoplomachus Champion** | 40 | Spear Thrust 3 (×2), Shield Rush: 2 damage and block 2 (×1), Battle Cry: gain 1 Rage (×1) | **Rage:** each Battle Cry permanently raises all its damage |
| **Bestiarius** (beast tamer) | 46 | Whip 2 (×2), Unleash 6 (×1), Feed the Beast: heal 2 (×1) | **Healing plus spikes:** a weak build can't out-damage its healing |

#### New Mechanics for Enemies

- **Multi-hit damage** (Dimachaerus): one effect that deals damage N times. Player faces can use it later too.
- **Escalation through Rage** (Essedarius, Hoplomachus, Arena Lion): rising damage uses the same visible Rage status as the player. An enemy can gain Rage from an intent (Battle Cry, Roar), which the player sees coming. Rage every turn (`escalationPerTurn`) is still supported but no enemy uses it: on Essedarius it became a wall that slow builds could not get past (Essedarius alone caused about half of the Bleed, Rage and Shield deaths), so it was replaced with Battle Cry. Its intents show the boosted numbers, and the Rage chip's tooltip explains why.

#### Enemy Traits

Inspired by Slay the Spire: enemies have always-on **traits** that react to what the player does, so every turn becomes a small puzzle ("which action is safe against this enemy with this face?"). Traits are shown next to the enemy with a tooltip and flash when they trigger.

| Trait | Rule | Tests | Hard on | Answer |
|---|---|---|---|---|
| **Thorns** | Each time you hit it, you take 1 damage | Multi-hit play | Rage, Twin Axes, Smoldering Gauntlet | One big hit, statuses |
| **Enrage** | Each hit that costs it HP gives it 1 Rage | Long, many-hit fights | Rage, Bleed (Lacerate) | Status ticks (not hits), finishing fast |
| **Curl Up** | Each hit that gets through with no block gives it 2 block at the start of its next turn | The opening move | Hammer's big hits | Make it block first, or strike when it is about to curl up |
| **Challenge** | Each time you choose Defense, it gains 1 Rage | Passive Shield play | Shield, Fortify | Defending by attacking, Spiked Shield |
| **Parry** | Each time you choose Attack while it has block, you take damage | Attacking into its block | Multi-hit builds | Waiting for its block to reset, statuses |
| **Second Wind** | Below half HP it changes its intents | Burst timing | Slow builds | Crossing the threshold on purpose |
| **Momentum** | Each reroll you spend gives it 1 Rage | The reroll decision | Fortune, heavy rerollers | Rerolling only when it matters |

- **Assignment (one trait or one new intent per enemy, Weak enemies stay simple):**
  - Murmillo: Curl Up (2 block at the start of its next turn). The gentle introduction to traits. It was "the first hit that costs it HP: gain 3 block", but block gained on the player's turn is gone at the start of the enemy's own turn, so it only mattered against a second hit in the same turn (play-test). The block now arrives at the start of its next turn, so it is there on the player's next turn, and the card shows it while it is waiting.
  - Retiarius: Net now also Entangles (no rerolls next turn).
  - Dimachaerus: Parry (2 damage).
  - Arena Lion: Enrage, capped at the first 2 HP-losing hits each battle, replacing its Roar intent (see the balance note below).
  - Venefica: a new Hamstring intent (Weak 2).
  - Secutor: Second Wind (drops its shield and attacks wildly).
  - Essedarius: Momentum, plus threat intents (see below).
  - War Elephant: Thorns (thick hide), capped at the first 6 hits each battle. Uncapped, its big HP pool made every build pay for every hit, and it killed 43% of the focused bots at battle 10 against 18-28% for the other Strong enemies.
  - Hoplomachus Champion: Challenge (the first 3 times you choose Defense each battle: +1 Rage).
  - Bestiarius: Whip also applies Vulnerable.
- **Threat intents** (Essedarius): telegraphed threats on the player's choice this turn. Run Down: "7 damage if you act with a face below 5"; Lash: "4 damage if you choose Attack". They hit on the enemy's turn only if the player's action set them off. With Momentum they make the dilemma the user asked for (inspired by Slay the Spire's Gremlin Nob): take the hit, or reroll for a safer face and give it permanent Rage. Faces whose value isn't a number (`hasNoValue`, for future special faces) count as below any threshold.
  - The first version (Momentum alone, weak attacks) was no threat: without rerolls it never grew. Run Down started at "below 3 for 6", which only caught starter faces; "below 5 for 7" catches most faces.
- **Rule: test, don't lock.** Every trait hurts some builds more, but every build keeps a way to win.
- **Balance note:** uncapped Enrage made the Arena Lion the deadliest enemy in the game (431 deaths in a 5-bot sample, against 88 without the trait, more than any boss). Capped at 2 per battle and without Roar, it is a little harder than before (106).
- **Traits doubled as the first difficulty step:** bot clear rates fell from 8–49% to 2–19%, close to the Phase A target of 5–15% for bots. The bots don't play around traits (e.g. they keep defending against Hoplomachus), so a player who reads them does better.
- **Second round (later):** punishing Magic, status resistance (e.g. ignore the first 2 statuses), splitting (needs multiple enemies, Phase C).
- **Technical:** traits reuse the equipment trigger system from the enemy's side: an `EnemySO` lists `EnemyTrait`s (a trigger such as "hit by the player", "lost HP from a hit", "the player chose an action", "the player rerolled", plus existing effect assets). Second Wind is a second intent list the brain switches to below half HP.

#### Difficulty Curve

- **The problem (play-test and simulation):** the Weak battles stayed the same while the player grew stronger in every Market, so battle 2 and 3 were easier than battle 1 (HP lost per battle: 4.5, 2.9, 2.6), and the Minotaur came as a cliff (20.1, about 8 times battle 3).
- **Battle scaling:** each tier climbs instead of staying flat. Normal enemies at some battle numbers get more HP and start with **Rage**. Rage is a visible status that adds to every hit and shows in the intents' numbers, so there is no hidden multiplier (Value Is the Effect). Bosses are tuned on their own. The Market's "next battle" line shows the starting Rage, and its hover shows the scaled HP.

| Battle | HP | Starting Rage |
|---|---|---|
| 1 | ×1 | 0 |
| 2 | ×1.7 | 0 |
| 3 | ×1.4 | 1 |
| 5 | ×1 | 0 |
| 6 | ×1.15 | 0 |
| 7 | ×1.3 | 1 |
| 9 | ×1 | 0 |
| 10 | ×1.15 | 0 |

- **Tuned by simulation (DevLog Day 66, HP lost per battle, the average of the Burn, Rage, Shield and Fortune bots):**

| Version | 1 | 2 | 3 | 4 (Minotaur) | Clear rate |
|---|---|---|---|---|---|
| No scaling | 4.5 | 2.9 | 2.6 | 20.1 | 28% |
| HP ×1.25 / ×1.5 + 1 Rage at battle 3 | 4.5 | 4.1 | 7.5 | 18.5 | 15% |
| + 1 Rage at battle 2, Stampede 9 | 4.7 | 8.2 | 7.6 | 17.5 | 15% |
| Battle 2 HP ×1.3, Minotaur 22 HP | 4.6 | 4.2 | 7.9 | 17.1 | 16% |
| **Battle 2 HP ×1.7, Minotaur 22 HP (chosen)** | **4.6** | **5.8** | **7.2** | **17.0** | **16%** |

  - 1 Rage on a Weak enemy is a big step (their hits are 1–2), so battle 2 climbs through HP and battle 3 adds the Rage.
  - Stampede 12 → 9 barely mattered: most of the Minotaur's damage comes from Gore and Trample in a long fight, so its HP went down instead (26 → 22).
  - The Minotaur is still the run's first peak (about 2.4 times battle 3, was 8), and it kills about 20% of the runs that reach it, as before.
- **All bots with the curve and the 20-gold Healer (700 runs):** Burn 10%, Poison 13%, Shield 17%, Rage 21%, Bleed 16%, Hammer 12%, Fortune 12%, Ritual 1%, Ritual + Poison 24%, Ritual + Bleed 11%, Shield + Rage 15%, Bleed + Rage 20%. Before the curve, the 20-gold Healer had pushed them to 22–48%.

- **Battle 10 became a wall (DevLog Day 68):** with 1 starting Rage it killed 50–55% of the runs reaching it, twice the Hydra. Removing the Rage gave 37%, and no scaling at all still 33%, because of one outlier: Hoplomachus Champion beat bots in 56% of forced fights (War Elephant 35%, Essedarius and Bestiarius 19%). Its uncapped Challenge and Battle Cry both pile up Rage. A cap of 3 gave 48%, removing Battle Cry barely helped (43–51%), and a cap of 3 with 40 HP (was 51) gave 37%; Spear Thrust 2 also gave 37%, but HP keeps its hits threatening.
- **After the fix (5 bots, 400 runs), the share of arrivals that die:** Minotaur 22%, battle 7 23%, Medusa 24%, battle 9 19%, battle 10 33%, Hydra 17%. Battle 10 is still above the final boss, and the Hydra is measured only on strong survivors.
- **Clear rates rose,** since many runs were stopped by Hoplomachus (700 runs): Burn 15%, Poison 23%, Shield 27%, Rage 30%, Bleed 31%, Hammer 18%, Fortune 23%, Ritual 1%, Ritual + Poison 28%, Ritual + Bleed 20%, Shield + Rage 23%, Bleed + Rage 33%, Opportunist 15%, unplanned buyer 4%, no shopping 0%. To be judged by play-tests against "Every Run Is Its Own Build".

#### Balance Targets

Enemy numbers are tuned with the run simulator (`Tools → Gladiator Dice → Run Balance Simulation`), where bots fight with the real battle rules and shop in the real Market. The difficulty curve is measured against these targets:

| Target | Result (600–1000 runs per bot) |
|---|---|
| An unbuilt die (no shopping) gets through the Weak tier losing about 10–15 HP | 11 HP lost, then it dies in the Medium tier (28% reach battle 8) |
| A die bought without a plan stalls around battles 9–10 | 71% reach battle 8, 11% reach battle 9, 0% clear the run |
| A well-built die mostly beats the Strong tier | Poison 63%, Burn 38% clear the run; Shield, Berserker and Ritualist about 0% |

Shield misses the target because its strong-tier fights take far longer than other builds' (about 24 turns versus 6), so enemies that build Rage win the race. This is an archetype problem, not an enemy one.

These results use the Run Start die (2 Sword, 2 Shield, Health Potion and a chosen starting face) and a 23-face Market pool. Adding the Berserker and Ritualist faces diluted the pool, so focused Burn and Poison bots find their faces less often. Berserker and Ritualist are not tuned yet, and the focused bots only buy faces of one tag, which doesn't suit Ritual (a hybrid glue). These archetypes need a balance pass and a hybrid-aware bot.

### Multiple Enemies

Some battles are fought against a group (Roadmap Phase C). Target choice adds a decision every turn: "the one in front, or the dangerous one?"

- **Targeting:** click an enemy to select it as the target; the default is the front living enemy, and the target moves there when the selected one dies. Attacks, statuses applied to the enemy, ignites and items hit the selected target. Defense and buffs on yourself are unaffected. The target can be changed before or after rolling.
- **Each enemy** has its own intent, traits and statuses (which tick separately). Enemies act in order, front first.
- **Traits** react to the player as before. A trait about being attacked (Parry) only fires for the enemy that is the target.
- **Groups:** normal battles can be groups of up to 3 enemies, each with less HP than a lone enemy (a multiplier per group). A group appears as often as any lone enemy of its tier and counts as meeting its members: nothing is met twice in a run.
- **Each group is tuned to play like a hard lone enemy of its tier.** Even with less total HP, two attackers deal double damage per turn while both live, so the multipliers sit well below 0.5:

| Group | Tier | Members (front first) | HP | Bots: HP taken / lost |
|---|---|---|---|---|
| Tiro Pair | Weak | Tiro, Tiro | ×0.4 (4 + 4) | 7.8 HP (lone Weak enemies: 2–7) |
| Wolf Pack | Weak | Arena Wolf, Arena Wolf | ×0.4 (4 + 4) | 8.6 HP |
| Net and Shield | Medium | Murmillo, Retiarius | ×0.5 (7 + 10) | 18.5% lost (lone Medium enemies: 2–24%) |
| Poison and Blades | Medium | Dimachaerus, Venefica | ×0.45 (9 + 9) | 19% lost |
| Beast Master | Strong | Arena Lion, Bestiarius | ×0.45 (10 + 21) | about 34% lost (lone Strong enemies: 20–58%) |

  - Secutor + Retiarius (the first idea) lost 29% even at ×0.35: two of the hardest Medium enemies are too much together. A tank in front (Murmillo) of a dangerous back-liner works better: kill the front one, or reach past it.
- **The battle is won** when every enemy is dead (the Hydra's body is the exception, see below).
- **Area effects:** see "Area Effects" below.

### Area Effects

Faces, enchantments and items that act on several enemies.

- **Value Is the Effect:** an area effect applies its full value to every enemy, never a split. "3 damage to every enemy" deals 3 to each.
- **Faces work standalone:** about a quarter of normal battles are groups, so every area face is a normal face against one enemy and raises its ceiling against groups.
- **Each enemy is a separate target:** every hit gets Rage and fires hit triggers (items, Thorns), and every status gets Ritual. This makes area effects a natural bridge for Berserker and Ritualist.
- **No exceptions:** area effects hit the Hydra's heads too, so killing them grows more heads unless the body has Burn.
- **Three kinds:**
  - **Direct:** the same effect on every enemy (Cleave, Wildfire, Sweep, Censer).
  - **Spread:** copy the target's stacks to the others (Contagion).
  - **Carry:** a killing hit's leftover damage goes to the next enemy (Follow Through).

| Name | Kind | Rarity | Build | Effect |
|---|---|---|---|---|
| **Cleave** | Face, value 3 | Common | Rage | Attack: deal my value to every enemy. Defense: block |
| **Wildfire** | Face, value 2 | Rare | Burn | Magic: apply my value as Burn to every enemy. Defense: block |
| **Contagion** | Face, value 2 | Rare | Poison | Magic: apply my value as Poison, then copy the target's Poison to every other enemy. Defense: block |
| **Follow Through** | Face, value 3, Charge | Rare | Hammer | Attack: deal my value; if it kills the target, the excess hits the next enemy |
| **Sweep** | Enchantment | Epic | Any | Magic: deal the face's value to every enemy |
| **Censer** | Item | Rare | Poison | End of your first 3 turns each battle: 1 Poison to every enemy |

- **Tuned by simulation (DevLog Day 56).** The test swapped one Sword on the starter die for the face and measured HP taken in Weak battles (400 runs each):

| Face | Lone Tiro | Tiro Pair | Lone Arena Wolf | Wolf Pack |
|---|---|---|---|---|
| Sword | 6.7 | 8.2 | 5.7 | 9.0 |
| Cleave at 2 | 7.9 | 8.3 | 6.8 | 8.9 |
| **Cleave at 3** | **6.8** | **7.5** | **5.8** | **8.1** |
| War Hammer | 5.6 | 7.4 | 5.1 | 8.3 |
| Follow Through | 5.7 | 7.2 | 5.2 | 7.8 |

  - Cleave at 2 was weaker than Sword alone and only equal in groups, because Sword's Bleed is worth a lot. At 3 it plays like Sword alone and is better against groups.
  - Follow Through plays like War Hammer alone and a little better against groups.
- **Whole runs:** the focused Burn, Poison, Rage and Hammer bots clear the same share with or without the new faces in the pool (within 1 point). Area effects are a situational tool, not a power jump. Most deaths happen at the bosses, where area effects matter little.
- **The Hydra:** with Cleave in the pool, the Rage bot's Hydra deaths are unchanged (44%). Cleave's 3 damage rarely kills an 8 HP head by accident.
- **Censer is capped at 3 per battle (DevLog Day 60):** uncapped, it was on 75% of Poison bots' gear at battle 8 and lifted Poison to 27% and Ritual + Poison to 36% (Ritual adds to every tick's Poison). Without it: 17% and 15%; capped at 3, like Plague Mask's cap: 16% and 25%.
- **Later:** area content for Bleed and Shield.
- **The Hydra** is a body plus head enemies (see "Hydra" under Bosses). When more than 3 cards are on screen, all of them shrink to fit.
- **Bots** focus fire: they target the living enemy with the lowest HP.
- **Screen:** each enemy has a card at the top right (name, HP, defense, statuses, intent, traits); the target's card has a gold TARGET frame, and a defeated enemy's card is dimmed. The rolled face sits in the middle of the screen.
- **Built in stages:** the battle rules first (checked with single-enemy regression runs), then the targeting UI and the groups, then the Hydra (DevLog Days 52–54).

### Bosses

Bosses appear at battles 4, 8 and 11 (the final boss). They are what players remember about a run, so each one is built around a mechanic of its own.

#### Boss Design Principles

- **Unique concepts, not mirror matches:** bosses never use the player's archetypes (Burn, Poison, Shield). Each boss has its own concept, its own die and boss-only mechanics. Fighting a stronger copy of your own build teaches nothing new; a boss that petrifies your die asks a question the player has never faced.
- **Bosses test the die:** each boss tests a different aspect of the player's die. The most threatening attack in a game about building a die is an attack on the die itself.
- **Test, don't hard-counter:** a boss can favor some builds, but every build must have a way to win. A boss mechanic never leaves the player with nothing to do (e.g. a petrified face still gives a little block). This is the boss version of the Standalone Faces rule.
- **Battle-scoped effects:** whatever a boss does to the player's die lasts only for that battle. The run's die is never changed permanently.

#### Boss Dice

- **Each boss has a 6-face die** built from boss-only faces. Boss faces use the same rule as the player's: the printed value is the effect.
- **Telegraphed roll:** the boss rolls its die at the start of the player's turn, and the result is shown as its intent (e.g. "Charge: 25 damage in 3 turns"). Luck still matters, since the boss's strongest face can come up, but the player always sees it coming and can react.
- **Fixed actions:** each boss face has one fixed action chosen by the designer. Bosses don't need an AI: the player makes the decisions, and the boss's job is to be a readable threat.
- **Visible die:** the boss's 6 faces are shown for the whole battle, so players can read the odds ("1 Stampede face: a 1 in 6 chance each turn"). Reading the boss's die is a skill to master.
- **Boss faces are normal die faces** (`DieFaceSO`) with one action each, so the effect system, Rage, Ritual and Value Is the Effect work for bosses exactly as for the player.
- **The run ends after the final boss:** winning battle 11 completes the run.

#### Minotaur (battle 4)

A raging bull of the labyrinth. The first boss teaches players to read a boss die and plan around a countdown.

- **Tests:** planning ahead.
- **Mechanic: Stampede countdown.** When the Stampede face is rolled, the Minotaur lowers its horns and a visible countdown starts: at the end of its third turn after that, it deals one huge hit. (Named Stampede, not Charge, so it doesn't clash with the Hammer's Charge.)
- **It keeps rolling during the countdown,** so the countdown is pressure, not free turns. A second Stampede rolled while one is running counts as a Gore.
- **Counterplay:** be ready with block when the hit lands, or **stagger** it: if the Minotaur loses enough HP from hits during the countdown, the Stampede is cancelled. Damage over time (Poison, Bleed) doesn't stagger, so status builds answer it with block. Defensive and aggressive builds both have an answer.

| Face | Count | Action |
|---|---|---|
| Gore | 2 | Attack: deal heavy damage |
| Trample | 2 | Attack: deal damage and gain block |
| Hide | 1 | Defense: gain a lot of block |
| Stampede | 1 | Start the Stampede countdown (huge hit in 3 turns, cancelled by stagger) |

- **HP 22** (was 26, lowered in the difficulty curve pass, DevLog Day 66, so the first boss is a peak rather than a cliff).

#### Medusa (battle 8)

The gorgon whose gaze turns gladiators to stone. Medusa attacks the player's die itself.

- **Tests:** reliance on single faces. Dice with backup faces (duplicates, several faces that do the same job) hold up better.
- **Mechanic: Petrify.** Medusa's gaze petrifies the player's **highest-value face**, by battle value (value plus Charge), so a charged Hammer is her natural target. The target is shown in the intent, so the player knows which face is in danger.
- **Stone faces:** a petrified face still exists on the die, but when it is rolled its only action is **Defense: gain 2 block**, so the player is never left with nothing to do. Its own actions and its enchantment don't work while it is stone. Petrify lasts for the player's next **2 turns**, then wears off, but Medusa can petrify again (always the highest-value face that isn't stone yet).
- **Stone and Charge:** a petrified Hammer keeps its Charge, but if it is rolled while stone, the stone turn spends it. Petrifying the charged Hammer is Medusa's answer to Hammer builds.
- Petrify lasts only for the battle and never changes the run's die.

| Face | Count | Action |
|---|---|---|
| Gaze | 2 | Petrify the player's highest-value face |
| Serpent Bite | 2 | Attack: deal damage |
| Coil | 1 | Attack: deal damage, plus 3 damage for each petrified face the player has (simulation showed the bonus barely changes the balance, so it was chosen for a clear threat) |
| Stone Skin | 1 | Defense: gain a lot of block |

#### Hydra (battle 11, final boss)

The many-headed serpent: cut off one head and two grow back, unless the wound is burned. In the myth, Heracles cauterized the necks with fire.

- **Tests:** how the player deals damage, and which enemy they choose to hit.
- **Mechanic: Heads.** The Hydra is a **body** plus **heads**, each head a separate enemy with its own card, HP and intent (see "Multiple Enemies"). It starts with **2 heads** (at most **3** alive).
  - **Cutting heads:** killing a head cuts it off.
  - **Regrowth:** a cut head grows back as two (up to 3 alive) at the start of the Hydra's second turn after the cut, **unless the body has Burn then**. Grown heads act from the next turn. Cutting a head also deals 5 damage to the body (it counts toward Scales). So cutting a head hurts the body and buys a turn without its attacks and its regeneration, and costs a second head later (the user's friend's play-test: heads used to come back at the very next turn, so cutting one was never worth it).
  - **Regeneration:** at the end of its turn the body heals 1 per living head. Burn stops this, and so does **Bleed** (an open wound doesn't close).
  - **The battle is won when the body dies,** even with heads still alive.
- **The decision:** the heads are the easy targets and the ones that bite, but killing one makes two. Without Burn the body is the target; with Burn on the body, clearing the heads first is safe. In simulation, bots that keep killing heads die in 90% of their Hydra fights, against 33% for bots that go for the body.
- **Every build has a path:**
  - **Burn** cauterizes the necks, so it can clear the heads safely, and it stops regeneration.
  - **Bleed** stops regeneration while it wears the body down.
  - **Poison** and **Rage** go for the body.
  - **Shield** survives the bites while it works on the body.
  - **Burst** builds (Hammer) kill the body faster; their big hits only grow heads if they are aimed at one.

| Enemy | HP | Die / intents |
|---|---|---|
| Hydra (body) | 60 | Scales: loses at most 12 HP a round. Thrash ×2 (Attack: 3 damage), Corrode ×1 (Attack: remove half of your Fortify, then 3 damage), Regrow ×1 (Magic: heal 5, blocked by Burn or Bleed), Hiss ×2 (Defense: 3 block) |
| Hydra Head | 8 | Bite: 1 damage (×1), Hiss: 2 block (×1) |

- **Tuned by simulation (DevLog Day 54):** the target was the old Hydra's difficulty (it killed about 37% of the runs that reached it). The first numbers (body 40, Bite 2 at weight 3, Regrow ×2) killed 74–78%. Body HP barely mattered (30: 73%); the bites did. Bite 1 brought it to 59%, fewer Regrow faces to 54%, and Bite only half the time to **40%** (later runs: 33%). Removing regeneration also reached 34%, but regeneration is what makes Burn and Bleed matter, so it stays.
- **The paths are more even than before:** Burn dies in about 20% of its Hydra fights (before: 3%), Shield 35%, Poison 33%, Bleed 50%, Hammer 57% (before: 54%).

All boss numbers were set by simulation (DevLog Days 44–46) and are still to be confirmed by play-testing.

### Technical Notes

- Bosses reuse `DieFace` and `EffectSO`, with boss-only mechanics written as new `EffectSO` subclasses.
- **`EffectContext` needs a `targetDie` field:** `die` is the source's die, but Medusa acts on the target's die. This is a new field, not a signature change.
- **Battle-scoped face states** (e.g. Petrified) live in battle data, never in `RunData`.
- **Brains:** normal enemies use the intent brain, and bosses use a dice brain that rolls at the start of the player's turn and exposes the rolled face as the intent. Both share one brain interface, so the battle flow and the simulator don't care which one they run.
- **Boss data:** a boss asset holds its name, HP, its 6 faces and its mechanic's numbers. Boss slots are listed per battle number, each with a pool of bosses, which is ready for several bosses per slot.
- **Boss mechanics live in battle data** (the Stampede countdown, petrified faces, the Hydra's heads), never in `RunData`.
- **Built in stages:** the architecture, the Minotaur and the end of the run first; then Medusa; then the Hydra. Until a slot's boss exists, that slot keeps using the next enemy tier.

### Future

- **Multiple bosses per slot (planned):** each boss slot will have a pool of bosses, and one is picked per run (e.g. battle 4: Minotaur or Trickster). This is required for replayability. Candidates:
  - **Trickster:** manipulates the player's luck ("roll twice, keep the lower"), testing consistency.
  - **The Emperor:** the arena's ruler, whose thumbs-up or thumbs-down die changes the rules of the fight.
- **Boss phases:** at 50% HP a boss upgrades or swaps one of its faces, mirroring what the player does in the Market.
- **Boss loot:** the Grand Market after a boss slightly favors Epic faces themed around that boss.

### Open Questions

- Healing amount after a boss (50% of max HP is a placeholder).

## Visual Style

- **Roman pixel art,** side view: the player stands on the left facing right, enemies on the right facing left, all on one ground line.
- **One look for every sprite:** dark brown outlines, a warm palette (terracotta, bronze, bone white, olive, deep red), hard pixel edges. Still sprites are cleaned onto a true pixel grid with a limited palette.
- **Sizes are relative to a gladiator** (`spriteHeight`): beasts and bosses are taller, never just filling the frame. A crowded row shrinks as a whole, so a boss stays bigger than its heads.
- **Animation matters most.** It is made where frames stay consistent (PixelLab); still images can come from any generator that matches the style.
- **Every generated sprite is disclosed** as AI content on the store page.
- **The battle screen is the arena:** a thin top bar (battle number, gold, items, the boss's die), the fight in the middle, and a bottom bar with the die net, the rolled face and the buttons. Each enemy's intent and traits float above its head and its plate (name, HP, statuses) sits under its feet, so the eye stays on the fighters; clicking an enemy targets it. Boss battles are fought in the night arena.
- **Card language: numbers on screen, words on hover.** Intents and statuses are an icon and a number (a sword and 6, a blood drop and 2); the intent's name and every rule stay in the tooltip. **Faces are the exception:** a face is always shown with its full rules, each action on its own line under its colored name (ATTACK:, DEFENSE:, MAGIC:), and the action buttons preview in words ("2 damage, 2 Bleed"); icon lines for faces read as a puzzle even to a player who knew them. Colored keywords show their definitions on hover. Titles are in Cinzel, numbers and text in a pixel font with unmistakable digits (VT323).
- **Face tiles: every face is recognized by its look.** A face is always the same object, an ivory die-face tile, in the Market, on the die net and in the dice tray, so buying one feels like fitting a new face to your die. Three layers, read from far to near: the tile's **ground color is its family** (the first tag other than Physical: Bleed dark red, Burn orange, Poison green, Ritual purple, Rage crimson brown, Block steel blue, Hammer iron grey, Fortune gold, General plain bone; the starting faces plain ivory; a face's other tags show in its rules, not on the tile), **its emblem is the face itself** (one bold object named by the face, never a status icon), and **its rim is its rarity** (bronze, silver, gold; a purple seal when enchanted). The value sits in a corner. Emblems are listed in `Docs/ArtPrompts.md`.
- **Items and enchantments have pictures too**, one look per kind so they never mix up: an item is the object alone (like a face emblem), a die enchantment is its motif on a round bronze medallion (bound to the die's places like a stamp), a face enchantment is its motif pressed into a purple wax seal, which also sits in the corner of the face it is inscribed on. In battle an item is only its picture (its name and rules on hover); in the Market its picture sits next to its name. Prompts are in `Docs/ArtPrompts.md`.
- **One look on every screen:** the Market and the run start screen sit on the dimmed arena, with the same dark brown panels, bronze buttons and fonts as the battle.

## Sound

Sound is feedback first: every thing the player does or suffers has its own short sound, so a fight can be followed with
the eyes half closed. Music sets each screen's mood and fades from one screen to the next.

- **The die is the star.** Rolling plays a real die thrown on a table (a different take each time).
- **Hits tell what happened:** a punch when a hit costs HP (heavier when the player is hit), a metal clang when Block or
  Fortify stops it, a wooden knock when Block is raised, a rising chime for healing and a thud for a death. Poison and
  Bleed ticks are silent: they come in a row at every turn's end and the numbers already show them.
- **Turns:** a blade drawn for YOUR TURN, a low bell for ENEMY TURN. An item that fires plucks a string as its chip flashes.
- **The Market:** coins for buying, an anvil for upgrades, glass for enchanting, shuffled cards for a reroll, a latch for
  a lock, a sliding card for moving a face. Every button clicks.
- **Music:** a feast tune at the run start, a market tune in the Market, an epic theme in battle and a heavier one for the
  bosses. Victory, defeat and the end of the run stop the music for a short jingle.
- **Small and repeatable:** every sound has a few takes and a little pitch variation, so the hundredth hit doesn't sound
  like the first one played again.
- **Settings:** a bronze gear in the top right corner of every screen (or Esc) opens a panel with Music and Effects volume sliders (the effects slider plays a sample as it moves) and, in a build, Quit Game. The choices are kept between sessions.
- All sounds and music are public domain (CC0); sources are in `Docs/Credits.md`.

