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
- Soft Cap
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
- **Gold after bosses:** winning a boss battle pays a 30 gold bonus on top of the normal reward, so the player reaches the Grand Market able to afford about one Epic (simulated bots arrive with about 60 gold; without the bonus they had about 25, less than any Epic).

### Run Start

Every run starts from the same five neutral faces plus one face the player picks.

| Face | Count | Rarity | Value | Actions |
|---|---|---|---|---|
| **Sword** | 2 | Common | 2 | Attack: deal damage equal to my value and apply 2 Bleed. Defense: gain block equal to my value. |
| **Shield** | 2 | Common | 2 | Attack: deal damage equal to my value. Defense: gain Fortify equal to my value. |
| **Health Potion** | 1 | Common | 3 (soft cap 4) | Magic: heal equal to my value. No Attack or Defense. |
| **Starting face** | 1 | Chosen | — | Picked by the player at the start of the run |

- **Sword and Shield mirror each other:** Sword is stronger on offense (its attack also bleeds) and weaker on defense (temporary Block). Shield is stronger on defense (permanent Fortify) and weaker on offense. If one of them proves too strong, it will be rebalanced.
- **Sword's Bleed is a fixed 2**, not its value, so upgrading Sword only raises its damage. With +1 value improving a single effect, Sword stays Common. It was 1, but Bleed applied on your turn ticks at the end of the enemy's turn, so 1 Bleed was gone before your next turn and "if the enemy is bleeding" faces never saw it.
- **Health Potion** does one strong thing: rolled at full HP, it does nothing. This is intended. A later reroll mechanic (see Future below) gives players a way out of rolls that don't fit the situation.

**Choosing the starting face:**

- At the start of a run, the player is offered **3 random faces** and picks one for the sixth slot of the die.
- Rarity weights: **Common 85%, Rare 13%, Epic 2%**.
- The three offers come from **different archetypes** (different tags), so the choice is a real decision about the build's first direction.
- Faces already on the starting die (Sword, Shield, Health Potion) are never offered.
- Every run starts different from the first moment, and the game's core question ("which face do I want?") is asked right away.

**Future:**

- **Die reroll** (in battle, not the Market reroll below): once per roll, the player may reroll the die. A bad roll can be escaped once, and if the reroll is also bad, that's the game's share of luck.

### Market

Each visit offers a limited, random selection. The player is never forced to spend.

- **Normal Market:** 3 face offers and 2 action module offers (see Action Modules), with rarity weights Common 70%, Rare 25%, Epic 5%. Epic faces can appear, but rarely.
- **Grand Market (after a boss):** 3 offers, **all Epic**. The Grand Market is the station for Epic faces. Because Grand Markets come at known points (after bosses), saving gold for one is a plannable strategy, not a gamble.
- A face is offered at most once per set of offers.
- **Build-aware offers:** offers are weighted toward the tags already on the player's die (around 60%, not 100%). A Burn die is more likely to be offered Burn faces such as Fuel, while some offers still come from outside the build so players keep discovering new faces.

Market services:

- **Buy a face:** the new face replaces one face of the die, chosen by the player.
- **Upgrade a face:** raise one die face's value by 1. There is no hard limit; past the face's soft cap, upgrades get much more expensive. The Normal Market upgrades Common and Rare faces; **Epic faces can only be upgraded at the Grand Market**.
- **Reroll the offers:** pay gold for a new set of offers, faces and modules alike (sold offers are replaced too). The new set avoids the faces it replaced when the pool allows.
- **Buy an action module:** attach it to an empty action slot of one die face (see Action Modules).

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

### Action Modules

You don't only build the die, you build its faces. An action module is a single action bought in the Market and attached to an **empty action slot** of a face on your die.

- **Example:** Venom Coating (Common), "Magic: apply Poison equal to my value". Attached to a Shield (Attack: damage, Defense: Fortify, Magic: empty), the Shield gains "Magic: apply Poison equal to my value".
- **Uses the host's value:** a module's effects use the value of the face it is attached to, so Value Is the Effect still holds and **upgrading the face upgrades its modules too**. This is where creative, game-breaking combinations come from: a module on a heavily upgraded face.
- **Empty slots only:** every face has three action slots (Attack, Defense, Magic). A slot is taken if the face itself or an earlier module has an effect for that action. This keeps faces readable (at most three actions) and turns "which face's empty slot gets which module?" into a decision.
- **Permanent:** modules can't be removed for now. Buying a face into a slot replaces the old face together with its modules, so a moduled face is costly to swap out.
- **An option, not an extra effect:** one roll still means one action. A module adds a new choice to a face, so its worth is "the module's action minus the face's best action": high on a face whose own actions are weak or off-build, low on a face that already does the job.
- **Hybrids:** modules connect builds. A Sword with Venom Coating bleeds on Attack and poisons on Magic.
- **Starting die:** most starter faces have empty slots (the Health Potion has empty Attack and Defense; Sword and Shield have an empty Magic slot), so modules can turn even the starting die into a build.
- **Rarity** follows the same scaling rule as faces (linear Common, conditional or multi-effect Rare, multiplicative Epic).
- **Price:** same as faces (Common 10, Rare 25, Epic 60). A module adds an action without costing a face, so it should not be cheaper.
- **Market:** the Normal Market offers 2 modules in their own row, next to the 3 face offers. Rerolls replace them too. The Grand Market stays Epic faces only for now.
- **Technical:** modules are stored on the `DieFace` instance, not on the `DieFaceSO` asset (the Definition / Instance split from Day 15), so only that copy of the face changes.

