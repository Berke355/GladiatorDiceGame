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

## Day 15 - Rarity Data & Die Face Instances

### Accomplished
- **Rarity & Tags:** Added the `Rarity` (Common, Rare, Epic) and `FaceTag` (Physical, Block, Lifesteal, Burn) enums. `DieFaceSO` now has `rarity`, `tags` and `maxValue`, and `maxValue` can never be set below `baseValue`.
- **Die Face Instances:** Added `DieFace`, a `DieFaceSO` definition plus this copy's own `value`. The run's die is now a `List<DieFace>`, and battle effects use the instance's value.
- **Economy Config:** Added `EconomyConfigSO` (`Assets/Config/EconomyConfig.asset`) with gold per victory, buy prices and upgrade multipliers, plus the `GetBuyPrice` and `GetUpgradeCost` formulas from the GDD. Moved `goldPerVictory` from `RunManager` into it.
- **Face Data:** Set rarity, tags and max value on all eight faces (Sword, Shield, Ember: Common; Bite, Detonate, Flame Ward: Rare; Fuel, Cauterize: Epic).
- Tested in Play Mode: upgrading one Ember leaves the other Ember and the asset unchanged, and the upgraded copy applies Burn equal to its own value.

### Decisions
- **Definition / Instance (Type Object pattern):** `DieFaceSO` describes what a face is and never changes at runtime. `DieFace` is a copy on the die with its own value. Upgrading the shared asset would upgrade every copy at once, and in the Editor the change would stay on disk after Play Mode.
- **Economy numbers live in one ScriptableObject.** They are design data, so an SO fits, and tuning them doesn't require touching code.
- **Max Value rule (placeholder):** Common base + 3, Rare base + 2, Epic base + 1, which keeps Fuel capped at 3.

### Next Steps
- Build the Market: generate offers (rarity chances, about 60% tag weighting), buy a face and choose which die face it replaces, and the upgrade service.
- Add boss battles and the Grand Market (guaranteed Epic) to the run structure.
- Resolve the remaining GDD open questions (Normal Market Epic chance, offers per Market, boss gold reward).
- Scale enemies over the run, show gold and the battle number during battle, and fix the player health bar fill.
- Play-test the Burn die. Sword and Shield are still identical.

## Day 16 - Market & Grand Market

### Accomplished
- **Market Rules (`Market`):** A plain C# class with no UI that generates offers, handles buying and handles upgrades.
  - **Normal Market:** 3 offers with rarity weights 70 / 25 / 5, weighted about 60% toward tags already on the die.
  - **Grand Market:** Epic offers only.
  - A face is offered at most once per visit.
  - **Buying:** the bought face replaces a die face chosen by the player, as a new `DieFace` instance.
  - **Upgrading:** the Normal Market upgrades Common and Rare faces, the Grand Market upgrades Epic faces. The Max Value cap and the rising upgrade cost apply.
- **Market Scene (`MarketUI`):** 3 offer cards (name, rarity, value and max, description, price), the 6 die faces with an upgrade button under each, a hint line, a Cancel button and Next Battle. Buying takes two steps: pick an offer, then pick the face it replaces. Other buttons are locked while choosing.
- **Run Structure:** `RunManager.bossBattles` (4 and 8) marks boss slots. Winning one opens the Grand Market.
- **Config:** `EconomyConfigSO` now holds the Market pool, offers per Market, rarity weights and the tag-match chance.
- **Tests:**
  - Tested the rules without a scene: over 2,000 Markets, rarity came out at 70.6 / 24.4 / 5.0 with no duplicate offers in a visit. Buying, upgrades and the Normal/Grand split all followed the rules.
  - Play-tested the scene by clicking through it: buy, replace, upgrade, cancel, Grand Market, and the next battle using the changed die.

### Decisions
- **Model / View split:** All Market rules live in `Market`, while `MarketUI` only draws its state and forwards clicks. The rules can be tested without a scene, and the UI can be rebuilt without touching them.
- **Grand Market:** 3 offers, all Epic. It is the station for buying and upgrading Epic faces, and Epic faces can only be upgraded there.
- **Normal Market:** 3 offers using the rarity weights 70 / 25 / 5.
- **No duplicate offers:** when the pool runs out (only 2 Epic faces exist today), the Market shows fewer offers instead of repeating a face.
- Offer and slot buttons are wired in code with `AddListener` because each needs its own index. Single buttons stay wired in the Inspector.

### Next Steps
- Design at least one more Epic face so the Grand Market can show 3 offers, and more faces overall so tag weighting reaches its 60% target.
- Center the offer cards when a Market has fewer than 3 offers.
- Add boss enemies, the final boss that ends the run (about 11 battles), and a boss gold reward.
- Scale enemies over the run, show gold and the battle number during battle, and fix the player health bar fill.
- Play-test the full loop and tune prices and values. Sword and Shield are still identical.

## Day 17 - Build Archetypes Design

### Accomplished
- Wrote the **Build Archetypes** section of the GDD:
  - **Standalone Faces, Build Potential** principle, with a test for it
  - the Starter → Engine/Bridge → Payoff archetype template
  - Burn documented as the reference archetype
  - full face designs for Poison and Shield
  - Future Archetypes, Hybrid Builds and open questions
- Updated the Rarity & Economy section with later decisions: the Grand Market offers 3 Epics, Epic faces can only be upgraded at the Grand Market, the 70/25/5 rarity weights, and the Max Value rule. Removed open questions that are now answered.

### Design Challenge
The Grand Market needs at least 3 Epic faces, and Epic faces need archetypes to be the payoff of. New archetypes also have to stay clearly different from Burn and keep runs flexible when the player can't find the faces they want.

### Decisions
- **Every face works on its own; its build unlocks its real potential.** Each face has a useful floor without its build (a fallback action or a useful minimum such as "value + X × value") and a much higher ceiling inside it.
- **Archetype template:** Starter (Common) produces the resource, Engine/Bridge (Rare) grows it or connects builds, Payoff (Epic) turns it into results multiplicatively. Rarity follows from the role.
- **Poison:** never decays and is never consumed. It is attrition, while Burn is burst, which makes Poison the boss-fighting build. Faces: Venom (Common), Toxic Blade and Miasma (Rare bridges), Plague (Epic).
- **Die-composition scaling:** a new Epic pattern ("for each Poison face on your die") that directly rewards building the die around an archetype.
- **Shield:** uses a new **Fortify** status, a block that doesn't reset at turn start. Cashing it out deals heavy damage but drops your stored protection. Faces: Shield reworked as the Common starter, Shield Bash (Rare), Shield Burst (Epic).
- **Reworking Shield** into the Shield starter ends the Sword/Shield duplication.
- **Future archetypes:** Hammer (Charge and die manipulation instead of breaking Max Value), Berserker, Crowd Favor and die topology are recorded for later.

### Next Steps
- Implement Poison: the status, its UI text, the Venom, Toxic Blade and Miasma faces, and Plague's die-composition scaling (effects can't see the die yet, so they need a way to get that information).
- Implement Shield: the Fortify status, the Shield rework, Shield Bash and Shield Burst.
- Add the new faces to the Market pool and check that the Grand Market shows 3 Epic offers.
- Center the offer cards when a Market has fewer than 3 offers.
- Add boss enemies, the final boss and a boss gold reward. Scale enemies over the run.

## Day 18 - Poison Archetype & EffectContext

### Accomplished
- **Poison Status:** Ticks at the end of the poisoned entity's turn, bypasses block and never decays. Shown in the battle UI.
- **Poison Faces:**
  - **Venom** (Common): Attack applies Poison, Defense gives block.
  - **Toxic Blade** (Rare): Attack deals damage and applies Poison.
  - **Miasma** (Rare): Defense gives block and applies Poison.
  - **Plague** (Epic): Magic applies Poison for each Poison face on the die, with an Attack fallback.
- **EffectContext Refactor:** Every effect now takes `Execute(EffectContext context)` instead of `(source, target, value)`. The context also carries the source's `die` and the triggering `face`, which are null for enemy intents. All 8 effects and both call sites were converted without changing any logic, and a 16-case regression test (every face action plus the enemy intents) passed.
- **Die-Composition Scaling:** Added `ApplyStatusPerTaggedFaceEffectSO`, which applies value × (faces on the die with a tag). The status and tag are set in the Inspector so other archetypes can reuse it. The triggering face counts itself; without a die (enemies) it counts as 1.
- **Market:** Added the four Poison faces to the pool. **The Grand Market now shows 3 Epic offers** (Fuel, Cauterize, Plague).
- **Cleanup:** Merged the duplicated status text in `UIManager` into `BuildStatusText`.

### Decisions
- **Parameter object over the alternatives:**
  - Storing the die on `Entity` would put player-only data on the shared combatant class and hide a dependency.
  - Reading `RunManager` from inside an effect would break source/target independence and make tests depend on the RunManager.
  - Computing the count in the battle state would move mechanic logic out of `EffectSO`.
  - `EffectContext` grows by adding fields, which the Hammer archetype will need (for example, which face triggered the effect).
- **Refactor first, then the feature:** The signature change was made and regression-tested on its own before Plague was added, so any failure could be traced to one change.
- **Serialized enums only grow at the end.** Unity stores enum values in assets as numbers, so inserting a value would silently change existing assets.

### Next Steps
- Implement Shield: the Fortify status (absorbs damage after Block, never resets), the Shield rework, Shield Bash and Shield Burst.
- Watch Poison's balance: a full Poison die beat the 30 HP test enemy in 5 turns (a Burn die takes about 10–12). Revisit once enemies scale; decaying by 1 per tick is the fallback.
- Center the offer cards when a Market has fewer than 3 offers.
- Add boss enemies, the final boss and a boss gold reward. Scale enemies over the run.

## Day 19 - Shield Archetype & Fortify

### Accomplished
- **Fortify Status:** `Entity.TakeDamage` now absorbs damage with Block first, then Fortify, then HP. Fortify never resets at turn start. Damage over time (Burn, Poison) bypasses it. Shown in the battle UI.
- **New Effects:**
  - `DefenseScaledDamageEffectSO`: deals value + the source's Block + Fortify, consuming nothing.
  - `ConsumeOwnStatusDamageEffectSO`: consumes one of the source's own statuses and deals value + stacks × value.
- **Shield Faces:**
  - **Shield**, reworked as the Common starter: Attack deals damage, Defense gains Fortify.
  - **Shield Bash** (Rare): Attack deals value + Block + Fortify, Defense gives block.
  - **Shield Burst** (Epic): Attack consumes all Fortify for value + Fortify × value, Defense gains Fortify.
- Added Shield Bash and Shield Burst to the Market pool. There are now 4 Epic faces.
- Marked Poison and Shield as implemented in the GDD.
- **Tests:**
  - Re-ran the regression suite after changing `TakeDamage` (18/18), then ran 10 Fortify and Shield tests (turn start, absorb order, DoT bypass, Bash, Burst including with 0 Fortify).
  - Ran a Play Mode battle with a Shield die: Console damage matched value + Fortify × value (Fortify 4 gave 10, 3 gave 8, 2 gave 6).

### Decisions
- **Shield is the Shield archetype starter now**, which ends the Sword/Shield duplication. Sword stays the plain Physical face.
- **Fortify absorbs after Block,** so the turn's temporary Block is spent before the stored protection.
- **Regression test before features:** `TakeDamage` is the core of combat, so the old tests were re-run before any Shield face was added.

### Observations
These come from a few runs against the weak test enemy, not real balance data.
- **Battle length:** a Poison die took about 5 turns, Burn about 10–12, Shield about 16–18.
- **Shield** feels very safe but slow. Shield Burst's risk doesn't show yet because the enemy only deals 1–2 damage.
- **Enemy attacks wear down stored Fortify,** which creates a "cash out now or keep stacking" tension, Shield's version of Burn's "consume or let it tick".

### Next Steps
- Enemy scaling and boss enemies: all three archetypes were only tested against one weak enemy, so balance can't be judged yet.
- Add the final boss that ends the run (about 11 battles) and a boss gold reward.
- Center the offer cards when a Market has fewer than 3 offers.
- Play-test full runs and tune Poison (decay?), Shield and prices.

## Day 20 - Enemies & Bosses Design

### Accomplished
- Wrote the **Enemies & Bosses** section of the GDD:
  - normal enemies as data, with pools for each run segment and weighted, non-repeating intents
  - boss design principles and how boss dice work
  - three bosses: Minotaur, Medusa and Hydra
  - technical notes, Future ideas and open questions

### Design Challenge
Every battle uses the same weak enemy, so neither archetype balance nor the save vs. tempo decision can be judged. Bosses also need to feel different from normal enemies and from the player's own builds.

### Decisions
- **Normal enemies keep the intent system,** defined as data, with enemy pools for early, middle and late battles.
- **Bosses fight with their own die,** rolled at the start of the player's turn and shown as the intent. Luck still matters, but the player always sees the threat coming. Rolling after the player acts was rejected because a lucky boss roll could end a run with no counterplay, which feels unfair when HP carries over.
- **Fixed actions per boss face** instead of an AI. The boss's job is to be a readable threat.
- **The boss die is visible all battle** so players can read the odds.
- **Bosses use unique concepts, not the player's archetypes.** Mirror matches against a stronger copy of your own build are dull. Boss-only mechanics can't leak into player builds because normal enemies have no die.
- **Each boss tests a different part of the player's die:**
  - **Minotaur** (battle 4): planning, through a visible charge countdown that block or a stagger can answer.
  - **Medusa** (battle 8): reliance on single faces, by petrifying the highest-value face for 2–3 turns.
  - **Hydra** (battle 11, final): damage type. Big hits cut heads that regrow as two unless it has Burn, and it regenerates while unburned.
- **Test, don't hard-counter:** every build needs a way to beat every boss. A petrified face still gives a little block.
- **Boss effects are battle-scoped** and never change the run's die.
- **Multiple bosses per slot is planned,** for replayability. Boss phases and boss loot are recorded as Future ideas.

### Next Steps
- Define normal enemies as data (`EnemySO`), add early, middle and late pools, and add weighted, non-repeating intents.
- Build the boss dice system (boss brain, telegraphed roll, visible die) with the Minotaur first.
- Then Medusa (`targetDie` in `EffectContext`, battle-scoped Petrify) and the Hydra (heads, regrowth, regeneration).
- End the run after the final boss, and set a boss gold reward.
- Center the offer cards when a Market has fewer than 3 offers.

## Day 21 - Enemy Roster & Difficulty Curve

### Accomplished
- Added the **Enemy Tiers** and **Enemy Roster** to the GDD:
  - **Weak (battles 1–3):** Tiro, Murmillo, Thraex, Lampadarius, Arena Wolf.
  - **Medium (battles 5–7):** Retiarius, Dimachaerus, Arena Lion, Venefica, Secutor.
  - **Strong (battles 9–10):** Essedarius, War Elephant, Hoplomachus Champion, Bestiarius.
  - Each enemy has HP and weighted intents. The new enemy mechanics (multi-hit, escalation) are recorded.
- Updated the run structure: the player has 50 max HP, and winning a boss battle restores 50% of max HP.

### Design Challenge
The difficulty curve has to produce a specific feeling at each stage of a run, and the final stretch has to reward a strong build instead of punishing it.

### Decisions
- **Each tier has a role:**
  - **Weak** enemies are a toll: plain damage and block, small Burn/Poison, "let me get through cheaply".
  - **Medium** enemies are the first real threat, each with one readable danger mechanic.
  - **Strong** enemies are the build check.
- **"Clock, not lock":** strong enemies never shut down combos; that's what bosses do. Their high HP and rising damage set a clock. A strong build beats the clock and shows big numbers, which is the game's power fantasy. A die that came together by chance loses the race.
- **The final boss** tests the whole run rather than simply being hard.
- **Escalation is enemy-only data,** with the grown value shown in the intent. A general Strength status is postponed until the Berserker archetype because it would touch every damage effect.
- **No enemy repeats within a run.** Each pool has 4–5 enemies for 2–3 battles.
- **Max HP 50 and healing after bosses:** with 25 HP, carried-over damage and no healing outside battle, runs would end from slow HP loss instead of build quality.
- Enemies are real Roman gladiator types and arena beasts, so the arena theme stays consistent.

### Next Steps
- Add `EnemySO` (name, HP, weighted intents) and the Weak, Medium and Strong pools, picked by run segment without repeats.
- Add weighted, non-repeating intent selection, the multi-hit effect and enemy escalation.
- Raise the player's max HP to 50 and heal 50% after boss victories.
- Then the boss dice system (Minotaur, Medusa, Hydra) and ending the run after the final boss.

## Day 22 - Enemy System & First Balance Check

### Accomplished
- **Enemies as Data:**
  - `EnemySO` (name, tier, HP, `escalationPerTurn`, intents) with all 14 enemies from the GDD roster in `Assets/Enemies/`.
  - `EncounterConfigSO` lists every enemy.
  - The Battle scene has one generic Enemy object whose stats and brain are set from the picked `EnemySO`.
- **Intents:** `EnemyIntent` now has a list of effects (all using the same value), a weight and an `attackBonusGain`.
- **Enemy Brain:**
  - picks intents by weight, with the same intent at most 2 times in a row
  - describes the intent with its grown value (e.g. "Ride Down (14)", "Twin Strike (2 x 4)")
  - handles enemy-only escalation (per turn, or from intents like Battle Cry)
- **Multi-Hit Effect:** `MultiHitDamageEffectSO`; each hit wears down block and triggers FireShield separately.
- **Run:**
  - The enemy tier comes from the battle number. Boss slots use the next tier until bosses exist.
  - No enemy repeats within a run.
  - Max HP is now 50, and boss victories heal 50% of max HP.
- **UI:** the enemy's name is shown, along with its intent description.
- **Tests:**
  - Intent weights and the repeat cap were checked for all 14 enemies.
  - Escalation (Essedarius 4 → 6 → 10 → 14), Battle Cry (7 → 13), Twin Strike against block and FireShield.
  - No repeats across 200 simulated runs, tier selection and the boss heal.
- **First Run Simulation:** 300 simulated 11-battle runs each for 5 dice, using the real `Entity`, `EnemyBrain` and effects with a simple bot.

### Findings
- **The roster numbers don't fit the player's damage scale.** Faces deal about 2–3 per turn, while enemies have 18–110 HP.
- **An unbuilt die (3 Sword + 3 Shield) never got past the Weak tier.** It took about 37 turns per Weak enemy and lost all 50 HP.
- **Even upgraded builds rarely reached battle 8,** except Poison at 68%, and **none cleared all 11 battles.**
- **Caveats:** the bot was simple and never shopped in the Market, and there were no bosses. Still, the gap is too large for better play to close.
- The roster numbers were proposed without checking them against face values; the simulation caught it.

### Decisions
- **Lower the enemy numbers (mainly HP)** instead of raising face values. Face values, upgrade costs (rarity multiplier × value) and Max Value rules are all built around small values, so changing the 14 enemy assets is the smaller and safer change.
- **Measurable targets for the difficulty curve:**
  - An unbuilt die gets through the Weak tier losing about 10–15 HP.
  - A well-built die mostly beats the Strong tier.
  - A die bought without a plan stalls at battles 9–10.
- **Neutral starting die** (3 Sword + 3 Shield): the player builds the archetype during the run, instead of starting with the Burn test set.
- **The simulation bot must shop in the Market** (buy and upgrade between battles), or balance can't be judged honestly.

### Next Steps
- Build a run simulator whose bots use the real `Market` to buy and upgrade: a focused bot for each archetype, and an unplanned buyer.
- Switch the starting die to 3 Sword + 3 Shield.
- Tune enemy HP and damage with the simulator until the targets are met.
- Then the boss dice system (Minotaur, Medusa, Hydra), ending the run after the final boss, and a boss gold reward.
- Center the offer cards when a Market has fewer than 3 offers.

## Day 23 - Run Simulator & Balance Findings (in progress)

### Accomplished
- **Run Simulator** (`Assets/Editor/RunSimulator.cs`, menu: Tools → Gladiator Dice → Run Balance Simulation):
  - Simulates full 11-battle runs with the real `Entity`, `EnemyBrain`, effects and `Market`, without loading scenes.
  - Enemy HP and damage can be scaled per tier on simulation-only copies, so numbers can be explored without editing assets.
  - Five bots: no shopping, an unplanned buyer, and one focused bot each for Burn, Poison and Shield.
- **Neutral starting die:** 3 Sword + 3 Shield.
- **Fixed a bot weakness:** the bot stacked Fortify with Shield even when its die had nothing that spends Fortify.

### Findings
- **Weak tier target met:** at enemy HP ×0.3 and damage ×0.5, the no-shopping die clears the Weak tier losing about 14 HP. Weak enemies would then have 6–8 HP and deal 1–2 damage.
- **Lowering enemies alone isn't enough.** Even with Medium and Strong HP ×0.3, focused builds almost never clear a run. The walls are battle 4 (the first Medium enemy) and battle 8 (the first Strong enemy).
- **Player power grows too slowly.** At 10 gold per victory, a build hasn't formed by the time Medium enemies arrive.
- **Gold income is the strongest lever.** At 20 gold per victory with the same enemy settings:
  - the focused Poison bot clears 17% of runs (0% at 10 gold)
  - the unplanned buyer reaches battle 8 in 19% of runs (5% at 10 gold)
- **Starting faces at value 3** help in the Medium tier but drop Weak tier HP loss to about 7, below the target, so they stay at 2.
- **Shield is the weakest build in every scenario,** either because of the bot's Shield strategy or the build itself.

### Open Decisions
- Raise gold per victory (e.g. 20), and possibly raise prices with it so saving for Epics stays a real decision.
- Whether 6–8 HP Weak enemies feel right, or all face values should be scaled up instead.

### Next Steps
- Decide on gold income and prices, then tune each enemy with the simulator (starting point: Weak HP ×0.3 / damage ×0.5, Medium ×0.4 / ×0.6, Strong ×0.4 / ×0.6) and write the final numbers into the assets.
- Look into why the Shield build underperforms.
- Then the boss dice system (Minotaur, Medusa, Hydra), ending the run after the final boss, and a boss gold reward.
- Center the offer cards when a Market has fewer than 3 offers.

## Day 24 - Economy & Enemy Balance Pass

### Accomplished
- **Economy:** 20 gold per victory. Buy prices are now Common 10, Rare 25, Epic 60, and the Epic upgrade multiplier is ×20. Common and Rare upgrades stay at ×3 and ×5.
- **Simulator fix:** enemy scaling now applies to every intent number (block, Fortify, heal, statuses, escalation, Battle Cry), not just attacks. Scaling only attacks left defensive intents huge next to the scaled-down HP. For example, Secutor gained Fortify 6 every turn with 14 HP, which no early die could break.
- **Enemy numbers:** after a grid search, wrote Weak HP ×0.3 / values ×0.5, Medium ×0.3 / ×0.5 and Strong ×0.25 / ×0.4 into all 14 enemy assets. Re-running the simulator on the assets reproduced the same results.
- **GDD:** updated prices, the upgrade table, tier HP/damage ranges and the enemy roster, and added a **Balance Targets** section with the simulator results.
- **CLAUDE.md:** documented the simulator and that it must stay in sync with the battle and reward rules.

### Results (1000 runs per bot)
| Target | Result |
|---|---|
| Unbuilt die loses about 10–15 HP in the Weak tier | 12 HP, then it dies in the Medium tier ✅ |
| Unplanned buyer stalls around battles 9–10 | 77% reach battle 8, 13% clear ✅ |
| Well-built die mostly beats the Strong tier | Poison 73%, Burn 57% ✅; Shield 23% ❌ |

### Decisions
- **Asymmetric price increase:** doubling gold and every price would have changed nothing. Common and Rare got relatively cheaper so builds grow faster, while an Epic stays about three victories of saving.
- **Weak enemies at 5–8 HP are fine.** The numbers are small, but the difficulty curve is what matters.
- **Shield is an archetype problem,** not an enemy one: its strong-tier fights take about 9 turns versus about 4 for other builds, so escalating enemies win the race. Lowering enemies further would make every other build too easy.

### Observations
- Focused builds did worse right after the price change (Poison 17% → 4% clear) because they rely on Rare and Epic faces, which got relatively more expensive. Lower enemy numbers balanced this out.
- Strong enemies now hit about as hard as weak ones (2–3 per attack). The difference between tiers comes from HP and escalation. Raising Strong damage to ×0.5 sharply lowered clear rates, so it stays at ×0.4. Play-testing may show that Strong enemies don't feel strong.
- Simulator caveats: the bots never save for the Grand Market, and boss slots (battles 4, 8, 11) use regular enemies until bosses exist. Balance needs re-checking once bosses are in.

