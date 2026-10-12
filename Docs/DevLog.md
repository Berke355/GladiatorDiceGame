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

## Day 8 - Enemy AI & Decoupled Architecture

### Accomplished
- **Enemy Intent System:** Implemented a Slay the Spire style intent system. Created `EnemyBrain` to manage and randomly select from a pool of `EnemyIntent`s.
- **UI Intent Display:** The UIManager now displays the enemy's planned action (e.g., Attack 5, Guard 3) at the very start of the turn (during `PlayerRollState`), giving the player tactical foresight.
- **Universal EffectSO Architecture:** Refactored `EffectSO.Execute` to use `source` and `target` instead of hardcoded `player` and `enemy`. This decoupled polymorphism allows both players and enemies to seamlessly use the exact same logic Legos (e.g., `DamageEffectSO`, `BlockEffectSO`) without duplicating scripts or adding if-statements.
- **Deckbuilder Block Rules:** Fixed the turn-based Block reset logic. A character's block is now correctly reset at the *start of their own turn* (`PlayerRollState` for player, `EnemyTurnState` for enemy).

### Next Steps
- **Status Effects (Phase 3 Continued):** Implement a robust status effect system in `Entity.cs` to handle Damage over Time (Poison, Bleed) and Buffs/Debuffs.

## Day 9 - Status Engine & Burn Build Mechanics

