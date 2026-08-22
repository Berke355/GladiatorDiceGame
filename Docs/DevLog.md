# Dev Log

## Day 1 - Project Initialization

### Accomplished
- Created the GitHub repository.
- Set up the documentation structure.
- Wrote the High Concept.
- Defined the Design Pillars.

### Decisions
- The die is the player's progression system.
- Build crafting is the core gameplay.

### Next Steps
- Define the Dice System.

## Day 2 - Combat Design Discussion

### Accomplished

- Continued defining the core gameplay systems.
- Finalized the overall Dice System Philosophy.
- Defined the anatomy of a die face.
- Started designing the first build archetype (Burn).

### Design Challenge

While discussing combat, we identified an important design problem.

The original idea was:

Choose an action → Roll the die.

This created situations where the rolled face could not interact with the chosen action, resulting in turns where nothing meaningful happened.

Several solutions were discussed:

- Allow every die face to have effects for every action.
- Introduce a reroll system.
- Change the combat flow.

### Decision

For the first prototype, the combat flow will be:

Roll the die → Choose an action.

Each die face can support multiple actions with different effectiveness, allowing the player to make meaningful decisions after seeing the rolled face while avoiding completely wasted turns.

This decision will be validated through gameplay prototyping rather than theory alone.

### Next Steps

- Build the first playable combat prototype.
- Test whether the new combat flow feels engaging.
- Continue designing the Build Archetypes.

## Day 3 - Battle State Machine & Core Loop

### Accomplished
- Implemented the core State Machine architecture for the combat loop.
- Created `BattleManager` as the central state controller.
- Developed `BattleState` abstract base class and concrete states (`PlayerRollState`, `PlayerActionSelectState`).
- Integrated `DieFaceSO` data array into the `BattleManager`.
- Implemented random dice rolling logic triggering on player input (Spacebar).
- Successfully managed state transitions using dependency injection (Constructor passing).

### Decisions
- Dice rolling logic is placed inside the `Execute` method of `PlayerRollState`, checking for explicit player input rather than rolling automatically upon entering the state. This increases game feel and interactivity.

### Next Steps
- Pass the rolled die face data from `PlayerRollState` to `PlayerActionSelectState`.
- Implement the Action Selection mechanics (Attack, Defense, Magic).
- Calculate and apply damage/effects based on the rolled face and chosen action.

## Day 4 - Action Selection & Effect Execution

### Accomplished
- Implemented `PlayerActionSelectState` to handle player input for actions.
- Shared state data between states by storing the `currentRolledFace` inside `BattleManager`.
- Mapped keyboard inputs (1, 2, 3) to `ActionType` enums (Attack, Defense, Magic).
- Created the effect execution logic that filters and applies the correct `ActionEffect` from the rolled face.
- Closed the core combat loop by transitioning back to the rolling state.

### Next Steps
- Introduce a basic Entity system with Health (HP) for Player and Enemy.
- Implement the `EnemyTurnState`.
- Apply actual damage/defense values to entities instead of using Debug.Log.

## Day 5 - Entity System & Combat Loop Finalization

### Accomplished
- Created the core `Entity` class to manage Health (HP) and temporary Block (armor) for both Player and Enemy.
- Implemented robust damage calculation logic where Block prioritizes damage mitigation before HP reduction.
- Implemented `EnemyTurnState` to automatically deal damage to the player and complete the turn cycle.
- Adjusted the Block reset mechanic to trigger *after* the enemy attacks, ensuring logical defensive gameplay.
- Added death checking logic (`currentHP <= 0`) and object destruction.
- Prevented the "attack from the grave" bug by ensuring the game only transitions to the enemy turn if the enemy is still alive.
- **Phase 1 Prototype is officially complete!** The core mechanical loop (Roll Die -> Select Action -> Apply Effect -> Enemy Turn -> Repeat) is fully functional and tested via the console.

### Next Steps (Phase 2)
- Implement a User Interface (UI) to visualize the Die, Action Buttons, Health, and Block.
- Create new `DieFaceSO` assets with complex mechanics (e.g., Stun, Burn) to test build synergies.
- Begin laying the foundation for the Market/Shop system to upgrade dice faces.

## Day 6 - Advanced Effects Architecture (Build Crafting Foundation)

### Accomplished
- **Polymorphism Implemented:** Replaced hardcoded switch statements in `PlayerActionSelectState` with a robust Strategy Pattern using `EffectSO` (ScriptableObject).
- **Decoupled Mechanics:** Created `DamageEffectSO`, `BlockEffectSO`, and `VampireEffectSO` as independent logic blocks (Lego pieces) that can be plugged into any die face.
- **Base Value System:** Refactored `DieFaceSO` to hold a single `baseValue` and `ActionEffect` to use a `multiplier` instead of hardcoded raw values. This dramatically improves UI readability and game design consistency (e.g., 6 Damage vs 3 Block from a "Base 6" roll).
- **Vampire Mechanic:** Implemented a new `Heal` method in `Entity` (with over-heal protection and UI event triggers) and successfully tested the `VampireEffect` which deals damage and heals simultaneously.
- **UI Event Hookups Completed:** Ensured `OnHealthChanged` events trigger correctly for all health modifications, preventing tight coupling with `UIManager`.

### Next Steps (Phase 3)
- Add more advanced mechanics like `ShieldBashEffect` (Damage scaling with Block).
- Introduce Status Effects (DoT like Poison or Burn) and EndTurn phase processing.
- Build out the visual UI (Buttons for actions instead of keyboard inputs, animated Health Bars).
- Begin designing the Roguelite progression loop (Map/Shop/Next Battle).

## Day 7 - Visual UI and Architecture Refinement

### Accomplished
- **Visual UI Integration:** Replaced keyboard-based combat inputs with interactive on-screen UI buttons (`RollTheDice`, `Attack`, `Defense`, `Magic`).
- **Health Bars:** Implemented visual Health Bars (Sliders) for both Player and Enemy to accurately track HP in real-time.
- **Dice Roll Display:** Added dynamic UI text to display the current rolled face, its base value, and description to improve testing clarity and UX.
- **State Machine Refactoring:** Removed legacy `Input.GetKeyDown` polling from `PlayerActionSelectState` and `PlayerRollState`. Input is now event-driven via `BattleManager` methods (`OnActionSelected`, `OnRolled`).
- **Singleton Pattern:** Refactored `UIManager` into a Singleton to optimize UI updates (e.g., `UpdateDiceText`) and eliminate the overhead of `FindFirstObjectByType`.

### Next Steps (Phase 3 Continued)
- **Enemy AI (Intent System):** Implement Slay the Spire style intent mechanics for the enemy using the existing `EffectSO` architecture.
- Create `EnemyBrain` and `EnemyIntent` structures.
- Display Enemy Intents on the UI before the player acts.