| Module | Rarity | Action | Build |
|---|---|---|---|
| **Venom Coating** | Common | Magic: apply Poison equal to my value | Poison |
| **Kindling** | Common | Magic: apply Burn equal to my value | Burn |
| **Rend** | Common | Magic: apply Bleed equal to my value | Bleed |
| **Iron Skin** | Common | Defense: gain block equal to my value | Shield / general |
| **War Cry** | Rare | Magic: gain Rage equal to my value | Berserker (Rare because Rage is permanent and a high-value host makes it strong) |
| **Ignition** | Rare | Attack: ignite the enemy's Burn | Burn (turns any face into an igniter) |

### Prices

Prices are placeholders, to be tuned through play-testing.

- **Gold income:** 20 gold per victory, plus a 30 gold bonus for boss victories.
- **Buy price by rarity:** Common 10, Rare 25, Epic 60. With current income, an Epic costs about three victories of saving, while Common and Rare faces are cheap enough to keep the build growing.
- **Upgrade price:** rarity multiplier × current value, so each upgrade costs more than the last.

| Rarity | Multiplier | 2 → 3 | 3 → 4 |
|---|---|---|---|
| Common | ×3 | 6 | 9 |
| Rare | ×5 | 10 | 15 |
| Epic | ×20 | 40 | 60 |

- **Soft cap, not a hard cap:** upgrades are unlimited, because game-breaking runs are part of the power fantasy. Every face has a soft cap; from the soft cap on, each level costs 2× the normal formula, then 4×, 8×... Breaking past the cap is possible but takes a run's worth of gold:
  - Ember (Common, cap 6): 6 → 7 costs 36 gold, 7 → 8 costs 84.
  - Fuel (Epic, cap 3): 3 → 4 costs 120 gold, 4 → 5 costs 320 (a run earns about 200).
  - Current soft cap rule: Common base + 3, Rare base + 2, Epic base + 1.

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
- **Three slots.** Equipment has no slot types (no helmet or armor slots); any item fits any slot. With a fourth item, you choose which one to give up, the same decision as replacing a die face.
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
- **No chains:** an effect run by a trigger does not fire triggers. Barbed Hook's extra hit does not fire Smoldering Gauntlet or Barbed Hook again, so items can never loop.
- **One place for the turn steps:** the battle states and the balance simulator call the same turn steps, so every trigger fires at the same point in both.
- **Future triggers:** "when you apply a status", "when the enemy dies to a tick", die manipulation ("reroll once per battle").

### Getting Equipment

- **Boss reward:** after a boss, choose 1 of 3 items for free.
- **Grand Market:** sells 2 items at 30 gold, next to the Epic faces ("an item or an Epic face?").
- Two bosses per run give at least two items; filling the third slot costs gold.

### First Set

| Item | Build | Trigger | Effect |
|---|---|---|---|
| **Plague Mask** | Poison | FaceRolled (Poison face) | Apply 2 Poison |
| **Smoldering Gauntlet** | Burn | HitDealt | Apply 1 Burn |
| **Barbed Hook** | Bleed | HitDealt | If the enemy is bleeding, hit again for 1 |
| **Tower Helm** | Shield | ActionChosen (Defense) | Gain 1 Fortify |
| **Berserker Torc** | Rage | HitTaken | Gain 1 Rage |
| **Bone Fetish** | Ritual | BattleStart | Gain 1 Ritual |
| **Laurel Wreath** | General | BattleWon | Heal 4 |
| **Iron Bracers** | General | TurnStart | Gain 1 block |

- **Bridges through existing rules:** Rage adds to Barbed Hook's extra hit (Bleed + Rage), Ritual adds to Plague Mask's Poison (Ritual + Poison), and Smoldering Gauntlet fires on every hit of a multi-hit (Twin Axes + Burn).
- **Plague Mask scales with the die:** the more Poison faces on the die, the more often it fires.
- **Berserker Torc is the risky one** (Pillar 4): it grows from damage taken, and Rage is permanent. Its numbers need the closest watch.
- **Laurel Wreath** is worth the most in long runs, since HP carries over between battles.
- Numbers are placeholders, to be tuned through simulation.

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

