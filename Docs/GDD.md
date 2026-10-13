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
- Max Value
- Primary Effect
- Secondary Effect (optional)
- Tags
- Description

### Value Is the Effect

The number printed on a face is exactly what its effects use. There are no hidden multipliers: a face showing 4 never secretly deals 8. Players should be able to trust the big number on a face at a glance. Faces differ through which actions and effects they support, not through hidden scaling.

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
- **Healing after bosses:** winning a boss battle restores 50% of max HP (placeholder). Without it, runs would end from slow HP loss instead of from how good the build is.

### Run Start

Every run starts from the same five neutral faces plus one face the player picks.

| Face | Count | Rarity | Value | Actions |
|---|---|---|---|---|
| **Sword** | 2 | Common | 2 | Attack: deal damage equal to my value and apply 1 Bleed. Defense: gain block equal to my value. |
| **Shield** | 2 | Common | 2 | Attack: deal damage equal to my value. Defense: gain Fortify equal to my value. |
| **Health Potion** | 1 | Common | 3 (max 4) | Magic: heal equal to my value. No Attack or Defense. |
| **Starting face** | 1 | Chosen | — | Picked by the player at the start of the run |

- **Sword and Shield mirror each other:** Sword is stronger on offense (its attack also bleeds) and weaker on defense (temporary Block). Shield is stronger on defense (permanent Fortify) and weaker on offense. If one of them proves too strong, it will be rebalanced.
- **Sword's Bleed is a fixed 1**, not its value, so upgrading Sword only raises its damage. With +1 value improving a single effect, Sword stays Common.
- **Health Potion** does one strong thing: rolled at full HP, it does nothing. This is intended. A later reroll mechanic (see Future below) gives players a way out of rolls that don't fit the situation.

**Choosing the starting face:**

- At the start of a run, the player is offered **3 random faces** and picks one for the sixth slot of the die.
- Rarity weights: **Common 85%, Rare 13%, Epic 2%**.
- The three offers come from **different archetypes** (different tags), so the choice is a real decision about the build's first direction.
- Faces already on the starting die (Sword, Shield, Health Potion) are never offered.
- Every run starts different from the first moment, and the game's core question ("which face do I want?") is asked right away.

**Future:**

- **Reroll:** once per roll, the player may reroll the die. A bad roll can be escaped once, and if the reroll is also bad, that's the game's share of luck.

### Market

Each visit offers a limited, random selection. The player is never forced to spend.

- **Normal Market:** 3 face offers, with rarity weights Common 70%, Rare 25%, Epic 5%. Epic faces can appear, but rarely.
- **Grand Market (after a boss):** 3 offers, **all Epic**. The Grand Market is the station for Epic faces. Because Grand Markets come at known points (after bosses), saving gold for one is a plannable strategy, not a gamble.
- A face is offered at most once per visit.
- **Build-aware offers:** offers are weighted toward the tags already on the player's die (around 60%, not 100%). A Burn die is more likely to be offered Burn faces such as Fuel, while some offers still come from outside the build so players keep discovering new faces.

Market services:

- **Buy a face:** the new face replaces one face of the die, chosen by the player.
- **Upgrade a face:** raise one die face's value by 1, up to that face's Max Value. The Normal Market upgrades Common and Rare faces; **Epic faces can only be upgraded at the Grand Market**.

### Prices

Prices are placeholders, to be tuned through play-testing.

- **Gold income:** 20 gold per victory.
- **Buy price by rarity:** Common 10, Rare 25, Epic 60. With current income, an Epic costs about three victories of saving, while Common and Rare faces are cheap enough to keep the build growing.
- **Upgrade price:** rarity multiplier × current value, so each upgrade costs more than the last.

| Rarity | Multiplier | 2 → 3 | 3 → 4 |
|---|---|---|---|
| Common | ×3 | 6 | 9 |
| Rare | ×5 | 10 | 15 |
| Epic | ×20 | 40 | 60 |

- **Max Value:** every face has an upgrade cap. Prices make upgrading Epic faces expensive, and the cap rules out extreme cases (e.g. Fuel capped at 3, so it can never multiply Burn by 4 or more). Current placeholder rule: Common base + 3, Rare base + 2, Epic base + 1.

