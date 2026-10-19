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