- Damage over time (Poison, Bleed) bypasses Block and Fortify.
- **Three distinct roles:** Burn is fuel (stored, then ignited or detonated), Poison is front-loaded attrition (big early ticks that halve), Bleed is duration (always 1, for as many turns as it has stacks).
- **Ignite:** deal damage equal to the target's Burn without consuming it. Detonate consumes Burn for one bigger hit. Small ignitions versus waiting for one big detonation is the Burn build's core decision.
- **Rage and Ritual** are visible statuses, not hidden multipliers, so they follow the Value Is the Effect rule: the face's value stays as printed, and previews show the boosted result (e.g. a Sword with 3 Rage previews "5 damage, 1 Bleed").
- **Ritual only boosts statuses applied to the opponent** (Burn, Poison, Bleed). Boosting self-applied statuses would break the "Fortify comes in small amounts" rule. Multiplying a status (Fuel) is not a new application, so Ritual does not apply to it.
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
| Fuel | Payoff (multiply Burn) | Epic |
| Cauterize | Payoff (consume Burn to heal) | Epic |

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
| **Plague** | Payoff | Epic | 1 | Magic: apply Poison equal to my value for each Poison face on your die. Attack: apply Poison equal to my value. |

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
| **Unleash** | Payoff | Epic | 2 | Attack: deal my value × your Rage as damage (at least my value). |

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
| **Dark Communion** | Payoff | Epic | 1 | Magic: gain Ritual equal to my value for each status type (Burn, Poison, Bleed) on the enemy, at least my value. |

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
| **Exsanguinate** | Payoff | Epic | 1 | Attack: consume the enemy's Bleed and deal my value × the consumed Bleed as damage (at least my value). |

- **Bleed timing:** Bleed applied on your turn ticks at the end of the enemy's turn, so N Bleed keeps the enemy marked for N − 1 of your turns. Bleed sources therefore apply at least 2 (Sword 2, Serrated Blade 4, Blood Rite 3, Witch's Curse 2); 1 Bleed would never reach your next turn.
- **Lacerate keeps Bleed a mark:** Bleed still deals its 1 per tick; Lacerate hits harder the longer the enemy stays marked. Unlike Exsanguinate it consumes nothing and adds instead of multiplying, which keeps it a Common.
- **Serrated Blade is a conditional Common on purpose:** conditions are normally Rare, but Bleed's whole identity is "if the enemy is bleeding", so its Starter has one. When it only applied Bleed, its rolls dealt no damage (Sword already keeps the mark up, so the extra duration was mostly wasted) and the Bleed bot cleared 0–8% of runs. With the hit and value 4 it clears about 27%. Its own Bleed comes after the check, so on an unmarked enemy it marks first and hits on later rolls.
- **Conditional faces keep a floor:** without Bleed, Open Wound still hits once and Bloodscent still blocks (Standalone Faces rule).
- **Exsanguinate vs. Detonate:** Detonate is additive (value + Burn), Exsanguinate is multiplicative (value × Bleed) and consumes the whole remaining duration. "Keep the mark for the conditional effects, or cash it in now?" is the Bleed build's core decision.
- **The life loop (Bleed + Berserker bridge):** Berserker uses HP as a resource. Bloodrage spends HP for Rage, and Bloodscent heals it back while the enemy bleeds: lose HP, apply strong effects, heal back. Open Wound's second hit also benefits from Rage. Bloodscent's healing is kept as is for now and will be revisited if it proves too strong.
- **"If" effects are one reusable box** (`ConditionalEffectSO`), so future conditions ("if the enemy is poisoned", "if you have Fortify") need no new code.

### Future Archetypes

Ideas to revisit after Poison and Shield.

- **Hammer:** Pour power into one face and set up the turn it lands for a one-turn kill. Uses in-battle growth instead of permanent upgrades, so it doesn't depend on expensive upgrades past the soft cap:
  - **Charge:** the face gains +1 temporary value every turn it isn't rolled.
  - **Die manipulation:** reroll the die, or trigger your highest-value face.
  - Needs changes to the battle state machine.
- **Crowd Favor (cross-build mechanic):** Showy plays (overkill, a turn without block, consecutive attacks) earn Favor from the arena crowd, which turns into bonus gold after the battle. Connects combat to the economy.
- **Die topology:** Face positions on the die matter, e.g. opposite faces (1–6, 2–5, 3–4) or adjacent faces. Turns "which face do I replace?" into "which face, in which slot?". Complex, so it stays a late idea.