### Save vs. Tempo

The decision only works if both paths are viable.

- **Spending now** makes the die stronger in the next battles, so the player wins faster and loses less HP.
- **Saving** keeps gold for an Epic face and its upgrades at the next Grand Market. Since **HP carries over between battles**, fighting with a weaker die costs HP that the player has to survive without.
- No interest mechanic for now. If play-testing shows that saving is never worth it, rewarding saved gold (e.g. interest) is the first thing to revisit.

### Open Questions

- Gold reward for boss victories.

## Status Effects

| Status | On | Effect at the end of its owner's turn | Decay | Notes |
|---|---|---|---|---|
| **Burn** | Target | Loses HP equal to its stacks | −2 per tick (not on the tick right after it was applied) | Can be consumed (Detonate, Cauterize) |
| **Poison** | Target | Loses HP equal to its stacks | Never decays | Never consumed |
| **Bleed** | Target | Loses **1** HP | −1 per tick | **Stacks are duration, not damage:** 15 Bleed means 1 damage for 15 turns |
| **FireShield** | Self | — | Cleared at the start of its owner's turn | Attackers gain Burn equal to its stacks |
| **Fortify** | Self | — | Never resets | Absorbs damage after Block |

- Damage over time (Burn, Poison, Bleed) bypasses Block and Fortify.
- **Fortify is permanent, so it must come in small amounts.** Enemy intents that grant Fortify are kept at 1. A player's Shield face can grant Fortify equal to its value because it pays for it with weaker attacks than Sword.

## Build Archetypes

A build archetype is a family of faces built around one shared resource or play style. Archetypes give the player directions to grow their die, while hybrids keep runs flexible.

### Standalone Faces, Build Potential

**Every face must be useful on its own. Its build unlocks its real potential.**

- **Floor:** On a die with no other faces from its build, a face still has at least one action that does something useful. A player who buys it without a plan is never punished.
- **Ceiling:** With the rest of its build on the die, the same face becomes much stronger.
- **Test:** imagine the face on a die with no other faces from its build. Does at least one action still do something meaningful? If not, give it a standalone fallback action, or make its effect have a useful minimum (e.g. "value + Fortify × value" instead of "Fortify × value").

Existing faces pass this test through their fallback actions: Fuel's Magic does nothing without Burn, but its Defense still gives block. Cauterize's Magic needs Burn, but its Attack still deals damage.

### Archetype Template

Every archetype follows the same three roles. This keeps archetypes readable for players and makes rarity follow naturally from the Rarity Tiers rule.

| Role | Purpose | Rarity |
|---|---|---|
| **Starter** | Produces the build's resource | Common |
| **Engine / Bridge** | Grows the resource, or connects the build to another one | Rare |
| **Payoff** | Turns the resource into results, multiplicatively | Epic |

### Burn (implemented)

Burst damage over a short time. Build Burn up, then cash it out.

- **Resource:** Burn on the enemy. It deals its stacks as damage at the end of the owner's turn, bypasses block and decays by 2 (not on the tick right after it was applied).
- **Identity:** Consume or let it tick? Detonating gives a big hit but resets the stacks.

| Face | Role | Rarity |
|---|---|---|
| Ember | Starter | Common |
| Detonate | Engine (cash-out) | Rare |
| Flame Ward | Bridge (Burn + Block) | Rare |
| Fuel | Payoff (multiply Burn) | Epic |
| Cauterize | Payoff (consume Burn to heal) | Epic |

### Poison (implemented)

Slow and inevitable. Strongest in long fights, which makes it the boss-fighting build.

- **Resource:** Poison on the enemy. It deals its stacks as damage at the end of the poisoned entity's turn and bypasses block. **It never decays.**
- **Identity:** You only apply Poison. Unlike Burn, nothing consumes or detonates it, and no face does anything extra with the stacks. The payoff is that it never goes away.
- **Difference from Burn:** Burn is burst (decays, gets consumed). Poison is attrition (never decays, never consumed).

