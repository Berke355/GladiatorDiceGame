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