### Accomplished
- **Status Engine:** `Entity` now stores statuses in a `Dictionary<StatusType, int>` with `ApplyStatus`, `GetStatus` and `ConsumeStatus`. Status changes raise `OnHealthChanged` so the UI stays in sync.
- **Turn Hooks:** Added `OnTurnStart` (reset block, clear FireShield) and `OnTurnEnd` (Burn tick) to `Entity`, called by the battle states for both sides.
- **Burn & FireShield:** Burn deals its stacks as damage at the end of the owner's turn and decays by 2. FireShield applies Burn to anyone who hits its owner.
- **Burn Build Effects:** Added `ApplyStatusEffectSO`, `ConsumeStatusDamageEffectSO` (consume a status for bonus damage), `ConsumeStatusHealEffectSO` (consume a status to heal) and `MultiplyStatusEffectSO` (e.g. double the target's Burn).
- **Status UI:** `UIManager` shows active Burn and FireShield stacks for both entities.
- Removed the per-effect `multiplier` from `ActionEffect`; every effect now receives the face's `baseValue` directly.

### Decisions
- **Value is the effect:** The number printed on a face is exactly what its effects use. With multipliers, a face showing 4 could really deal 8, and players would learn to ignore the big number on the face. Faces differ through which actions and effects they support, not through hidden scaling.

### Next Steps
- Create effect and face assets for the Burn build to test it in the scene.
- Handle battle end (win/lose) instead of letting the loop run on.

## Day 10 - Combat Rule Fixes & Battle End

### Accomplished
- **FireShield Fix:** `DamageEffectSO` and `VampireEffectSO` now pass `source` to `TakeDamage`, so regular attacks trigger FireShield.
- **Burn Decay Fix:** Burn added since the last tick no longer decays on that tick, even when it was applied during the opponent's turn. The flag is now reset after the tick instead of at turn start.
- **Burn Ignores Block:** Burn ticks reduce HP directly through a private `LoseHP` helper instead of `TakeDamage`.
- **Battle End:** Added `Entity.IsDead`, `BattleManager.CheckBattleOver()` and `BattleOverState`. The check runs after every action and every end-of-turn tick, so the loop stops when either side dies (including deaths from Burn).
- **Face Data Cleanup:** Rewrote the Sword, Shield and Bite descriptions to match the value-based rules and removed the leftover `multiplier` fields from the face assets.

### Decisions
- Damage over Time bypasses block, so Block counters direct attacks but not Burn.

### Next Steps
- Sword and Shield are now mechanically identical (Attack: damage, Defense: block). Give them distinct identities.
- Create Burn build assets and test the build in play mode.
- Show a win/lose screen and start the Gold → Market loop.

## Day 11 - Burn Build Faces

### Accomplished
- **Self-targeted Statuses:** Added `applyToSelf` to `ApplyStatusEffectSO` so a face can apply a status to its user (e.g. FireShield on the player).
- **Burn Effect Assets:** Created `ApplyBurnEffect`, `ApplyFireShieldSelfEffect`, `MultiplyBurnEffect`, `ConsumeBurnDamageEffect` and `ConsumeBurnHealEffect`.
- **Burn Faces:**
  - **Ember (3):** Attack: apply Burn. Defense: block. The starter that seeds Burn.
  - **Fuel (2):** Magic: multiply the enemy's Burn. Defense: block as a fallback when there is no Burn.
  - **Detonate (2):** Attack: consume Burn for value + stacks damage. Magic: apply Burn. Cash out or keep building.
  - **Cauterize (1):** Magic: consume Burn and heal value × stacks. Attack: damage.
  - **Flame Ward (2):** Defense: block and FireShield on self, so attackers get burned.
- **Test Die:** 2× Ember, Fuel, Detonate, Cauterize, Flame Ward. Raised test HP to Player 25 / Enemy 30 so Burn has time to play out.

### Decisions
- **Every face keeps a fallback action** so a bad roll is never a fully wasted turn (Day 2 decision).
- **Consume vs. let it tick:** Burn deals damage every turn but decays; consuming it gives one big hit or heal and resets the stacks. This is the core decision of the build.
- Cauterize stays at value 1 so its heal equals the consumed stacks.

### Next Steps
- Play-test the Burn die and tune values.
- Watch Fuel's scaling: its value is the multiplier, so upgrading it would grow very fast. Consider keeping it fixed at 2.
- Multiplying Burn counts as adding Burn, so Fuel also skips the next decay. Keep it as a feature or change it.
- Sword and Shield are still identical. Give them distinct identities.
- Show a win/lose screen and start the Gold → Market loop.

## Day 12 - Battle Result Screen

### Accomplished
- **Result Screen:** Added `BattleResultPanel` to the Canvas: a full-screen dark overlay with a large "VICTORY" / "DEFEAT" text and a Restart button. The overlay blocks clicks on the battle UI behind it.
- **UI Hookup:** `BattleOverState` calls `UIManager.ShowBattleResult(playerWon)` when the battle ends.
- **Restart:** `BattleManager.RestartBattle()` reloads the active scene to start a fresh battle.
- **Singleton Safety:** `UIManager` clears its `Instance` in `OnDestroy`, so reloading the scene never leaves a stale reference.
- Play-tested win (a full Burn-die fight), loss and restart. Input after the battle ends is ignored.

### Decisions
- Restart reloads the scene instead of resetting state by hand. It is simple and reliable for a single-battle prototype, and will be replaced when battles chain into a run.

### Next Steps
- Start the Gold → Market loop: reward gold on victory and replace Restart with "Continue" to the Market.
- Keep the player's die between battles (it currently lives on the scene's `BattleManager`).
- Fix the player health bar fill, which renders as a solid gray block.
- Play-test the Burn die and tune values. Decide on Fuel's scaling and its decay-skip side effect.
- Sword and Shield are still identical. Give them distinct identities.

## Day 13 - Run Architecture & Market Scene

### Accomplished
- **RunData:** A plain `[Serializable]` C# class holding everything that lives for a whole run: the die, max/current HP, gold and battles won.
- **RunManager:** A `DontDestroyOnLoad` singleton that owns `RunData` and drives scene flow (`GoToMarket`, `GoToNextBattle`, `RestartRun`).
- **Bootstrap:** `RunManager` is not placed in any scene. A `[RuntimeInitializeOnLoadMethod(BeforeSceneLoad)]` method instantiates `Resources/RunManager.prefab` before the first scene loads, so any scene can be played directly from the Editor and duplicates can't happen. The starting die, starting HP (25) and gold reward (10) are set on the prefab.
- **Battle ↔ Run:** `BattleManager` borrows the die and the player's HP from the run in `Awake`. `BattleOverState` reports the result to `RunManager.OnBattleEnded`, which stores HP and awards gold.
- **Result Screen:** Victory shows "+10 Gold" and a Continue button to the Market. Defeat shows New Run, which starts a fresh run.
- **Market Scene:** A new placeholder `Market` scene (`MarketUI`) shows gold, HP and battles won, plus a Next Battle button.
- Renamed `SampleScene` to `Battle` (GUID kept). Both scenes are in Build Settings.
- Play-tested the full loop: win → Market → next battle with the same HP → lose → New Run with a reset run. Also tested starting Play Mode directly from the Market scene.