| Face | Role | Rarity | Value | Actions |
|---|---|---|---|---|
| **Venom** | Starter | Common | 2 | Attack: apply Poison equal to my value. Defense: gain block equal to my value. |
| **Toxic Blade** | Bridge (Poison + Physical) | Rare | 2 | Attack: deal damage and apply Poison equal to my value. |
| **Miasma** | Bridge (Poison + Block) | Rare | 2 | Defense: gain block and apply Poison equal to my value. |
| **Plague** | Payoff | Epic | 1 | Magic: apply Poison equal to my value for each Poison face on your die. Attack: apply Poison equal to my value. |

Plague introduces a new Epic pattern: **die-composition scaling** ("for each X face on your die"). It directly rewards building the die around one archetype, and alone it still applies its value (it counts itself).

### Shield (implemented)

Defense that turns into offense. Stack up protection, then trade it for one big hit.

- **Resource:** **Fortify**, a new status on yourself. Fortify absorbs damage like Block (Block first, then Fortify), but it **does not reset at the start of your turn**. Damage over time (Burn, Poison) bypasses it like it bypasses Block.
- **Identity:** High risk, high reward. Cashing out Fortify deals heavy damage but leaves you without your stored protection.

| Face | Role | Rarity | Value | Actions |
|---|---|---|---|---|
| **Shield** (reworked) | Starter | Common | 2 | Attack: deal damage equal to my value. Defense: gain Fortify equal to my value. |
| **Shield Bash** | Engine | Rare | 2 | Attack: deal damage equal to my value plus your Block and Fortify (nothing is consumed). Defense: gain block equal to my value. |
| **Flame Ward** | Bridge (Burn + Block) | Rare | 2 | (existing face) |
| **Shield Burst** | Payoff | Epic | 2 | Attack: consume all Fortify and deal my value plus Fortify × my value as damage. Defense: gain Fortify equal to my value. |

Reworking Shield into the Shield starter also ends the Sword/Shield duplication: Sword stays the plain Physical face.

### Future Archetypes

Ideas to revisit after Poison and Shield.

- **Hammer:** Pour power into one face and set up the turn it lands for a one-turn kill. Uses in-battle growth instead of permanent upgrades, so it never needs to break Max Value:
  - **Charge:** the face gains +1 temporary value every turn it isn't rolled.
  - **Die manipulation:** reroll the die, or trigger your highest-value face.
  - Needs changes to the battle state machine.
- **Berserker:** Power for blood. Some faces hurt you, others grow stronger as your HP drops. Since HP carries over between battles, self-damage is a real run-long cost. Fits the gladiator theme, and needs no new status.
- **Crowd Favor (cross-build mechanic):** Showy plays (overkill, a turn without block, consecutive attacks) earn Favor from the arena crowd, which turns into bonus gold after the battle. Connects combat to the economy.
- **Die topology:** Face positions on the die matter, e.g. opposite faces (1–6, 2–5, 3–4) or adjacent faces. Turns "which face do I replace?" into "which face, in which slot?". Complex, so it stays a late idea.

### Hybrid Builds

Hybrids add variety and give players a way to pivot when they can't find the faces they want, instead of giving up on the run.

- **Bridge faces** carry two archetype tags (e.g. Flame Ward: Burn + Block, Toxic Blade: Poison + Physical) and naturally connect two builds.
- **Die-composition payoffs** can be written for any tag, or even reward variety (e.g. "for each different tag on your die").
- **Tag weighting stays around 60%** so about 40% of offers come from outside the current build and pivoting stays possible.

### Open Questions

- Poison may prove too strong if it never decays. If so, let it decay by 1 per tick.
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
| **Lampadarius** (torchbearer) | 13 | Torch Swing: Burn 1 (×2), Strike 2 (×2), Guard: block 1 (×1) | Small Burn |
| **Arena Wolf** | 9 | Bite 2 (×3), Howl: block 2 (×1) | Low HP but hits hard; finish it fast |

**Medium (battles 5–7), each with one danger mechanic:**

