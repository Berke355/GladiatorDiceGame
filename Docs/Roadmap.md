# Roadmap

The big steps after the core game (all archetypes, equipment, enchantments and the three bosses, DevLog Day 46). They come from the user's first play-tests: the game felt too easy, decisions felt shallow ("the game plays itself"), and a battle offered little choice beyond the rolled face's one or two actions.

Slay the Spire is the main reference for this phase. The order matters: the battle itself has to be fun first, since every later layer multiplies it.

## Phase A: Battle Depth

1. **Rerolls.** A pool of rerolls per battle (2 to start, refilled every battle). Rerolling replaces the rolled face; the new face must be used. Every turn becomes a risk decision: keep this face, or spend a reroll hoping for better and have none left for later. Content around it: an Epic item for +1 reroll, faces that reward unused rerolls or rerolling, a new role for Steady Hand.
2. **Reactive enemy intents** (inspired by Slay the Spire). Normal enemies get rules that react to the player's choices, so each turn is a small puzzle:
   - Reactions: gains Rage when hit (punishes multi-hits), thorns (hurts you when you Attack), punishes Defense, curls up and blocks on the first hit.
   - Mode shifts: changes behaviour after taking X damage.
   - Split: splits into two at half HP (a bridge to multiple enemies).
   - Charged attacks: wind-up turns, then a big hit (the Minotaur's Stampede already works like this).
3. **Difficulty and gold.** Bots decide far worse than people, so the old 20–35% bot clear target left the game easy for a player. New target: bots clear about 5–15%, so a good player clears about 40–50%. More gold per victory to match, with a cheaper upgrade curve if needed. (Upgrades already have no cap since Day 41.)

## Phase B: Run Structure

- **Map.** A branching path between battles (inspired by Slay the Spire) instead of battle → Market → battle:
  - Node types: normal battle, elite (harder, extra reward such as an Epic face or item), rest (heal or upgrade a face), event (risk/reward choices, e.g. pay HP for an enchantment), Market and boss.
  - The Market becomes a node, so the economy needs a new pass.
  - **Ideas from play-tests:**
    - Healing moves from the Market to rest nodes, so the route is a choice between healing and shopping.
    - The Enchanting Table may become its own node.
    - "?" event nodes with risk/reward choices, as in Slay the Spire.
    - The map shows what is coming, which replaces the Market's "next battle" line.

## Phase C: Multiple Enemies

- Battles against several enemies at once: target choice ("the one in front, or the dangerous one?"), area effects, an intent per enemy, bot targeting.
- The most expensive architecture change (the battle flow, targeting UI and effects all change), so it comes after the battle rules settle.

## Phase D: Heroes and Meta Progression

- **Heroes** with their own starting dice (and possibly a small passive). The current die (2 Sword, 2 Shield, Health Potion) is the **Knight**, the starting hero. Later: Archer, Necromancer, Robot, Pirate, Wizard...
  - Heroes exist for theme and play style, never power: no starting die may be stronger than another. Each hero gets a simulator bot, and they are compared in the same simulation.
- **Main menu** outside runs, with a **menu market**: money and achievements earned from runs unlock heroes, faces (added to the pools), outfits.
  - Unlocks add variety, not power (as in Slay the Spire): no permanent "+5 HP", which would bring the difficulty problem back.
- **Difficulty levels** (like Slay the Spire's Ascension): finishing a run unlocks the next level of difficulty modifiers. A lasting answer to "the game is too easy".
- **Technical prerequisite:** a save system for unlocks and meta money. `RunData` was designed to be saved as JSON from the start.

## Principle

Simulation can't measure how decisions feel. After each step, the user's play-tests decide the balance and the direction.