### Decisions
- **Data lifetimes:** Design data (faces, effects) stays in ScriptableObjects. Run data lives in `RunData`, owned by `RunManager`. Battle data stays in the Battle scene. Mutable run state is never stored in an SO asset, because Play Mode changes to assets persist in the Editor but not in builds.
- **DontDestroyOnLoad manager + separate data class:** Chosen over ScriptableObject run data (the Editor persistence trap), static classes (invisible in the Inspector) and a single scene (manual resets, harder to grow). Keeping `RunData` as plain data makes JSON saves and tests easy later.
- **Dependency direction:** Battle doesn't know about the Market. It only borrows from the run and reports its result, and `RunManager` decides what comes next.
- **HP carries over between battles**, so HP becomes a run-long resource (Slay the Spire style).
- **The Market is a separate scene.**

### Next Steps
- Build the Market: offer a limited random selection of die faces with prices, buy with gold, and pick which face of the die to replace.
- Add `Rarity` and `Price` to `DieFaceSO` (from the GDD face structure).
- Scale enemies with `battlesWon`, or define an encounter list, so later battles get harder.
- Show gold and the battle number during battle.
- Fix the player health bar fill, which renders as a solid gray block.
- Play-test the Burn die and tune values. Decide on Fuel's scaling and its decay-skip side effect.
- Sword and Shield are still identical. Give them distinct identities.

## Day 14 - Rarity & Economy Design

### Accomplished
- Wrote the **Rarity & Economy** section of the GDD: goals, rarity tiers, run structure, Market and Grand Market, prices and upgrades, and the save vs. tempo decision.
- Added the **Value Is the Effect** rule and the **Max Value** property to the GDD's die face section.

### Design Challenge
Some faces become far stronger than others when their value goes up (e.g. Fuel multiplies Burn by its value). These faces should be rare and expensive, but never pure luck: most runs should still find the key face of their build. Gold should also create a real decision: save for a powerful late-game face, or spend now for tempo.

### Decisions
- **Rarity comes from scaling, not raw power:** Common = linear, Rare = conditional or multi-effect, Epic = multiplicative. Fuel and Cauterize are Epic.
- **Run structure:** about 10–11 battles, a boss every 3–5 battles, and a final boss that ends the run.
- **Grand Market after bosses,** with at least one guaranteed Epic. Its timing is predictable, so saving gold for it is a plan rather than a gamble. This was chosen over a hidden pity timer, which keeps players from planning.
- **Build-aware offers:** offers are weighted (around 60%) toward the tags already on the die, so players can find their build's key face while still discovering new ones.
- **Upgrading is a separate Market service.** Cost = rarity multiplier (Common ×3, Rare ×5, Epic ×10) × current value, and every face has a Max Value cap.
- **Placeholder prices:** 10 gold per victory; buy prices Common 8, Rare 15, Epic 30.
- **No interest mechanic for now.** Carrying HP over between battles already makes saving costly. Interest is the first thing to revisit if saving never pays off.

### Next Steps
- Add `rarity`, `maxValue` and `tags` to `DieFaceSO`, with price derived from rarity, and set them on the existing faces.
- Build the Market: random offers, buying a face and choosing which die face it replaces, and the upgrade service.
- Add boss battles and the Grand Market to the run structure.
- Resolve the GDD's open questions (Normal Market Epic chance, offers per Market, boss gold reward, Max Values).
- Scale enemies over the run, show gold and the battle number during battle, and fix the player health bar fill.
- Play-test the Burn die. Sword and Shield are still identical.
