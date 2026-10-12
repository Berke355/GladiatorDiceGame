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

- **Normal Market:** 3 face offers, with rarity weights Common 70%, Rare 25%, Epic 5%. Epic faces can appear, but rarely.
- **Grand Market (after a boss):** 3 offers, **all Epic**. The Grand Market is the station for Epic faces. Because Grand Markets come at known points (after bosses), saving gold for one is a plannable strategy, not a gamble.
- A face is offered at most once per visit.
- **Build-aware offers:** offers are weighted toward the tags already on the player's die (around 60%, not 100%). A Burn die is more likely to be offered Burn faces such as Fuel, while some offers still come from outside the build so players keep discovering new faces.

Market services:

- **Buy a face:** the new face replaces one face of the die, chosen by the player.
- **Upgrade a face:** raise one die face's value by 1, up to that face's Max Value. The Normal Market upgrades Common and Rare faces; **Epic faces can only be upgraded at the Grand Market**.

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

- **Max Value:** every face has an upgrade cap. Prices make upgrading Epic faces expensive, and the cap rules out extreme cases (e.g. Fuel capped at 3, so it can never multiply Burn by 4 or more). Current placeholder rule: Common base + 3, Rare base + 2, Epic base + 1.

### Save vs. Tempo

The decision only works if both paths are viable.

- **Spending now** makes the die stronger in the next battles, so the player wins faster and loses less HP.
- **Saving** keeps gold for an Epic face and its upgrades at the next Grand Market. Since **HP carries over between battles**, fighting with a weaker die costs HP that the player has to survive without.
- No interest mechanic for now. If play-testing shows that saving is never worth it, rewarding saved gold (e.g. interest) is the first thing to revisit.

### Open Questions

- Gold reward for boss victories.

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