| Enemy | HP | Intents | Danger |
|---|---|---|---|
| **Retiarius** (net and trident) | 18 | Jab 2 (×2), Net: block 3 (×1), Trident Thrust 5 (×1) | **Telegraphed spike:** ignore the intent and take 5 |
| **Dimachaerus** (two swords) | 17 | Twin Strike: 2 × 2 damage (×2), Parry: block 2 (×1) | **Multi-hit:** each hit wears down block separately, but each hit also triggers FireShield |
| **Arena Lion** | 20 | Maul 4 (×2), Feast: 2 damage and heal 2 (×2) | **Self-healing:** punishes slow damage |
| **Venefica** (poisoner) | 17 | Poison Dart: Poison 2 (×2), Strike 2 (×1), Smoke: block 2 (×1) | **Poisons the player:** Poison never decays, so long fights become deadly |
| **Secutor** (pursuer) | 20 | Brace: Fortify 1 (×2), Shield Slam 4 (×2) | **Stacks Fortify:** its protection slowly builds up, so deal damage early |

**Strong (battles 9–10), the build check:**

| Enemy | HP | Intents | Clock |
|---|---|---|---|
| **Essedarius** (chariot fighter) | 42 | Ride Down 2 (×2), Wheel Spikes 2 (×1), Rein In: block 3 (×1) | **Gathering speed:** all its attacks gain +1 damage every turn |
| **War Elephant** | 56 | Stomp 6 (×1), Trunk Swat 2 (×2), Thick Hide: Fortify 1 (×1) | **Huge HP pool** with a telegraphed crushing blow, a big target for combos |
| **Hoplomachus Champion** | 44 | Spear Thrust 3 (×2), Shield Rush: 2 damage and block 2 (×1), Battle Cry: +1 damage (×1) | **Rage:** each Battle Cry permanently raises all its damage |
| **Bestiarius** (beast tamer) | 40 | Whip 2 (×2), Unleash 5 (×1), Feed the Beast: heal 2 (×1) | **Healing plus spikes:** a weak build can't out-damage its healing |

#### New Mechanics for Enemies

- **Multi-hit damage** (Dimachaerus): one effect that deals damage N times. Player faces can use it later too.
- **Escalation** (Essedarius, Hoplomachus): rising damage is **enemy-only data**. An enemy's attack intents grow by a fixed amount each turn (Essedarius), or permanently after a specific intent (Hoplomachus' Battle Cry). The grown value is shown in the intent. A general "Strength" status was considered but postponed until the Berserker archetype, because it would touch every damage effect.

#### Balance Targets

Enemy numbers are tuned with the run simulator (`Tools → Gladiator Dice → Run Balance Simulation`), where bots fight with the real battle rules and shop in the real Market. The difficulty curve is measured against these targets:

| Target | Result (600–1000 runs per bot) |
|---|---|
| An unbuilt die (no shopping) gets through the Weak tier losing about 10–15 HP | 12 HP lost, then it dies in the Medium tier (15% reach battle 8) |
| A die bought without a plan stalls around battles 9–10 | 75% reach battle 8, 2% clear the run |
| A well-built die mostly beats the Strong tier | Poison 87%, Burn 72% clear the run; Shield only 7% |

Shield misses the target because its strong-tier fights take about 17 turns versus about 5 for other builds, so escalating enemies win the race. This is an archetype problem, not an enemy one.

These results use the Run Start die (2 Sword, 2 Shield, Health Potion and a chosen starting face). The starting face rarity is currently skewed toward Rare and Epic (about 60% Common instead of 85%) because only two Common faces (Ember, Venom) can be offered; more Common faces are needed.

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
- **Visible die:** the boss's 6 faces are shown for the whole battle, so players can read the odds ("1 Charge face: a 1 in 6 chance each turn"). Reading the boss's die is a skill to master.

#### Minotaur (battle 4)

A raging bull of the labyrinth. The first boss teaches players to read a boss die and plan around a countdown.