### Hybrid Builds

Hybrids add variety and give players a way to pivot when they can't find the faces they want, instead of giving up on the run.

- **Bridge faces** carry two archetype tags (e.g. Flame Ward: Burn + Block, Toxic Blade: Poison + Physical) and naturally connect two builds.
- **Die-composition payoffs** can be written for any tag, or even reward variety (e.g. "for each different tag on your die").
- **Tag weighting stays around 60%** so about 40% of offers come from outside the current build and pivoting stays possible.

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
| **Retiarius** (net and trident) | 18 | Jab 2 (×2), Net: block 3 (×1), Trident Thrust 5 (×1) | **Telegraphed spike:** ignore the intent and take 5 |
| **Dimachaerus** (two swords) | 17 | Twin Strike: 2 × 2 damage (×2), Parry: block 2 (×1) | **Multi-hit:** each hit wears down block separately, but each hit also triggers FireShield |
| **Arena Lion** | 20 | Maul 1 (×2), Roar: gain 1 Rage (×1), Feast: 1 damage and heal by the damage (×1) | **Builds Rage:** harmless at first, deadly if the fight drags on |
| **Venefica** (poisoner) | 17 | Poison Dart: Poison 2 (×2), Strike 2 (×1), Smoke: block 2 (×1) | **Poisons the player:** repeated darts keep Poison ticking, so long fights cost HP |
| **Secutor** (pursuer) | 20 | Brace: Fortify 1 (×2), Shield Slam 4 (×2) | **Stacks Fortify:** its protection slowly builds up, so deal damage early |

**Strong (battles 9–10), the build check:**

| Enemy | HP | Intents | Clock |
|---|---|---|---|
| **Essedarius** (chariot fighter) | 42 | Ride Down 2 (×2), Wheel Spikes 2 (×1), Rein In: block 3 (×1), Battle Cry: 1 Rage (×1) | **Gathering speed:** builds Rage with Battle Cry |
| **War Elephant** | 56 | Stomp 6 (×1), Trunk Swat 2 (×2), Thick Hide: Fortify 1 (×1) | **Huge HP pool** with a telegraphed crushing blow, a big target for combos |
| **Hoplomachus Champion** | 44 | Spear Thrust 3 (×2), Shield Rush: 2 damage and block 2 (×1), Battle Cry: gain 1 Rage (×1) | **Rage:** each Battle Cry permanently raises all its damage |
| **Bestiarius** (beast tamer) | 40 | Whip 2 (×2), Unleash 5 (×1), Feed the Beast: heal 2 (×1) | **Healing plus spikes:** a weak build can't out-damage its healing |

#### New Mechanics for Enemies

- **Multi-hit damage** (Dimachaerus): one effect that deals damage N times. Player faces can use it later too.
- **Escalation through Rage** (Essedarius, Hoplomachus, Arena Lion): rising damage uses the same visible Rage status as the player. An enemy can gain Rage from an intent (Battle Cry, Roar), which the player sees coming. Rage every turn (`escalationPerTurn`) is still supported but no enemy uses it: on Essedarius it became a wall that slow builds could not get past (Essedarius alone caused about half of the Bleed, Rage and Shield deaths), so it was replaced with Battle Cry. Its intents show the boosted numbers, and the Rage chip's tooltip explains why.

#### Balance Targets

Enemy numbers are tuned with the run simulator (`Tools → Gladiator Dice → Run Balance Simulation`), where bots fight with the real battle rules and shop in the real Market. The difficulty curve is measured against these targets:

| Target | Result (600–1000 runs per bot) |
|---|---|
| An unbuilt die (no shopping) gets through the Weak tier losing about 10–15 HP | 11 HP lost, then it dies in the Medium tier (28% reach battle 8) |
| A die bought without a plan stalls around battles 9–10 | 71% reach battle 8, 11% reach battle 9, 0% clear the run |
| A well-built die mostly beats the Strong tier | Poison 63%, Burn 38% clear the run; Shield, Berserker and Ritualist about 0% |

Shield misses the target because its strong-tier fights take far longer than other builds' (about 24 turns versus 6), so enemies that build Rage win the race. This is an archetype problem, not an enemy one.

These results use the Run Start die (2 Sword, 2 Shield, Health Potion and a chosen starting face) and a 23-face Market pool. Adding the Berserker and Ritualist faces diluted the pool, so focused Burn and Poison bots find their faces less often. Berserker and Ritualist are not tuned yet, and the focused bots only buy faces of one tag, which doesn't suit Ritual (a hybrid glue). These archetypes need a balance pass and a hybrid-aware bot.

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

- Exact Petrify duration (2 or 3 turns).
- Healing amount after a boss (50% of max HP is a placeholder).