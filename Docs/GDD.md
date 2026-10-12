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

### Market

Each visit offers a limited, random selection. The player is never forced to spend.

- **Normal Market:** a few face offers. Epic faces can appear, but rarely.
- **Grand Market (after a boss):** at least **one Epic face is guaranteed**. Because Grand Markets come at known points (after bosses), saving gold for one is a plannable strategy, not a gamble.
- **Build-aware offers:** offers are weighted toward the tags already on the player's die (around 60%, not 100%). A Burn die is more likely to be offered Burn faces such as Fuel, while some offers still come from outside the build so players keep discovering new faces.

Market services:

- **Buy a face:** the new face replaces one face of the die, chosen by the player.
- **Upgrade a face:** raise one die face's value by 1, up to that face's Max Value.

### Prices

Prices are placeholders, to be tuned through play-testing.

- **Gold income:** 10 gold per victory.
- **Buy price by rarity:** Common 8, Rare 15, Epic 30. With current income, an Epic costs about three victories of saving.
- **Upgrade price:** rarity multiplier × current value, so each upgrade costs more than the last.

| Rarity | Multiplier | 2 → 3 | 3 → 4 |
|---|---|---|---|
| Common | ×3 | 6 | 9 |
| Rare | ×5 | 10 | 15 |
| Epic | ×10 | 20 | 30 |

- **Max Value:** every face has an upgrade cap. Prices make upgrading Epic faces expensive, and the cap rules out extreme cases (e.g. Fuel capped at 3, so it can never multiply Burn by 4 or more).

### Save vs. Tempo

The decision only works if both paths are viable.

- **Spending now** makes the die stronger in the next battles, so the player wins faster and loses less HP.
- **Saving** keeps gold for an Epic face and its upgrades at the next Grand Market. Since **HP carries over between battles**, fighting with a weaker die costs HP that the player has to survive without.
- No interest mechanic for now. If play-testing shows that saving is never worth it, rewarding saved gold (e.g. interest) is the first thing to revisit.

### Open Questions

- Epic chance in a Normal Market.
- Number of offers per Market and Grand Market.
- Gold reward for boss victories.
- Max Value for each existing face.