- **Tests:** planning ahead.
- **Mechanic: Charge countdown.** When the Charge face is rolled, the Minotaur lowers its horns and a visible countdown starts: in 3 turns, it deals one huge hit.
- **Counterplay:** be ready with block when the charge lands, or **stagger** it: if the Minotaur takes enough damage during the countdown, the charge is cancelled. Defensive and aggressive builds both have an answer.

| Face | Count | Action |
|---|---|---|
| Gore | 2 | Attack: deal heavy damage |
| Trample | 2 | Attack: deal damage and gain block |
| Hide | 1 | Defense: gain a lot of block |
| Charge | 1 | Start the charge countdown (huge hit in 3 turns, cancelled by stagger) |

#### Medusa (battle 8)

The gorgon whose gaze turns gladiators to stone. Medusa attacks the player's die itself.

- **Tests:** reliance on single faces. Dice with backup faces (duplicates, several faces that do the same job) hold up better.
- **Mechanic: Petrify.** Medusa's gaze petrifies the player's **highest-value face**. The target is shown in the intent, so the player knows which face is in danger.
- **Stone faces:** a petrified face still exists on the die, but when it is rolled it only gives a little block, so the player is never left with nothing to do. Petrify wears off after **2–3 turns** (exact number to tune), but Medusa can petrify again.
- Petrify lasts only for the battle and never changes the run's die.

| Face | Count | Action |
|---|---|---|
| Gaze | 2 | Petrify the player's highest-value face |
| Serpent Bite | 2 | Attack: deal damage |
| Coil | 1 | Attack: deal damage, plus extra damage for each petrified face the player has |
| Stone Skin | 1 | Defense: gain a lot of block |

#### Hydra (battle 11, final boss)

The many-headed serpent: cut off one head and two grow back, unless the wound is burned. In the myth, Heracles cauterized the necks with fire.

- **Tests:** how the player deals damage.
- **Mechanic: Heads.** The Hydra starts with a few heads. Its Bite face attacks once per head.
  - **Cutting heads:** a single big hit cuts off a head.
  - **Regrowth:** at the start of its turn, every cut head grows back as two, **unless the Hydra has Burn**.
  - **Regeneration:** the Hydra also heals every turn based on its head count, and Burn stops this too.
- **Every build has a path:**
  - **Burn** cuts heads safely and stops regeneration.
  - **Poison** never cuts heads (so no regrowth) and, since it never decays, eventually outpaces regeneration.
  - **Burst** builds (e.g. Shield Burst) deal big hits but grow more heads, a risk/reward decision about when to strike.

| Face | Count | Action |
|---|---|---|
| Bite | 3 | Attack: deal damage once per head |
| Thrash | 1 | Attack: deal heavy damage |
| Regrow | 1 | Magic: heal a large amount (blocked by Burn) |
| Hiss | 1 | Defense: gain block |

All boss numbers (HP, damage, countdown length, stagger threshold, head count) are placeholders, to be tuned through play-testing.

### Technical Notes

- Bosses reuse `DieFace` and `EffectSO`, with boss-only mechanics written as new `EffectSO` subclasses.
- **`EffectContext` needs a `targetDie` field:** `die` is the source's die, but Medusa acts on the target's die. This is a new field, not a signature change.
- **Battle-scoped face states** (e.g. Petrified) live in battle data, never in `RunData`.
- **Brains:** normal enemies use the intent brain, and bosses need a dice brain that rolls at the start of the player's turn and exposes the rolled face as the intent.

### Future

- **Multiple bosses per slot (planned):** each boss slot will have a pool of bosses, and one is picked per run (e.g. battle 4: Minotaur or Trickster). This is required for replayability. Candidates:
  - **Trickster:** manipulates the player's luck ("roll twice, keep the lower"), testing consistency.
  - **The Emperor:** the arena's ruler, whose thumbs-up or thumbs-down die changes the rules of the fight.
- **Boss phases:** at 50% HP a boss upgrades or swaps one of its faces, mirroring what the player does in the Market.
- **Boss loot:** the Grand Market after a boss slightly favors Epic faces themed around that boss.

### Open Questions

- Gold reward for boss victories.
- Exact Petrify duration (2 or 3 turns).
- Healing amount after a boss (50% of max HP is a placeholder).