### Next Steps
- Rework the Shield archetype so it can win against escalating enemies (e.g. Shield Burst's numbers, a faster payoff), then re-run the simulator.
- Boss dice system (Minotaur, Medusa, Hydra), ending the run after the final boss, a boss gold reward, then re-balance with bosses in.
- Center the offer cards when a Market has fewer than 3 offers.
- Play-test a full run by hand to check how the new numbers feel.

## Day 25 - Run Start, Bleed & Readable Intents

### Accomplished
- **Play-test feedback:** the Secutor could not be beaten. Its Brace stacked permanent Fortify faster than the starting die could break it, and its "Shield Slam (4)" intent was misread because intents only showed a name and a number.
- **Bleed status:** deals 1 damage at the end of its owner's turn, then loses 1 stack, so stacks are duration rather than damage. It bypasses Block and Fortify.
- **Sword:** Attack deals its value and applies a fixed 1 Bleed (`ApplyStatusEffectSO.fixedAmount`); Defense gives block. Shield is unchanged (2 damage / 2 Fortify).
- **Health Potion** (Common, value 3, max 4): Magic heals its value, with no Attack or Defense.
- **Enemy Fortify:** Secutor's Brace and the War Elephant's Thick Hide now grant 1 Fortify (was 3).
- **Readable intents:**
  - Added `EffectSO.Describe(value)`, and `EnemyBrain` builds intent text from it (e.g. "Shield Slam: 4 damage", "Brace: 1 Fortify", "Battle Cry: +1 damage on all attacks").
- **Run Start:**
  - The starting die is now 2 Sword, 2 Shield and a Health Potion, plus a sixth face picked on the new RunStart scene. RunStart is the first scene in Build Settings.
  - `StartingFaceOffers` offers 3 faces (weights 85/13/2) from different archetypes, never a starter face.
  - Playing the Battle scene directly picks a random offer.
- **Shared card text:** `OfferCardUI.DescribeFace`, used by the Market and RunStart.
- **Simulator:** bots now pick a starting face (focused bots pick their archetype).
- **Rebalance:** the new starting die made the game far too easy (Burn/Poison bots cleared 98–99% of runs). Raised enemy HP only (Weak ×1.8, Medium ×1.4, Strong ×2.0); damage is unchanged.
- **GDD:** added the Run Start and Status Effects sections, readable intents, the enemy Fortify changes, the new HP numbers, updated balance results, and reroll as a Future idea.

### Results (600–1000 runs per bot)
| Target | Result |
|---|---|
| Unbuilt die loses about 10–15 HP in the Weak tier | 12 HP, then it dies in the Medium tier ✅ |
| Unplanned buyer stalls in the Strong tier | 75% reach battle 8, 2% clear ✅ |
| Well-built die mostly wins | Poison 87%, Burn 72% ✅; Shield 7% ❌ |

### Decisions
- **Sword and Shield mirror each other:** Sword is stronger on offense (it also bleeds) and weaker on defense (temporary Block). Shield is the opposite (permanent Fortify, weaker attacks).
- **Fortify must come in small amounts** because it never resets. Enemy Fortify intents are kept at 1.
- **Bleed is a fixed 1 on Sword,** so upgrading Sword only raises its damage and it stays Common.
- **Single-action faces are fine** (Health Potion does nothing at full HP). A future once-per-roll reroll is the planned answer to rolls that don't fit.
- **Starting face choice:** 3 random faces from different archetypes, mostly Common. Every run starts different and points toward a build from the first moment.
- **Rebalance through HP only,** so enemy hits stay readable and small.

### Observations
- **Too few Common faces:** with starter faces excluded, only Ember and Venom are Common. The third starting offer is therefore always Rare or Epic, so the real mix is about 60% Common instead of 85%.
- **Shield is still far too slow:** its strong-tier fights take about 17 turns versus about 5 for other builds.

### Next Steps
- Rework the Shield archetype and design more Common faces (for Shield, Lifesteal and a possible Bleed archetype).
- Boss dice system (Minotaur, Medusa, Hydra), ending the run after the final boss, a boss gold reward, then re-balance with bosses in.
- Reroll (once per roll).
- Center the offer cards when a Market has fewer than 3 offers.

## Day 26 - Soft Cap & Battle UI Readability

### Accomplished
- **Soft cap upgrades:**
  - Face upgrades are now unlimited. `DieFaceSO.maxValue` was renamed to `softCap` with `[FormerlySerializedAs("maxValue")]`, so every face asset kept its data.
  - From the soft cap on, each level costs `aboveSoftCapFactor` (2) times the normal formula, then 4×, 8× and so on. For example, Ember 6 → 7 costs 36 gold and Fuel 3 → 4 costs 120.
  - The Market shows "Break cap" on those upgrades.
- **Battle UI rebuilt:**
  - Combatant panels: name, HP bar with numbers, Block/Fortify, and status chips with tooltips.
  - An intent box with a detailed tooltip.
  - A rolled-face panel.
  - A strip with all six die faces, the rolled one highlighted.
  - Action buttons that preview what each action does (e.g. "ATTACK — 2 damage, 1 Bleed") and are disabled when the rolled face lacks that action. The Roll button is only active when it's time to roll.
- **Root cause of clipped text:** every canvas used Constant Pixel Size, so layouts only fit 2560×1440. All three scenes now Scale With Screen Size (2560×1440 reference), and long texts auto-size. The HP bars were sliders with a white fill and a drag handle (the old "gray block"); they are now plain fill bars.
- **Keyword colors and glossary:** `GameText` is the single source of keyword colors and definitions (damage, block, Fortify, Burn, Poison, Bleed, FireShield, heal). It's used in battle, the Market and RunStart.
- **Tooltips:** one `TooltipUI` per scene and a `TooltipTrigger` component for anything hoverable. Tooltips list the definitions of the keywords they mention.
- **Effect descriptions:** every effect now overrides `Describe` (used by intents and action previews).
- **Bug:** "MAGIC" showed as "MAGİC" because `ToUpper()` follows the system culture (on Turkish systems "i" → "İ"). Switched to `ToUpperInvariant()` and recorded the rule in CLAUDE.md.

### Decisions
- **Soft cap instead of a hard cap:** game-breaking runs are part of the power fantasy (Pillar 4), so they are made expensive rather than impossible. The cost doubles per level past the cap, and an Epic past its cap takes most of a run's gold.
- **Readability before art:** this phase uses only text, color and layout, so play-testing improves now without waiting for icons.
- **One source for player-facing rules text** (`GameText`), so colors and definitions can't drift between screens.

### Next Steps
- UI phase 2: status and mechanic icons from game-icons.net (an icon list to download).
- Play-test with the new UI: tooltip placement with a real mouse, other window sizes, and the feel of Bleed and the Health Potion.
- Rework the Shield archetype and design more Common faces.
- Boss dice system (Minotaur, Medusa, Hydra), ending the run after the final boss, then re-balance with bosses in.
- Reroll (once per roll). Center the offer cards when a Market has fewer than 3 offers.

## Day 27 - Status Rework, Rage & Ritual

### Accomplished
- **Play-test feedback:** Poison didn't behave as intended, and the Arena Lion (4 damage almost every turn) ended two runs.
- **Status rework:**
  - **Burn** deals no damage on its own. It is fuel that decays by 2, unless it was applied that turn.
  - **Poison** deals its stacks, then decays by 1.
  - **Bleed** is unchanged: 1 damage, then −1.
- **Ignite:** added `IgniteEffectSO`, which deals damage equal to the target's Burn without consuming it. Ember's Attack now applies Burn and ignites it. Detonate still consumes Burn for one big hit.
- **Rage** (new status, lasts the battle): +1 damage per stack on every hit, and each hit of a multi-hit gets it.
- **Ritual** (new status, lasts the battle): +1 per stack on every status applied to the opponent.
- **One path for damage and statuses:**
  - Every damage effect now hits through `EffectContext.Hit` (adds Rage), and every status put on the opponent goes through `EffectContext.ApplyToTarget` (adds Ritual).
  - Self-applied statuses and Fuel's multiply bypass Ritual on purpose.
- **Real-number previews:** `EffectSO.Describe` now takes the `EffectContext`, so action previews and enemy intents show Rage, Ritual and current stacks (e.g. "5 damage, 1 Bleed" with 3 Rage, "7 damage, consume Burn").
- **Enemies:**
  - The hidden enemy attack bonus and `EnemyIntent.attackBonusGain` were removed. Escalation now uses the visible Rage status: Essedarius gains 1 Rage per turn, and Battle Cry gains Rage through `GainRageSelfEffect`.
  - Lampadarius gained an Ignite intent.
  - The Arena Lion now starts weak and builds Rage: Maul 1, Roar gains 1 Rage, Feast deals 1 and heals by the damage.
  - `EnemyBrain.Setup` now takes the enemy and its opponent.
- **Tests:** 15 rule tests passed, including Sword with 3 Rage (5 damage + 1 Bleed), Sword with 3 Ritual (2 damage + 4 Bleed), Twin Strike with 3 Rage (2 × 5), Ritual not boosting Fortify or Fuel, Plague with Ritual, and the Lion after two Roars (Maul 5).
- **GDD and CLAUDE.md** updated (status table, archetype texts, roster, escalation through Rage, balance results).

### Results (1000 runs per bot)
| Target | Result |
|---|---|
| Unbuilt die loses about 10–15 HP in the Weak tier | 8 HP ⚠️ slightly low (enemy Burn/Poison hurt less now) |
| Unplanned buyer stalls in the Strong tier | 89% reach battle 8, 0% clear ✅ |
| Well-built die mostly wins | Poison 85%, Burn 57% ✅; Shield 1% ❌ |

### Decisions
- **Three distinct damage-over-time roles:** Burn is fuel (ignite or detonate), Poison is front-loaded attrition, Bleed is duration.
- **Rage replaces hidden enemy escalation:** one visible mechanic for both sides. The player can see why an enemy hits harder, and the same status will power a Berserker archetype.
- **Rage stacks per hit,** so multi-hit faces benefit the most.
- **Ritual only boosts statuses applied to the opponent,** because boosting self-applied Fortify would break the "Fortify comes in small amounts" rule.
- **Rage and Ritual are visible statuses, not hidden multipliers,** so Value Is the Effect still holds and previews show the boosted numbers.
- **Lion's Roar gives 1 Rage, not 2:** at 2 the Lion alone ended about half of the unbuilt runs and most Shield runs, taking over the Strong tier's "clock" role in the Medium tier.

### Next Steps
- Design player faces that grant Rage (Berserker) and Ritual (Ritualist), and use them to help the Shield archetype.
- Consider a small HP bump for Weak enemies (the unbuilt die loses only about 8 HP now).
- UI phase 2: status icons from game-icons.net.
- Boss dice system (Minotaur, Medusa, Hydra), ending the run after the final boss, then re-balance with bosses in.
- Reroll (once per roll). Center the offer cards when a Market has fewer than 3 offers.

## Day 28 - Starting Face Fix, Berserker & Ritualist Faces

### Accomplished
- **Bug fix, starting face skipped:** pressing Play with the Battle scene open in the Editor skipped the run start screen, because `EnsureStartingFace` picked a random offer. Now `BattleManager.Awake` disables itself and sends the player to RunStart whenever no starting face has been chosen, so the choice can never be skipped. The redirect picks no enemy, so it doesn't use up the first battle.
- **Berserker faces (Rage):**
  - **Fury** (Common): Magic gains Rage, Attack deals damage.
  - **Bloodrage** (Common): Magic loses HP and gains Rage, Attack deals damage.
  - **Twin Axes** (Rare): Attack hits twice.
  - **Iron Rage** (Rare, bridge to Shield): Defense gains Fortify and 1 Rage.
  - **Unleash** (Epic): value × Rage damage, at least the value.
- **Ritualist faces (Ritual):**
  - **Incantation** (Common): Magic gains Ritual, Defense gives block.
  - **Witch's Curse** (Rare): Magic applies Bleed and Poison.
  - **Blood Rite** (Rare): Attack deals damage and applies Bleed.
  - **Dark Communion** (Epic): Ritual per status type on the enemy.
- **New effects:**
  - `PayHPEffectSO`, with `Entity.PayHP` for HP costs.
  - `OwnStatusScaledDamageEffectSO` (value × own Rage).
  - `GainStatusPerOpponentStatusEffectSO` (Ritual per enemy status type).
  - New assets for gaining Ritual, gaining 1 Rage and applying value-based Bleed.
- **Content:** added the `Rage` and `Ritual` face tags. The Market pool is now 23 faces, with three new Commons for the starting face choice.
- **Simulator:** bot rules for Fury, Incantation and Bloodrage (only pays HP with HP to spare), plus focused Rage and Ritual bots (7 bots total).
- **Tests:** 13 face tests passed, for example Twin Axes with 4 Rage dealing 2 × 5, Unleash with 5 Rage dealing 15, Witch's Curse with 2 Ritual applying 3 Bleed + 3 Poison, Dark Communion giving 1/2/3 Ritual, and Bloodrage able to kill its user.

### Results (700 runs per bot)
| Bot | Reached battle 8 | Cleared |
|---|---|---|
| No shopping | 28% | 0% (Weak tier HP lost: 11 ✅) |
| Unplanned buyer | 71% | 0% ✅ |
| Poison | 93% | 63% |
| Burn | 80% | 38% |
| Shield | 90% | 0% |
| Rage | 46% | 0% |
| Ritual | 19% | 0% |

### Decisions
- **Twin Axes hits twice, not three times** (the user's call): with Rage on every hit, three hits scale like a multiplier and would belong in Epic.
- **Bloodrage added** as a high-risk Berserker Common: Rage for HP, and HP carries over between battles.
- **The starting face choice is never skipped:** it's part of every run, so the Battle scene redirects to it instead of picking for the player.

### Observations
- **Pool dilution:** going from 14 to 23 Market faces means each archetype's faces show up less often. Focused Burn and Poison bots got weaker (Burn 57% → 38%, Poison 85% → 63%).
- **Berserker and Ritualist are not tuned yet** (0% clears). Ritual does nothing alone, so a bot that buys only Ritual-tagged faces is the wrong test. It needs a hybrid-aware bot (Ritual plus a status archetype).
- **Shield still can't win the strong tier.** Iron Rage was meant to help but rarely comes up for a bot that focuses on the Block tag.
- **The Common shortage is solved:** Fury, Bloodrage and Incantation give the starting face choice three more Commons.

### Next Steps
- Balance pass for the new archetypes:
  - tune Berserker values and costs
  - add a hybrid bot (e.g. Ritual + Poison/Bleed) to measure Ritualist
  - revisit Shield with Iron Rage in mind
- Decide how the Market should handle a larger pool (more offers, stronger tag weighting, or archetype-themed Markets).
- UI phase 2: status icons from game-icons.net.
- Boss dice system (Minotaur, Medusa, Hydra), ending the run after the final boss, then re-balance with bosses in.
- Reroll (once per roll). Center the offer cards when a Market has fewer than 3 offers.

## Day 29 - Market Reroll & Hybrid Bots

### Accomplished
- **Market reroll:**
  - `Market.Reroll()` pays for a fresh set of offers. Sold offers are replaced too, and the new set avoids the replaced faces when the pool allows.
  - The price rises within a visit and resets at the next Market: Normal 5 / 10 / 15, Grand 15 / 30 / 45. The numbers live in `EconomyConfigSO` (`GetRerollCost`).
  - A "Reroll Xg" button in the Market scene always shows the next price.
- **Bleed tag:** added `FaceTag.Bleed` to Sword, Blood Rite and Witch's Curse, so build-aware offers and bots recognize Bleed faces.
- **Hybrid bots:** bots can now focus on several tags. Added Ritual + Poison, Ritual + Bleed and Shield + Rage (10 bots total).
- **Bot rerolls:** focused bots reroll the Market when nothing fits their build and they keep at least `GoldToKeepAfterReroll` (40) gold.
- **Tests:** in Play Mode, three rerolls cost 5 + 10 + 15 gold, every reroll replaced all three offers with new faces, and the button disabled itself when the next price (20) was unaffordable.
- **GDD and CLAUDE.md** updated (Market reroll, the die reroll renamed to keep the two apart, bots).

### Results (400–500 runs per bot)
- **Hybrid bots give healthier data:** Ritual alone clears 0% of runs, Ritual + Poison clears 31–35%.
- **Reroll value depends on how it's used:** rerolling whenever nothing fits dropped Poison from 63% to 48%, because the gold went to rerolls instead of upgrades. Keeping at least 40 gold after a reroll made rerolls neutral to slightly positive (Poison 67%, Burn 44%).
- **Still 0%:** Shield, Rage, Ritual + Bleed and Shield + Rage. Bleed is the clearest weakness, at only 1 damage per turn.

### Decisions
- **Paid rerolls instead of a few free ones** (the user's call): a price brings the decision back every visit and gives gold a third use (buy, upgrade, reroll), while free rerolls would only matter a few times per run.
- **Price rises within a visit** so plenty of gold can't buy endless fishing for Epics. It resets every Market.
- **The Grand Market reroll is much more expensive** (3×), since it only offers Epics.
- **Market reroll and die reroll are separate mechanics,** named apart in the GDD.

### Next Steps
- Balance pass: Bleed, Rage, Shield and their hybrids (all at 0%). Likely face value changes rather than enemy changes.
- UI phase 2: status icons from game-icons.net.
- Boss dice system (Minotaur, Medusa, Hydra), ending the run after the final boss, then re-balance with bosses in.
- Die reroll (once per roll in battle). Center the offer cards when a Market has fewer than 3 offers.

## Day 30 - Bleed Archetype

### Accomplished
- **Bleed as a mark:** Bleed keeps its rule (always 1 damage, −1 per tick). Its stacks are how long the enemy stays marked, and faces react to it with "if the enemy is bleeding" effects.
- **`ConditionalEffectSO`:** a reusable "if" box that runs its inner effects only when a status is present (on the target, or on the source).
- **`ConsumeStatusMultiplyDamageEffectSO`:** consumes a status and deals value × stacks. It's the multiplicative counterpart to Detonate's value + stacks.
- **Bleed faces:**
  - **Serrated Blade** (Common): Attack applies Bleed, Defense gives block.
  - **Open Wound** (Rare): hits, then hits again if the enemy is bleeding.
  - **Bloodscent** (Rare, Bleed + Rage bridge): blocks, and heals if the enemy is bleeding.
  - **Exsanguinate** (Epic): consumes Bleed for value × stacks damage, at least the value.
- **Content:** Market pool now 27 faces.
- **Simulator:** added a focused Bleed bot and a Bleed + Rage hybrid (12 bots).
- **Tests:** 8 tests passed, for example Open Wound with 3 Rage against a bleeding enemy (5 + 5), Bloodscent healing only while the enemy bleeds, and Exsanguinate consuming 6 Bleed for 6 damage (1 with no Bleed).
- **GDD and CLAUDE.md** updated (Bleed archetype, the life loop, conditional effects).

### Results (500 runs per bot)
| Bot | Reached battle 8 | Cleared | Strong-tier turns |
|---|---|---|---|
| Poison | 96% | 61% | 6 |
| Burn | 75% | 33% | 7 |
| Ritual + Poison | 69% | 31% | 6 |
| Bleed | 80% | 0% | 13 |
| Bleed + Rage | 74% | 0% | 10 |
| Rage | 52% | 1% | 10 |
| Shield + Rage | 74% | 0% | 12 |
| Shield | 90% | 0% | 21 |

### Decisions
- **Bleed's rule stays** (the user's call). Bleed builds don't use Bleed as their main damage; their faces react to it.
- **Exsanguinate consumes Bleed,** so cashing in costs the mark that the conditional faces rely on.
- **Bloodscent heals its value for now,** to be revisited if it proves too strong. Its healing is the bridge to Berserker's "lose HP → strong effect → heal back" loop, so it carries both the Bleed and Rage tags.

### Observations
- **One pattern explains every losing build:** winners kill strong enemies in 6–7 turns, losers take 10–21 turns. Bleed, Rage and Shield clear the Medium tier fine (74–90% reach battle 8) but lose the race against strong enemies that gain Rage every turn.

### Next Steps
- Balance pass on face values (not enemies, since Poison and Burn are on target): find which Bleed, Rage and Shield faces move the needle most, aim for about 30% clears for each, and keep Poison and Burn where they are.
- UI phase 2: status icons from game-icons.net.
- Boss dice system (Minotaur, Medusa, Hydra), ending the run after the final boss, then re-balance with bosses in.
- Die reroll (once per roll in battle). Center the offer cards when a Market has fewer than 3 offers.

## Day 31 - Balance Investigation, Bleed Timing & Boss Gold

### Accomplished
- **Balance investigation (simulator only, no assets changed during experiments):**
  - +1, +2 and +4 to every Bleed face, more frequent Rares/Epics in the Market (55/35/10 and 45/40/15), +1 to weak Commons, a fixed Rage bot, and a Fury that also gains Rage on Attack.
  - None of these lifted the weak archetypes (Bleed, Rage, Shield) meaningfully.
- **Simulator tooling:**
  - `WithFaceValueChanges` (temporary, always-restored face value changes), `ClearRates` and `BotsMatching` for short experiments.
  - New report lines: the average die at battle 8 and gold when entering the first Grand Market.
  - Rage bots now switch from building Rage to attacking at `RageToStartAttacking` (3).
- **Play-test findings from the user:**
  - Bleed never stayed on the enemy until the player's next turn.
  - Gold never added up: the user reached the first Grand Market unable to afford anything.
- **Bleed timing fix:** Sword now applies a fixed 2 Bleed (the effect asset was renamed to `ApplyBleedTwoEffect`, keeping its GUID). Serrated Blade 2 → 3, Blood Rite 2 → 3, Witch's Curse 1 → 2.
- **Boss gold:** `EconomyConfigSO.bossVictoryBonus` (30) is added on boss victories (`RunManager.OnBattleEnded` and the simulator).
- **GDD and CLAUDE.md** updated (Sword's 2 Bleed and why, Bleed timing, boss gold, new face values).

### Results (500 runs per bot)
No shopping 0% · Unplanned 0% · Burn 34% · Poison 70% · Ritual + Poison 36% · Shield 2% · Rage 5% · Ritual 1% · Bleed 0% · Ritual + Bleed 0% · Shield + Rage 2% · Bleed + Rage 1%. Bots now enter the first Grand Market with about 60 gold (25 before).

### Findings
- **Builds are made of Commons.** At battle 8 the focused bots' dice hold Ember ×3.5 (Burn), Venom ×2.9 (Poison), Sword ×3.2 + Serrated Blade ×1.8 (Bleed), Fury ×2.1 + Bloodrage ×2.0 (Rage). Rares and Epics appear only 0.2–0.4 times per run, so an archetype's real strength is its Common.
- **The winning Commons grow during a fight; the losing ones don't.** Ember stacks Burn and ignites the total (3 → 6 → 9). Venom's Poison stacks and ticks by itself. Sword, Serrated Blade, Fury and Shield do the same small thing every turn, and against strong enemies that gain Rage every turn ("clock, not lock") that loses the race.
- **Bleed's timing flaw:** Bleed applied on your turn ticks at the end of the enemy's turn, so 1 Bleed never reached the player's next turn and "if the enemy is bleeding" faces almost never triggered.
- **Gold:** without a boss bonus, bots entered the Grand Market with about 25 gold while the cheapest Epic costs 60. Raising gold per victory to 25 made Poison too strong (86–94%), so a boss bonus fixes the Grand Market without changing the normal economy.
- **A Burn bot signal:** with more gold, the Burn bot replaced its Embers with Epic Burn faces and got worse. Burn's Epics can be weaker in practice than its Common.

### Decisions
- **Sword applies 2 Bleed, and every Bleed source applies at least 2** (the user's call), so the mark reaches the player's next turn.
- **Boss victories pay +30 gold;** gold per victory stays at 20.
- **Weak archetypes need Commons that grow during a fight** (recommended direction; designs to be agreed).

### Next Steps
- Design and implement Action modules: buy an action ("Magic: apply Poison equal to my value") and attach it to a face's empty action slot. The module uses the host face's value.
- Then equipment (passive items) on a trigger system ("at turn start", "when hit"...).
- Give Bleed, Rage and Shield Commons that grow during a fight.
- UI phase 2 (icons), the boss dice system, die reroll, centering offer cards.

## Day 32 - Action Modules

### Accomplished
- **`ActionModuleSO`:** a bought action (name, rarity, tags, `action`, effects) attached to an empty action slot of a die face. Six modules in `Assets/Modules/`: Venom Coating, Kindling, Rend, Iron Skin (Common), War Cry, Ignition (Rare).
- **`DieFace` owns its modules:** `GetEffects(action)` merges definition and module effects, `HasAction`, `CanAttach` (the face has no effect for that action yet), `Attach`, `DisplayName` ("Sword +1") and `FullDescription`. Battle, UI and simulator now read effects only through `DieFace` (regression check: 84 comparisons, 0 mismatches).
- **Market:** the Normal Market offers 2 modules next to the 3 faces (`modulesPerMarket`, same rarity/tag rules, rerolls replace them too); the Grand Market sells none. Prices follow face prices (10/25/60).
- **Market scene:** "FACES" and "ACTION MODULES" rows; buying a module highlights only the faces with that slot empty; die slots list their empty slots.
- **Simulator:** focused bots buy modules of their archetype with the gold left after upgrades, attach them to faces outside the archetype and play them there; unplanned bots sometimes buy random modules. `BuyModules` switches this off for comparisons, and reports show modules on the die at battle 8.
- **GDD and CLAUDE.md** updated.

### Results (500 runs per bot)
Without / with modules: Burn 32% / 26% · Poison 73% / 72% · Ritual + Poison 39% / 40% · others unchanged. The difference is within noise.

### Findings
- **A module is an option, not an extra effect.** One roll is still one action, so a module is only worth "its action minus the face's best action". On a value-2 Sword, Venom Coating (2 Poison, about 3 damage) is weaker than the Sword itself (2 damage + 2 Bleed).
- Naive bots that bought modules first lost a lot (Burn 32% → 5%): the gold came out of upgrades, and Kindling replaced damage with fuel that nothing ignited.
- Ignition fits almost only on the Health Potion, the only starter face with an empty Attack slot.

### Decisions
- **Modules stay as they are for now** (prices, set, no removal). They are a flexibility and hybrid tool, not a power source; revisit them after the equipment / trigger system.
- Replacing a face discards its modules.

### Next Steps
- Equipment (passive items) on a trigger system ("at turn start", "when hit"...), then re-evaluate modules with it.
- Give Bleed, Rage and Shield Commons that grow during a fight.
- UI phase 2 (icons), the boss dice system, die reroll, centering offer cards.

## Day 33 - Growing Commons & Essedarius

### Accomplished
- **Design rule (GDD "Build Archetypes"):** every archetype needs a Common that turns its accumulated resource into results during a fight, additively (multiplying stays Epic), with its value as a floor.
- **`StatusStacksDamageEffectSO`:** damage from a status's stacks without consuming them, counted on the target or on yourself, as value + stacks or as stacks with the value as a minimum.
- **Three new Commons** (value 2, soft cap 5), added to the Market pool (27 → 30 faces):
  - **Lacerate** (Bleed): Attack: my value + the enemy's Bleed as damage. Defense: block.
  - **Frenzy** (Rage): Attack: deal my value, then gain 1 Rage.
  - **Spiked Shield** (Shield): Attack: damage equal to your Fortify, at least my value. Defense: Fortify.
- **Essedarius:** its Rage every turn (`escalationPerTurn` 1) was replaced with a Battle Cry intent (1 Rage, weight 1, the same as Hoplomachus Champion's).
- **Simulator:** Spiked Shield rule (stack Fortify to 6, then hit; block big hits).
- **GDD and CLAUDE.md** updated.

### Results (500 runs per bot)
| Bot | Before | New faces | + Essedarius |
|---|---|---|---|
| Burn | 26% | 13% | 30% |
| Poison | 72% | 64% | 76% |
| Shield | 2% | 2% | 21% |
| Rage | 6% | 6% | 30% |
| Bleed | 0% | 0% | 6% |
| Ritual + Poison | 40% | 34% | 55% |
| Shield + Rage | 2% | 5% | 36% |
| Bleed + Rage | 2% | 2% | 12% |

### Findings
- **The new faces alone changed almost nothing,** even at +4 value (Lacerate at 6 lifted Bleed only to 4%). The bottleneck was elsewhere.
- **Essedarius was a wall:** the only Strong enemy with permanent Rage every turn. Slow builds fight Strong enemies for about 11 turns, by which point it hit for 2 + 11. It caused about half of the Bleed, Rage and Shield deaths and was the top killer of every bot. Lowering its HP to 34 did not help; removing the unbounded growth did.
- With Battle Cry, deaths spread across the Strong tier, and the new faces are used (about 1.4 Spiked Shield and 1.5 Frenzy per die at battle 8).
- Bleed is still weak (6%), and Lacerate's value barely matters (+2 value: +2 points). The problem is the archetype as a whole.
- Poison (76%) and Ritual + Poison (55%) are now well above the other builds.

### Decisions
- Visible, intent-based escalation (Battle Cry) instead of a hidden per-turn counter, which matches the game's readable-threat style. `escalationPerTurn` stays in code but no enemy uses it.
- The new Commons keep value 2 for now.

### Next Steps
- Find Bleed's structural problem.
- Bring Poison and Ritual + Poison down towards the other builds.
- Equipment (passive items) on a trigger system, then re-evaluate modules with it.
- UI phase 2 (icons), the boss dice system, die reroll, centering offer cards.

## Day 34 - Bleed Diagnosis & Serrated Blade

### Accomplished
- **Simulator diagnostics:** `RunSimulator.Stats` (`FightStats`, off when null) records, for one enemy tier, damage per player turn from actions vs. ticks, HP taken, how often the enemy is bleeding when the player acts, and damage per roll for each face.
- **Serrated Blade:** Attack: if the enemy is already bleeding, deal damage equal to my value; then apply Bleed equal to my value. Value 3 → 4 (soft cap 6 → 7), set by simulation.
- **GDD and CLAUDE.md** updated (Bleed's identity: the faces deal the damage, Bleed powers them).

### Diagnosis (Strong tier, 300 runs)
| Build | Damage per turn | HP taken per turn | Battle length | Won |
|---|---|---|---|---|
| Poison | 7.2 (6.9 from ticks) | 1.85 | 6.2 turns | 95% |
| Rage | 5.7 | 1.71 | 7.3 turns | 77% |
| Burn | 4.9 | 1.64 | 8.6 turns | 84% |
| Bleed (before) | 4.0 | 1.72 | 9.6 turns | 61% |
| Shield | 1.7 | 0.65 | 24 turns | 71% |

- Bleed's problem was damage, not survival: it lost HP as fast as the other attack builds but killed slowest.
- The mark was not the problem either: the enemy was bleeding at 71% of the player's actions.
- **Serrated Blade was a dead roll:** 18% of the Bleed die's rolls dealt 0 damage. Sword already keeps the mark up, so its extra duration was mostly wasted.

### Results (600 runs per bot)
| Serrated Blade | Bleed | Ritual + Bleed | Bleed + Rage |
|---|---|---|---|
| Before (apply Bleed only) | 8% | 4% | 12% |
| With the hit, value 2 | 17% | 12% | 16% |
| With the hit, value 3 | 17% | 13% | 19% |
| **With the hit, value 4** | **26%** | **20%** | **23%** |

All bots: Burn 28% · Poison 76% · Shield 13% · Rage 26% · Bleed 27% · Ritual + Poison 49% · Ritual + Bleed 16% · Shield + Rage 41% · Bleed + Rage 26%.

### Findings
- **"Bots play Bleed like Poison" did not hold up.** Most Bleed faces have one damaging action, so there is little to misplay in battle. Letting bots replace their starting Swords made them worse (Bleed + Rage 23% → 9%): Sword deals damage and applies Bleed, and replacing it loses its upgrades. The experiment was reverted.
- The real cause matches the design intent: **the faces should deal the damage, and Bleed should power them.** A face that only applies Bleed does nothing, because Bleed's own 1 damage per tick is a token.

### Decisions
- Bleed grows through more "if the enemy is bleeding" effects and faces that scale with its stacks, not through Bleed ticking harder.
- Serrated Blade is a conditional Common on purpose (conditions are normally Rare), because the condition is Bleed's identity.
- If new "if bleeding" faces push Bleed above the target band, Serrated Blade goes back to value 3.

### Next Steps
- Bring Poison (76%) and Ritual + Poison (49%) down towards the other builds.
- More "if the enemy is bleeding" effects (faces or modules); lift Ritual + Bleed.
- Equipment (passive items) on a trigger system, then re-evaluate modules with it.
- UI phase 2 (icons), the boss dice system, die reroll, centering offer cards.

## Day 35 - Poison Halves

### Accomplished
- **Poison rule:** after dealing its stacks, Poison now halves (rounded down) instead of losing 1. Updated in `Entity`, the tooltip glossary (`GameText`), the GDD and CLAUDE.md.
- **Simulator diagnostics:** `FightStats.trackedStatus` follows one status on the enemy: stacks added per roll for each face, the average stacks when the player acts, and the stacks by battle turn.
- **GDD:** Poison's resource, identity and a "Why it halves" note. Venefica's row said "Poison never decays", which was already wrong under the old rule.

### Diagnosis (Strong tier, 300 runs, old rule)
- Poison dealt 7.1 damage per turn from ticks and 0.3 from actions; battles lasted 6 turns, 95% won.
- Venom made up 35% of rolls and added 4.4 Poison per roll (a cheap Common with cheap upgrades).
- Enemy Poison by turn: 2.5, 4.9, 7.1, 8.3.
- Poison won with half a build: 3.6 of 6 faces were Poison faces at battle 8 (other builds: 5.6–6.0).
- **Root cause:** with "then lose 1", one application of N Poison dealt N(N + 1)/2 damage (value 4: 10, value 6: 21). Its damage grew with the square of the value, breaking the "Common scales linearly" rule.

### Experiments (400 runs, a temporary knob in `Entity`, restored with git afterwards)
| Variant | Poison | Ritual + Poison |
|---|---|---|
| −1 per tick (old) | 77% | 50% |
| Venom value 1 | 73% | 48% |
| −2 per tick | 45% | 33% |
| **Halves per tick** | **28%** | **19%** |

- Lowering Venom's value did nothing: cheap upgrades bring it back.
- Halving keeps one application at about twice its stacks (4 → 7 damage, 8 → 15), so the damage is linear.

### Results (600 runs per bot, new rule)
Poison 76% → 32% · Ritual + Poison 49% → 23% · Burn 26% · Rage 28% · Bleed 21% · Shield + Rage 35% · Bleed + Rage 23% · Ritual + Bleed 16% · Shield 10% · Ritual 1%.

For the first time in these balance passes, the main builds (Burn, Poison, Rage, Bleed) sit close together, at 21–32%.

### Decisions
- Poison is front-loaded and quickly fading, so it has to be reapplied. The slow, boss-killing side could come back later through an Epic ("Poison does not decay this turn").
- The rule applies to everyone, including Poison the enemies apply to the player (Venefica, Thraex).

### Next Steps
- Shield on its own (10%) and Ritual on its own (1%, a hybrid glue by design).
- More "if the enemy is bleeding" effects (faces or modules); lift Ritual + Bleed.
- Equipment (passive items) on a trigger system, then re-evaluate modules with it.
- UI phase 2 (icons), the boss dice system, die reroll, centering offer cards.

## Day 36 - Equipment Trigger System

### Accomplished
- **GDD "Equipment" section:**
  - Design rules: equipment talks to the die, three slots with no slot types, small fixed numbers, no upgrades.
  - The eight triggers and the no-chain rule.
  - How items are obtained: a free pick of 1 of 3 after a boss, and 2 items for 30 gold at the Grand Market.
  - The first set of eight items.
- **`EquipmentSO`:** name, description, rarity, tags and `TriggeredEffect`s. Each one has a `TriggerType`, an optional face-tag or action condition, existing `EffectSO` assets and a fixed value.
- **`TriggerSystem`:** runs the player's items. Hits come from new `Entity` events: `HitDealt` (raised on the attacker for each hit) and `HitTaken` (a hit that cost HP). Effects run by a trigger never fire triggers, so items cannot loop.
- **`BattleFlow`:** the turn steps in one place: battle start, turn start, roll, action, turn end, enemy turn, victory. The battle states and `RunSimulator` both call it, so the simulator no longer mirrors the states by hand.
- **`RunData.equipment`** with `EquipmentSlots = 3`.
- **CLAUDE.md** updated.

### Tests
- **Regression:** with the same random seed, all 12 bots gave identical results before and after the refactor (no equipment), so the turn order did not change.
- **Trigger checks (14/14) with temporary in-memory items:**
  - Each trigger fires at its point, and the face-tag and action conditions work.
  - Twin Axes' two hits give 2 Burn through a HitDealt item.
  - Barbed Hook's extra hit does not fire Smoldering Gauntlet.
  - Fully blocked hits and Poison ticks don't count as HitTaken.
  - After `Dispose`, items stay silent.
- **Play Mode:** two battles played through the real input methods. One had no items. The other had a temporary Bone Fetish (Ritual at battle start) and Laurel Wreath (healing on victory, before the HP is saved).

### Decisions
- **Turn steps are shared (`BattleFlow`)** instead of firing triggers from the states. The order is explicit, and the simulator cannot drift from the game.
- **Hits are the only Entity events;** every other trigger fires from a turn step.
- **No chains:** simpler and loop-free. The cost is that items can't trigger each other.

### Next Steps
- Boss reward choice and equipment sales at the Grand Market (UI included).
- The eight item assets, equipment-aware bots, and numbers tuned by simulation.
- Re-evaluate action modules alongside equipment.

## Day 37 - Boss Rewards & Equipment in the Market

### Accomplished
- **Market model:**
  - The Grand Market sells 2 items at 30 gold.
  - After a boss (`RunData.bossRewardPending`), the Market offers a free pick of 1 of 3 items; it can also be skipped.
  - With full slots, a new item replaces a chosen one. Owned items are never offered, and rerolls replace the items for sale.
- **Market UI:**
  - The second card row shows action modules in the Normal Market and equipment in the Grand Market.
  - A row of 3 equipment slots sits at the bottom, and Next Battle moved to the bottom right.
  - The boss reward panel covers the Grand Market. With full slots it hides while you pick the item to replace; Cancel brings it back. Next Battle stays locked until the reward is taken or skipped.
- **The eight items** in `Assets/Equipment/`, added to `EconomyConfigSO.equipmentPool`.
- **Trigger limits:** `maxPerRound` and `maxPerBattle` on each reaction (0 = no limit).
- **Simulator:** bots take the boss reward and buy items they want (`Bot.Want`: their archetype first, then general items), and replace the item they want least when slots are full. `UseEquipment` switches this off. Reports show the items on hand at battle 8.

### Tests
- Market model checks 13/13: reward choices and sale items never overlap, the reward is free, sold or owned items can't be bought, and full slots need a replacement.
- Play Mode, clicking through the real buttons: took a reward into a free slot, bought two items, then took a reward with full slots (Cancel, then replace slot 2).

### Findings (500 runs per bot)
| Bot | No equipment | With equipment |
|---|---|---|
| Burn | 28% | 41% |
| Poison | 31% | 82% |
| Shield | 12% | 46% |
| Rage | 30% | 93% |
| Bleed | 22% | 84% |

- **Equipment is far too strong,** even with only 1.5 items on hand at battle 8. HP carries over and runs are lost narrowly in the last battles, so a few HP per battle compound. Even the smallest values are a lot: healing 1 per battle lifts Bleed 22% → 37%, and at most 2 Rage per battle lifts Rage 30% → 63%.
- **The items are uneven:** alone, Iron Bracers adds about 67 points, Plague Mask 44, Berserker Torc 40, Smoldering Gauntlet 9 and Bone Fetish 0.
- Raising Medium and Strong enemies' HP and damage by ×1.2 / ×1.3 brings the builds back to about 10–36%.

### Decisions
- **Equipment is part of the power curve** (the user's call): first even out the items against each other, then make the Medium and Strong tiers stronger so builds with equipment land in the target band.

### Next Steps
- The equipment balance pass (item values and limits, then enemy tiers).
- Show the equipped items in the battle UI.
- Re-evaluate action modules alongside equipment.

## Day 38 - Equipment Balance Pass

### Accomplished
- **Items evened out:** measured one at a time (only that item in the pool, on its own build's bot). Every item now adds about +13 to +21 points to its build, instead of +0 to +67. Items that fire often got limits instead of smaller numbers, since numbers can't go below 1.

| Item | New rule | Before | Now |
|---|---|---|---|
| Plague Mask | First 2 Poison face rolls each battle: 1 Poison | +44 | +18 |
| Smoldering Gauntlet | Each hit: 3 Burn | +9 | +13 |
| Barbed Hook | Unchanged | +20 | +19 |
| Tower Helm | Defense: 1 Fortify, up to 5 times each battle | +22 | +15 |
| Berserker Torc | First HP-losing hit each battle: 1 Rage | +40 | +19 |
| Bone Fetish | Unchanged (measured on Ritual + Poison) | — | +21 |
| Laurel Wreath | Heal 1 on victory | +47 | +13 |
| Iron Bracers | 1 Fortify at battle start | +67 | +15 |

- **Enemies made stronger:** Medium ×1.1 and Strong ×1.15 (HP and intent values, rounded the same way the simulator rounds).
  - Medium HP +2, and Retiarius's Trident Thrust 5 → 6.
  - Strong: Essedarius 42 → 48, War Elephant 56 → 64 (Stomp 6 → 7), Hoplomachus Champion 44 → 51, Bestiarius 40 → 46 (Unleash 5 → 6).
- **GDD:** new item values, the "equipment is part of the power curve" note with the tuning rule, and the enemy roster.

### Results (500 runs per bot, with equipment, final assets)
| Bot | No equipment (Day 35) | First item set | Now |
|---|---|---|---|
| Burn | 26% | 41% | 35% |
| Poison | 32% | 82% | 23% |
| Rage | 28% | 93% | 26% |
| Bleed | 21% | 84% | 30% |
| Ritual + Poison | 23% | 63% | 33% |
| Shield + Rage | 35% | 82% | 41% |
| Bleed + Rage | 23% | 93% | 38% |
| Ritual + Bleed | 16% | 62% | 16% |
| Shield | 10% | 46% | 10% |

### Findings
- The enemy scale is very sensitive. With equipment on, Strong ×1.2 drops the builds to 13–28%, and ×1.25 to 7–26%.
- Hybrids sit a little higher (38–41%), since they can use two archetypes' items.
- Shield alone (10%) and Ritual + Bleed (16%) are still weak, independent of equipment.

### Decisions
- Each item alone should add about +15 to +20 points to its build. Limits ("first 2 times each battle") are preferred over smaller numbers.
- The enemy tiers carry the power that equipment adds.

### Next Steps
- Show the equipped items in the battle UI.
- Re-evaluate action modules alongside equipment.
- Shield on its own and Ritual + Bleed.
- UI phase 2 (icons), the boss dice system, die reroll, centering offer cards.

## Day 39 - Equipment in the Battle UI

### Accomplished
- **Equipment strip:** three `EquipmentChipUI` chips under the player panel, mirroring the intent box under the enemy panel. Empty slots are hidden.
- **Tooltips:** `GameText.EquipmentTooltip` shows the name, rarity, colored description and the definitions of its keywords.
- **Flash on trigger:** an item's chip fades from gold back to normal over 0.6 seconds when it fires, using the same highlight color as the rolled face in the die strip.
- **Wiring:**
  - `TriggerSystem.ItemTriggered` is raised after an item's effects run.
  - `BattleManager.Start` connects it to `UIManager.FlashEquipment` before `StartBattle`, so BattleStart items (Iron Bracers, Bone Fetish) flash too.
  - The simulator doesn't listen, so simulations are unchanged.
- **CLAUDE.md** updated.

### Tests (Play Mode)
A real run with Iron Bracers, Smoldering Gauntlet and Barbed Hook:
- Iron Bracers gave 1 Fortify at battle start.
- A Sword hit added 3 Burn, and the Gauntlet's chip flashed.
- A Shield hit on a bleeding enemy fired Barbed Hook's extra hit, and its chip flashed. Enemy HP 10 → 6: Shield 2, Hook 1, and 1 Bleed tick at the end of the enemy's turn.

### Next Steps
- Re-evaluate action modules alongside equipment.
- Shield on its own and Ritual + Bleed.
- UI phase 2 (icons), the boss dice system, die reroll, centering offer cards.

## Day 40 - Action Modules Re-measured

### Accomplished
- **Simulator diagnostics:** `FightStats` counts the turns where a module's action was played and its damage. `RunSimulator.PlayModules` (off: bots buy modules but never play them) separates the gold cost of modules from the value of playing them.
- **GDD:** modules are a niche combo tool for very high-value faces.

### Measurements (600 runs, with equipment)
| | Burn | Poison | Ritual + Poison |
|---|---|---|---|
| No modules | 46% | 30% | 34% |
| Buy, never play | 41% | 23% | 29% |
| Buy and play | 32% | 22% | 33% |

- Buying modules costs gold that upgrades and equipment need (Burn −5, Poison −7).
- Playing them added nothing or less. Kindling replaced a Sword hit, and with Smoldering Gauntlet a hit is now worth 3 Burn more. 2 Poison from a value-2 Sword is no better than the Sword's own action.
- Module actions were played on 8–13% of turns. Rage, Bleed and Shield dice hardly have off-build faces, so those bots bought almost none (0.0–0.2).

### Findings
- **The simulation measured the wrong use.** The bots attach modules to low-value starter faces (Sword, Shield at value 2), where an alternative action changes nothing. Modules are meant for faces with a very high value, which spend that value on another action. The user's examples: Poison on a Hammer's Magic, or an Epic "Magic: gain Ritual equal to my value", a permanent buff from one roll.
- Average clear rates can't measure a niche combo tool: if most players never buy modules, they won't move the average.
- **The content isn't there yet:** base values are 3–4 at most, soft caps 5–7, and breaking past them is expensive. There are no high-value faces (the planned Hammer archetype) for modules to shine on.

### Decisions
- Modules stay as they are (the user's call). Revisit them once high-value face content exists, with combo tests (a high-value face with and without the module) instead of average bot results.

### Next Steps
- Shield on its own and Ritual + Bleed.
- The Hammer archetype (high-value faces), and Epic modules with it.
- UI phase 2 (icons), the boss dice system, die reroll, centering offer cards.

## Day 41 - Hammer Archetype & No Upgrade Cap

### Accomplished
- **Hammer archetype (GDD "Hammer"):** power stored on the die itself.
  - **Charge:** `DieCharge` holds battle-only temporary value per die face, owned by `BattleFlow`. At the end of the player's turn, the face you acted with spends its Charge, and every other face with the `charges` rule gains +1 (up to +5).
  - Effects get the battle value (value + Charge), so Value Is the Effect holds. Previews and die chips show it, e.g. "3 +4".
  - **Faces:**
    - War Hammer (Common, value 3): Attack only, Charge.
    - Maul (Common, value 3): Attack and Defense, Charge.
    - Anvil (Rare): block, or add its value as Charge to the highest other face.
    - Steady Hand (Rare): reroll and act again, or block.
    - Earthshaker (Epic): hit for the highest other face's value × its value, keeping that face's Charge.
  - **New effects:** `AddChargeToHighestEffectSO`, `RerollEffectSO` and `HighestFaceDamageEffectSO`, plus `FaceTag.Hammer`. `EffectContext` gained `charge`, `ValueOf(face)` and `rerollRequested`.
  - **Rerolls:** a reroll sends `PlayerActionSelectState` back to `PlayerRollState` without starting a new turn, so Block stays and the intent doesn't change. The simulator loops the same way.
  - **Simulator:** a focused Hammer bot, and rules for Anvil (aim Charge unless a big hit is coming) and Steady Hand (fish when another face has 2+ Charge).
- **No upgrade cap (the user's call):** `softCap`, `aboveSoftCapFactor` and the "Break cap" button are gone. An upgrade costs the rarity multiplier × the current value, with no jumps, and the "(cap N)" texts were removed from cards, tooltips and slots.
- **GDD and CLAUDE.md** updated.

### Tests
- **BattleFlow checks (9/9):**
  - Charge +1 per turn, capped at +5.
  - The Hammer hits for 2 + 5 = 7, then resets.
  - Earthshaker hits for the charged Hammer's 7 and keeps its Charge.
  - Anvil adds +2 Charge, and Steady Hand requests a reroll.
  - Venom Coating on a Hammer with +5 Charge applies 7 Poison: the module combo the user described.
- **Play Mode:** a War Hammer / Maul / Steady Hand / Anvil / Earthshaker die.
  - The chips showed growing Charge, and Earthshaker's preview followed it (6, then 7 damage).
  - Steady Hand's reroll reactivated the roll button with the same intent and Block kept.

### Results
| Hammer set | Hammer bot |
|---|---|
| First set (War Hammer 2, the only Common) | 7% |
| War Hammer +2 | 15% |
| + a second Attack-only Common | 7% |
| + a Charge face with Defense | 16% |
| **Maul 3 + War Hammer 3** | **32%** |

- The first set's problem was defenselessness: the Hammer bot lost the most HP per turn of any build (2.0) and filled only 3.2 of 6 faces with Hammer faces.
- Removing the soft cap moved the builds by 0–8 points (Burn and Rage +8).

All bots (500 runs): Burn 29% · Poison 25% · Shield 20% · Rage 31% · Bleed 26% · Hammer 32% · Ritual + Poison 30% · Ritual + Bleed 17% · Shield + Rage 35% · Bleed + Rage 39%. For the first time, every main build sits at 20–39%.

### Decisions
- War Hammer and Earthshaker stay Attack-only (the archetype's cost). Maul is the safe Hammer.
- Maul's Block tag gives the Shield archetype a face that grows during a fight (Shield bot 6% → 20%).
- Upgrades have no cap; the price growing with the value is the only brake.

### Next Steps
- Combo tests for action modules on charged Hammers, and Epic modules (e.g. "Magic: gain Ritual equal to my value").
- Ritual + Bleed (17%).
- UI phase 2 (icons), the boss dice system, centering offer cards.

## Day 42 - The Enchanting Table

### Accomplished
- **Action modules became enchantments** (the user's idea): a Market station, the **Enchanting Table**, where you pay to inscribe an enchantment into a die face's **Magic** action. GDD "Action Modules" was rewritten as "Enchanting Table".
  - **Magic only, one per face:** a face can be enchanted only if its Magic action is empty. Faces with their own Magic (Health Potion, Fury, Plague, Anvil...) are already magical. Most weapon faces, including the Hammers, can be enchanted.
  - **Same logic as face offers:** 3 enchantments in every Market. The Normal Market uses the face rarity weights (70/25/5), and the Grand Market shows only Epic enchantments. Build-aware tags, rerolls and prices (10/25/60) are the same as for faces.
- **Code:**
  - `ActionModuleSO` → `EnchantmentSO` and `Assets/Modules` → `Assets/Enchantments`. Renamed through Unity, so GUIDs, asset references and the economy config's pool were kept (`[FormerlySerializedAs]` for renamed fields).
  - `DieFace` holds a single `enchantment` (`CanEnchant`, `Enchant`), and enchanted faces show their name in purple.
  - `Market` has `enchantmentOffers` / `BuyEnchantment`, generated like face offers.
- **Market scene:** the card row is a layout group of three sections: FACES (3), ENCHANTING TABLE (3) and EQUIPMENT (2, Grand Market only). In the Normal Market the equipment section is hidden and the row re-centers. Die slots say "Can be enchanted", "Has its own Magic" or "Enchanted: X".
- **Enchantments:**
  - Common: Venom Coating, Kindling, Rend, and Ward (the former Defense module Iron Skin, now Magic).
  - Rare: Ignition (now Magic).
  - Epic: War Cry (was Rare: permanent Rage on a high-value face is as strong as Dark Rite), and the new **Dark Rite** (Ritual equal to the value), **Whirlwind** (hit twice for the value) and **Resonance** (the value as Charge to every other Charge face, keeping this face's Charge).
- **Simulator:**
  - Epic enchantments go on the highest-value face (Charge faces count +3), with per-enchantment play rules (`PlaysEnchantment`).
  - Combo hooks: `RunSetup` (change the starting die) and `BeforeBattle` (e.g. enchant at the first Grand Market).
  - Knobs renamed to `BuyEnchantments` / `PlayEnchantments`.

### Tests
- **Play Mode:** the Grand Market showed 3 Epic enchantments, faces and items. While enchanting, only faces with an empty Magic action were clickable (not the Health Potion). The War Hammer was enchanted with Dark Rite for 60 gold.
- **Resonance:** with value 3, it filled another War Hammer from +2 to the +5 cap while the host kept its +2. Attacking with the host still spends its Charge.

### Combo tests (the bot's own face enchanted from battle 5, right after the first Grand Market)
| Combo | Without | With |
|---|---|---|
| Hammer + Whirlwind | 31% | 64% |
| Hammer + War Cry | 30% | 60% |
| Rage + Whirlwind | 24% | 44% |
| Rage + War Cry | 32% | 38% |
| Ritual + Poison + Dark Rite | 30% | 33% |
| Hammer + Resonance (first version) | 31% | 25% |
| Hammer + Resonance (keeps its Charge, with a better play rule) | 32% | 30% |

### Findings
- **Whirlwind and War Cry on a charged Hammer are the run-changing combos the user described.** On a charged Hammer at about 8, Whirlwind hits 2 × 8.
- **Resonance stays neutral.** The +5 Charge cap limits it: other Hammers fill up on their own anyway. Keeping its own Charge removed the harm, but not the limit.
- **Dark Rite has no high-value host yet.** The Ritual + Poison bot's best face is about 3.6, so it adds little.
- **In real runs, bots never reach an Epic enchantment:** none had 60 gold at battle 5, since they spend everything at the Grand Market. An Epic enchantment is a real save-vs-tempo decision.
- Buying Common enchantments drains the Burn and Poison bots' gold. That is the bots buying every enchantment that fits, not the enchantments themselves.

### Decisions
- Whirlwind and War Cry stay as they are (the user's call): they are the intended game-breaking combos, they cost 60 gold saved for a Grand Market, and most players won't buy them.
- Resonance keeps its host's Charge. It is still a weak Epic and is the first candidate to revisit.

### Next Steps
- Revisit Resonance, or replace it with another Epic.
- Ritual + Bleed (17%).
- UI phase 2 (icons), the boss dice system, centering offer cards.

## Day 43 - Resonance Breaks the Charge Cap

### Accomplished
- **Resonance's Charge ignores the +5 cap** (the user's call). `DieCharge.Add` has a `capped` flag. Capped Charge (the +1 every turn, and Anvil) stops at +5 but never pulls Charge that is already above the cap back down.
- **Simulator:** Resonance is played when the Charge it hands out (its battle value to each other Charge face) is more than the hit it would deal now.
- **GDD:** the Resonance row and the Charge cap rule.

### Tests
- **Rule checks (5/5):**
  - A War Hammer at the +4 cap went to +11 from a value-7 Resonance (3 + 4 Charge).
  - The Resonance face kept its +4.
  - The next turn didn't pull the +11 back to the cap.
  - That Hammer then hit for 3 + 11 = 14 and reset.

### Combo tests
| Setup | Without | With Resonance |
|---|---|---|
| The bot's own die, enchanted from battle 5 | 32% | 26% |
| 3 War Hammers from the start, one with Resonance | 33% | 42% |

### Findings
- **Resonance is a die-density payoff,** like Plague: it rewards a die full of Hammers (+9 points with three).
- In the bot's normal runs, the enchanted face has only 0.8 other Charge faces next to it at battle 5, so Resonance has little to fill, and the result is neutral within noise.

### Decisions
- Resonance stays as an Epic for players who build deep into Hammer.

### Next Steps
- Ritual + Bleed (17%).
- UI phase 2 (icons), the boss dice system, centering offer cards.

## Day 44 - Boss Dice: The Minotaur

### Accomplished
- **GDD (boss design brought up to date):**
  - The Minotaur's Charge became **Stampede**, so it doesn't clash with the Hammer's Charge. It keeps rolling during the countdown, and a second Stampede counts as a Gore.
  - Medusa targets the highest **battle value** (charged Hammers). Petrify lasts 2 turns, and a stone face's enchantment doesn't work.
  - The Hydra: Bleed also stops regeneration, and every archetype has a path.
  - The run ends after the final boss, and bosses are built in stages.
- **Architecture:**
  - `IEnemyBrain`, implemented by `EnemyBrain` (intents) and `BossBrain` (dice); `BattleFlow`, `BattleManager.brain` and the simulator only use the interface.
  - `BossSO`: name, HP, 6 faces and countdown settings. Boss faces are normal `DieFaceSO` assets with one action each.
  - `EncounterConfigSO.bossSlots` (`BossSlot { battle, bosses }`): a slot without a boss keeps using normal enemies of the next tier.
  - `EffectContext.targetDie` is ready for Medusa.
- **The Minotaur (battle 4):**
  - Die: Gore ×2 (3 damage), Trample ×2 (2 damage, 2 block), Hide (4 block) and Stampede.
  - Stampede (`StartCountdownEffectSO`) starts a 3-turn countdown to 12 damage. Losing 8 HP from hits during it staggers the Minotaur and cancels it; blocked damage and Poison or Bleed ticks don't count.
- **UI:**
  - The boss's 6 faces sit under the intent box, with the rolled one highlighted.
  - The intent shows the countdown and the stagger still needed, e.g. "Stampede: 12 damage in 2 turns (stagger: 6 more damage)".
- **End of the run:** winning battle 11 shows RUN COMPLETE, and New Run starts a new run.
- **Simulator:** fights bosses in their slots (`EnemyTier.Boss` labels them in `FightStats`), and bots block big hits through `IEnemyBrain.IncomingDamage`.

### Tests
- **BossBrain checks:**
  - The Stampede lands exactly at the end of the third turn.
  - Stagger works; blocked hits and Poison ticks don't stagger.
  - No second Stampede is rolled during a countdown.
  - The forecast includes the Stampede on its last turn.
  - One check failed at first because the Minotaur still had block from earlier Hide turns, an error in the test, and passed when rerun with the block reset.
- **Play Mode:**
  - A real Minotaur battle showed the boss die strip and the countdown text (3 → 2 → "at the end of this turn"), with the stagger meter shrinking with each hit.
  - Winning battle 11 showed RUN COMPLETE, and New Run went back to RunStart with a fresh run.

### Balance
- The first numbers (HP 40, Gore 5, Stampede 18) killed 58–97% of runs at battle 4. Simulation brought them to HP 26, Gore 3, Trample 2, Hide 4, Stampede 12 and stagger 8.
- **Bot fix:** bots inscribed Common enchantments on starter faces and played Kindling instead of hitting, so the Burn bot died at battle 4 twice as often (52% → 23% without them). Bots now only buy Epic enchantments, which also matches the design: most players won't enchant in most runs.
- Results (400 runs): Burn 25% · Poison 22% · Shield 20% · Rage 26% · Bleed 21% · Hammer 24% · Ritual + Poison 25% · Ritual + Bleed 13% · Shield + Rage 30% · Bleed + Rage 31%.
- Deaths at battle 4 range from 1% to 32% by build (0–10% against the Medium enemy before). Burn struggles most (32%): it starts slowly and has to block the Stampede.

### Next Steps
- Medusa (battle 8): Petrify through `targetDie`, battle-scoped stone faces.
- The Hydra (battle 11): heads, regrowth and regeneration.
- Ritual + Bleed (13%), UI phase 2 (icons).

## Day 45 - Medusa

### Accomplished
- **Medusa (battle 8), HP 50.** Die: Gaze ×2, Serpent Bite ×2 (4 damage), Coil (4 + 3 per stone face) and Stone Skin (8 block).
- **Petrify:**
  - Gaze petrifies the player's highest battle-value face (value plus Charge) that isn't stone yet, for its value in turns (Gaze has value 2, so Value Is the Effect holds).
  - A stone face's only action is **Defense: gain 2 block**; its own actions and its enchantment don't work. It wears off after the player's next 2 turns.
  - A stone Hammer keeps its Charge, but rolling it while it is stone spends it.
- **Architecture:**
  - `Petrification` is battle-only, owned by `BattleFlow` like `DieCharge`. `BattleFlow.CanAct` and `PlayAction` handle stone faces.
  - `IEnemyBrain.Bind(flow)` gives boss effects the player's die, stone faces and Charge (`EffectContext.targetDie`, `targetPetrification`, `targetCharge`).
  - New effects: `PetrifyHighestFaceEffectSO` and `PetrifiedFacesDamageEffectSO` (Coil).
- **UI:** stone faces show "STONE N" in grey on the die strip, with a tooltip line. The rolled-face title says Petrified, and only the Defense button stays active ("2 block (petrified)"). `UIManager.charge` became `UIManager.flow`, so previews see both Charge and stone faces.
- **GDD and CLAUDE.md** updated.

### Bug found in Play Mode
- The intent said "petrify War Hammer", but other faces turned to stone. Medusa showed a target at the start of the player's turn and picked again when she acted; attacking with the War Hammer spent its Charge in between, which changed the highest face.
- **Fix:** `BossBrain` locks the target (`context.targetFace`) when it rolls the intent, so the telegraphed face is always the one that turns to stone, as the GDD promises.

### Tests
- **Checks (15/15):**
  - The charged War Hammer (3 + 5) is targeted before a value-6 Venom, and a second Gaze takes Venom.
  - Coil's forecast and hit with 2 stone faces are right.
  - A stone face gives 2 block, its hit and enchantment do nothing, and it wears off after 2 turns.
  - The locked target holds even after the Hammer spends its Charge.
- **Play Mode:** in a real Medusa battle, both Gazes petrified the face named in the intent (screenshot: "STONE 2" War Hammer, next Gaze aiming at the charged Maul).

### Balance (400 runs)
| Coil bonus per stone face | +1 | +2 | +3 | +4 |
|---|---|---|---|---|
| Average deaths at battle 8 | 20% | 20% | 20% | 21% |

- The bonus barely matters: Coil is one face of six, and usually lands with 1–2 stone faces. **+3** was chosen (the user's call) for a clearer "don't let two faces turn to stone" threat (4 + 6 = 10).
- With Medusa: Burn 26% · Poison 22% · Shield 35% · Rage 31% · Bleed 26% · Hammer 28% · Ritual + Poison 24% · Ritual + Bleed 16% · Shield + Rage 37% · Bleed + Rage 37%.
- Deaths at battle 8 are 10–29%, about the same as the Strong enemy it replaced. Shield handles her best (10%): its faces have low values, and a stone face's 2 block suits it.

### Next Steps
- The Hydra (battle 11, final boss): heads, regrowth (stopped by Burn), regeneration (stopped by Burn or Bleed).
- Ritual + Bleed (16%), UI phase 2 (icons).

## Day 46 - The Hydra, Final Boss

### Accomplished
- **The Hydra (battle 11, the final boss), HP 50.** All three bosses now exist, and beating the Hydra completes the run.
  - **Heads:** starts with 2, at most 3.
  - **Cutting:** a single hit that costs it 10+ HP cuts a head, never the last one.
  - **Regrowth:** cut heads grow back as two at the start of its turn, unless it has Burn (cauterized).
  - **Regeneration:** it heals 1 per head at the end of its turn, unless it has Burn or Bleed.
  - **Die:** Bite ×3 (1 damage per head), Thrash (3), Regrow (heal 5, blocked by Burn or Bleed) and Hiss (3 block).
  - **Intent text** shows the heads, what happens to cut ones and the regeneration, e.g. "Heads: 2 | a hit of 10+ cuts a head | regenerates 2 (blocked)".
- **Architecture:**
  - The heads rules live in `BossBrain` and are configured on `BossSO` (`startingHeads`, `maxHeads`, `cutDamage`, `regenPerHead`, `regrowthBlockedBy`, `regenBlockedBy`). Bosses without heads (`startingHeads` 0) are unaffected.
  - `EffectContext.heads` passes the head count after regrowth.
  - New effects: `PerHeadHitEffectSO` (Bite) and `HealUnlessStatusEffectSO` (Regrow).
- **GDD and CLAUDE.md** updated.

### Tests
- **Rule checks:**
  - Bite hits once per head, and the forecast includes regrowing heads.
  - A 5 hit doesn't cut and a 9 hit does (tested with the earlier cut-at-8 threshold).
  - A cut head grows back as two, and doesn't under Burn.
  - Burn and Bleed block regeneration, and Burn blocks Regrow.
  - The last head is never cut, and regrowth stops at the head cap.
  - The two failures in the first run were in the test script (a wrong expectation, then a string-parsing error), not in the code.
- **Play Mode:** a charged War Hammer cut a head while the Hydra had Burn, so it didn't grow back (2 → 1). Beating the Hydra showed RUN COMPLETE.

### Balance
- **First numbers** (HP 60, 3 heads up to 6, cut at 8+, Bite 2): the Hydra killed 75–81% of the runs that reached it, against 37% for the Strong enemy that used to be in that slot.
- **Cause:** players reach the last battle worn down, and the many bites plus grown heads dealt about 3.4 damage per turn (Strong enemies: about 1.7). Lowering HP, regeneration or the head count one at a time didn't fix it; damage and the head cap had to come down.
- **Final numbers:** the Hydra kills about 37% of the runs that reach it, as hard as the old Strong enemy overall.

| Build | Dies to the Hydra | Clears the run |
|---|---|---|
| Burn | 3% | 30% |
| Poison | 52% | 16% |
| Shield | 41% | 27% |
| Rage | 42% | 31% |
| Bleed | 42% | 31% |
| Hammer | 54% | 21% |
| Ritual + Poison | 47% | 18% |
| Ritual + Bleed | 39% | 19% |
| Shield + Rage | 30% | 35% |
| Bleed + Rage | 31% | 43% |

### Open point
- **The paths are uneven.** Burn barely ever dies to the Hydra, since it stops both regrowth and regeneration (the Heracles path, as designed, but too easy).
- Hammer and Poison struggle most: Hammer's big hits keep cutting heads, and Poison's hits are too small to cut and too slow against regeneration.
- Bots always attack, while a player can wait (e.g. Burn first, then the big Hammer hit). This is recorded in the GDD as a known open point, to revisit with play-testing.

### Next Steps
- Even out the Hydra's paths (options: Poison also stops regeneration; Burn stops only regrowth), after play-testing.
- Ritual + Bleed (19%) and Poison (16%) clear rates.
- UI phase 2 (icons), centering offer cards.

## Day 47 - Roadmap & Rerolls

### Accomplished
- **Play-test feedback (the user, two runs: Poison and Bleed):**
  - The game felt too easy, as if it played itself.
  - Battles offered little choice beyond the rolled face's one or two actions.
  - Choices rarely mattered and rarely carried risk.
- **Roadmap (`Docs/Roadmap.md`),** inspired by Slay the Spire, in this order:
  - A. Battle depth: rerolls, reactive enemy intents, then difficulty and gold.
  - B. Run structure: a branching map with elites, rests, events and the Market as nodes.
  - C. Multiple enemies.
  - D. Heroes and meta progression: the current die is the Knight; a main menu and menu market; difficulty levels; a save system.
  - Principles: unlocks add variety, not power; no hero's starting die may be stronger than another.
- **Rerolls (GDD "Rerolls"):**
  - 2 rerolls per battle, refilled every battle (`RunData.rerollsPerBattle`). They are spent after the die lands and before an action; several can be spent on the same turn.
  - The new face must be used. The enemy's intent stays and Block stays, since it isn't a new turn. FaceRolled items fire again.
  - `RerollPool` (`Left`, `UsedThisTurn`, `UsedThisBattle`) is owned by `BattleFlow` and passed to effects as `context.rerolls`, ready for the reroll content.
  - A "Reroll (N)" button sits next to Roll, active only after the die lands.
  - Bots reroll stone faces, faces that can't block a big incoming hit, and faces well below the die's average value (keeping their last reroll for big hits).
- **GDD and CLAUDE.md** updated.

### Tests
- **Checks (7/7):**
  - 2 rerolls at battle start; no reroll before the die lands or once they run out.
  - Block and the intent stay.
  - The per-turn count resets each turn while the pool doesn't refill; a new battle refills it.
  - Plague Mask fires on rerolls too.
- **Play Mode:** the real button rerolled Ember → Shield → Sword (2 → 1 → 0), then a third click did nothing.

### Results (400 runs per bot)
| Bot | No rerolls | 2 rerolls |
|---|---|---|
| Burn | 30% | 33% |
| Poison | 21% | 27% |
| Shield | 26% | 28% |
| Rage | 28% | 29% |
| Bleed | 34% | 37% |
| Hammer | 21% | 21% |
| Ritual + Poison | 19% | 23% |
| Ritual + Bleed | 20% | 21% |
| Shield + Rage | 37% | 38% |
| Bleed + Rage | 42% | 50% |

Rerolls add about 0–8 points with the bots' simple policy. The difficulty pass comes later in Phase A.

### Decisions
- Rerolls don't change the enemy's intent, and the rerolled face must be used (the user's call).
- Steady Hand stays as it is: it rerolls without spending the pool, so it is still its own thing.
- **Approved reroll content set:**
  - Loaded Die (Epic item: +1 reroll at battle start).
  - Patient Blade (Common: value + 1 per reroll left).
  - Gambler's Strike (Rare: hits again if you rerolled this turn).
  - Fate's Edge (Epic: value × rerolls used this battle).
  - Lucky Charm (Rare enchantment: gain 1 reroll).

### Next Steps
- The reroll content set above.
- Reactive enemy intents (Slay the Spire style).
- Difficulty and gold pass (bots about 5–15%).

## Day 48 - Fortune: Reroll Content

### Accomplished
- **Fortune content** (new `FaceTag.Fortune`, so the Market offers it to a player building around rerolls):
  - Patient Blade (Common, value 3): Attack: my value + 1 per reroll left; Defense: block.
  - Lucky Buckler (Common, value 2): Attack: damage; Defense: block and 1 reroll.
  - Gambler's Strike (Rare, value 3): hit, and hit again if you rerolled this turn.
  - Fate's Edge (Epic, value 2): my value × rerolls used this battle.
  - Loaded Die (Epic item): +1 reroll at battle start.
  - Lucky Charm (Rare enchantment): Magic: gain 1 reroll.
- **New effects:** `GainRerollEffectSO` (`fixedAmount`), `RerollScaledDamageEffectSO` (`RerollsLeft` / `UsedThisBattle`) and `IfRerolledEffectSO`.
- **UI:** action previews now get the reroll pool, so Patient Blade shows its real damage, and it drops as rerolls are spent.
- **Simulator:**
  - A focused Fortune bot.
  - Lucky Buckler defends (filling the pool).
  - A die with Patient Blade only rerolls to survive (stone faces, big incoming hits).
- **GDD and CLAUDE.md** updated.

### Tests
- **Checks (6/6):**
  - Loaded Die starts the battle with 3 rerolls.
  - Patient Blade hits 2 + 3 with 3 left (at its first value of 2).
  - Gambler's Strike hits 3, or 3 + 3 after a reroll.
  - Fate's Edge hits 2 × 4 used = 8.
  - Lucky Charm on a Sword gives +1 reroll.
- **Play Mode:** with the real buttons, Patient Blade's preview dropped from 5 to 3 as rerolls were spent, and Gambler's Strike's preview showed both hits after a reroll.

### Balance (400 runs, Fortune bot)
| Patient Blade | Lucky Buckler | Fortune |
|---|---|---|
| 2 | — (none yet) | 1–3% |
| 2 | 2 | 7% |
| **3** | **2** | **15%** |
| 3 | 3 | 11% |
| 4 | 2 | 13% |
| 4 | 3 | 13% |

- Rerun with 500 runs, the chosen setting (Patient Blade 3, Lucky Buckler 2) gave 8%, so Fortune is about 10% with wide noise.
- All bots (500 runs): Burn 26% · Poison 22% · Shield 18% · Rage 32% · Bleed 40% · Hammer 18% · Fortune 8% · Ritual + Poison 22% · Ritual + Bleed 26% · Shield + Rage 30% · Bleed + Rage 49%.

### Findings
- **With only Patient Blade as a Common, Fortune cleared 1–3%,** like Hammer before Maul.
- **Lucky Buckler's value barely mattered (2–4) until the bot played it right.** The bot attacked with Buckler instead of defending to fill the pool, and spent the rerolls Patient Blade wants kept. Fixing that lifted Patient Blade to 6.4 damage per roll.
- **Fortune dies most at the bosses:** the Minotaur (28%; a slow start like Burn's) and Medusa (26%; she petrifies the grown Patient Blade, a nice interaction).
- "Save or spend" is the hardest decision for a simple bot, so Fortune likely plays stronger for a person than the simulation shows.

### Decisions
- Lucky Buckler is Fortune's second Common (the user's call): Defense fills the reroll pool, which is how Fortune grows during a fight.
- Fortune stays at about 10% for now; it gets balanced with every other build in the Phase A difficulty pass.

### Next Steps
- Reactive enemy intents (Slay the Spire style), Phase A step 2.
- Difficulty and gold pass (bots about 5–15%).

## Day 49 - Enemy Traits

### Accomplished
- **Enemy traits (GDD "Enemy Traits"),** inspired by Slay the Spire: always-on rules that react to the player's choices.
  - `EnemySO.traits` lists `EnemyTrait`s. The triggers are `HitByPlayer`, `LostHPFromHit`, `PlayerChoseAction` (with an optional action and a "while it has block" condition), `PlayerRerolled`, and `Passive` (shown only).
  - `EnemyTraitSystem` runs them: the equipment trigger idea from the enemy's side. Effects run by a trait never fire traits.
  - Traits show as reddish chips with tooltips where the boss die strip sits (bosses show their die instead), and flash when they fire.
- **New statuses:** Vulnerable (+1 per hit taken), Weak (−1 per hit dealt) and Entangled (no rerolls). Each loses 1 stack at its owner's turn end.
- **New effect:** `RetaliateDamageEffectSO` (Thorns/Parry damage, no Rage, no source, so it can't loop).
- **Assignment:**

| Enemy | Change |
|---|---|
| Murmillo | Curl Up: the first HP-losing hit gives it 3 block |
| Retiarius | Net also Entangles |
| Dimachaerus | Parry: choosing Attack while it has block deals you 2 |
| Arena Lion | Enrage: +1 Rage on the first 2 HP-losing hits each battle; Roar removed |
| Venefica | New Hamstring intent (Weak 2) |
| Secutor | Second Wind: below half HP it drops its shield and switches to Frenzied Slam 6 and Wild Swing 3 (`EnemySO.secondWindIntents`) |
| Essedarius | Momentum: +1 Rage per reroll you spend |
| War Elephant | Thorns: 1 damage per hit you land |
| Hoplomachus Champion | Challenge: +1 Rage when you choose Defense |
| Bestiarius | Whip also applies Vulnerable |

- Parry was first written as "each hit while it has block". Block is gone by the time the hit lands, so it became "choosing Attack while it has block".

### Tests
- **Checks (15/15):** Thorns, Enrage (and Poison ticks not counting), Curl Up once, Challenge, Parry (only with block), Momentum, Entangled (one turn), Weak, Vulnerable and Second Wind.
- **Play Mode:** War Elephant's Thorns dealt 2 back to Twin Axes, and its chip flashed. A single chip stretched across the row, so the chips got a fixed width.

### Balance
| Arena Lion setting | Deaths to the Lion (5-bot sample) |
|---|---|
| No trait | 88 |
| Enrage, no cap, with Roar | 431 |
| Enrage, at most 4, no Roar | 280 |
| Enrage, at most 3, no Roar | 203 |
| Enrage, at most 2, with Roar | 231 |
| **Enrage, at most 2, no Roar** | **106** |

- Uncapped Enrage made the Arena Lion the deadliest enemy in the game, more than any boss.
- **Traits became the first difficulty step:** bot clear rates fell from 8–49% to 2–19%, close to the Phase A target for bots (5–15%). The bots don't play around traits; a player who reads them does better.

## Day 50 - Market Locks, Hover Help, Grand Market Upgrades

### Accomplished (the user's play-test requests)
- **Market locks:**
  - 5 gold (15 in the Grand Market). A locked face, enchantment or item survives rerolls and waits in the next Market that sells that kind (items only in a Grand Market).
  - Unlocking is free, without a refund; buying releases the lock.
  - `RunData.lockedFaces` / `lockedEnchantments` / `lockedEquipment`, placed first by the next Market.
  - Every sale card has a Lock button.
- **Hover help:**
  - Die faces and equipment slots in the Market show their full description.
  - Every highlighted keyword (Ritual, Bleed, Poison...) shows its definition when the pointer is over the word. `GameText.Colorize` now wraps keywords in TMP links in one pass, and `KeywordLinkTooltip` shows the hovered one.
  - This works in the Market, RunStart and Battle (rolled face, intent, action buttons). The tooltip panel became `Assets/UI/Tooltip.prefab`, shared by all three scenes.
- **The Grand Market upgrades every face** (the Normal Market still can't upgrade Epics).
- **Boss healing 50% → 30% of max HP:** at 50%, boss fights felt safe in play-tests ("my HP will come back anyway").

### Tests
- **Checks (11/11):**
  - Lock costs (5/15).
  - Locked offers survive a reroll and appear in the next Market.
  - Unlocking is free; buying releases the lock; sold offers can't be locked.
  - A locked item skips Normal Markets and appears in the next Grand Market.
  - The Grand Market upgrades every face, while the Normal Market still can't upgrade Epics.
- **Play Mode:**
  - A locked card stayed through a reroll, and a die face's tooltip was right.
  - Pointer hit-testing at the position of "Fortify" on a card found the link and showed its definition (screenshot).
- **Simulation:** with 30% healing and Grand Market upgrades, bots clear 1–14%.

## Day 51 - Essedarius: Threats and Momentum

### Accomplished
- **The user's design** (Slay the Spire's Gremlin Nob as the reference): an enemy whose own difficulty pushes the player to reroll, while rerolling makes it stronger. The first Momentum version had weak attacks, so without rerolls it never grew and was no threat.
- **Threat intents** (`PunishChoiceEffectSO`) hit on the enemy's turn only if the player's action this turn met a condition. `BattleFlow` records the player's last action for them.
  - Essedarius: Run Down (7 damage if you act with a face below 5, ×2), Lash (4 damage if you choose Attack, ×1), Wheel Spikes 3, Rein In: block 3. Battle Cry was removed, since Momentum is now its Rage source.
- **Faces without a numeric value** (`DieFaceSO.hasNoValue`, for future special faces) count as below any threshold.
- **Bots:**
  - Threat intents don't count as incoming damage; `IEnemyBrain.ThreatAgainst` reports them.
  - Bots reroll a face that no action can keep safe, and switch to a safe action when they can.

### Tests
- **Checks (8/8):**
  - Run Down's text is right, and it doesn't count as incoming damage.
  - A value-2 Sword is punished and a value-4 Serrated Blade isn't (at the first threshold, 3).
  - Lash punishes Attack, not Defense.
  - Rerolling to dodge gives Essedarius 1 Rage, which raises the threat from 6 to 7.

### Balance
| Run Down | Deaths to Essedarius (7-bot sample) |
|---|---|
| Below 3 for 6 | 20 |
| Below 4 for 6 | 30 |
| Below 4 for 8 | 34 |
| **Below 5 for 7** | **62** |

- Bots dodge threats perfectly, so Essedarius was the least deadly enemy at first. "Below 5" puts most faces under threat, so the pressure to reroll (and feed Momentum) is real.
- All bots: 0–12%.

### Next Steps
- **Multiple enemies (Roadmap Phase C),** agreed design:
  - Click an enemy to target it; the default target is the front living enemy.
  - Enemy groups of up to 3.
  - Each enemy has its own intent and traits.
  - Area-effect architecture first, content later.
  - The Hydra becomes a body plus head enemies: killing a head cuts it, and cut heads regrow as two unless the body has Burn.
- **In stages:** infrastructure with a single-enemy regression test, then targeting UI and groups, then the Hydra.
- After that, the difficulty and gold pass (Phase A step 3).

## Day 52 - Multiple Enemies: Battle Rules

### Accomplished
- **Stage 1 of Roadmap Phase C** (GDD "Multiple Enemies"): the battle rules now hold a list of enemies instead of one.
  - `BattleEnemy` (entity, brain, its own traits). `BattleFlow.enemies` is in turn order, and dead enemies stay in the list but are skipped.
  - **Target:** `BattleFlow.Target` (`SelectTarget`) is the enemy the player's attacks, statuses and items hit. It defaults to the front living enemy and falls back there when the target dies.
  - **Turns:** every living enemy picks its own intent at the player's turn start. They act front to back, and their statuses tick separately. The battle is over when the player or every enemy is dead.
  - **Traits:** each enemy has its own trait system. `EnemyTrait.requireTargeted` limits a trait to the enemy being attacked (Parry).
  - **Area effects:** `EffectContext.opponents` / `AllOpponents` list every living opponent. This is architecture only; no area content yet.
  - **Bots:** `IncomingDamage` and `ThreatAgainst` sum the living enemies, and bots focus the living enemy with the lowest HP.
- **`EnemyBrain` is now plain C#** (like `BossBrain`), created per enemy per battle; its component was removed from the Battle scene. That makes spawning several enemies simple, at the cost of no longer seeing the intent in the Inspector.
- The battle UI still shows one enemy (the front one).

### Tests
- **Checks (15/15),** with a hand-built two-enemy battle:
  - Attacks hit only the target.
  - A dead target falls back to a living enemy, and a dead enemy can't be selected.
  - Parry fires only when it is the target.
  - Both enemies act.
  - The battle isn't over while one enemy lives.
  - The single-enemy constructor still works.
- **Single-enemy regression (1000 runs per bot):** the same within noise. Burn 12%, Poison 5%, Shield 6–8%, Rage 5%, Ritual 0%, Bleed 6%, Hammer 1%, Fortune 2%, Ritual + Poison 8%, Ritual + Bleed 3%, Shield + Rage 12%, Bleed + Rage 6%.
- **Play Mode:** a full battle played through the real buttons, ending on the victory panel with no Console errors.

### Findings
- **Fixed in passing:** an enemy that died from its own status ticks still ran its brain's turn end. A dead Hydra could regenerate, and a dead Minotaur's Stampede could still hit. Dead enemies are now skipped.

### Next Steps
- **Stage 2:**
  - A panel, intent and trait strip for each enemy, spawned at battle start.
  - Click an enemy to target it.
  - Enemy groups of up to 3 with lower HP each, in the encounter config.
- **Stage 3:** the Hydra as a body plus 2 head enemies.

## Day 53 - Multiple Enemies: Targeting UI and Enemy Groups

### Accomplished
- **Enemy cards** (`EnemyCardUI`):
  - Every enemy gets a card at the top right, copied from a template in the Battle scene: name, HP, Block/Fortify, statuses, intent and trait chips.
  - The old enemy panel, intent box and trait strip became the template. The rolled face panel moved to the middle of the screen, and the boss die strip sits under the cards.
- **Click targeting:**
  - Clicking a card calls `BattleManager.OnEnemyClicked`, which selects the target during the roll and action phases.
  - The target's card shows a gold TARGET frame, and the action previews are rebuilt for the new target.
  - A defeated enemy's card is dimmed and shows "Defeated"; the target moves to a living enemy.
- **Enemy groups:**
  - `EnemyGroup` (name, tier, up to 3 members in turn order, HP multiplier), listed in `EncounterConfigSO.groups`.
  - `EncounterConfigSO.PickEncounter` is shared by `RunManager.PickEncounterForNextBattle` and the simulator. Lone enemies and groups are equally likely, and a group counts as meeting its members, so nothing is met twice in a run.
  - Duplicate members are named "Tiro A" / "Tiro B".
- **Simulator:** fights groups with up to 3 enemy objects. `RunSimulator.ForceEncounter` lets experiments fight one encounter at every battle of its tier.
- `EnemyBrain.DescribeCurrentIntent` returns nothing before the first intent is picked.

### Tuning
- **Method:** each encounter was forced at every battle of its tier for the 12 focused and hybrid bots (480 runs), then the HP taken and the loss rate per battle were compared.
- **The first try, every group at ×0.6, was far too hard:**
  - Tiro Pair cost 13 HP and Wolf Pack 10.8, while lone Weak enemies cost 2–7.
  - Net and Shield (Secutor + Retiarius) killed 52% of bots, while lone Medium enemies kill 2–24%.
  - The cause: two attackers deal double damage per turn while both live, even with less total HP.

| Group | Members | Tried | Chosen |
|---|---|---|---|
| Tiro Pair | Tiro, Tiro | ×0.35: 7.6 HP, ×0.45: 9.7 HP | **×0.4: 7.8 HP** |
| Wolf Pack | Arena Wolf ×2 | ×0.35: 7.2 HP, ×0.45: 8.7 HP | **×0.4: 8.6 HP** |
| Net and Shield | Secutor + Retiarius, then Murmillo + Retiarius | Secutor version ×0.35: 29% lost; Murmillo version ×0.6: 23% | **Murmillo + Retiarius ×0.5: 18.5% lost** |
| Poison and Blades | Dimachaerus, Venefica | ×0.35: 12.5%, ×0.5: 23.5% | **×0.45: 19% lost** |
| Beast Master | Arena Lion, Bestiarius | ×0.4: 26%, ×0.5: 46% | **×0.45: about 34% lost** |

- Two of the hardest Medium enemies together were too much. A tank in front of a dangerous back-liner works better and makes the target choice matter.
- **All bots with groups in the pool (1000 runs each):** 0–11% clear the run, 1–2 points below the rates without groups:

| Bot | Clear rate |
|---|---|
| Burn | 9% |
| Poison | 4% |
| Shield | 8% |
| Rage | 4% |
| Ritual | 0% |
| Bleed | 3% |
| Hammer | 3% |
| Fortune | 1% |
| Ritual + Poison | 5% |
| Ritual + Bleed | 2% |
| Shield + Rage | 11% |
| Bleed + Rage | 6% |

### Tests
- **Checks (7/7):**
  - Weak groups are picked about as often as expected (105 of 400 picks, about 2/7).
  - Picking a group marks every member met, and Tiro Pair never comes after meeting Tiro.
  - Repeats are allowed when the tier runs out.
  - Duplicate names get letters, member HP is scaled, and a lone enemy keeps full HP.
- **Play Mode:**
  - In Wolf Pack, clicking the second card through its real button moved the frame. The attack hit only that wolf.
  - When it died, its card dimmed and the target moved to the other wolf; the battle ended in victory.
  - Single-enemy (Thraex) and trait (Murmillo, Curl Up) cards look right. The trait chips were right-aligned and are now left-aligned.

### Known Issues
- A defeated enemy's card still shows its last statuses.
- The scene's placeholder enemy sprites overlap for group members.

### Next Steps
- **Stage 3:** the Hydra as a body plus 2 head enemies.
  - Killing a head cuts it, and cut heads regrow as two at the Hydra's turn start unless the body has Burn.
  - The body heals per head unless it has Burn or Bleed.
  - The battle ends when the body dies.
- The user's play-test of the cards, targeting and groups.

## Day 54 - Multiple Enemies: The Hydra's Heads

### Accomplished
- **The Hydra is a body plus head enemies** (stage 3 of Roadmap Phase C, GDD "Hydra").
  - Each head is a separate enemy (`Assets/Bosses/Hydra/HydraHead.asset`) with its own card, HP and intent. It joins behind the body through `BattleFlow.SpawnEnemy`.
  - **Cutting:** killing a head cuts it. At the start of the body's turn, every head cut since its last turn grows back as two. Dead heads are revived first, then new ones are spawned, up to 3 alive. Burn on the body stops this (the necks are cauterized).
  - **Grown heads** act from the next turn (`EnemyBrain.ClearIntent` / `HasIntent`).
  - **Regeneration:** the body heals 1 per living head at the end of its turn, unless it has Burn or Bleed.
  - **Victory:** the battle ends when the body dies, even with heads alive. Every boss is created with `BattleEnemy.endsBattleOnDeath`.
  - The body's intent shows its living heads, the cut ones (growing back or cauterized) and its regeneration.
- **Battle rules for joining enemies:**
  - `BattleFlow` takes an entity factory: a copy of the scene's enemy object in battle, a hidden object in the simulator, destroyed after the fight.
  - `EnemyAct` / `EndEnemyTurn` loop by index, since the list can grow.
  - `EnemyAdded` lets the UI add a card. Past 3 cards, every card shrinks to fit.
- **`Entity.Heal` does nothing to a dead entity;** `Entity.Revive` brings a head back with full HP and no statuses.
- **Removed:**
  - The old head rules: "a hit of 10+ cuts a head" and the head counter.
  - The body's Bite face, `PerHeadHitEffectSO` and `EffectContext.heads`.
- **Simulator:**
  - `DeathRateAt(battle, bots, runs)`: the share of the runs reaching a battle that die in it.
  - `HydraFocusBody`: bots go for the body unless Burn blocks regrowth.

### Tuning
- **Target:** the old Hydra's difficulty (it killed about 37% of the runs that reached it).
- **Method:** 12 focused and hybrid bots, 500 runs each.

| Version | Arrivals that die |
|---|---|
| Body 40, head Bite 2 (weight 3), Regrow ×2, Hiss ×2 | 74–78% |
| Body 30 | 73% |
| Body 40, head Bite 1 | 59% |
| Body 40, Bite 1, Regrow ×1, Hiss ×3 | 54% |
| Same, body 30 | 49% |
| Body 30, Bite 1 at weight 3, no regeneration | 34% |
| **Body 30, Bite 1 half the time (Bite 1 / Hiss 1), Regrow ×1, Hiss ×3, regeneration 1 per head** | **40% (33% in a later run)** |

- Body HP barely matters; the bites decide the fight.
- Removing regeneration also hit the target, but regeneration is what makes Burn and Bleed matter, so it stays.
- **The plan matters:** bots that keep killing heads die in 90% of their Hydra fights, against 33% for bots that go for the body.
- **Paths per build (1000 runs; share of arrivals that die):**

| Build | Hydra deaths |
|---|---|
| Burn | 20% (before: 3%) |
| Shield + Rage | 23% |
| Poison | 33% |
| Ritual + Poison | 33% |
| Shield | 35% |
| Rage | 44% |
| Bleed | 50% |
| Fortune | 50% |
| Bleed + Rage | 56% |
| Hammer | 57% (before: 54%) |
| Ritual + Bleed | 60% |

- **All bots (1000 runs each):** 0–13% clear the run, the same range as before.

### Tests
- **Checks (15/15):**
  - The heads join behind the body.
  - A killed head counts as cut, and the intent says it will grow back.
  - It grows back as two, and the grown heads don't act that turn.
  - Never more than 3 heads alive.
  - Burn cauterizes the necks and blocks regeneration.
  - Regeneration heals 1 per head, and Bleed blocks it.
  - A dead head can't be healed.
  - The body's death ends the battle with heads alive.
  - Without a factory nothing spawns.
- **Play Mode (battle 11):**
  - Clicking a head's card and killing it made two heads at the Hydra's turn; the cards shrank to fit 4.
  - Killing the body showed RUN COMPLETE with 3 heads alive.
  - No Console errors.

### Next Steps
- **The user's play-test of Phase C:** targeting, groups and the Hydra.
- **Open points:**
  - A defeated enemy's card still shows its statuses.
  - Placeholder sprites overlap for group members.
- **Area-effect content** (faces, enchantments, items) on top of `EffectContext.opponents`.
- **Then the difficulty and gold pass** (Phase A step 3), or the map (Phase B).

## Day 55 - Multiple Enemies: Small Fixes

### Accomplished
- **A defeated enemy's card shows no statuses.** `CombatantPanelUI` skips the status chips for a dead entity, so the card shows only "Defeated". This is display only; the entity's statuses are unchanged.
- **Enemy sprites no longer overlap.** `BattleManager.ArrangeEnemySprites` lays the placeholder sprites side by side on the right side of the screen, in turn order, and shrinks them when there are several. It runs at battle start and whenever an enemy joins (a Hydra head).
  - A dead enemy's sprite is hidden but keeps its place, so the others don't shift. A Hydra head that grows back shows again.
  - A lone enemy keeps its old position and size.

### Tests
- **Play Mode (battle 11):**
  - A dead head with 3 Bleed showed a "Defeated" card with no chips, and its sprite was hidden.
  - After the Hydra's turn the head grew back as two, and the 4 sprites stood side by side at a smaller size.
  - No Console errors.

### Next Steps
- **The user's play-test of Phase C:** targeting, groups and the Hydra.
- **Area-effect content** (faces, enchantments, items) on top of `EffectContext.opponents`.
- **Then the difficulty and gold pass** (Phase A step 3), or the map (Phase B).

## Day 56 - Area Effects: The First Set

### Accomplished
- **Design (GDD "Area Effects"):**
  - An area effect applies its full value to every enemy (Value Is the Effect, no split).
  - Area faces work as normal faces against one enemy.
  - Each enemy is a separate target: Rage per hit, hit triggers per enemy, Ritual per status.
  - Area effects hit the Hydra's heads too.
- **Three shared effects; all content reuses the existing damage, Burn and Poison assets:**
  - `ForEachOpponentEffectSO` runs its inner effects once per living opponent, with the target swapped.
  - `SpreadStatusEffectSO` copies the target's stacks to the other enemies.
  - `OverkillDamageEffectSO` carries a killing hit's excess (beyond HP, Block and Fortify) to the next living enemy.
- **New content, added to the Market, Enchanting Table and Grand Market pools:**

| Name | Kind | Rarity | Effect |
|---|---|---|---|
| Cleave | Face, value 3 | Common | Rage: every enemy |
| Wildfire | Face, value 2 | Rare | Burn: Burn on every enemy |
| Contagion | Face, value 2 | Rare | Poison: Poison, then copied to every other enemy |
| Follow Through | Face, value 3, Charge | Rare | Hammer: excess carries |
| Sweep | Enchantment | Epic | Every enemy |
| Censer | Item | Rare | 1 Poison on every enemy at turn end |

- **Bots:** Wildfire and Contagion use Magic unless a big hit needs blocking. Sweep is played against 2+ enemies.
- **Simulator:** damage per face now counts every enemy's HP, so area damage and Follow Through's carry are measured too.

### Tests
- **Checks (11/11):**
  - Cleave hits each of 3 enemies for value + Rage, with 3 hit events; against one enemy it is a normal hit, and its text is "3 damage to every enemy".
  - Wildfire puts value + Ritual Burn on each enemy.
  - Contagion takes the target from 4 to 6 Poison and copies 6 to the others; alone it applies 2 Poison.
  - Follow Through: 8 damage into 3 HP + 2 Block carries 3; without a kill nothing carries.
  - Sweep on a value-6 War Hammer deals 6 to each enemy.
  - Censer puts 1 Poison on each enemy at turn end.
  - Cleave at 8 kills both Hydra heads (2 cut) and hits the body.

### Balance
- **Method:** one Sword on the starter die was swapped for the face; HP taken was measured in Weak battles (no-shopping bot, 400 runs each):

| Face | Lone Tiro | Tiro Pair | Lone Arena Wolf | Wolf Pack |
|---|---|---|---|---|
| Sword | 6.7 | 8.2 | 5.7 | 9.0 |
| Cleave at 2 | 7.9 | 8.3 | 6.8 | 8.9 |
| Cleave at 3 | 6.8 | 7.5 | 5.8 | 8.1 |
| War Hammer | 5.6 | 7.4 | 5.1 | 8.3 |
| Follow Through | 5.7 | 7.2 | 5.2 | 7.8 |

- **Cleave 2 → 3:** at 2 it lost to Sword alone (Sword's Bleed is worth a lot) and only tied in groups. At 3 it plays like Sword alone and is better against groups.
- **Follow Through** plays like War Hammer alone and a little better against groups.
- **Whole runs (focused bots, 1000 runs):**

| Bot | With the new faces in the pool | Without them |
|---|---|---|
| Burn | 6% | 7% |
| Poison | 10% | 10% |
| Rage | 5% | 6% |
| Hammer | 2% | 3% |

  The new faces are a situational tool, not a power jump; most deaths are at the bosses.
- **The Hydra:** the Rage bot dies in 44% of its Hydra fights with or without Cleave in the pool. 3 damage rarely kills an 8 HP head by accident.

### Findings
- Bots only buy enchantments tagged with their archetype, so the untagged Sweep is never bought in simulation. Wildfire deals no damage alone (Burn is fuel), so its value only shows in Burn runs. Both need the user's play-test.

### Next Steps
- **The user's play-test** of the area content against groups.
- **Area content for Bleed and Shield** (second round).
- **Then the difficulty and gold pass** (Phase A step 3), or the map (Phase B).

## Day 57 - Starting Gold and the Starting Reroll

### Accomplished
- **New rules in the GDD** (the user's play-test ideas, agreed after discussion):
  - Starting gold and the starting reroll ("Run Start").
  - Consumable and Change ("Consumable and Change").
  - The Healer ("Market").
  - Encourage, planned ("Status Effects").
- **Every run starts with 20 gold** (`EconomyConfigSO.startingGold`).
- **The run start screen has a Reroll button:**
  - It uses the Normal Market's rising price (5, 10, 15... gold), counted by `RunData.startingOfferRerolls`.
  - It replaces all three offers and avoids the faces just shown (`StartingFaceOffers.Generate(..., avoid)`), still from different archetypes.
  - The logic lives in `StartingFaceOffers.Reroll`, shared by `RunManager.RerollStartingFaces` and the simulator.
  - `RunStartUI` shows the gold and wires the card buttons by index, so they keep working after a reroll.
- **Why gold instead of free rerolls:** one of three random faces decided too much of a run. Gold spent at the start is missing in the first Market, which makes it a Save vs. Tempo decision instead of a free gift.
- **Simulator:** bots start with the starting gold. Focused bots reroll up to twice when no offer fits their archetype (`UseStartingReroll`).

### Tests
- **Checks (10/10):**
  - The run starts with 20 gold; the first reroll costs 5, then 10, then 15.
  - Rerolled offers are three new faces from different archetypes.
  - A reroll is refused without enough gold or after choosing.
  - Starting die faces are never offered.
- **Play Mode:**
  - The Reroll button showed Lucky Buckler, War Hammer and Serrated Blade instead of Venom, Anvil and Fury. Gold went 20 → 15 and the next price showed 10.
  - Choosing War Hammer put it in the sixth slot and kept the 15 gold.
  - No Console errors.

### Balance (1000 runs per bot)

| Bot | 0 gold, no reroll | 20 gold, no reroll | 20 gold + reroll |
|---|---|---|---|
| Burn | 7% | 10% | 13% |
| Poison | 9% | 19% | 19% |
| Shield | 13% | 16% | 17% |
| Rage | 5% | | 9% |
| Bleed | 4% | | 7% |
| Hammer | 2% | | 4% |
| Fortune | 1% | | 2% |
| Ritual + Poison | 14% | | 27% |
| Ritual + Bleed | 4% | | 4% |
| Shield + Rage | 10% | | 12% |
| Bleed + Rage | 4% | | 7% |

- Most of the gain comes from the gold (an early upgrade); the reroll adds a little.
- Clear rates are now 0–27%, above the Phase A target for bots (5–15%). The starting gold stays: the freedom is the point. The Healer will soak up some gold, and the difficulty and gold pass will bring the numbers back.

### Next Steps
- **Consumable + Change + Health Potion** (Magic: heal 4, Consumable).
- **The Healer** in the Market, then the difficulty and gold pass.
- **Encourage:** its content set is to be discussed.

## Day 58 - Consumable and Change

### Accomplished
- **Consumable** (a keyword on an action, GDD "Consumable and Change"):
  - `DieFaceSO.consumableActions` lists actions that can be used once per battle.
  - `BattleFlow.consumables` (`Consumables`, battle-only) records the used ones per face instance, so every copy has its own use.
- **Change:** a used Consumable action becomes Change, which rolls the die again at once while the turn goes on.
  - It is not a reroll: it spends no rerolls, doesn't trigger Momentum or "if you rerolled" effects, and Entangled doesn't stop it.
  - Items, traits and threats don't see it as the turn's action.
  - At most 5 per turn (`MaxChangesPerTurn`).
  - A petrified face is never Change.
  - `PlayerActionSelectState` rolls again right away; the simulator loops like Steady Hand.
- **Health Potion:** Magic (Consumable): heal 4 (was a repeatable heal of 3). Once drunk, it is Change for the rest of the battle, so rolling it later is never a dead roll.
- **UI:**
  - Action previews show "(Consumable)" and "Change: roll again".
  - The die strip marks a used face "used: Change".
  - Consumable and Change are keywords with hover definitions.
- **Bots:** they skip a near-full-HP drink with a reroll and press Change on a used potion.
- **Why Change, not a small extra action:** the user pointed out that a potion with "Defense: 1 block" is as dead as one with nothing, since the player rerolls it either way. A dead roll goes against the design: the player always has something to do.

### Tests
- **Checks (11/11):**
  - The drink heals 4; afterwards the action is Change.
  - Change doesn't heal, rolls again, spends no reroll and doesn't add Momentum Rage.
  - Change leaves `LastActedFace` empty (threats ignore it), and Entangled doesn't block it.
  - The 5-per-turn cap holds and resets the next turn.
  - Each copy has its own use; a stone potion only defends; a new battle restores the potion.
- **Play Mode:**
  - The Magic button read "heal 4 (Consumable)", then "Change: roll again"; the chip showed "used: Change".
  - Two Changes in one turn rolled again each time (the second landed on Serrated Blade) with 2 rerolls still left.
  - No Console errors.

### Balance (6 focused bots, 800 runs)

| Potion | Burn | Poison | Shield | Rage | Bleed | Hammer |
|---|---|---|---|---|---|---|
| Old (heal 3 every time) | 15% | 19% | 17% | 8% | 6% | 4% |
| Consumable 4 + Change | 19% | 25% | 16% | 10% | 8% | 4% |
| Consumable 3 + Change | 16% | 16% | 19% | 6% | 7% | 3% |

- Change's die thinning costs little; the extra point of healing is most of the gain.
- The value stays 4 (one strong drink). The difficulty and gold pass will balance the total.

### Next Steps
- **The Healer** in the Market (10 HP, 15 then 25 gold, at most 2 per visit).
- **Then the difficulty and gold pass.**
- **Encourage:** its content set is to be discussed.

## Day 59 - The Healer

### Accomplished
- **Healer in every Market** (Normal and Grand, GDD "Market"):
  - 10 HP for 40 gold, once per visit.
  - `Market.Heal` / `CanHeal` / `GetHealCost`; the numbers live in `EconomyConfigSO` (`healAmount`, `healBaseCost`, `healCostStep`, `maxHealsPerVisit`).
  - The button sits on the right of the Gold / HP line. It is disabled at full HP or without the gold, and reads "Healer: done" once used. It was first placed under Reroll, where the screenshot showed it overlapping the first die slot.
- **Bots** that shop heal when below half HP (`UseHealer`, `HealBelowFraction`).

### Tests
- **Checks (6/6)** with the first plan (10 HP for 15, then 25 gold, 2 per visit):
  - No heal at full HP; healing stops at max HP.
  - The second heal costs 25, and no third heal in the same visit.
  - The price resets in the next Market (Grand too).
  - No heal without the gold.
- **Play Mode:** HP 20 → 30 → 40 and gold 60 → 45 → 20; then the button read "Healer: done" and was disabled.

### Balance (6 focused bots, 800 runs)

| Healer | Burn | Poison | Shield | Rage | Bleed | Hammer |
|---|---|---|---|---|---|---|
| None | 17% | 23% | 21% | 11% | 11% | 4% |
| 10 HP for 15 → 25 gold, 2 per visit (the first plan) | 35% | 62% | 19% | 52% | 48% | 26% |
| 10 HP for 15 gold, 1 per visit | 37% | 62% | 23% | 50% | 49% | 29% |
| 6 HP for 20 gold, 1 per visit | 25% | 36% | 19% | 28% | 27% | 11% |
| 5 HP for 20 gold, 1 per visit | 24% | 35% | 16% | 24% | 27% | 11% |
| **10 HP for 40 gold, 1 per visit (chosen)** | **20%** | **27%** | **18%** | **13%** | **13%** | **5%** |

### Findings
- HP is the tightest limit in the game. Cheap healing over 9 Markets undid the 30% boss heal's pressure, most of all for long attrition builds (Rage, Bleed).
- The heal amount mattered more than the per-visit limit: bots heal once, below half HP, and that one heal is enough.
- At 40 gold (two victories) a heal competes with an upgrade or two rerolls. Players will use it better than bots (e.g. right before a boss), so it stays expensive.

### Next Steps
- **The difficulty and gold pass:** starting gold, the 4-HP Consumable potion and the Healer all raised clear rates (focused bots now 5–27%).
- **Encourage:** its content set is to be discussed.
- **The user's play-test** of the starting reroll, Consumable / Change and the Healer.

## Day 60 - Difficulty and Gold Pass: Capping the Outlier Items

### Findings (simulation first)
- **Clear rates after the new economy** (starting gold, Consumable potion, Healer) ran from 0% to 36%, against the Phase A target of 5–15% for bots:

| Bot | Day 52 | Before this pass |
|---|---|---|
| Ritual + Poison | 8% | 36% |
| Poison | 5% | 27% |
| Burn | 12% | 21% |
| Shield | 7% | 19% |
| Shield + Rage | 12% | 15% |
| Rage | 5% | 13% |
| Bleed | 6% | 10% |
| Bleed + Rage | 6% | 10% |
| Hammer | 1% | 7% |
| Ritual + Bleed | 3% | 5% |
| Fortune | 2% | 3% |
| Ritual | 0% | 0% |

- **Where bots die:**
  - The Minotaur stops slow starters (Ritual 61%, Fortune 30%, Burn 24% of runs).
  - Medusa stops the middle builds (Bleed + Rage 46%, Bleed 39%, Rage 37%, Hammer 36%).
  - The Strong tier takes 10–35% and the Hydra 4–11%.
  - The Weak tier is almost riskless (0–1% deaths, 5–15 HP lost).
  - Bots enter the first Grand Market with 54–62 gold.
- **Raising every enemy is the wrong tool:** Medium and Strong HP ×1.2 took Rage 13% → 5% and Bleed 10% → 3%, but Ritual + Poison only 36% → 25% (damage ×1.25: 9%, 4%, 24%). It crushes the builds that are already weak and widens the spread.
- **The outliers were two uncapped items:**
  - **Censer** (Day 56): 1 Poison to every enemy every turn, boosted by Ritual, on 75% of Poison bots at battle 8. Without it, Poison 27% → 17% and Ritual + Poison 36% → 15%.
  - **Smoldering Gauntlet:** 3 Burn per hit, ignited at once by Ember, on 92% of Burn bots. Without it, Burn 22% → 12%; Ember −1 only gave 17%.

### Accomplished
- **Censer:** at most 3 per battle (the first 3 turns), like Plague Mask's cap.
- **Smoldering Gauntlet:** 1 Burn per hit (was 3).
- GDD rule: items that fire on every hit or every turn need small numbers or a cap.

### Experiments (1000 runs)

| Change | Result |
|---|---|
| Censer, at most 3 per battle | Poison 17%, Ritual + Poison 26% |
| Censer, at most 2 per battle | Poison 17%, Ritual + Poison 19% |
| Gauntlet, at most once per turn | Burn 22% (bots rarely hit twice a turn) |
| Gauntlet, 2 Burn per hit | Burn 20% |
| Gauntlet, first 4 hits per battle | Burn 19% |
| Gauntlet, 1 Burn per hit (chosen) | Burn 16% |
| Ritual + Poison without Bone Fetish | 21% |

- Ritual + Poison's strength is the hybrid synergy itself (Ritual adds to every Poison application), not one item, so it stays.

### Results (1000 runs)

| Bot | Clear rate |
|---|---|
| Ritual + Poison | 23% |
| Shield | 16–20% |
| Poison | 18% |
| Burn | 15% |
| Rage | 13% |
| Bleed | 12% |
| Shield + Rage | 11% |
| Bleed + Rage | 10% |
| Ritual + Bleed | 6% |
| Hammer | 5% |
| Fortune | 3% |
| Ritual | 1% |
| Unplanned buyer | 0% |
| No shopping | 0% |

- Shield plays long fights (20 turns in the Strong tier) with Tower Helm on 97% of its runs, but no single piece stands out, so it stays.

### Next Steps
- **The user's play-test** of the new economy (starting reroll, potion and Change, Healer) and the capped items. If the game still feels easy, harden the Weak tier (battles 1–3 are almost riskless).
- **Bring up the bottom:** Fortune, Hammer, Ritual + Bleed (archetype passes). Focused Ritual staying low is acceptable as a glue archetype.
- **Encourage:** its content set is to be discussed.

## Day 61 - Fortune Pass: Gambit, Hedge and +2 per Reroll

### Findings
- **Fortune fell from 8–15% (Day 48) to 2–4%.**
  - The enemy traits hit it hardest: Momentum punishes rerolls and Entangle forbids them.
  - It gained the least from the new economy.
- **Where it dies:** the Minotaur 30% and Medusa 23% (she petrifies the grown Patient Blade).
- **One damage face:** in Medium battles only Patient Blade dealt real damage (6.4 per roll), about 2 damage per turn overall.
  - The spending payoffs were Rare and Epic, and bots almost never found them (0.2 and 0.1 per run at battle 8).
  - Patient Blade +1 or Lucky Buckler +1 only reached 5%.

### Accomplished
- **Gambit** (Common, value 2, Fortune): Attack: my value + 2 per reroll used this battle. Defense: block equal to my value.
  - The Common rung of spending, an additive little brother of Fate's Edge.
- **Hedge** (Common, value 2, Fortune): Defense: block equal to my value + 1 per reroll left. Attack: my value.
  - Patient Blade's defensive twin.
- **Patient Blade:** + 2 per reroll left (was 1).
- **Code:**
  - `RerollScaledDamageEffectSO` gets `UsedThisBattleAdded` (appended to the enum) and a `perReroll` bonus.
  - New `RerollScaledBlockEffectSO`.
- **Bots:** a die with Gambit spends its last reroll too; savers (Patient Blade, Hedge) keep theirs. Hedge defends against big hits or with 2+ rerolls left.

### Tests
- **Checks (8/8, run with the first +1 bonus):**
  - Gambit deals 2 with no rerolls used and 4 after two, and its text reads "4 damage". Its Defense gives 2 block.
  - Hedge gives 4 block with 2 rerolls left and 2 with none; its attack deals 2.
  - Fate's Edge keeps its multiply mode.

### Balance (Fortune bot, 1000 runs)

| Version | Clear rate |
|---|---|
| Before | 2–4% |
| + Gambit and Hedge | 2% |
| + Gambit +2 per reroll | 6% |
| + Patient Blade +2 per reroll | 9% |
| + both +2 | 14–15% |

- **Why the new faces alone weren't enough:**
  - With them, Fortune got further (48% reached battle 8, was 37%).
  - But 37% of its rolls only defend (Lucky Buckler, Hedge). It took 1 HP per turn in Strong battles yet dealt only 2.7 damage, so fights lasted 15 turns.
  - One point per reroll was small next to Rage, Ritual or stacking statuses.
- **The other builds are unchanged within noise** after two faces joined the pool: Burn 12%, Poison 19%, Rage 12%, Bleed 12%.

### Next Steps
- **Hammer pass** (5%), then Ritual + Bleed (6%).
- **The user's play-test.**
- **Encourage:** its content set is to be discussed.

## Day 62 - Hammer Pass: War Hammer 5

### Findings
- **Hammer fell from about 32% to 5%.**
  - Medusa, the Hydra and the enemy traits all came after it.
  - Hammer grows in battle, not through upgrades, so the new economy helped it least (like Fortune).
- **Its damage was fine:** 4.5 per turn in Strong battles, 70% of them won.
- **It dies at the bosses:**

| Boss | Hammer arrivals that die | Other builds |
|---|---|---|
| Minotaur | 5% | 0–25% |
| Medusa (petrifies the charged Hammer, her designed answer) | 56% | 39–45%; Shield 16%, since a stone face still blocks |
| Hydra | 62% | 22–48% |

### Experiments (Hammer bot, 1000 runs)

| Change | Hammer | Other effects |
|---|---|---|
| Medusa HP 50 → 42 | 5% | Medusa's kill rate fell about 8 points for everyone (Hammer 56% → 40%), but Hammer died later instead; Ritual + Poison 23% → 28% |
| Charge cap +7 / +8 | 5% / 6% | Faces rarely reach the cap |
| War Hammer and Maul +1 | 9–12% | Shield 20% (Maul has the Block tag) |
| War Hammer 3 → 5 (chosen) | 10% | Shield 17–18%, Rage 13%, Shield + Rage 14% |

### Accomplished
- **War Hammer is value 5** (was 3). It is targeted at Hammer and leaves Shield and Medusa alone. Its cost: it only attacks, and its upgrades cost more.
- The Charge cap was made settable only for the experiment and is a constant again.

### Next Steps
- **Ritual + Bleed** (6%), the last bottom build; focused Ritual stays low as a glue archetype.
- **The user's play-test.**
- **Encourage:** its content set is to be discussed.

## Day 63 - Ritual + Bleed: a Weak Pairing by Design

### Findings (1000 runs)

| | Bleed | Ritual + Bleed |
|---|---|---|
| Clear rate | 12% | 7% |
| Enemy Bleed at the player's action (stacks) | 4.1 | 6.3 |
| Tick damage per turn | 0.91 | 0.97 |
| Action damage per turn | 3.83 | 3.82 |
| Deaths at battle 7 | 7% | 17% |

- **Why:** Bleed's stacks are duration, not damage (1 per tick; the damage comes from the faces, as the user designed it). Ritual's extra stacks only pay off through Lacerate (Common, value + Bleed, 15% of rolls) and Exsanguinate (Epic, value × consumed Bleed, 1% of rolls).
- **The Ritual pieces cost slots:** Incantation (0.4 per die) and Bone Fetish (40% of runs) take die and item slots from Bleed faces, so the hybrid is weaker than plain Bleed.
- **Experiments:**

| Change | Bleed | Ritual + Bleed |
|---|---|---|
| Lacerate +1 | 14% | 6% |
| Exsanguinate as Rare | 11% | 5% |

### Decision
- **Ritual + Bleed stays a weak pairing on purpose** (the user: Bleed's design doesn't match Ritual, and it doesn't have to).
  - Ritual is the glue for statuses whose stacks are damage (Poison, Burn).
  - Bleed's strong hybrid is Bleed + Rage.
  - Not chosen: a bridge face, or Ritual raising Bleed's tick damage. Both would blur Bleed's identity.
- Written into the GDD ("Hybrid Builds").

### Next Steps
- **The user's play-test** of the new economy and the balance passes (Days 57–62).
- **Encourage:** its content set is to be discussed.
- **If the game still feels easy:** harden the Weak tier.

## Day 64 - Encourage

### Accomplished
- **Encourage** (GDD "Status Effects" and "Encourage"): Ritual's bright mirror, the crowd's favor.
  - Encourage N adds N to every Rage, Fortify and FireShield its owner gains on itself. It never boosts Block, Ritual or itself.
  - It is permanent for the battle and appended to `StatusType`; it is also a keyword with a hover definition.
- **One path for self-buffs:** every buff an effect gives its own source now goes through `EffectContext.GainSelf`, the mirror of `ApplyToTarget`. Existing content (Shield, Fury, Tower Helm, Flame Ward...) is boosted automatically.
- **`GainStatusPerOpponentStatusEffectSO.countOwn`** counts the source's own buff types (Crowd's Favor).
- **The set** carries the Rage and Block tags, so it is a bridge, not a new archetype:

| Name | Kind | Rarity | Value | Effect | Ritual's counterpart |
|---|---|---|---|---|---|
| Rally | Face | Common | 1 | Magic: gain Encourage; Defense: block | Incantation |
| Battle Hymn | Face | Rare | 2 | Defense: Fortify equal to my value; if you have Encourage, also 1 Rage | (the bridge) |
| Crowd's Favor | Face | Epic | 1 | Magic: Encourage per buff type you have | Dark Communion |
| Inspire | Enchantment | Epic | | Magic: Encourage equal to the face's value | Dark Rite |

- **Bot rules:** Rally builds Encourage to 2, then defends. Crowd's Favor uses Magic. Battle Hymn defends. Inspire is played when there is no Encourage yet.

### Tests
- **Checks:**
  - Rally gives 1 Encourage, and Encourage doesn't boost itself (2 after two Rallies) or Block.
  - With Encourage 2, Battle Hymn's preview and result are 4 Fortify and 3 Rage; without Encourage, 2 Fortify and no Rage.
  - Crowd's Favor gives at least 1, and 3 in total with Rage and Fortify.
  - Herald's Horn and Tower Helm sum correctly (5 Fortify).
  - Ritual and statuses on the enemy aren't boosted.
  - One check first failed because the test itself was wrong (Ritual was gained before the Poison it boosted); in the right order it passes.

### Balance (simulation)

| Set | Shield | Rage | Shield + Rage |
|---|---|---|---|
| Without Encourage | 18% | 12% | 14% |
| First set | 28% | 23% | 26% |

- **Removing one piece at a time:**

| Change | Shield | Rage | Shield + Rage |
|---|---|---|---|
| No Battle Hymn | 30% | 19% | 27% |
| No Rally | 28% | 25% | 22% |
| No Crowd's Favor | 30% | 22% | 23% |
| No Herald's Horn | 24% | 16% | 17% |
| No Horn, no Battle Hymn | 21% | 13% | 18% |
| No Horn, Battle Hymn value 1 | 24% | 16% | 18% |
| No Horn, Battle Hymn Epic | 25% | 14% | 17% |
| No Horn, Battle Hymn as block + 1 Fortify + 1 Rage | 25% | 15% | 17% |
| No Horn, Battle Hymn's Rage only with Encourage (chosen) | 23% | 12% | 17% |

- **Herald's Horn** (1 Encourage at the start of every battle) doubled Tower Helm, which almost every Shield bot carries. It is out of the pool; the asset stays for a later, delayed version.
- **Battle Hymn** with unconditional Rage was a strictly better Shield face: the Shield bot defends all the time and its Rage piled up. Now the Rage needs Encourage.
- **Final (800 runs):** Shield 24%, Rage 14%, Shield + Rage 16%, Bleed + Rage 9%.

### Next Steps
- **The user's play-test** (economy, balance passes, Encourage).
- **Encourage:** a delayed Herald's Horn, and watch Shield, now near the top (23–24%).
- **If the game still feels easy:** harden the Weak tier.

## Day 65 - Play-test Fixes: Market, Curl Up, Change and Pass

### Accomplished (from the user's play-test)
- **Healer:** 10 HP for 20 gold (was 40, which felt far too expensive). It will be measured together with the steeper early battles.
- **Locks:** 2 gold, 5 in the Grand Market (were 5 and 15). Locking is how players save up for an offer, so a high price worked against saving, above all for Epics. A deposit model was considered and dropped as too unfamiliar.
- **The next battle is shown in the Market** ("Next battle (1): Murmillo (Weak)"), with each enemy's HP, intents and traits on hover.
  - `RunManager.GoToMarket` picks the battle ahead (`PickNextBattle`, stored in `RunData`), and `BattleManager` takes it (`TakeNextBattle`).
  - Until the map exists, this is how the player plans purchases.
- **Curl Up (Murmillo)** didn't work in practice. Its block came on the player's turn and was gone at the start of Murmillo's own turn, so it only mattered against a second hit in the same turn.
  - Now, following the user's design: each hit that gets through with no block gives it 2 block at the start of its next turn.
  - New trait options: `requireUnblocked` (`Entity.LastHitAbsorbed`) and `nextTurn` (run by `EnemyTraitSystem.RunPending` after the enemy's Block reset).
  - The card shows "Curl Up: 2 block (at the start of its turn)" while it waits.
  - No other trait had the same problem.
- **Change:**
  - Once per turn (was up to 5), and it guarantees a face other than its own (it used to roll the whole die, so a used potion could land on itself).
  - The once-per-turn limit keeps Change from thinning the die, so future Consumable faces don't become too strong (the user's plan for more Consumable content).
- **Pass button:**
  - It ends the turn without acting; the rolled face counts as unused, so a Hammer keeps its Charge.
  - It is the way out when a reroll lands on a Change that was already spent this turn, and it also lets the player keep an unused potion at full HP.
  - Bots pass when the face can't act.
- **Roadmap:** the user's map ideas (healing at rest nodes instead of the Market, an Enchanting Table node, "?" events).

### Tests
- **Checks:**
  - Change never landed on its own face in 300 tries.
  - The first Change in a turn works, and a second one in the same turn (on either used potion) is refused.
  - Passing keeps the rolled Hammer's Charge (3 → 4), the pass flag resets the next turn, and acting still spends the Charge.
- **Play Mode:**
  - The Market showed "Next battle (1): Murmillo (Weak)" with its Curl Up text on hover, and the battle was against Murmillo.
  - An unblocked Shield hit set Curl Up waiting (shown on the card); Murmillo had 2 block after its turn start, still there on the player's next turn.
  - Change worked on three turns in a row, each time landing on another face.
  - The Pass button is active only after the roll; passing went to the enemy's turn and back to the roll phase.
  - No Console errors.

### Findings
- Serrated Blade's action preview reads "if enemy has Bleed: 4 damage, 4 Bleed", as if its Bleed were conditional too, which its description says it isn't. To fix later.

### Next Steps
- **The difficulty curve:** the Weak battles are too easy, and the Minotaur is a cliff (play-test, matching the simulation). Make the climb gradual.
- **Fortune:** its faces are hard to find, and the found ones are weak in damage.

## Day 66 - The Difficulty Curve

### Findings
- **The curve ran backwards** (play-test, confirmed by simulation): Weak enemies stayed the same while the player grew stronger in every Market.
  - HP lost per battle: 4.5, 2.9, 2.6, then 20.1 at the Minotaur, about 8 times battle 3.
- **The 20-gold Healer** (Day 65) pushed clear rates to 22–48%.
- **New simulator report:** "avg HP lost by battle (fight only)", per battle number.

### Accomplished
- **Battle scaling** (`EncounterConfigSO.battleScaling`, GDD "Difficulty Curve"): normal enemies at some battle numbers get more HP and start with Rage.
  - Rage is visible on the card and in the intents' numbers, so there is no hidden multiplier.
  - `BattleManager` and the simulator apply it; the Market preview shows the starting Rage and the scaled HP.
  - Bosses aren't scaled.

| Battle | HP | Starting Rage |
|---|---|---|
| 2 | ×1.7 | 0 |
| 3 | ×1.4 | 1 |
| 6 | ×1.15 | 0 |
| 7 | ×1.3 | 1 |
| 10 | ×1.15 | 1 |

- **Minotaur:** 22 HP (was 26).

### Tuning (Burn, Rage, Shield, Fortune bots, 400 runs, HP lost per battle)

| Version | 1 | 2 | 3 | 4 | Minotaur deaths | Clear rate |
|---|---|---|---|---|---|---|
| No scaling | 4.5 | 2.9 | 2.6 | 20.1 | 19% | 28% |
| A: HP ×1.25 / ×1.5, Rage 1 at battle 3 | 4.5 | 4.1 | 7.5 | 18.5 | 22% | 15% |
| B: + Rage 1 at battle 2, Stampede 9 | 4.7 | 8.2 | 7.6 | 17.5 | 24% | 15% |
| C: battle 2 HP ×1.3, Minotaur 22 HP | 4.6 | 4.2 | 7.9 | 17.1 | 19% | 16% |
| **D: battle 2 HP ×1.7 (chosen)** | **4.6** | **5.8** | **7.2** | **17.0** | **21%** | **16%** |

- **1 Rage on a Weak enemy is a big step** (its hits are 1–2), so battle 2 climbs through HP and battle 3 adds the Rage.
- **Stampede 12 → 9 barely mattered:** most of the Minotaur's damage comes from Gore and Trample in a long fight, so its HP went down instead.
- **The Minotaur is still the first peak,** about 2.4 times battle 3 (was 8).
- **All bots (700 runs):**

| Bot | Clear rate |
|---|---|
| Burn | 10% |
| Poison | 13% |
| Shield | 17% |
| Rage | 21% |
| Bleed | 16% |
| Hammer | 12% |
| Fortune | 12% |
| Ritual | 1% |
| Ritual + Poison | 24% |
| Ritual + Bleed | 11% |
| Shield + Rage | 15% |
| Bleed + Rage | 20% |
| Unplanned buyer | 1% |
| No shopping | 0% |

### Tests
- **Play Mode:** in battle 3, Murmillo started with 20/20 HP (base 14 × 1.4) and 1 Rage.

### Next Steps
- **Fortune:** its faces are hard to find, and the found ones are weak in damage (the user's play-test).
- **The user's play-test** of the curve.

## Day 67 - Fortune Investigation and Redesign Plan

### Findings
- **The user's play-test:** Fortune faces are hard to find, and the ones found are weak in damage. The defensive ones (Lucky Buckler, Hedge) left Sword as the only real attack.
- **Finding faces is hard for every build.** Build-aware offers match any tag on the die, including the starting die's (Sword: Physical + Bleed, Shield: Block), so the matching pool is huge.
  - Over 2000 Markets: with one Patient Blade, 21% of offers were Fortune; with one Ember, only 11% were Burn.
- **Tried: weight offers by the build's tags.** The die's tag counts minus the starting die's (`RunData.startingDie`, `Market.BuildTagCounts`, `EconomyConfigSO.weightByBuildTags`). Offers rise to 41% Fortune and 47% Burn, but the builds shift very unevenly (600 runs):

| Bot | Current rule | Build tags | Build tags, 35% match chance |
|---|---|---|---|
| Burn | 9% | 43% | 38% |
| Poison | 17% | 32% | 31% |
| Hammer | 10% | 20% | 21% |
| Shield | 19% | 17% | 18% |
| Bleed | 17% | 15% | 13% |
| Rage | 20% | 13% | 19% |
| Fortune | 10% | 8% | 11% |

- Burn's faces are very strong once assembled; it only looked balanced because they were hard to find.
- Fortune didn't gain: its problem is its design, not its offers. The option is in the code but **off**, until a decision after the Fortune redesign (with a Burn pass).

### Decision: the Fortune Redesign (the user's design, GDD "Die Layout" and "Rerolls")
- **Fortune never lived up to its name.** Gaining and saving rerolls made it a weaker copy of Fortify + Rage.
- **New identity:** Fortune manipulates which face comes up and when, using the die's layout. Gaining rerolls becomes a small part of it.
- **Two sides:**
  - Engines (Change, Flip to the opposite face, Tumble to a neighbor or a named position, choosing a face, effects that also Change, gaining rerolls).
  - Payoffs (first turn, second time this turn, came from a Change, what the opposite face is).
- **The die layout:**
  - A sideways cross net (Top; Left, Middle, Right, Rightmost; Bottom), as in Slice & Dice. Named positions plus cube relations: opposites Left ↔ Right, Middle ↔ Rightmost, Top ↔ Bottom.
  - Placement is free in the Market: the decision is the arrangement puzzle, not a tax. The layout is fixed in battle, except through Fortune abilities.
- **Rules:**
  - At most 3 actions per turn.
  - The Consumable Change stays once per turn; Fortune's Change actions are separate, held by rarity.
- **Unlocks:** Fortune will be unlocked through meta progression (Roadmap Phase D), so new players aren't hit by it first.
- **Stages:**
  1. The die layout and free swapping.
  2. Roll steering, per-turn landing counts and the action limit.
  3. Content.
  4. Bots, simulation and play-tests.

### Next Steps
- Stage 1 of the Fortune redesign: the die layout.
- Decide the build-tag offers after the redesign, with a Burn pass.
- Serrated Blade's misleading preview (Day 65).

## Day 68 - The Four Kinds of Run, and Hoplomachus

### Accomplished
- **Design (the user's, GDD "Every Run Is Its Own Build"):** the aim of a run isn't a perfect build.
  - A patchwork die made from what the run offers can finish the game, and a complete build is the rare power fantasy.
  - Four kinds of run: bad decisions, bad luck, the normal run (the most common) and the lucky build. Each one gives a reason to play again.
  - Build-aware offers wait until content is plentiful; they will be the dial between the normal run and the lucky build.
- **Simulator:**
  - **Opportunist bot:** no fixed build. Its focus is the two most common tags beyond the starting die, it takes Rare and Epic opportunities, and it never pays to reroll for faces.
  - **New report line for focused bots:** "clear rate by build faces at battle 8".
- **Hoplomachus Champion:** 40 HP (was 51); Challenge fires the first 3 times each battle (was uncapped).
- **Battle 10:** no starting Rage (HP ×1.15 stays).
- **CLAUDE.md:** fixed stale numbers (locks, Healer, the Change preview).

### Findings (1000 runs per bot)
- **Bad decisions:** the unplanned buyer clears 2%. This is healthy; an earlier worry about it was wrong.
- **The normal run:** the Opportunist clears 10%.
- **The lucky build,** by faces fitting the build at battle 8, among runs reaching it:

| Faces | 1 | 2 | 3 | 4 | 5 | 6 |
|---|---|---|---|---|---|---|
| Burn | 4% | 15% | 41% | 76% | 83% | 67% |
| Poison | 0% | 13% | 25% | 58% | 70% | 67% |
| Rage | 0% | 9% | 20% | 31% | 36% | 27% |
| Hammer | 0% | 1% | 11% | 32% | 31% | 26% |
| Fortune | 0% | 7% | 13% | 26% | 25% | 25% |

  - Burn and Poison have the intended power fantasy, and their complete builds are rare. Their 5–6 face groups are small (3–20 runs), so those numbers are rough.
  - Rage, Hammer and Fortune stop growing at 4 faces. Rage's complete build isn't rare either (148 of 721 runs).
  - Bleed and Shield aren't listed, because the starting faces count as their build faces.
- **Bad luck can't be measured yet:** focused bots stay rigid, so few build faces doesn't mean bad luck.
- **Battle 10 was a wall:** it killed 50–55% of the runs reaching it, twice the Hydra, because of the starting Rage added on Day 66 and one outlier enemy.

### Tuning

| Battle 10 (5 bots, 400 runs) | Arrivals that die |
|---|---|
| HP ×1.15, Rage 1 | 50% |
| HP ×1.15, no Rage | 37% |
| No scaling | 33% |

| Hoplomachus, forced fights | Lost |
|---|---|
| As it was | 56% |
| Challenge at most 3 / at most 2 | 48% / 44% |
| No Battle Cry, uncapped / at most 3 | 51% / 43% |
| Cap 3, Spear Thrust 2 | 37% |
| Cap 3, 40 HP (chosen) | 37% |

- For comparison: War Elephant 35%, Essedarius and Bestiarius 19%.

### Results
- **Arrivals that die (5 bots, 400 runs):** Minotaur 22%, battle 7 23%, Medusa 24%, battle 9 19%, battle 10 33%, Hydra 17%. Battle 10 is still above the Hydra, which is measured only on strong survivors.
- **Clear rates rose (700 runs):**

| Bot | Clear rate |
|---|---|
| Bleed + Rage | 33% |
| Bleed | 31% |
| Rage | 30% |
| Ritual + Poison | 28% |
| Shield | 27% |
| Poison | 23% |
| Fortune | 23% |
| Shield + Rage | 23% |
| Ritual + Bleed | 20% |
| Hammer | 18% |
| Burn | 15% |
| Opportunist | 15% |
| Unplanned buyer | 4% |
| Ritual | 1% |
| No shopping | 0% |

- The Opportunist and the unplanned buyer fit the four kinds of run. The focused bots are above the old 5–15% target; to be judged by the next play-test before raising the difficulty.

### Next Steps
- **Fortune stage 1:** the die layout (net, positions, opposites, free swapping).
- **Rage and Hammer's ceiling:** a complete build should be a power fantasy.
- **Play-test:** does the Hydra feel like the peak, and is the game still too easy?

## Day 69 - Fortune Stage 1: The Die Layout

### Accomplished
- **`DieLayout`:** the die is a sideways cross net, and the order of `RunData.dieFaces` is the position (0 Top, 1 Left, 2 Middle, 3 Right, 4 Rightmost, 5 Bottom).
  - Opposites are Left/Right, Middle/Rightmost and Top/Bottom; every face has four neighbors.
  - The starting face chosen at the run start takes Bottom.
- **Free swapping in the Market** (`Market.SwapFaces`, `MarketUI.pendingSwap`): click a face, then another. A face keeps its upgrades and enchantment. Clicking the same face twice or Cancel leaves the die as it was.
- **The net is on screen:**
  - In the Market, each slot shows its position, with Upgrade beside it, and the "next battle" line moved to the net's empty corner.
  - In battle, the die strip became the net on the left, and the player's placeholder sprite moved up and shrank.
  - Tooltips show the position and the opposite face.
- No face reads the layout yet; that comes with stages 2 and 3.

### Tests
- **Checks (6/6):**
  - The opposites are right and symmetric.
  - Each face has four neighbors, neither itself nor its opposite.
  - A swap moves the instance with its upgrade, for free.
  - Invalid swaps are refused, and the description text is right.
- **Play Mode:**
  - Swapping Top and Bottom kept the gold (20) and the instances.
  - Clicking the same face twice and Cancel both left the order unchanged.
  - In battle the rolled face (Sword at Top) was highlighted on the net, and its tooltip read "Position: Top. Opposite: Bottom (Hedge)".
  - No Console errors.

### Next Steps
- **Fortune stage 2:** steering the next roll (opposite, neighbor, position, chosen face), per-turn landing counts and the limit of 3 actions per turn.

## Day 70 - Fortune Stage 2: Steering the Roll

### Accomplished
- **`SteerRollEffectSO`** (`RollSteer`): after an action the die rolls again in the same turn, onto a steered face.
  - Change is any other face, Flip the opposite face, Tumble a neighbor or a named position, and Chosen lets the player pick.
  - `BattleFlow.ApplySteer` reads the request after the action's effects.
- **Choosing a face:** `PlayerChooseFaceState` waits for a click on a face of the net (`DieChipUI.button`, `UIManager.ShowChooseFace`). Bots take the strongest attacker.
- **`RollConditionEffectSO`** for payoffs: `FirstTurn`, `LandedTimesThisTurn`, `ArrivedBySteer` and `OppositeHasTag`, fed by new `EffectContext` fields.
- **At most 3 actions per turn** (`MaxActionsPerTurn`): the third action ends the chain, which also limits Steady Hand. The Consumable Change doesn't count as an action.
- **Charge fix:** every face that acted this turn now spends its Charge, not only the last one. Otherwise a Hammer acting early in a chain would have hit and still gained Charge.
- **Keywords:** Flip and Tumble. Change is now the general keyword, and the Consumable text carries the once-per-turn limit.
- **The first content set is agreed** (GDD "Rerolls"): Turnabout, Opening Gambit, Ricochet, Sidestep, Mirror Blade, Fate's Hand and Encore. Encore's Magic is Flip rather than the first idea's Change, so its second landing comes from the arrangement instead of luck.

### Tests
- **Checks (13/13, with temporary faces):**
  - Flip runs its effect and lands on the opposite face.
  - Tumble lands on Rightmost, and a neighbor roll never lands on itself or its opposite (100 tries).
  - Chosen waits for the choice, and that face comes up.
  - The third action ends the chain; landing counts and the turn number are right and reset.
  - The second-time and opposite-tag payoffs work, with the right text.
  - A Hammer that acted earlier in a chain spends its Charge.
- **Play Mode, with a temporary face:**
  - The preview read "3 block, choose the next face".
  - The action gave 3 block and opened "Choose a face", with only the faces active.
  - Clicking Right brought that face up. No Console errors.
- **Regression (700 runs):** Opportunist 16%, Burn 14%, Shield 22%, Rage 35%, Hammer 16%, Fortune 19%, all close to Day 68.

### Next Steps
- **Fortune stage 3:** build the agreed set.
- **Fortune stage 4:** bots, simulation and the user's play-test.

## Day 71 - Fortune Stage 3: The First Set

### Accomplished
- **Seven new Fortune faces** (GDD "Rerolls"), added to the Market pool (55 faces):

| Face | Side | Rarity | Value | Effect |
|---|---|---|---|---|
| Turnabout | Engine | Common | 2 | Attack: my value. Magic: Flip |
| Opening Gambit | Payoff | Common | 3 | Attack: my value, hit again on the first turn. Defense: block |
| Ricochet | Engine | Rare | 2 | Attack: my value, then Flip |
| Sidestep | Engine | Rare | 2 | Defense: block, then the Rightmost face comes up |
| Mirror Blade | Payoff | Rare | 3 | Attack: my value, hit again if I came from a Change, Flip or Tumble |
| Fate's Hand | Engine | Epic | 2 | Defense: block, then choose the next face |
| Encore | Payoff | Epic | 3 | Attack: my value, 2 more hits if this is my second time this turn. Magic: Flip |

- **Previews:** during a turn, a condition already met shows just its result ("2 x 3 damage").
- **The example combo:** with Encore opposite Ricochet, Encore's Magic Flips, Ricochet hits 2 and Flips back, and Encore hits 9: 11 damage in the 3 actions of a turn.

### Tests
- **Checks (13/13):**
  - Each face works.
  - Opening Gambit hits 6 on turn 1 and 3 later.
  - Mirror Blade hits 3 alone and 6 after a Flip.
  - The combo works step by step and ends at the limit.
- **Play Mode:** the full combo with real buttons (2, then 9 damage; then the turn passed to the enemy). No Console errors.

### Findings
- Sidestep on Rightmost lands on itself and repeats until the limit (6 block at value 2). It is allowed as a small discovery; to watch in play-tests.
- **The Fortune bot fell from 19% to 12%** (700 runs): it buys the faces but can't arrange or combo them. The other bots are in place (Burn 14%, Rage 31%, Opportunist 17%).

### Next Steps
- **Fortune stage 4:** simple bot rules (arranging the die, Encore's Flip, Fate's Hand's choice), then the user's play-test.

## Day 72 - Fortune Stage 4: Bots and the Ceiling

### Accomplished
- **Bot rules for the new Fortune faces:**
  - `ArrangeFortune` uses the free swap after shopping: each payoff (Encore, Mirror Blade) opposite a Flip engine (Ricochet, Turnabout), and an attacker on Rightmost for Sidestep. `ArrangeDie` switches it off.
  - Encore Flips first when its partner is opposite, and attacks on its second landing.
  - Turnabout Flips toward a payoff while actions are left.
  - Fate's Hand picks Encore (if it already came up this turn) or Mirror Blade, otherwise the strongest attacker.

### Results
- **Fortune bot, normal runs (1000 runs):** 14% (12% before these rules), and 14% with arranging off. It rarely finds the Rare and Epic pieces: Encore is 1% of its rolls, Ricochet 2%.
- **With the die built from the start (500 runs):**

| Die | Clear rate |
|---|---|
| Burn (3 Ember, Detonate, Fuel, Flame Ward) | 48% |
| Hammer (2 War Hammer, 2 Maul, Anvil, Earthshaker) | 46% |
| Fortune (Encore + Ricochet, Mirror Blade + Turnabout, Fate's Hand, Opening Gambit) | 43% |
| Only the Encore + Ricochet pair | 42% |

- Fortune's ceiling matches the other builds even with bots' simple play, so these rates are a floor.
- The pair alone does as well as the full setup. Either the other pieces add little or the bot uses them poorly; simulation can't tell which.
- Even dice built from the start stay under 50%, dying mostly at battles 9–10. Bots' weak play may be part of it, but it matters for "a complete build should be very strong".

### Next Steps
- **The user's play-test of Fortune:** the combos, the arrangement and whether the pieces can be found. A temporary test setting (Fortune offers first, or a built die) was offered.
- **Rage and Hammer's ceiling,** and whether complete builds are strong enough in the late game.
- **Serrated Blade's misleading preview** (Day 65).

## Day 73 - Clearer Conditional Previews

### Accomplished
- **Serrated Blade's preview** read "if enemy has Bleed: 4 damage, 4 Bleed", as if its Bleed were conditional too. Action previews join effects with commas, so nothing showed where a condition ended.
- **`ConditionalEffectSO` and `RollConditionEffectSO`:** a condition not met now goes in parentheses, and one already met shows just its result. Only the text changed, not the rules.

| Face | Not met | Met |
|---|---|---|
| Serrated Blade | (if enemy has Bleed: 4 damage), 4 Bleed | 4 damage, 4 Bleed |
| Open Wound | 2 damage, (if enemy has Bleed: 2 damage) | 2 damage, 2 damage |
| Battle Hymn | gain 2 Fortify, (if you have Encourage: gain 1 Rage) | gain 4 Fortify, gain 3 Rage (with Encourage 2) |

### Tests
- The preview text of the real faces was generated in both states and matches the table. Encore and Opening Gambit were checked in the not-met state. Not looked at on screen in Play Mode.

### Next Steps
- **The user's play-test of Fortune** and the difficulty curve.
- **Rage and Hammer's ceiling.**

## Day 74 - Die Enchantments and the Enchanting Table Tab

### Accomplished (the user's play-test: placement only mattered for Fortune, and face enchantments were rarely bought)
- **Die enchantments** (`DieEnchantmentSO`, GDD "Die Enchantments"), inscribed on the die's positions rather than on faces:
  - A bonus stays on its position when faces move, so the arrangement is a puzzle for every build.
  - Up to 2 at a time; a third replaces one. They can be locked, the Grand Market offers only Epics, and what's on the die isn't offered.
  - The bonus is part of battle value (`DieCharge.placementBonus`), never below 0, shown on the die, and it counts for Medusa's target. Roll weights go through `BattleFlow.PickFace`; steers ignore them.

| Enchantment | Rarity | Effect |
|---|---|---|
| Honed Edge | Common | Rightmost +2, Left -1 |
| Balance | Common | Top +2, Bottom -2 |
| Heavy Core | Rare | Middle +3, Top and Bottom -1 |
| Twin Fangs | Rare | Left and Right +1, Middle -2 |
| Kindred | Epic | Faces sharing a tag with their opposite +1 |
| Weighted Die | Epic | Rightmost twice as often, Left never on a roll |

- **The Enchanting Table is a tab in the Market:** "Enchanting Table" / "Back to Market" swaps the faces and items for the face and die enchantments, with the die net always in view. Two slots in the net's corner show what's on the die.
- **Bots:** `BuyDieEnchantments` and `ArrangeForDieEnchantments` (highest values on the best positions).

### Tests
- **Checks (14/14):**
  - Bonuses are right, and Kindred reads opposite tags.
  - Over 6000 rolls with Weighted Die, Left never came up and Rightmost came up about twice as often as Top.
  - Battle value applies the bonus and never goes below 0.
  - Market rules: 2 offers, price, owned ones not offered, the third replaces, Epic only in the Grand Market, locks survive a reroll and the next Market.
- **Play Mode:** bought Honed Edge and Balance; moved a face and the bonus stayed on its position. In battle, Sword 2 -1 had battle value 1 and Shield 2 +2 had 4, as the chips showed. The title first overflowed; widened.

### Balance
- **Free from the start (350 runs):**

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

- Common and Rare ones are a small gain, mostly within noise; the Epics are strong. Balance's versions can't be told apart, and +2 / -2 makes placement a heavier decision.
- **Gold is the real cost:** bots buying them before upgrades did much worse (Rage 34% to 18%, Shield 25% to 13%, 500 runs). Now they buy a Common or Rare only with 30 gold left over.
- **With that rule (500 runs):**

| Bot | Clear rate |
|---|---|
| Bleed + Rage | 37% |
| Bleed | 33% |
| Ritual + Poison | 33% |
| Rage | 29% |
| Shield | 28% |
| Poison | 23% |
| Shield + Rage | 23% |
| Ritual + Bleed | 20% |
| Burn | 18% |
| Hammer | 15% |
| Opportunist | 14% |
| Fortune | 12% |
| Unplanned buyer | 4% |

### Findings
- Weighted Die may be too strong: a weak face on Left is effectively removed from the die. To watch in play-tests; the fallback is "Left half as often".
- Bots only put high values on bonus spots, and don't use the minus spots cleverly, so the puzzle's real value shows in play.

### Next Steps
- **The user's play-test:** die enchantments, Fortune, the difficulty curve.
- **Rage and Hammer's ceiling.**

## Day 75 - War Elephant's Thorns and What Epics Are Worth

### Accomplished
- **War Elephant's Thorns is capped at the first 6 hits each battle.** The description says so; the enemy keeps its identity as a big target that bites back.
- **Bot switches for experiments, all off by default:**
  - `DefenseFacesToKeep`: focused bots and the opportunist keep this many protective faces (Defense or Health Potion) when replacing faces.
  - `GoldToSaveForGrandMarket`: they hold back this much gold in the Market before a boss.
  - `GrandMarketFaceFirst`: focused bots buy a fitting Epic face at the Grand Market before anything else.

### Tests
- **War Elephant, forced at battle 10 with battle 9 fixed to Essedarius (focused bots, 720 runs per row):**

| Thorns | Died | Arrivals |
|---|---|---|
| No cap (before) | 43% | 255 |
| First 6 hits | 28% | 260 |
| First 3 hits | 22% | 227 |
| None | 19% | 245 |

- The other Strong enemies, forced at battle 10 without fixing battle 9: Hoplomachus Champion 28%, Bestiarius 20%, Essedarius 18%. A first measure of the caps didn't fix battle 9 either, so changing the Elephant also changed who arrived; caps 6 and 8 both read 41% there.
- These were in-memory measurements; a full report with the saved asset is still to come.

### Findings
- **The Day 68 table is out of date.** Hammer no longer plateaus (64% and 80% at 4 and 5 build faces, small samples), and complete Hammer dice are rare. Rage rises to 46-59% at 4-5 faces, dips at 6 (44%), and complete dice aren't rare (218 of 534).
- **It isn't Rally:** without the Encourage faces in the pool, Rage still flattens after 4 faces.
- **Baked-in dice overstated the ceiling.** Fury, 2 Frenzy, Unleash, Iron Rage and Potion cleared 78-84% when the die was ready from battle 1. In ordinary runs (saving 10 gold, no die enchantments), the few dice with both Unleash and Iron Rage at battle 9 reached the Hydra 64% of the time (25 runs), against 49% with neither.
- **Keeping defense didn't help (500 runs):** keeping 0, 1 or 2 protective faces gave Rage 32%, 29% and 35%, within noise.
- **Focused bots almost never own an Epic face:** 3 of 284 Rage runs at battle 8. They arrive at the Grand Market with about 54 gold, an Epic costs 60, and when they save, an Epic die enchantment takes the gold first.
- **Buying Epics made Rage worse (300 runs):**

| Rage bot | Clear rate | Epic face at battle 9 |
|---|---|---|
| Today | 36% | 0 of 167 |
| Face first | 25% | 9 of 163 |
| Face first, save 10 | 19% | 71 of 121 |
| Epic at 50 gold, face first | 14-18% | 107 of 121 (first run) |

- Hammer shows no clear change (17%, 20%, 14%).
- **Giving the Epic for free before battle 5 separates the face from its price (300 runs):** Earthshaker takes Hammer from 16% to 33%, Unleash takes Rage from 29% to 37%, and Unleash at value 3 to 45%. Rage's no-gift rate moved between 29% and 36% across measurements, so its gains may be smaller than they look.
- Unleash deals 10-11 damage per roll against Common Frenzy's 8-9. Buying it costs the upgrades that feed every other face (Frenzy 9 to 8, Fury 7.4 to 6.9).
- Crowd's Favor dealt no damage on 7% of rolls in the bot's hands; removing it moved Rage from 18% to 24%, within noise, and the bot may simply play it badly.

### Decisions
- **Rage and Hammer's faces stay as they are for now.** Unleash at value 3 is a candidate, but bought at 50 gold with Crowd's Favor in the pool it still gave only 16%, so it wouldn't make buying pay by itself.
- **The bot switches stay off:** they make the bots worse, so the default bots remain the baseline.

### Next Steps
- **Epic scan:** bought vs. free Epic per archetype, to see if the problem is general (Epic price, cheap upgrades) or Rage's.
- **Full report** with the capped Thorns.
- **The user's play-test:** die enchantments, Fortune, the difficulty curve.

## Day 76 - Epics Bring Their Own Fuel

### Accomplished (the user's play-test: without a working build an Epic barely works, so it only gives Epic impact in the lucky run)
- **New rule, GDD "Epics Bring Their Own Fuel":** an Epic must beat a good Sword on any die, the turn it comes up, and explode in its build. Epics that reward a patchwork die directly are planned for later.
- **Seven Epics changed:**

| Epic | Value | Change |
|---|---|---|
| Unleash | 2 | Attack gains 1 Rage first |
| Exsanguinate | 1 to 2 | Attack applies value Bleed first |
| Fuel | 2 | Magic applies value Burn first and ignites the Burn at the end |
| Cauterize | 1 to 2 | Attack also applies value Burn |
| Crowd's Favor | 1 | Magic gains 1 Rage first |
| Plague | 1 to 2 | - |
| Dark Communion | 1 to 2 | Magic applies value Poison first |

- Fate's Edge stays as it is: an extra reroll barely changed it, so it needs another idea. Earthshaker, Fate's Hand and Shield Burst already work on any die.
- **Epic price 60 to 45:** a boss victory alone pays 50 gold, so the Grand Market can always sell one Epic. The price also applies to Epic enchantments.
- **Previews see earlier effects:** `EffectContext.Stacks` / `NotePreviewGain`, so Fuel with no Burn previews its real 4 damage. Effects that read stacks use `Stacks`.
- **Bots:** `RollWorth` judges a multiplying Epic by what it would really do (they were being rerolled away for their low printed value); Fuel plays Magic when it makes its own Burn; `BuyFittingFace` / `BuyBestEpic` take Crowd's Favor and Fate's Edge last. A correction to an earlier note: Exsanguinate, Earthshaker and others have a single action, so they never needed action rules; the reroll policy was the flaw.

### Tests
- **Previews against real results:** 10 cases over the changed faces, all equal (e.g. Unleash at 3 Rage: 12 damage; Exsanguinate on 5 Bleed: 14; Fuel on 3 Burn: 10). Not yet seen on screen in Play Mode.
- **Default bots at the new price (300 runs):** Bleed 49%, Poison 35%, Bleed + Rage 30%, Rage 29%, Ritual + Poison 27%, Hammer 24%, Shield 19%, Shield + Rage 18%, Burn 16%, Fortune 15%, Opportunist 14%, Unplanned buyer 1%. No new wall. Opportunist and Unplanned read slightly lower than before (16-20% and 3-5%), which is close to noise but worth watching.

### Balance
- **Epic given free before battle 5 (200 runs). Reached battle 9, with 67 and 26 without a gift:**

| Epic | Opportunist, before to after | Unplanned, before to after | Own archetype, before to after |
|---|---|---|---|
| Unleash | 65 to 92 | 23 to 33 | Rage 43% to 52% (no gift 30%) |
| Dark Communion | 51 to 80 | 18 to 46 | Ritual + Poison 48% (no gift 36%), before not measured |
| Fuel | 54 to 75 | 18 to 46 | Burn 13% to 30% (no gift 16%) |
| Fate's Edge, with a reroll (not kept) | 72 to 80 | 26 to 31 | Fortune 21% to 20% (no gift 15%) |
| Cauterize | 57 to 70 | 19 to 26 | Burn 20% to 28% (no gift 16%) |
| Plague | 65 to 70 | 20 to 30 | Poison 30% to 45% (no gift 27%) |
| Crowd's Favor | 54 to 69 | 14 to 37 | Rage 20% to 29% (no gift 30%) |
| Exsanguinate | 55 to 62 | 17 to 36 | Bleed 26% to 47% (no gift 47%) |

- The "before" figures of the last column were measured before the reroll fix, so part of those gains can come from it.
- **Bought (250 runs). Bots that buy an Epic first at the Grand Market, in-memory price:**

| Bot | Habits today, Epic 60 | Epic 60, save 10 | Epic 45 |
|---|---|---|---|
| Burn | 13% | 23% | 27% |
| Poison | 25% | 22% | 35% |
| Hammer | 22% | 20% | 29% |
| Rage | 29% | 26% | 30% |
| Shield | 25% | 22% | 17% |
| Fortune | 15% | 10% | 9% |
| Bleed | 47% | 24% | 26% |

- **Opportunist (500 runs):** habits today 20%; Epic first at 60 with saving 7%; Epic first at 45 10%.

### Findings
- **The floor is largely fixed, the turning point isn't:** a free Epic no longer hurts a normal run, but paying for one still cuts the Opportunist's clear rate about in half. The rival is upgrades: on a patchwork die, the same gold in upgrades does more than one Epic outside its tags.
- The bots buy an Epic every time; a player chooses, so these costs can be heavier than in real play.
- **Bleed:** Exsanguinate consumes the Bleed that the other faces' conditions read, and the bot is forced to buy it. **Fortune:** a free Fate's Hand is strong, but buying Epics still lowers it; the bot's arrangement may be the limit.
- Many runs that reach battle 9 still fall at battles 9 and 10.

### Next Steps
- **To discuss with the user:** upgrades against Epics (e.g. the upgrade cost, or an upgrade right at the Grand Market).
- **The user's play-test:** the new Unleash and Fuel, Epics at 45, die enchantments, Fortune, the difficulty curve.
- Epics that reward a patchwork die.

## Day 77 - Equipment in Every Market and a Second Set

### Accomplished (the user's play-test: a locked item waited 4 battles, Laurel Wreath was too weak, and the biggest problem is variety)
- **Every Market sells items:** 1 in a Normal Market, 2 in the Grand Market, still at 30 gold. A locked item is in the very next Market.
- **Laurel Wreath heals 2** (was 1).
- **Ten new items, pool 10 to 20.** Each changes how you play or what you decide, and they fill empty spots: Hammer and Consumable had no item, and a patchwork die had nothing of its own.

| Item | For | Effect |
|---|---|---|
| Whetstone | Hammer | Charge faces start each battle with 1 Charge |
| Alchemist's Belt | Consumable | Consumable actions work 2 times per battle before they become Change |
| Lucky Charm | Fortune | First time each turn a steer brings a face: 2 block |
| Spiked Greaves | General | First 3 hits per battle that cost HP: 2 damage to the attacker |
| Ambusher's Knife | General | First hit each battle: hit again for 3 |
| Blood Pact | Rage | Battle start: lose 3 HP (never your last), gain 1 Rage |
| Motley Cloak | Patchwork die | First 3 turns per battle: 1 block for every 2 different faces |
| Balance Scales | Patchwork die | An action other than your last one: 1 block |
| Compass | Die layout | First time each turn Rightmost comes up: 2 block |
| Merchant's Purse | Economy | Every victory pays 5 more gold |

- **New item rules:** reactions can require a die position, a steered face or a change of action; `goldPerVictory` and `extraConsumableUses` are rules outside the triggers; a HitTaken reaction answers the attacker (`Entity.LastAttacker`), not the selected target.

### Decisions
- **Motley Cloak counts faces, not tags:** the player doesn't see tags on the cards, while duplicate faces are visible at a glance.
- **Whetstone reuses Resonance's effect,** so it needed no new class.

### Tests
- **Checks (20/20):** every new item's rule on the real `BattleFlow`, and the Market's offers, locks, buying and reroll.
- **Play Mode:** the Normal Market shows the item card properly, with no console errors. The items in battle were only checked through code, not on screen.

### Balance
- **Motley Cloak was broken at first:** 2-3 block every turn took the Opportunist from 15% to 69% (250 runs).

| Motley Cloak version | Opportunist |
|---|---|
| Per 2 faces, every turn | 69% |
| Per 3 faces, every turn | 47% |
| Per 2 faces, first 3 turns (chosen) | 32% |
| Per 3 faces, first 3 turns | 23% |
| Per 2 faces, first 2 turns | 18% |

- **Each item given free from the start (250 runs):**

| Item | Bot | Without | With |
|---|---|---|---|
| Spiked Greaves | Opportunist | 15% | 27% |
| Laurel Wreath (heal 2) | Opportunist | 15% | 24% |
| Balance Scales | Opportunist | 15% | 23% |
| Compass | Opportunist | 15% | 21% |
| Ambusher's Knife | Opportunist | 15% | 20% |
| Merchant's Purse | Opportunist | 15% | 18% |
| Alchemist's Belt | Opportunist | 15% | 15% |
| Whetstone | Hammer | 31% | 39% |
| Blood Pact | Rage | 23% | 24% |
| Lucky Charm | Fortune | 14-15% | 22% |

- **Default bots with items on sale everywhere (300 runs, before Motley Cloak's cap; it was on 46 of 1136 dice at battle 8):** Bleed 50%, Hammer 35%, Poison 30%, Ritual + Poison 27%, Rage 25%, Shield 21%, Bleed + Rage 21%, Opportunist 19%, Burn 18%, Shield + Rage 16%, Fortune 14%, Unplanned buyer 0%. No explosion.

### Findings
- Bots own only 1-1.6 items at battle 8, so these rates don't show a player who buys an item at every Market.
- Alchemist's Belt does nothing for these bots, which usually have one Consumable. Blood Pact shows no gain in this measure.
- **Small block every turn is very strong,** which may be part of why the user's Fortify die had no trouble.
- The user's Fortify die beat the Hydra in seconds, while the Shield bot clears 21%: the bot plays that build far worse than a player, so the simulator underrates it. The user's verdict on the Hydra: closer to "the last boss shouldn't be this easy"; a friend would say "is that it?".

### Next Steps
- **The user's play-test:** are items tempting in the Market, and does the list feel varied?
- **The Shield bot and a Fortify question for the Hydra.**
- **The map (Roadmap Phase B),** and a boss pool per slot.
- Later: upgrades against Epics, and Epics that reward a patchwork die.

## Day 78 - Stacking on One Face

### Accomplished (the user's play-test: a patchwork die stacked on Bite beat every Strong enemy and the Hydra with eyes closed)
- **Bite's Attack now deals my value and heals 2** (it healed the whole hit, Rage included). Its Magic still heals my value. A fixed heal keeps Value Is the Effect; "heal half the damage" would have been a hidden multiplier.
- **Weighted Die no longer stops Left from coming up:** Rightmost still comes up twice as often. A player could erase the die's weakest face with it.
- **New bot, One-face carry** (`ShopCarry`, `Bot.carryName`): its carry stands on Rightmost, it buys die enchantments that favor Rightmost and puts all its remaining gold into that face. Bots used to spread upgrades, so the simulator had never seen this strategy.
- `HealEffectSO.fixedAmount`.

### Tests
- **One-face carry (250-300 runs):**

| Carry | Before | Only Bite changed | Both changes |
|---|---|---|---|
| Bite, on the die from the start | 87% | 59% | 33-34% |
| Sword | 64-69% | - | 32-36% |
| Shield | 65% | - | 36% |

- **Weighted Die, checked by switching the rule on and off in memory (250 runs each):** Sword 68% and 63% with "Left never", 36% and 32% without; Bite 59% and 33%. The effect looked too large for its small change per roll, so it was measured twice.
- Not yet seen in Play Mode.

### Balance
- **Default bots (300 runs), before these changes and after:** Bleed 50% to 39%, Shield 21% to 21%, Hammer 35% to 20%, Poison 30% to 19%, Burn 18% to 18%, Ritual + Poison 27% to 18%, Rage 25% to 17%, Bleed + Rage 21% to 17%, One-face carry 32% to 17%, Shield + Rage 16% to 14%, Fortune 14% to 10%, Opportunist 19% to 8%, Unplanned buyer 0% to 0%.
- **Upgrade cost, in memory:** Common x3 to x4 takes the Sword carry from 33% to 15%, x5 to 14%. It is a very sharp lever that hits every bot, so it stays as it is for now.

### Findings
- **Before these changes, building nothing and stacking on the starting Sword beat every archetype.** Now the carry sits among the archetypes.
- **"Left never" had been inflating every bot,** so older clear rates from Day 74 on overstate those builds.
- **The Opportunist at 8% is too low** for the normal run in GDD "Every Run Is Its Own Build", which should finish or nearly finish.
- The user clearly plays better than the bots: their first run lost to Medusa after a real dilemma between buying power and buying HP, which is the healthy kind of loss, and their second found the Bite stack halfway through. Real difficulty can be lower than these tables say.
- Unaddressed: the user's last die also held Dark Communion, Venom, Contagion, Censer and Plague Mask, so Ritual + Poison may have helped Bite carry that run.

### Next Steps
- **Pragmatist faces:** faces that read general states, so a patchwork die finds its own combos. They should lift the Opportunist without making a one-face stack dominant. Add a General tag.
- **The Shield bot and a Fortify question for the Hydra.**
- **Upgrade cost,** after the Pragmatist set.
- **The map (Roadmap Phase B),** and a boss pool per slot.

## Day 79 - Pragmatist Faces

### Accomplished (the user's idea: faces for the patchwork die, which find different combos in every run)
- **GDD "Pragmatist Faces":** they read a general state that many builds produce, work like a Sword on any die, add instead of multiply, and stay Common or Rare.
- **`FaceTag.General`:** these faces carry it instead of an archetype tag, and it fits any die in the Market's tag match.
- **Ten faces, pool 55 to 65:** Exploit, Veteran, Feint, Drill Sergeant, Throwing Knife, Spare Blade (Common), Catalyst, Mimic, Last Stand, Sharpshooter (Rare).
- **New effects:** `StatusTypesBonusEffectSO`, `ConditionalBonusEffectSO`, `AddToPresentStatusesEffectSO`, `CopyOppositeAttackEffectSO`, `MissingHPDamageEffectSO`, `AddValueToNeighborsEffectSO`. `DieCharge.AddBattleBonus` holds value that acting doesn't spend.
- **Bots:** the Opportunist takes General faces as part of its plan; rules for Drill Sergeant and Catalyst.

### Decisions
- **Sharpshooter replaced Follow Up,** which read as complicated, and its "half" came close to a hidden multiplier.
- **Catalyst has a Defense action:** without one it did nothing on an enemy without statuses.
- **Runs get longer without new systems (Roadmap Phase B):** more enemies and bosses between the start and the last boss, plus a map that slows growth. An Act 2, a second die, face evolution and die levels were decided against, to keep the project finishable.

### Tests
- **Checks (19/19):** every face on the real `BattleFlow`, previews included. Mimic copies an Attack, falls back to a plain hit without one, doesn't loop with another Mimic and doesn't spend a Consumable. Not yet seen in Play Mode.

### Balance
- **One-face carry, the face on the die from the start (220 runs):** Bite 32%, Last Stand 21%, Exploit 10%, Feint 10%, Spare Blade 6%, Veteran 5%, Sharpshooter 3%, Mimic 1%. None of them makes stacking dominant.
- **Free before battle 5 to the Opportunist (200 runs, no gift: 4% cleared, 29 reached battle 9). Reached battle 9:** Last Stand 61, Throwing Knife 32, Spare Blade 29, Sharpshooter 29, Drill Sergeant 28, Feint 23, Mimic 23, Veteran 22, Exploit 20, Catalyst 19. Only Last Stand stands out.
- **Default bots (300 runs), before and after:** Bleed 39% to 35%, Shield 21% to 22%, Bleed + Rage 17% to 22%, Rage 17% to 19%, Ritual + Poison 18% to 13%, Poison 19% to 12%, Shield + Rage 14% to 12%, Burn 18% to 11%, One-face carry 17% to 11%, Hammer 20% to 10%, Fortune 10% to 10%, Opportunist 8% to 5%, Unplanned buyer 0% to 1%. Most bots are now in the 5-15% target of Roadmap Phase A.

### Findings
- **Correction to Day 78:** the Opportunist's 8% isn't too low; Roadmap Phase A's target is bots at 5-15%, since bots decide far worse than people.
- The Opportunist uses these faces (1.6 per die at battle 8, all ten seen) but can't build combos with them, so the simulator can't show their value. The user's play-test decides.
- General faces take about 25% of offers (225 of 900) while they are about 15% of the pool, because they fit any die. It may need tuning.

### Next Steps
- **Equipment without a slot limit, as relics:** remove the limit, scale items down, grow the pool.
- **The Shield bot and a Fortify question for the Hydra.**
- **The map, more battles and the difficulty curve.**

## Day 80 - Items as Relics

### Accomplished (the user's play-test: three slots were too few; items should pile up like Slay the Spire's relics)
- **No item limit.** The replace flow is gone from `Market`, `MarketUI` and the simulator.
- **Small chips:** the Market and the battle show one chip per item, copied as items pile up, with the description on hover. Market: 8 columns, 2 rows. Battle: 5 columns, 3 rows.
- **Items cost 15 gold** (was 30).
- **The strongest items scaled down:** Motley Cloak (1 block per 3 different faces, was 2), Spiked Greaves (1 damage, was 2), Censer (first 2 turns, was 3), Plague Mask (once per battle, was twice), Barbed Hook (first 4 hits, was unlimited), Tower Helm (3 times, was 5). Laurel Wreath stays at 2 as the user asked.
- **Face enchantments are cheaper:** Common 5, Rare 10, Epic 20 (the same as faces before), since the user rarely found them worth buying. Faces and die enchantments keep their prices.
- `RunSimulator.BuyItemsFirst`: an experiment switch for a player who takes every item first.

### Decisions
- **No limit instead of 5 or 6 slots:** a bigger limit keeps the "which one do I give up" question and doesn't give the relic feeling.
- **Die enchantments stay at 2,** because they feed one-face stacking directly.
- **Items went ahead of the Hydra's Fortify question,** since they change the difficulty that question will be tuned against.

### Tests
- **Play Mode:** 15 items fit in the Market and in battle, with no console errors; it's tight but readable in battle. More than 16 in the Market or 15 in battle would overflow, which matters once runs get longer.

### Balance
- **Items given from the start to the Opportunist (250 runs), before and after scaling down:** 3 items 18% to 12%, 6 items 40% to 28%, 10 items 63% to 58%, 15 items 86% to 82%. No items: 4%.
- **A bot that takes every item first, after scaling down (200 runs):**

| Item price | Items at the Hydra | Opportunist | Poison | Shield | Rage | Bleed |
|---|---|---|---|---|---|---|
| 30 | about 5 | 2% | 5% | 12% | 9% | 21% |
| 20 | about 10 | 5% | 1% | 6% | 7% | 11% |
| 15 | about 11-12 | 13% | 10% | 18% | 21% | 35% |
| 30, normal habits | about 2-3 | 5% | 8% | 19% | 18% | 32% |

- 15 gold makes taking every item about even with upgrades, and clearly better on a patchwork die. At 20 all the income goes on items and the die stays weak.
- **Default bots at 15 gold (300 runs):** Bleed 33%, Bleed + Rage 22%, Shield 19%, Rage 19%, One-face carry 17%, Ritual + Poison 14%, Hammer 13%, Fortune 11%, Poison 11%, Shield + Rage 10%, Burn 9%, Opportunist 3%, Unplanned buyer 1%. They own about 4 items at the Hydra.
- **Enchantment prices:** bots almost never buy them (0-0.01 per die at battle 8), so these measures can't show their value; clear rates stayed within noise.

### Findings
- Items are strong in numbers: a good player taking many early may find the game too easy. The user's play-test decides.
- 11-12 items per run against a pool of 20 means runs will see similar items until the pool grows.
- The Opportunist at 3% is below the 5-15% band; bots don't use items on purpose yet.
- A Rare item at 15 gold against a Rare face at 25 is odd; prices per rarity come when Common and Epic items exist.

### Next Steps
- **The user's play-test:** is piling up items fun, is 15 gold right, did the game get easy, and are enchantments worth buying now?
- **Ten more items.**
- **The Shield bot and a Fortify question for the Hydra.**
- **The map, more battles and the difficulty curve.**

## Day 81 - Third Item Set and Tooltip Layering

### Accomplished (the user's play-test: piling up items is fun, and gold holds the reins of the difficulty)
- **Ten new items, pool 20 to 30:** Tinder Box, Witch's Knot, Rabbit's Foot, Drill Banner (Common), Second Blade, Scribe's Quill, Hourglass, Smelling Salts (Rare), Challenger's Mark, Haggler's Scale (Epic).
- **Item prices by rarity:** Common 10, Rare 15, Epic 25 (`EconomyConfigSO.GetEquipmentPrice`). Merchant's Purse is Epic now, since gold is the limit on items and it pushes hardest against it. Loaded Die was already Epic.
- **New item rules:** the `Rerolled` trigger; `requireBelowHalfHP` and `onTurn` on reactions; `enchantedFaceBonus` and `freeMarketRerolls` on items; `AddValueToLowestFaceEffectSO`; `ApplyStatusEffectSO.ignoreRitual`.
- **Tooltip layering (the user's play-test):** the Pass button and the Market's next battle line drew over the tooltip, because the scenes added them after it. `TooltipUI` now moves itself to the top of the canvas whenever it opens, so later additions can't cover it either. Its panel is opaque, since text below could be read through it.
- **Curses are noted in the Roadmap** for later. Cursed Idol was renamed Challenger's Mark so the name won't clash with that mechanic.

### Tests
- **Checks (14/14):** every new item's rule, Second Blade's limits before its final numbers, and prices by rarity.
- **Play Mode:** the tooltip covers the next battle line in the Market and the Pass and Roll buttons in battle, with no console errors. The new items were only checked through code, not on screen.

### Balance
- **Numbers changed after measuring (free from the start, 200 runs):**

| Item | First version | Its effect | Final version | Its effect |
|---|---|---|---|---|
| Second Blade | Every turn | Rage 38% to 69% | First 2 turns | Rage 25% to 34% |
| Hourglass | 2 Rage on turn 5 | Opportunist 6% to 20% | 1 Rage on turn 5 | 6% to 9% |
| Smelling Salts | Heal 5 | Opportunist 6% to 15% | Heal 3 | 6% to 12% |
| Challenger's Mark | 1 Rage for you | Opportunist 6% to 4% | 2 Rage for you | 6% to 13% |

- The final Mark on the Rage bot read 30% against no-gift rates of 25-38%, so it is within noise.
- **Default bots (300 runs):** Bleed 36%, Bleed + Rage 28%, Rage 20%, Shield 18%, Poison 16%, Hammer 14%, Ritual + Poison 14%, Shield + Rage 13%, Burn 11%, Fortune 11%, One-face carry 10%, Opportunist 5%, Unplanned buyer 1%.
- **A bot taking every item first (200 runs):** Opportunist 14% against 5% with normal habits, Rage 32% against 20%.

### Findings
- At these prices items are usually a better buy than upgrades. That fits the user's wish and helps patchwork dice, but the prices may be too low. If the Market stops feeling like a choice, Common 15 and Rare 20 are the first thing to try.
- Bots still don't buy enchantments, so Scribe's Quill and the cheaper enchantments can only be judged in play.

### Next Steps
- **The user's play-test:** do prices force a choice, is Challenger's Mark worth it, do Scribe's Quill and cheap enchantments make enchanting attractive?
- **The Shield bot and a Fortify question for the Hydra.**
- **The map, more battles and the difficulty curve.**

## Day 82 - The Hydra's Scales and Face Keywords

### Accomplished (the user's play-tests: the last boss fell in seconds, even with a broken die, and the same die enchantment combo came up in every run)
- **The Hydra grew into a final exam:** 60 HP (was 30), and a new face, Corrode (Attack: remove half of your Fortify, then 3 damage), which replaces one Hiss.
- **Scales:** the Hydra's body loses at most 12 HP a round (`BossSO.maxHPLossPerRound`, `Entity.maxHPLossPerRound`), ticks included. It lengthens fights against strong dice and doesn't touch weak ones. Slay the Spire's last boss has a similar cap.
- **Face Keywords (the user's idea):** rules on a whole face. `FaceKeyword` on `DieFaceSO.keywords`; Consumable and Charge are the first two, and the old fields were migrated.
- **Consumable is now on the face:** it's usable once per battle whichever action is chosen, and afterwards every action on it is Change. As a keyword on one action, an enchantment could use a high-value Consumable face without limit (Throwing Knife with Whirlwind).
- **Eight new die enchantments, pool 6 to 14:** Keystone, Right Hand (Common), Outer Ring, Low Guard, Burnout (Rare; the user's idea), Center of Gravity, Twin Weights, Twin Soul (Epic; the x2 is the user's idea, with Bottom at 0 as its price).
- **`DieEnchantmentSO`:** `positionMultipliers` and `consumablePositions`. Chips and the Market show the real battle value.

### Decisions
- **A cap instead of only more HP:** more HP alone walls weak dice, while a cap only touches strong ones.
- **4-5 minutes like Slay the Spire isn't realistic:** a turn here is usually one decision. The aim is 5-6 turns at least for the strongest die and about 10 for a normal one.
- **Added Face Keywords come after a play-test:** Growing, Heavy and Brittle at the Enchanting Table, one per face. Consumable stays printed only at first, since adding it to a weak face could erase that face from the die.
- **The Shield bot stays as it is.** It plays a complete Fortify die well (clears 31% at values 2 and 87% at values 4, from the start); its low rate comes from rarely completing that die. Earlier notes blaming its play were wrong.

### Tests
- **Checks (14/14):** the cap (ticks included, reset each round), Consumable faces, Knife with Whirlwind as one burst of 20 at value 10, the potion, Alchemist's Belt, the new die enchantments, and Weighted Die no longer guaranteed (121 of 300 Grand Markets; it was 300 of 300).
- **Play Mode:** the Hydra fight opens with a readable card and the correct values on the die, with no console errors. A whole Hydra fight wasn't played on screen.

### Balance
- **The Hydra among arrivals:** the old one (30 HP, 3 damage) lost to 73-98% of arriving bots, so the last boss was the easiest late fight.

| Hydra | Bots beat it | The user's Fortify die, values 4, beat it |
|---|---|---|
| 45 HP, 5 damage, Corrode | 41% | 168 of 174 |
| 60 HP, cap 12, 5 damage | 19% | 20 of 125 |
| 50 HP, cap 12, 4 damage | 33% | 51 of 115 |
| 60 HP, cap 15, 3 damage | 43% | 81 of 119 |
| **60 HP, cap 12, 3 damage (chosen)** | **44%** | **69 of 118** |

- A longer fight needs less damage per turn: at 5 damage even that die wore down.
- **Default bots (300 runs):** Bleed 20%, Rage 12%, Bleed + Rage 10%, Ritual + Poison 8%, Burn 7%, Poison 7%, Shield 7%, Shield + Rage 5%, Fortune 3%, Hammer 2%, One-face carry 1%, Opportunist 0%, Unplanned buyer 0%.
- **A die enchantment free from the start (180 runs):** Rage 3% with none, 7% with Burnout, 4% with Twin Soul. Bots put their weakest face on those spots, so the trick of erasing a face was tried; it did not explode. Samples are small, and bots don't use Twin Soul on purpose.

### Findings
- **The Opportunist reached the Hydra only 11 times in 300 runs and never won,** below the 5-15% target. Most of those runs die earlier, so the Hydra isn't the only cause. The user plays far better than the bots, so enemies stay as they are until a play-test.
- Stacking on one face is no longer strong (1%).

### Next Steps
- **The user's play-test:** does the Hydra feel like a final or a slog, can a patchwork die beat it, is Knife with Whirlwind still fun as one burst, do the new die enchantments lead to other arrangements?
- **Growing, Heavy and Brittle at the Enchanting Table.**
- **A Windows build and the first friends' play-test.**
- **The map, more battles and the difficulty curve.**

## Day 83 - Fuel Burns It All Up

### Accomplished (the user's play-test: a Burn die with two Fuels one-shot many enemies; the Hydra felt a little tense, dropping to 10 HP at times; a run took 12-13 minutes)
- **Fuel's Magic now consumes the Burn:** it applies Burn, multiplies it, then consumes it all for that much damage (`IgniteEffectSO.consume`). The Day 76 version kept the Burn, so two Fuels compounded each other.

### Balance
- **Burn bot, Fuel free before battle 5 (220 runs):** none 10%, 1 Fuel 21%, 2 Fuels before 46%, 2 Fuels now 25%. One Fuel is still strong, and compounding is gone.
- **Tag match chance, measured directly on a die with Burn faces (2000 Markets):**

| Match chance | Burn in offers | Markets with a Burn face |
|---|---|---|
| 60% (today) | 7% | 21% |
| 40% | 7% | 21% |
| 25% | 7% | 21% |
| 0% | 6% | 18% |

### Decisions
- **The tag match chance stays at 60%.** It looks for faces sharing any tag with the die, and the starting die has Physical, Bleed and Block, so nearly every face already matches. Lowering it would change nothing. It only becomes a real dial with `weightByBuildTags` on.

### Findings
- **"Building is too easy" isn't about how often offers appear:** only about 1 Market in 5 shows a Burn face. More likely a few pieces are enough: a few Burn faces, Tinder Box, Smoldering Gauntlet and Fuel. That is a matter of content and item power, not one number. Cheap rerolls and locks may also help; the user's reroll use is still to be asked.
- **Correction to Day 79:** General faces at about 25% of offers aren't mainly due to fitting any die; with matching off they are still 18%, since most are Common.
- **The user's feedback on patchwork dice:** a patchwork die needs faces that talk to each other, not disconnected ones. New faces should talk to at least two archetypes as content grows.
- The new die enchantments did lead the user to try other arrangements.

### Next Steps
- **A Windows build and the first friends' play-test.**
- **Growing, Heavy and Brittle at the Enchanting Table,** after that test.
- **The map, more battles and the difficulty curve.**

## Day 84 - A Friend's Play-Test

### The play-test
The user had a friend who likes the genre play a run, with the user explaining the game along the way.
- **The text was overwhelming at first;** after 2-3 battles the friend got used to the Market.
- **"It's very hard to enjoy without a UI."** The friend didn't want a second run, but asked for the game to be developed and wants to try it with a UI.
- **Ritual's description wasn't understood.**
- **Iron Bracers felt weak.**
- **Exsanguinate broke the run in the middle:** with a single Bleed face it hit for 24, and the friend, who had been building around Flip, said "it's over then, what is this". Every later fight was very short, the Hydra included.
- **The Hydra's heads:** the friend hit them first, since they have less HP, and found that cutting one gave nothing back: the heads came back at once as two.
- **Suggestion:** characters that change the starting die. They're already on the Roadmap (Phase D).

### Accomplished
- **Exsanguinate and Fuel apply a fixed 2 Bleed or Burn as their fuel,** not their value. With value-sized fuel the value counted twice, so upgrades grew their damage with the square of the value. GDD "Epics Bring Their Own Fuel" now says so.
- **The Hydra's heads:** cutting one deals 5 damage to the body (`BossSO.headCutDamage`, through `PayHP`, so it counts toward Scales), and the head grows back as two one body turn later instead of at once (`cutSeenAt`). Burn applied in that turn still cauterizes. The card says when each cut head grows back.
- **Iron Bracers:** 3 Fortify (was 1).
- **Ritual's description:** "Each time you apply Burn, Poison or Bleed, apply 1 more for each Ritual you have. Lasts the whole battle."

### Tests
- **Exsanguinate on 2 Bleed:** 8 / 12 / 16 / 24 damage at values 2 / 3 / 4 / 6; it was 24 at value 4. Fuel on a target without Burn: 4 / 6 / 8 / 12.
- **The Hydra's heads (9 checks):** a cut head waits a turn and then grows back as two, Burn in that turn cauterizes, two cuts on different turns regrow on different turns; a cut deals 5 once, a regrown head pays again when cut again, and it counts toward Scales. Not yet seen in Play Mode.

### Balance
- **Free before battle 5 (200 runs):** Bleed 17% without, 24% with Exsanguinate (it was 26% to 47% before); Burn 4% without, 17% with Fuel; Opportunist with Iron Bracers at 1 cleared 0% and reached battle 9 in 38 runs, at 3 cleared 4% and reached it in 64.
- **The Hydra, focused bots and the Opportunist (110 runs each):**

| Heads | Bots going for the body | Bots cutting every head first |
|---|---|---|
| Back at once (before) | about 40% | not measured |
| Back a turn later | 40% | 1% |
| Back a turn later, cut deals 5 | 39% | 6% |

- **Default bots (300 runs):** Bleed 20%, Rage 12%, Bleed + Rage 12%, Burn 11%, Ritual + Poison 11%, Poison 9%, Shield 7%, Shield + Rage 4%, Hammer 3%, Fortune 3%, One-face carry 3%, Opportunist 1%, Unplanned buyer 0%.

### Findings
- **Cutting heads is still mostly a trap for bots,** but they cut every head every time. A player can cut one when a turn's breath is worth it. If play-tests show it never is, the cut damage can go up (8).
- **The friend's run needed the user's guidance,** so the text load is even heavier for a new player than this test showed. The UI is now the most important problem.

### Next Steps
- **Three style mock-ups of the battle screen,** so the user can choose a look.
- **A UI redesign:** icons and a short card language.
- **Growing, Heavy and Brittle at the Enchanting Table.**
- **The map, more battles and the difficulty curve.**

## Day 85 - Faces for the Enemies

The friend's play-test said the game is hard to enjoy without visuals. After comparing art styles, the user chose Roman pixel art. Sprites drawn in code weren't good enough and a paid PixelLab plan was out of budget, so this day tried free tools.

### Accomplished
- **Every enemy and boss has a sprite** (`Assets/Art/Enemies`, `Assets/Art/Bosses`): 15 enemies (Hydra Head included) and the 3 bosses. The user made them with Gemini from `Docs/ArtPrompts.md` (one style block plus a subject line per enemy, the player gladiator attached as the style reference). A cleanup script then put them on a true pixel grid (Gemini draws on a 240-pixel grid), removed the magenta background and stray specks, cut the palette to 32 colors and flipped the ones facing right.
- **The player is a PixelLab gladiator** (`Assets/Art/Player/Gladiator.png`, character "Gladiator HD", 128-pixel canvas, 3 generations), in place of the blue circle. The first one was drawn on a 64-pixel canvas, so its pixels were twice the size of the enemies'; at 128 the gladiator is 123 pixels tall, like the human enemies.
- **`EnemySO` / `BossSO.sprite` and `spriteHeight`** (in human heights: Arena Wolf 0.65, War Elephant and bosses 1.3–1.35), copied onto `BattleEnemy`. `BattleManager.ArrangeEnemySprites` stands every enemy on one ground line; each takes its own width, and a row that doesn't fit shrinks by one factor, so the Hydra's body stays bigger than its heads.
- **The battle scene's sprites were moved** clear of the UI: the player stands above the die net, the enemies below the enemy cards.
- **GDD "Visual Style".**

### Decisions
- **Gemini for still images, PixelLab for animation.** Gemini's still sprites were good on the first try, but its 4-frame attack changed the character from frame to frame. PixelLab animates the character itself; blending its skeleton mode (which swings the sword) with its smooth mode (as start and end frames) gave the best attack so far.
- **Tripo and Leonardo were not used:** Tripo makes 3D models (a render pipeline we don't need), and Leonardo's pixel art isn't on a grid and can't animate a character consistently.

### Tests
- **Play Mode, Hydra:** the body and two heads stand on the ground line in turn order, the body bigger than the heads. Groups and the other bosses were checked in an edit-mode preview with the UI drawn.

### Next Steps
- **Hit and idle animations** for the player (PixelLab), and fixing the shield that disappears in the attack.
- **The boss die strip overlaps the enemies' heads;** it moves with the UI redesign.
- **The attack animations were made on the 64-pixel gladiator;** they have to be made again on the new one.
- **A UI redesign:** icons and a short card language.

## Day 86 - The Arena Takes the Stage

The friend said the game is hard to enjoy without a UI. Before touching Unity, a mock-up of the battle screen was made on a design canvas with the real sprites, and a "card language" page for later (numbers on screen, words on hover). The user approved the layout and the fonts (Cinzel for titles, Pixelify Sans for numbers, later replaced by VT323, see Day 87), then made two arena backgrounds with Gemini (day, and night for bosses) from prompts in this session.

### Accomplished (stage 1 of the redesign: the layout)
- **The arena is the battle screen:** a world sprite behind everything (`BattleManager.arenaBackground`), the night arena in boss battles (`bossArena`). Both were cleaned onto a 480-pixel grid with 64 colors (`Assets/Art/Backgrounds`).
- **Enemy cards stand on their enemies.** `UIManager.LateUpdate` places every `EnemyCardUI` on its sprite: the plate (name, HP, statuses, TARGET frame) under the feet, the intent and the traits above the head, and the card itself covers the sprite, so clicking an enemy targets it. A card is as wide as its slot in the row (`BattleEnemy.slotWidth`, at least `MinSlotWidth`), and a crowded row (the Hydra with three heads) shrinks the cards with the sprites. An enemy that joined this turn shows no empty intent box.
- **Block and Fortify are status chips** (first in the row) instead of a text line, on the player too.
- **The player's plate is under the player's feet.** The characters are a bit smaller (`HumanHeight` 2.2) and stand on the arena floor.
- **A top bar** (`ShowBattleInfo`: battle number, BOSS, gold) with the items and the boss's die under it, and **a bottom HUD:** the die net on the left, the rolled face in the middle, Roll / Reroll / Pass over the three action buttons on the right, in the arena's colors.

### Tests
- **Play Mode** (started through reflection, as the Editor was in the background): a Murmillo + Tiro + Arena Wolf group, and the Hydra with three heads in the night arena. Plates and intents no longer overlap; the boss die shows under the top bar.

### Next Steps
- **Stage 2, the card language:** statuses, intents and action previews as icon + number, with the full text in the tooltip.
- **Stage 3, the icon set** (Gemini, prompts to come) and **the fonts** (Cinzel and a pixel font as TMP font assets).
- **The Market and the run start screen** get the same look after the battle screen.

## Day 87 - Numbers on Screen, Words on Hover

Stage 2 of the redesign, with the art the user made in the meantime: 24 icons from two Gemini sheets (the first came out 5 by 3; its three extra icons were dropped) and the two fonts.

### Accomplished
- **The icons are inline text:** `Assets/Art/Icons` packed into an atlas and a TMP sprite asset (`Resources/Sprite Assets/Icons`, glyphs at 1.4×), used as `<sprite="Icons" name="Bleed">`.
- **`GameText.Iconize`** turns every effect description into icon + number: "N damage", "N block", "heal N", "gain N Rage" (with a +), "lose N HP", "2 x 3 damage", "N + rerolls left", a status's stacks, "Burn x3" and a status standing alone ("consume Bleed"). It marks each icon as one private-use character first, so later patterns never match inside a sprite tag. Checked on every description the faces and enemy intents produce.
- **Over an enemy's head** (`GameText.IntentSummary`): the intent's first line without its name ("Shield Wall: 2 block" → [Block]2), plus a waiting trait; the full text, name and rules stay in the tooltip. The Hydra's three-line intent no longer covers the arena.
- **Status chips** are an icon and a number; **action buttons** show the action in Cinzel and its preview in icons; **gold and Reroll** carry their icons.
- **Fonts:** Cinzel for titles (names, the battle number, the rolled face, the buttons' first line), VT323 for the rest. Pixelify Sans was dropped: its 5 reads as an S and its 2 as a Z, so "50 / 50" read "S0 / S0". VT323, Press Start 2P and Silkscreen were compared side by side; VT323 has the clearest digits and is narrow enough for the face texts. Font sizes went up 1.4× for it.

### Tests
- **Play Mode** with a Murmillo + Venefica + Arena Wolf group and statuses on every side: intents, chips, buttons and HP read as numbers and icons, nothing overlaps.

### Next Steps
- **The die net and the rolled face** still use words; face icons would finish the card language.
- **The Market and the run start screen** in the same look.
- **Animations** for the player (idle, attack, hit) on the new gladiator.

## Day 88 - The Market Joins the Arena

After the battle screen, going to the Market dropped the player back into the old blue screens. This day gave the Market and the run start screen the battle screen's look, without touching their layout or rules.

### Accomplished
- **A theme pass over Market.unity and RunStart.unity:** a dimmed arena behind everything (a `Backdrop` image), the camera's background the arena's dark brown, the old translucent black panels now dark brown with a bronze outline, every white button a dark bronze one with a bronze outline (the main ones, Next Battle, Choose and Buy, in a lighter bronze), dark text turned bone white, the titles (MARKET, CHOOSE YOUR STARTING FACE, BOSS REWARD) in gold Cinzel and every other text in VT323 (1.4× its old size).
- **The tooltip prefab** (`Assets/UI/Tooltip.prefab`, in all three scenes) in the same colors and font.
- **Market text colors for a dark background:** "Can be enchanted", "Moving" and a die enchantment's bonus in gold, "Has its own Magic" in a light grey (they were picked for the old light buttons).
- **Sizes that overflowed after the font change:** the hint line, the Enchanting Table and Heal buttons and Next Battle.

### Tests
- **Play Mode:** the run start screen and a Market after the first battle. Nothing overlaps; the hint fits on one line. The Enchanting Table tab and the boss reward panel use the same objects but were not seen, since the Editor didn't draw new frames in the background.

### Next Steps
- **The Market's card language:** offers still describe faces in words; a line of icons on each card would make them faster to compare.
- **Face icons** for the die net and the rolled face.
- **Animations** for the player.

## Day 89 - Every Face Has a Face

The user said the game doesn't feel like selling die faces, the faces are small and unreadable, and a player who knows the game should recognize a face by its look, without reading it. A second round of mock-ups (battle at a fighting scale with a dice tray, a Market selling die-face tiles) led to the face tile language, now in the GDD.

### Accomplished
- **An emblem for each of the 66 faces**, made by the user with Gemini from six sheets in `Docs/ArtPrompts.md` ("Face emblems"), each object named by its face (Anvil an anvil, Exsanguinate a chalice of blood). Sheet 2 came out on an uneven grid and was cut by hand; sheet 6 drew 12 emblems instead of 6, and the extra six (a net, laurel dice, a gladiator helmet, a gold pouch, a trident, a dice cup) are kept as spares for future faces. All cleaned onto a pixel grid with 24 colors, in `Assets/Art/Faces`.
- **`DieFaceSO.emblem` and `plainTile`**, linked for every face by name.
- **`FaceTileUI` and `Assets/UI/FaceTile.prefab`:** the rim is the rarity (bronze, silver, gold), a thick band the family's color, the center a pale tint of it, the emblem in the middle, a gem for a second family and a value badge.
- **The battle's die net and the rolled face use the tiles:** the chips are square tiles (the rolled one framed in gold; stone and used faces dimmed with a word), and the rolled face panel shows the face as a big tile next to its name and rules.

### Decisions
- **The family is a band, not the whole tile.** With the whole tile in the family's color, dark emblems (Dark Communion's hood, the Anvil, Iron Rage's helmet) vanished on dark grounds; with a pale center every emblem stands out and the color still reads from afar.
- **Emblems never reuse status icons:** a face is what it is, a status what happens.

### Tests
- **Play Mode** with a die of six families (Exsanguinate, Shield, Iron Rage, Venom, Fate's Hand, Sword): every tile shows its family, rarity and second-family gem; the rolled face shows big.

### Next Steps
- **The battle at a fighting scale** (bigger characters and intents) and **the dice tray** with a roll animation, from the v2 mock-up.
- **The Market with face tiles**, from the v2 mock-up.
- **The starting Sword's emblem** is thin and horizontal, so it reads small; a diagonal one would fill its tile.

## Day 90 - A Market That Sells Faces

The Market from the v2 mock-up, built on the face tiles, with its rules untouched.

### Accomplished
- **Face offers are cards with a big tile:** the tile on top, then the name in Cinzel, the rarity and one line of icons per action (`OfferCardUI.DescribeFace`, from each effect's `Describe` with the base value), plus the face's keywords; the full rules are in the card's tooltip. The run start screen's three offers use the same cards.
- **The die is a net of big tiles** with a small "+1, gold" upgrade button beside each one. A slot's text is now only a caption: MOVING while swapping, the empty-Magic note on the Enchanting Table, a die enchantment's effect on that place.
- **A new layout:** a top bar (title, gold, HP, battles won, Heal, Next Battle), a row for the Enchanting Table tab, Reroll, the hint and Cancel, the shelf with every offer in the middle, the die panel at the bottom left (with the die enchantments beside the net) and the next battle and the items at the bottom right.
- **Prices show the gold icon** on every card and upgrade button; gold and HP in the top bar carry their icons.
- **The face tile is anchored in proportion** (bands, emblem, gem, value badge), so the same prefab works from the die net to the shelf.

### Decisions
- **No "selected face" panel yet.** The mock-up's panel needs a new selection step, and clicking a die face already starts a swap; an upgrade button beside each tile keeps today's clicks.

### Tests
- **Play Mode:** the run start screen (Frenzy, Feint, Maul with its Charge keyword) and a Market after the first battle with Iron Rage on the die (its second-family gem shows). The Enchanting Table tab and the boss reward weren't seen (the Editor drew no new frames in the background).

### Next Steps
- **The battle at a fighting scale and the dice tray**, from the v2 mock-up.
- **The starting Sword's emblem** reads small (thin and horizontal).

## Day 91 - A Fight You Can Read From the Couch

The battle from the v2 mock-up: a fighting scale and a dice tray.

### Accomplished
- **Bigger fighters:** `HumanHeight` 3.0 (was 2.2), the enemies' row from x 0.2 to 8.6, the ground line lower, the player further left. Plates sit just under the feet and are a little shorter.
- **Bigger intents:** 56-point numbers with icons to match over every head.
- **The dice tray:** the rolled face panel is a red felt tray with a bronze rim. Roll is a big button inside it during the roll phase; once the die lands the tray shows the face as a big tile at a slight angle, next to its name and rules. A roll, a reroll and Change/Flip/Tumble play a short tumble (the tile flips through the die's faces, grows and lands), only for the picture: `ShowRolledFace` got a `landed` flag so a refresh (choosing a target) doesn't replay it.
- **Bigger buttons:** Reroll and Pass above three tall action buttons on the right; the die net's tiles grew a little (98).

### Tests
- **Play Mode:** a Murmillo + Venefica + Arena Wolf group (roll phase with Roll in the tray; action phase with Fate's Hand landed) and the Hydra with three heads in the night arena. Nothing overlaps. The tumble itself wasn't seen: the Editor drew no frames in the background, so it needs a look in a real play.

### Next Steps
- **Watch the tumble in a real play** and tune its speed.
- **The selected-face panel in the Market**, so upgrades and details live in one place.
- **Player animations** (idle, attack, hit) on the new gladiator.
- **The starting Sword's emblem** reads small.

## Day 92 - The Selected Face

The Market's selected face panel from the v2 mock-up, which Day 90 left out.

### Accomplished
- **Clicking a face on the die selects it** (framed in gold) and shows it in a panel beside the net: a big tile, its name, rarity, place and opposite face, one line of icons per action at its current value (`OfferCardUI.DescribeActions`, now shared with the offer cards) and its Magic status. Without a selection the panel says what it is for.
- **Upgrade and Move live in the panel:** "Upgrade 2 → 3" with its gold price (or a note that Epic faces are upgraded at the Grand Market), and Move, which starts a swap; the next face clicked swaps places, and the selection follows the moved face. A bought face is selected right away.
- **The die net's tiles are bigger** (150, from 140 with an upgrade button beside each); the per-slot upgrade buttons are hidden when the panel exists.
- **The die enchantments moved to the right panel**, under the items, with their own label.

### Tests
- **Play Mode, through MarketUI's own methods:** selecting Iron Rage, Move then another face (they swap and the selection follows), one upgrade (2 → 3, gold spent once), selecting another face. Pressing the buttons through their listeners upgraded twice in the test only because the test had run `Start` a second time.

### Next Steps
- **Watch the dice tray's tumble in a real play.**
- **Player animations** (idle, attack, hit) on the new gladiator.
- **The starting Sword's emblem** reads small.

## Day 93 - The Gladiator Moves

Animations for the 128-pixel PixelLab gladiator (the 64-pixel one's attacks no longer fit it).

### Accomplished
- **Three animations from PixelLab**, east only: idle (smooth mode, 8 frames, 4 generations: a ready guard, breathing, the cape moving), attack (skeleton mode's slash, 8 frames, 3 generations: the sword raised behind, a lunge with the cape flying, the blade coming down, recovery) and hit (skeleton mode's taking-punch, 6 frames, 3 generations: the head snaps back behind the shield). 10 generations in all; 13 are left plus the daily bonus.
- **`SpriteAnimator`** plays them on the player's SpriteRenderer: idle loops (6 fps), attack (14 fps) and hit (12 fps) play once and return to idle; a multi-hit action swings once, and a counter-hit doesn't cut a swing short. All frames share one 176-pixel canvas with the pivot at the feet, so they line up on the ground line.
- **`BattleManager` plays attack on `player.HitDealt` and hit on `player.HitTaken`** (a hit that cost HP), so items' and Thorns' hits animate too, without the rules knowing.

### Decisions
- **No blend this time:** at 128 pixels the skeleton slash already looked good and kept the shield, so the smooth-mode blend (4 more generations) wasn't needed.

### Tests
- **Play Mode:** a hit dealt by the player starts the attack clip; the gladiator stands on the ground line at the same size as before. The clips themselves weren't watched: the Editor drew no frames in the background.

### Next Steps
- **Watch the animations and the dice tray in a real play** and tune their speeds.
- **Enemy reactions:** a hit flash or shake on the enemy that takes damage, since enemies have one still sprite each.
- **The starting Sword's emblem** reads small.

## Day 94 - Enemies Feel the Blows

Enemies have one still sprite each, so they now react on screen without new art.

### Accomplished
- **`HitReaction`**, on the scene's Enemy object (every group member and Hydra head is a copy of it) and on the player:
  - a hit that costs HP flashes the sprite red and shakes it for a quarter second;
  - dealing a hit lunges the enemy toward the player and back (0.3 s), standing in for an attack animation; the player has none, since it swings;
  - a dying enemy fades out instead of vanishing (`SetVisible`, which `BattleManager.UpdateEnemySprites` now calls); a revived Hydra head shows again at once.
- **The row and the reactions don't fight over positions:** `ArrangeEnemySprites` tells each reaction its resting place (`SetRest`), and a reaction only moves its sprite while it plays and then puts it back.

### Tests
- **Play Mode:** a hit on the Murmillo starts its shake and flash, its hit on the player starts its lunge and the player's flash, and both end with the Murmillo back in place in its own colors; a killed Tiro fades and is hidden, its alpha reset for a revival. The motion itself wasn't watched (the Editor drew no frames in the background).

### Next Steps
- **Watch the reactions, the gladiator's animations and the dice tray in a real play** and tune their timing.
- **Floating damage numbers** over the one who was hit would make every hit readable.
- **The starting Sword's emblem** reads small.

## Day 95 - Every Hit Has a Number

### Accomplished
- **Floating numbers:** whenever a combatant's HP changes, a big number pops at its chest height, rises and fades in 0.9 s: red "-5" for HP lost, green "+3" for HP healed. They come from HP changes, not from hits, so Poison, Bleed and HP costs show too. `FloatingNumbersUI` keeps each combatant's last HP, so setting up the battle shows nothing; `UIManager` tracks the player and every enemy card's entity, Hydra heads included.
- **A dark outline** makes the numbers read over the sand. Each number outlines its own copy of the font's material (destroyed with it): a separate outline material asset drew blocks, because a dynamic font's atlas texture changes under it.

### Tests
- **Play Mode:** a hit on the Thraex (-5), a hit on the player (-3) and HP lost then healed on the Arena Wolf each pop a number in place. First placed over the heads, the numbers collided with the intents, so they now start at chest height.

### Next Steps
- **A real play** to judge the feel of the tray, the animations, the reactions and the numbers together.
- **Blocked hits** could show their own grey number (today a fully blocked hit shows nothing).
- **The starting Sword's emblem** reads small.

## Day 96 - Blocked Hits Show Too

### Accomplished
- **A grey shield and the amount** pops when Block or Fortify absorbs a hit, so a fully blocked hit is no longer silent. A partly blocked hit shows both numbers: the shield to the left and below, the red HP loss to the right.
- **`Entity.HitAbsorbed`**, raised by `TakeDamage` with `LastHitAbsorbed` when it is above 0. It is for the UI only: nothing in the rules listens to it, and the turn-start Block reset, which isn't a hit, doesn't raise it.

### Tests
- **Play Mode:** the player with 4 Block taking 6 shows the shield 4 and -2; the Murmillo with 5 Block taking 3 shows the shield 3 and no HP number. The shield icon first drew at the number's full size and covered the red number; it is now half size.

### Next Steps
- **A real play** to judge the tray, the animations, the reactions and the numbers together.
- **The starting Sword's emblem** reads small.

## Day 97 - A Turn You Can Follow

The user played the new screens and found the worst problem: the enemies answered in the same frame as the player's action, all at once, so their hits landed with the player's.

### Accomplished
- **The enemy turn is paced** (`EnemyTurnState`, a coroutine on `BattleManager`): 0.7 s after the player's action (the swing, the numbers, a death's fade), then each living enemy acts in turn order with 0.65 s after each (its lunge, its numbers), then their statuses tick and, 0.35 s later, the player's turn starts. A Hydra head that joins mid-turn still gets its turn, as the loop goes by index.
- **`BattleFlow.EnemyActOne(index)`:** one enemy's turn. `EnemyAct` now loops it, so the simulator's turns are unchanged and unpaced.
- **`UIManager.ShowEnemyTurn`:** every button is disabled and the tray reads "Enemy turn" while the enemies act.
- **The starting Sword's emblem** is the user's new one: a wide gladius on the diagonal with a bronze guard, which fills its tile and doesn't look like the Damage icon.

### Tests
- Compiled; the simulator's steps are unchanged. The pacing can only be seen in a real play (the Editor draws no frames in the background), so the user will judge the pauses.

### Next Steps
- **Tune the three pauses** from the user's play.
- **A turn banner** ("Your turn" / "Enemy turn") could mark the change of side, as the tray's title is easy to miss.

## Day 98 - Whose Turn Is It

From the user's play of Day 97: a banner for the change of side, longer pauses, and no actions while the die still tumbles.

### Accomplished
- **Turn banners:** "YOUR TURN" (at the start of each player turn, `PlayerRollState`) and "ENEMY TURN" (after the pause that follows the player's action) pop across the middle of the screen in gold Cinzel on a dark band, hold and fade in 0.9 s (`UIManager.ShowTurnBanner`). The band never takes clicks.
- **Longer pauses:** 0.9 s after the player's action (was 0.7), then the banner and 0.6 s, 0.85 s after each enemy (was 0.65) and 0.5 s after the enemies' statuses tick (was 0.35).
- **No action while the die tumbles:** the action, Reroll and Pass buttons stay off until the tumble ends (`UIManager.IsRolling`), and `BattleManager.OnActionSelected` ignores clicks meanwhile. The tumble's end opens them by redrawing the rolled face, unless the phase moved on (`actionPhase`, set false by the roll, choose-face, enemy-turn and result screens). Change/Flip/Tumble rolls are covered too, as their state redraws the face while it still tumbles.

### Tests
- **Play Mode:** YOUR TURN shows at the battle's start; right after a roll every button is off and clicking Attack and Defense changes nothing (the Murmillo's HP stays 20); once the die lands, the face's actions and Pass open. The ENEMY TURN band reads clearly over the arena.

### Next Steps
- **The user's play** to judge the new pauses and the banners.

## Day 99 - Less Box, More Gladiator

From the user's play: the timing is right now, but the big rectangles around the fighters took away from them, and a face's actions, its main parts, were too faint.

### Accomplished
- **Slim combatant UI:** the plate under each fighter has no background or name any more, only an outlined HP bar (the number inside) and the status chips under it; the player's too. An enemy's name is in its intent's tooltip.
- **Intents as wide as their content:** the intent box over a head is just the icon and the number (its layout group no longer expands to the card's width), and so are the trait chips.
- **Actions stand out in every face description:** each line starts with the action's icon and its name in its color, Attack red, Defense blue, Magic purple (`GameText.ActionLabel`), in the Market's cards, the selected face, the run start's offers and now the dice tray, which shows these lines instead of the face's prose (the rules stay in the tooltip).
- **`Iconize` reads "N + Bleed damage"** (Lacerate before a target) as [Damage]N + [Bleed].

### Tests
- **Play Mode:** a Murmillo + Venefica + Arena Wolf battle (slim bars, tight intents, the tray's Lacerate with its two colored lines) and a Market with Bloodscent, Gambler's Strike and Shield Burst on offer and Lacerate selected.

### Next Steps
- **The Market's cards are narrow** for two action lines with conditions (Shield Burst, Bloodscent); wider cards or a smaller font may help.
- **A real play** of the slim UI.

### Day 99, later: from the user's screenshot
- **The action lines are further apart** (a half-height empty line between them), **without the icon in front** and **with a colon** after the name: "ATTACK:", "DEFENSE:".
- **The TARGET frame wraps just the HP bar**, tightly: it is now a child of the bar instead of the whole plate, with 5-pixel sides.
- **Face tooltips put each action on its own line** under its colored name, with the same half-height gap (`GameText.ActionLines`, used by both `FaceTooltip`s): "Attack: ... Defense: ..." reads as ATTACK: ... / DEFENSE: .... A keyword before the actions (Consumable) stays on top, and Charge's rule after them gets its own line.
- **Ignitions read in one line:** "ignite Burn (0 damage)" over Lampadarius's head was two lines with a 0 in it; `Iconize` now writes it [Damage]N = [Burn] (damage equal to the target's Burn, the real number when it burns), and Detonate-like ones add "consume [Burn]".
- **Wider Market cards:** 440 pixels (were 340), with wider Buy and Lock buttons, so a face's two action lines no longer shrink to fit. The fullest shelf, a Grand Market's three faces and two items (or the Enchanting Table's three and two), still fits.
- **Special effects read in the card language too** (`Iconize`, display only): Earthshaker [Damage]1 × highest face, Shield Burst [Damage]2 + [Fortify]×2, Shield Bash [Damage]2 + [Block] + [Fortify], Spiked Shield [Damage] = [Fortify] (min 2), Ember and Fuel [Damage] = [Burn], Exsanguinate and Unleash [Damage]2×[Bleed] / ×[Rage], Last Stand "+1 per 5 [Health] lost", Drill Sergeant "+1 value to the other faces (not my opposite)". All 66 faces' lines were listed and checked.

## Day 100 - Faces Speak in Words

The user, who knows every face, still couldn't tell what a face did from its icon line ("icons, numbers, commas... nothing reads") without hovering over it. Faces now show their full rules everywhere.

### Accomplished
- **Full rules on every face** (`GameText.FaceRules`): the Market's cards, the selected face, the run start's offers and the battle's dice tray write the face's description out, each action on its own line under its colored name (ATTACK:, DEFENSE:, MAGIC:), keywords colored. `OfferCardUI.DescribeActions`, the icon lines, is gone.
- **Keyword definitions on hover:** every text that shows a face's rules has a `KeywordLinkTooltip` (the selected face's panel got one), so hovering Bleed or Burn opens its definition. Face cards and the tray have no tooltip of their own any more, since the rules are written out.
- **The action buttons preview in words**, with real numbers and colored keywords ("2 damage, 2 Bleed").
- **Intents over the heads and status chips stay icons:** they must read at a glance, and the user had no trouble with them.
- **Room for the prose:** the run start's and the Market's face tiles are a little smaller and the card texts can shrink further (20) before they would overflow.

### Decisions
- **Faces are the exception to the card language** (GDD "Visual Style"). A face is the thing the player builds and compares, so its rules must read as sentences; icons and numbers are for what the enemies are about to do.

### Tests
- **Play Mode:** the run start (Frenzy, Exploit, Spiked Shield, whose long rules first ran under the Choose button) and a Grand Market (Shield Burst, Dark Communion, Fate's Hand, Health Potion selected).

### Next Steps
- **The corner gem** (a face's second family) puzzled the user; keep it, explain it on hover, or drop it.
- **The corner gem is gone** (`FaceTileUI`, the `FaceTile` prefab, the GDD and the prompts): it showed a face's second family, but nothing explained it and the user couldn't tell what it meant. A face's other tags speak through its rules, which are written out now.

## Day 101 - Shields on the Bar

### Accomplished
- **Block and Fortify on the HP bar** (the user's idea): after the red HP come a light blue segment for Fortify and a grey one for Block, each as wide as its share of max HP (`CombatantPanelUI.RefreshShields`), on the player and every enemy. They follow the order a hit is absorbed in, from the outside in: Block at the very end, then Fortify, then HP. When the bar has no room left (HP near full), the shields slide back over the HP's end, half transparent, so they always show. The chips still give the exact numbers.
- **The run start's cards are taller** (780) with Choose at their bottom, and the gold line and Reroll moved down: Gambit's Defense line was cut off.

### Tests
- **Play Mode:** the Murmillo at 10/20 with 3 Fortify and 4 Block (red, blue, grey in a row), the Arena Wolf at full HP with 5 Block (grey over the bar's end), the player at full HP with 6 Block; the run start with Gambit, Exploit and Rally.
- **The end of the turn on the HP bar** (the user's next idea): the HP that Poison and Bleed will take at the end of the owner's turn is marked inside the HP from its end, Poison in dark green (taken first, at the very end), Bleed in dark red before it. `Entity.NextTicks` computes it next to the `OnTurnEnd` rules it mirrors (Poison's stacks, then 1 for Bleed, never more than the HP left or the round's cap). Burn shows nothing: it deals no damage on its own. The marks are drawn over the shields, which can slide over a full bar's end, so a tick is never hidden. The first, light poison green vanished on the player's green bar.
- **Tested in Play Mode:** the player at 40/50 with 3 Poison and 1 Bleed, the Murmillo at 14/20 with 5 Poison and Bleed, the Arena Wolf at full HP with 3 Block and Bleed.

## Day 102 - Pictures for Items and Enchantments

### Accomplished
- **57 new pictures** from six Gemini sheets (prompts in `Docs/ArtPrompts.md`), cut and cleaned like the face emblems: 31 items in `Assets/Art/Items` (plus a spare whip kept outside the project), 14 bronze medallions in `Assets/Art/DieEnchantments`, 12 purple wax seals in `Assets/Art/Enchantments`.
- **`icon`** on `EquipmentSO`, `DieEnchantmentSO` and `EnchantmentSO`, assigned to every asset by name.
- **Where they show:** the Market's item, enchantment and die enchantment cards (the picture where a face card has its tile, `OfferCardUI.icon`), the boss reward cards (taller for it), the Market's item and die enchantment slots (picture left of the name, `EquipmentSlotUI.Show`), the battle's item strip (picture only, name and rules on hover, `EquipmentChipUI.icon`; trait chips keep their names) and the face tile: an enchanted face carries its enchantment's seal in the top right corner (`FaceTileUI.seal`), on the die net, in the dice tray and in the Market.
- **Item and enchantment card titles** use Cinzel like the face cards.

### Decisions
- **One look per kind** (object, medallion, seal), so a player never confuses an item with an enchantment, and the seal on a face says at once that it is enchanted.
- **The magenta mask was too loose for purple:** it took the seals' violet for background. Cutting now only removes colors close to pure magenta.

### Tests
- **Play Mode:** a Market with four items, Twin Soul and War Cry on the Top Sword (seal on its tile), both tabs; the battle's item strip with seven items in edit mode.

### Next Steps
- Sound and music, then a playtest build for friends.

## Day 103 - Sound and Music

### Accomplished
- **The game has sound** (GDD "Sound"). PixelLab's sound and music tools need a paid tier, so everything comes from public domain (CC0) packs: five Kenney packs for the effects and four OpenGameArt tracks for the music (`Docs/Credits.md`).
- **`AudioManager`** on the RunManager prefab, so it lives through scene loads: `AudioManager.Play(Sfx.X)` plays one of a sound's takes with a little pitch variation (on a pool of 10 sources; the same sound within 0.06 s is skipped, so a multi-hit isn't just louder), `PlayMusic(MusicTrack.X)` crossfades to a screen's track, `StopMusic` clears it for a jingle. Music and effect volumes are kept in PlayerPrefs (no settings screen yet). Every interactable button clicks: a mouse press is checked against what the pointer is over, so copied cards click without wiring.
- **`SoundLibrarySO`** (`Assets/Audio/SoundLibrary.asset`): every `Sfx` and `MusicTrack` with its clips, volume and pitch variation. Both enums are append only.
- **`CombatSounds`** watches each combatant like the floating numbers: a hit that costs HP (heavier for the player), Block or Fortify absorbing, Block gained, healing, death. Ticks are silent.
- **Hooks:** the roll animation (die thrown), the turn banners, an item firing, the battle result (jingle), each Market action (buy, upgrade, enchant, reroll, lock, move, heal, boss reward) and the run start (choose, reroll). Each screen starts its music: run start, Market, battle, boss.

### Decisions
- **Callers name what happened, the library decides how it sounds**, so a sound can be swapped in the asset without touching code, and the rules never know about sound.
- **The jingles were picked by measuring them**, as nothing here can listen: the start and end pitch of 51 candidates, a rising one for victory, a longer rising one for the end of the run and a falling one for defeat.
- **Music is streamed** (Vorbis 0.5), effects are decompressed on load.

### Tests
- Compiles; in Play Mode the AudioManager comes from the prefab, the run start's track is loaded and effects play on the pool. Not yet heard: the user's play-test will tell which sounds and volumes to change.

### Next Steps
- Play-test the sounds and tune volumes; a settings screen with music and effect sliders.
- A playtest build for friends.

## Day 104 - Settings Screen

### Accomplished
- **A settings screen** (GDD "Sound"): a bronze pixel gear in the top right corner of every screen, or Esc, opens a panel over a dimmed screen with Music and Effects sliders (percent shown; the effects slider plays a coin sound at most four times a second while it moves, so the volume can be heard) and Close. In a build it also has Quit Game, which a playtest build needs.
- **`SettingsUI`** on its own prefab, `Resources/SettingsOverlay` (an overlay canvas above the scenes' canvases), created by `RunManager.Bootstrap` next to the RunManager and kept through scene loads. The volumes are `AudioManager.MusicVolume` / `SfxVolume` (PlayerPrefs, saved when the panel closes); the music changes at once.
- **Room for the gear:** the Market's Heal, Next Battle and Cancel buttons and the battle's gold moved 90 px left.
- The gear is drawn in code (Python, 26 px, the bronze and gold of the buttons) at `Assets/Art/UI/Gear.png`, outside the icon atlas.

### Decisions
- **One persistent overlay instead of a copy per scene:** the settings work the same everywhere and a new scene gets them for free.

### Tests
- Play Mode in the Market: the gear in the corner, the panel with both sliders, the music slider changing the saved volume.

### Next Steps
- A playtest build for friends.

## Day 105 - Boss Reward Fix and Action Pips

### Accomplished
- **The boss reward could not be taken from its third card** (the user's Iron Bracers): the reward panel sat in the middle of the Market canvas's children, so four later ones (Heal, the next battle line and two labels) were drawn over it, and the next battle line lay right on the third card's Take button and swallowed the click. The panel is now the last child before the Tooltip.
- **The reward screen was redone:** a framed window like the settings, the title and hint above the cards instead of on them, taller cards with the picture on top, and the card text at 40 instead of 62 (it overflowed).
- **Action pips on every face tile** (the user's idea): a blue bubble top left for Defense, a red one bottom left for Attack, a purple one top right for Magic, from the face's own actions. An enchanted face shows its seal there instead. The die slots' "Can be enchanted" / "Has its own Magic" caption on the Enchanting Table is gone: an empty top right corner says it.
- **The Market's owned items are pictures only** (`EquipmentSlotUI.iconOnly`), ten to a row, the name and rules on hover: with many items the named slots ran out of room.

### Decisions
- **The Blacksmith goes to the Roadmap** (the user's idea): fitting an empty Attack or Defense action onto a face, the Enchanting Table's twin. The pips already make empty actions visible. It needs a rule that keeps it a choice (a rising price or one forge per face) and may become a map node.

### Tests
- Play Mode: a boss reward with three items, a raycast at the center of each Take button hits that button first, taking the third card adds it; 13 owned items in two rows of pictures; the pips on the die net and the face offers.

### Next Steps
- A playtest build for friends.

## Day 106 - The Difficulty Moves Forward

### Findings
- **The user's play-test:** buying random faces, clicking at random and only attacking still beat the first boss. "The curve doesn't rise evenly."
- **The simulation agreed (200 runs per bot):** battles 1–3 killed 0–3% of every bot, even the one that never shops; HP lost 4–11 per battle out of 50. The no-shopping die beat the Minotaur 45% of the time, the unplanned buyer 62%. Most deaths came later: battles 7, 8 (Medusa), 10 and 11.
- **The bots are a poor measure of total difficulty:** the user plays far better (Burn one-shots, the Hydra "a little tense"), so the late game isn't too hard for people even though bots rarely clear it. They still show the curve's shape.

### Experiments (unbuilt: no shopping + unplanned; built: every focused and hybrid bot)

| Change | Minotaur kills, unbuilt | Minotaur kills, built | Built clear rate |
|---|---|---|---|
| Current | 52% | 18% | 7.6% |
| Minotaur 28 HP | 61% | 31% | 8% |
| + battle 3 Rage 2 | 69% | 34% | 6% |
| Minotaur 22 HP, a Bellow face (gain 2 Rage) | 67% | 39% | 5% |
| Battle 2 ×2, battle 3 Rage 2, Minotaur 28, battle 7 no Rage | 66% | 33% | 8.0% |
| **Same with Minotaur 25 (chosen)** | **~70%** | **28%** | **8.0%** |

- **Every way of toughening the Minotaur hit built dice almost as hard as unbuilt ones:** three Markets in, builds barely exist. So difficulty was moved forward instead of added: harder battles 2–3 and Minotaur, a softer battle 7 (the bots' biggest killer).

### Accomplished
- **Battle 2: enemy HP ×2 (was ×1.7). Battle 3: 2 starting Rage (was 1). Minotaur: 25 HP (was 22). Battle 7: no starting Rage (was 1).**
- **HP lost per battle (built bots), before → after:** 5/6/7/17 → 5/7/12/19 over battles 1–4; battle 7: 16 → 12, and its deaths 20% → 8%.

### Next Steps
- **The user's play-test:** does the early game feel like a climb now, and is the first boss beaten only with some thought?
- A playtest build for friends.

## Day 107 - Pack Tactics and Tidier Pictures

### Findings (the user's play-test of the new curve)
- Battles 1 and 2 cost nothing, then two wolves at battle 3 (2 starting Rage) bit for 4 + 4 in the same turn and took almost half the player's HP at once. A forced Wolf Pack at battle 3 confirmed it: bots lost 23 HP and 19% died there.
- The Lampadarius opened with Ignite while the player had no Burn: a wasted turn that made it harmless.

### Accomplished
- **Pack tactics** (`EnemySO.packTactics`, on Tiro and Arena Wolf): a pack member picks something other than an attack when one earlier in the turn order already attacks this turn. A pair now trades hits and guards. Forced Wolf Pack at battle 3: 23 → 16 HP lost, 19% → 5% deaths.
- **Conditional intents** (`EnemyIntent.requiresTargetStatus`): an intent is only picked while the player has the status. The Lampadarius's Ignite waits for Burn.
- **The Lampadarius is a Burn threat:** Torch Swing applies 2 Burn (was 1), so Burn piles up and the player wants to end the fight before it is ignited.
- **Intent picking** (`EnemyBrain.Allowed`): the rules (no third repeat, the required status, pack tactics) filter the choices, and each is dropped again if nothing would be left.
- **The Market's die enchantments are pictures** (96 px squares like the items, name and rules on hover; an empty slot says Empty).
- **The action bubbles sit side by side in the top left corner** (red Attack, blue Defense, purple Magic, no gaps), instead of three corners that looked scattered.
- **Curve after the change (HP lost per battle 1–4, built bots):** 5 / 6 / 10 / 19; built clear rate 9.3%.

### Decisions
- **Face merging (the user's idea, a "Mad Scientist" on the map) is discussed, not built:** technically possible (the merged face kept as a recipe of its two parents, built into a definition at runtime, the emblem split along a diagonal), but it would be strictly stronger than its parents unless it costs something, and per-action randomness fights the build-crafting pillar. See the answer in this day's conversation; to be decided with the map.

### Next Steps
- **The user's play-test** of battles 1–4 with pack tactics and the Burn torchbearer.
- A playtest build for friends.

## Day 108 - Packs Tire, and the Mad Scientist's Rules

### Accomplished
- **Pack tactics break down in a long fight** (the user's idea, `EnemySO.packTacticsTurns`): Tiro and Arena Wolf keep them for 3 turns; from turn 4 a pack can attack at once again. Forced Wolf Pack at battle 3 (100 runs × 3 bots): 16.2 HP lost with tactics for the whole battle, 17.4 with 3 turns, 16.4 with 4. Most pack fights end before turn 4, so this only punishes dragging one out.
- **The Mad Scientist's rules are written into the Roadmap** (Phase B, next to the Blacksmith), as agreed with the user.

### Decisions (face merging, the user's design)
- **The price of a merge is a Blank face** in the freed place: no action, so landing on it costs a reroll or a pass, at least until the next Market. (My worry was that a merged face is strictly stronger than both parents; this makes concentrating power a real trade.)
- **The 50% chances stay:** the player knows the rules, so they can merge two faces that don't clash and get no randomness at all, or knowingly take the risk with a pair that does. If play-tests show it feels bad, the player gets the choice instead.
- **A keyword action** is one whose text names a status (Bleed, Burn, Poison, Ritual, Fortify, Rage...); damage and block don't count. My earlier "unclear" was wrong: the status words are a fixed list (`GameText`'s glossary).
- Left open: the child's tags and rarity, and merging two copies of one face.

### Next Steps
- **The user's play-test** of battles 1–4.
- A playtest build for friends.

## Day 109 - The Hydra's Heads on Its Body

### Findings
- **The Hydra's heads stood on the ground beside its body** (the user: "they should be one with the body"). Not on purpose: every enemy, heads included, was laid out in one row on the ground line, which made sense with the grey placeholders. The body picture already has cut neck stumps, so the art was ready for heads on it.
- Serrated Blade's misleading preview (an old Next Step) was already fixed on Day 70 (`cbe079a`).

### Accomplished
- **Heads are mounted on the body** (`BossSO.headMounts`, `mountedHeadScale` 0.62): three mounts on the stump cluster, the front one leaning toward the player (30°), one in the middle (8°), one rising from the shoulder (−10°). Heads are drawn behind the body, so the stumps hide where the necks join, and they shrink with the body if the row ever does.
- **Mounted heads are left out of the row** (`ArrangeEnemySprites`), so the body stands alone and bigger, and `PlaceMountedHeads` puts them on it.
- **A head never jumps to another neck** (`AssignMounts`): living heads keep their mount, a cut head lets its neck go and fades where it was (its stump shows again), and a new or regrown head takes a free mount. Tested: cut, regrow, a fourth head while one is dead, and a dead head growing back while the fourth lives.
- **The heads' cards:** each spans its head's picture (clicking a head still targets it), and all the heads' HP bars stand in one row over them with their intents above, below the boss's die (`EnemyCardUI.Place(..., plateOnTop)`, `UIManager.MountedHeadsTop`).

### Decisions
- **Tuned by eye in Play Mode:** at a larger size the heads' intents ran into the boss's die strip, so the heads are 0.62 of their own height and the plate row sits a little below their tilted tops.

### Next Steps
- A playtest build for friends.

## Day 110 - The Playtest Build

### Accomplished
- **How to Play:** a short rules page (the die, actions and their bubbles, reading intents, targeting, Reroll and Pass, statuses and Burn as fuel, the Market, the run's 11 battles and three bosses). It opens by itself the first time the game starts on a computer, and from the settings panel (gear or Esc) any time after. Esc closes it.
- **The Windows build** `0.1-playtest` (`Builds/Playtest/`, git-ignored): the three scenes, full screen. A headless smoke test (`-batchmode -nographics`) started it and loaded the run start with no errors in the log.
- **The zip for testers:** 46 MB, without the Burst debug folder (`_DoNotShip`).
- **`Docs/PlaytestInvite.md` updated:** the visuals, sound and rules page are in now; a sixth question asks whether the first battle made sense and the rules page was enough; notes for the user on sharing the zip and on what changed since the friend's first play-test (Day 85).

### Next Steps
- **The user plays the build once** (a real, rendered run start to finish) before sending it.
- Send it to two friends; their answers decide what comes before the map.

## Day 111 - Retiarius, the First New Archetype

### Findings
- **Content is the gap now** (the user's play while friends test the build), and the detailed balance waits for the map: the map changes how many fights, shops and heals a run has, so numbers tuned before it would be tuned again.
- **Archetype sizes:** Physical, Block and Fortune have 13 faces each, Rage 11, General 10, Bleed 8, Burn, Poison and Hammer 6, Ritual 4, Lifesteal 1. Burn, Poison and Ritual have one Common each, while 70% of Normal Market offers are Common, so those builds are hard to start.

### Accomplished
- **Retiarius** (`FaceTag.Net`, sea teal tiles): Net, Trident, Harpoon (Common), Tangle, Snare (Rare), Haul In (Epic), in the Market pool. The player nets enemies with Vulnerable and Weak, whose rules already worked on any entity.
- **Damage previews count Weak and Vulnerable** (`EffectContext.PreviewHit`), on the player's faces and the enemies' intents: a Weak enemy's 5-damage intent previews 4, a hit on a Vulnerable enemy previews one more. Tested on all six faces: every preview matched the damage dealt.
- New effects: `StatusStacksDamageEffectSO.alsoStatus` (Harpoon reads Vulnerable and Weak), `MultiHitPerStatusEffectSO` (Haul In). The simulator has a Net bot and a Net + Ritual bot; Tangle nets unless a big hit is coming.
- **Emblems** for the six faces (ArtPrompts Sheet 7, from Gemini): Gemini drew a 4 x 3 sheet with six extra objects, so only the first six cells were cut. Thin lines blended with the magenta turned purple when shrunk (the Net's mesh), so pixels with blue well above green are left out of each pixel's color.
- **The next archetypes are written into the GDD** (Future Archetypes): Provocator (Riposte, reading intents) and Bestiarius (a beast summoned by faces), with the user's decisions.

### Decisions
- **The beast is summoned by a face, not a hero** (the user asked how): any run can find it and mix it with its build; a Bestiarius hero can start with it later.
- **Lifesteal stays one face** (the user's worry that it would be too strong): healing every battle would undo HP carrying over.
- **Net got an Attack:** with Defense only, the Net bot replaced its Swords with Nets and a quarter of its rolls dealt nothing (0% cleared).
- **Net's Weak and Trident's Vulnerable follow the value** instead of a fixed 2: the Net bot went from 0% to 2% (Net + Ritual 3%, 150 runs), next to Ritual 1%, Hammer 2%, Poison 5%, Burn 7% today. Haul In keeps a fixed 2 (Epic fuel rule).
- Not tuned further: Medusa kills the Net bot most (61% of its arrivals; Bleed 32%), to revisit after the map.

### Next Steps
- The user plays Retiarius.
- Provocator, then Bestiarius; Commons for Burn, Poison and Ritual.
- The friends' feedback on the playtest build.

## Day 112 - Provocator

### Accomplished
- **Provocator** (`FaceTag.Riposte`, wine tiles): Parry, Bait, Grudge (Common), Vigil, Wrath (Rare), Perfect Counter, Provoke (Epic), in the Market pool. Three new statuses: Riposte, Vigil, Reversal (their chips show the word until their icons exist).
- **Striking back** happens in `Entity.TakeDamage`, per hit and without a source, so it never chains. Tested on a real `BattleFlow` against Murmillo: Riposte, Vigil waiting through a turn without a hit and spent by the first one, Perfect Counter's turn (3 Vigil + 4 Riposte + 2 Reversal = 9), Grudge counting the strikes, Wrath counting the hits, and Provoke turning Shield Wall into Strike with 1 Rage, then doing nothing while the enemy already attacks. (Riposte 4 once dealt 2: Murmillo's Curl Up had given it 2 block, which is right.)
- **Enemy intents can be read and changed by effects:** `IEnemyBrain.IntendsAttack` and `ProvokeToWeakestAttack` (normal enemies: their weakest attack by value × hits; bosses: their weakest attack face). The boss's die strip follows a changed face.
- **Emblems** for the seven faces (ArtPrompts Sheet 8, from Gemini). This sheet framed every cell with dark grid lines, so the cutter keeps only the magenta panel inside each frame.
- **Status icons** for Riposte, Vigil and Reversal (icon Sheet 3): added as a new top row of the icon atlas (768 x 640 now), so the old icons keep their glyph rects; `GameText.IconNames` and its status words know them.
- The simulator has a Provocator bot and a Provocator + Shield bot; Vigil is set once and Provoke is used on utility (Magic) intents.

### Decisions (with the user)
- **Taunt became Vigil:** the user saw that Taunt was close to a stun with no weakness. Vigil keeps Riposte waiting for the first hit instead, and the enemy keeps its plan.
- **Intent control lives on as the Epic Provoke** (the user's idea): weakest attack and 1 Rage (the user's numbers, lighter than my strongest attack and 2 Rage), only for a non-attack intent (my addition, or it cuts every big attack).
- Riposte is cleared every turn and Rage doesn't add to it.
- **Utility intents** go into the GDD as future enemy content (the user's idea): most non-attack intents are plain blocks today.

### Findings
- **Provoke is the weakest Epic for the bots:** free from battle 5 it did worse than a Sword in the same place. With Defense it was a block face on a die that lacks damage, so it got an Attack; its provoking is about even with never provoking, for bots that don't judge which utility is worth a permanent Rage. Left for the user's play, then cheaper costs or stronger utility intents.
- Provocator bots: 3% (with Shield 2%), Net 1%, Burn 11% in the same 150-run batch: the new archetypes sit with the old weak ones. Detailed tuning waits for the map.
- A Describe that returns "" makes the action preview show the effect's asset name ("ProvokeEffect"); Provoke now says the enemy already attacks.

### Next Steps
- The user plays Retiarius and Provocator.
- Bestiarius; Commons for Burn, Poison and Ritual.

## Day 113 - The Beastmaster

### Accomplished
- **Beastmaster** (`FaceTag.Hound`, leather brown tiles): Whistle, Command, Guard Dog (Common), Feed, Bloodhound (Rare), Sic 'Em (Epic), in the Market pool. A new status, Hound: the player's beast.
- **The bite** (`Beast.Bite`): the player's own hit for the Hound stacks, at the end of a turn the player didn't only defend in, and value times from Sic 'Em. Tested on a real `BattleFlow`: no bite without a beast, Whistle summons it and it bites for 2, Feed pays 3 HP for 3 Hound, Guard Dog and Command add the Hound, Bloodhound doubles on a bleeding enemy, and Sic 'Em with 11 Hound and 1 Rage bit 2 x 12, as its preview said.
- **The beast on screen** (`UI/BeastView`, the Battle scene's `Beast` object in front of the player): hidden until the first Hound, it fades in, and lunges on every bite. Its picture is the war dog the user made in Gemini (`Assets/Art/Beast/WarDog.png`, cut at a 12-pixel grid to 190 x 125, the Arena Wolf's density), 0.6 human heights tall.
- **Art from Gemini:** the war dog, the six emblems (Sheet 9) and the Hound paw icon, added to the icon atlas's top row next to the Provocator icons.
- Simulator: a Hound bot and a Hound + Rage bot; Whistle and Bloodhound grow the beast unless a big hit is coming, Feed only with HP to spare.

### Decisions (with the user)
- The beast is "Beastmaster" in the documents, because Bestiarius is an enemy; a bite is the player's own hit (Rage, items); the user makes the war dog in Gemini (a still, which lunges like the enemies).
- **"Twice my value" broke Value Is the Effect,** so Feed and Sic 'Em use their value (Feed 3, Sic 'Em bites value times).

### Findings (the beast was too strong)
- **A bite every turn:** the Hound bot cleared 26–29%, Strong battles ended in 8 turns (12–14 for others), and one Whistle on a die lifted it to 19%. The beast is a free extra hit every turn, and each Whistle grows it for the rest of the fight (the total grows with the square of the turns, like Poison before it halved).
- Tried: Whistle at 1 (26%), bites only on attack turns (2%: the build's own Magic faces silenced the beast), only on attack turns but every Hound Magic bites once (23%), no Rage on bites (19%), a Magic-only Whistle (17–19%).
- **Kept: no bite on a defend-only turn, Whistle at 1:** Hound 20%, Bleed 20%, Rage 12% in the same runs. A free Whistle still lifts the Opportunist from 2% to 5–8% (500 runs; Fury, Venom and Ember give 2–3%), mostly because it then shops for the whole Hound build. One of the strong archetypes; to revisit after the map.

### Next Steps
- The user plays the three new archetypes.
- Commons for Burn, Poison and Ritual.

## Day 114 - Commons for Burn, Poison and Ritual

### Accomplished
- **Six Commons, two per archetype** (agreed with the user on Day 111): Pitch Pot and Smolder (Burn), Blowgun and Smoke Pot (Poison), Offering and Hex (Ritual), in the Market pool. All made from existing effects. Tested with 2 Ritual on the player: Pitch Pot 5 Burn, Smolder ignited 5 and the Burn stayed, Blowgun 3 + 3 Poison, Smoke Pot 4 Poison on both enemies, Offering 4 damage, Hex 3 Burn and 3 Poison.
- Bots use Smoke Pot's and Hex's Magic unless a big hit is coming.

### Findings (200 runs per bot, before → after)
- Burn 3% → 8%, Poison 7% → 22%, Ritual 1% → 10%, Ritual + Poison 5% → 22%, Ritual + Bleed 14% → 10% (noise).
- **Blowgun first applied its value twice,** so each upgrade, cheap on a value-1 Common, added 2 Poison: Poison 27%. Without Blowgun 12%, without Smoke Pot 21%. Now it applies its value, then 1 more: Ritual still counts twice, upgrades count once. Poison stays about 22%, near its 28% from when halving came in, in the strong group with Bleed and Hound (20%).

### Next Steps
- The emblems (ArtPrompts Sheet 10).
- The user plays them.

