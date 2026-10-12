# CLAUDE.md

This file provides guidance to Claude Code (claude.ai/code) when working with code in this repository.

## Project

**Gladiator Dice Game** — a turn-based gladiator roguelite where the player upgrades the faces of a six-sided die instead of leveling a character ("You don't build a hero. You build a die."). Design lives in `Docs/`:

- `Docs/GDD.md` — high concept, design pillars, core loop (Battle → Gold → Market → modify die → next battle), die face structure. Check new mechanics against the pillars (build crafting first, synergies over raw numbers, high risk/high reward).
- `Docs/DevLog.md` — day-by-day log of what was built and why. Each feature so far has been followed by a DevLog entry (`## Day N - Title` with Accomplished / Decisions / Next Steps); the latest "Next Steps" is the current to-do list. Some entries describe designs the code no longer matches (e.g. Day 6 mentions an `ActionEffect.multiplier` that does not exist) — the code is the source of truth.
- `Docs/Roadmap.md` — empty for now.

Code, comments, docs and commit messages are in English. Commits use Conventional Commit style (`feat: ...`).

## Environment

- Unity **6000.4.0f1** (Unity 6), URP 2D, new Input System, uGUI + TextMesh Pro.
- No assembly definitions: all scripts compile into `Assembly-CSharp`. No tests exist yet (Unity Test Framework is installed but unused).
- There is no CLI build/test setup. Code is compiled and run from the Unity Editor. Scenes: `Assets/Scenes/Battle.unity` and `Assets/Scenes/Market.unity` (both in Build Settings, loaded by name from `RunManager`).
- Testing in Play Mode through MCP: `PlayerSettings.runInBackground` is off, so when the Editor is not the focused window Play Mode stops advancing frames. Code invoked directly still runs, but `Start()` of a freshly loaded scene does not until the user focuses the Editor.
- The Editor is reachable through **MCP for Unity** (`com.coplaydev.unity-mcp`, pinned in `Packages/manifest.json`; set up via `Window → MCP for Unity`). Its tools only work while the Editor is open with the bridge running. Use them to read the Console after script changes and to inspect the scene; without them, ask the user to paste compile errors.
- When creating or moving files under `Assets/`, Unity generates `.meta` files on import. Never hand-write or delete `.meta` files without their asset; moving a script must move its `.meta` too, or scene/asset references break.

## Architecture (`Assets/Scripts`)

### Run (survives scene loads)
- `RunData` — plain `[Serializable]` C# class with everything that lives for a whole run: the player's die (`dieFaces`, a `List<DieFace>`), `maxHP`/`currentHP` (HP carries over between battles), `gold`, `battlesWon`. Keep it free of Unity objects other than asset references so it can be saved as JSON later.
- `RunManager` — singleton MonoBehaviour that owns `RunData`, calls `DontDestroyOnLoad` and handles scene flow (`GoToMarket`, `GoToNextBattle`, `RestartRun`). It is never placed in a scene: a `[RuntimeInitializeOnLoadMethod(BeforeSceneLoad)]` bootstrap instantiates `Assets/Resources/RunManager.prefab`, so every scene can be played directly. The starting die and starting HP are configured on that prefab, which also references the economy config.
- `EconomyConfigSO` (`Assets/Config/EconomyConfig.asset`) — every economy number (gold per victory, buy price per rarity, upgrade multiplier per rarity) and the `GetBuyPrice` / `GetUpgradeCost` formulas from the GDD's "Rarity & Economy" section.
- Dependency direction: the battle borrows the die and HP from the run in `BattleManager.Awake` and reports the result through `RunManager.OnBattleEnded`. The run decides what happens next; battle code knows nothing about the market.
- `RunManager.bossBattles` (currently 4 and 8) marks boss slots; `IsGrandMarket` is true right after winning one. Boss enemies and the final boss are not implemented yet.
- Market is split into model and view:
  - `Market` — plain C# rules, no UI: generates offers from `EconomyConfigSO.marketPool` (Normal: rarity weights 70/25/5, tag-match chance; Grand: Epic only; a face is offered at most once per visit, so the Grand Market shows fewer offers if the pool has fewer Epics), `Buy(offer, dieSlot)` replaces a die face with a new `DieFace`, `Upgrade(dieSlot)` raises an instance's value. Upgrade rule: Normal Market upgrades Common/Rare, Grand Market upgrades Epic. Test new rules on `Market` directly, without a scene.
  - `MarketUI` — draws the `Market` state and forwards clicks. `OfferCardUI` / `DieSlotUI` only hold references to the scene objects. Their buttons are wired in `MarketUI.Start` with `AddListener` because each needs its own index; Next Battle is wired in the Inspector.

### Battle state machine
- `BattleManager` (MonoBehaviour in the Battle scene) owns the current `BattleState`, the player and enemy `Entity`, the enemy's `EnemyBrain`, `currentDiceFaces` (a reference to the run's `List<DieFace>`, not serialized) and the `currentRolledFace` (`DieFace`) shared between states.
- `States/` contain plain C# classes (not MonoBehaviours) deriving from `BattleState` (`Enter`/`Execute`/`Exit`), constructed with the manager: `new PlayerRollState(battleManager)`.
- Turn flow: `PlayerRollState` (player `OnTurnStart`, enemy picks and shows its intent, waits for roll) → `PlayerActionSelectState` (waits for an action, runs the rolled face's effects, player `OnTurnEnd`) → `EnemyTurnState` (enemy `OnTurnStart`, executes its intent, `OnTurnEnd`) → back to `PlayerRollState`.
- `BattleManager.CheckBattleOver()` runs after every action and every `OnTurnEnd`; when a side is dead it switches to `BattleOverState`, and the caller must `return` immediately. `BattleOverState` reports to `RunManager`, shows the result panel, and on the result button goes to the Market (victory) or restarts the run (defeat).
- Input is event-driven: UI buttons call `BattleManager.OnRolled()`, `BattleManager.OnActionSelected(int)` and `BattleManager.OnResultButton()`, which forward only if the current state is the right type. The int is cast to `ActionType` (0 Attack, 1 Defense, 2 Magic), so button wiring depends on enum order.

### Data-driven effects (ScriptableObjects)
- `DieFaceSO` — the definition of a face: `faceName`, `description`, `rarity` (`Rarity`), `tags` (`FaceTag`), `baseValue` (starting value of a new copy), `maxValue` (upgrade cap), and a list of `ActionEffect { ActionType, EffectSO }`. A face can respond to several actions; choosing an action runs every effect tagged with it.
- `DieFace` — an instance on the die: `definition` + its own `value`. Effects use the instance's `value`, and upgrades change only the instance. Never modify a `DieFaceSO` at runtime: two copies on the die share one asset.
- Rarity follows value scaling (Common linear, Rare conditional/multi-effect, Epic multiplicative). Current `maxValue` rule: Common base+3, Rare base+2, Epic base+1. Design rule: the printed value is exactly what the effects use — no per-effect multipliers (removed on purpose, see DevLog Day 9).
- ScriptableObjects are for design data only. Never store mutable run state in an SO asset: changes made in Play Mode persist in the Editor but not in builds.
- `Effects/EffectSO` — abstract strategy: `Execute(EffectContext context)`. `EffectContext` carries `source`, `target`, `value`, `die` (the source's `List<DieFace>`) and `face` (the triggering `DieFace`). `die` and `face` are null for enemy intents, so effects that read them need a fallback. New information for effects goes into `EffectContext` instead of changing the signature. Every effect is source/target-agnostic so the same asset works for the player's die faces and for `EnemyIntent`s. Effects must not reach the die through globals such as `RunManager`.
- `ApplyStatusPerTaggedFaceEffectSO` implements die-composition scaling (value × faces on the die with a tag, e.g. Plague); without a die it counts as 1. New mechanics are added as a new `EffectSO` subclass with `[CreateAssetMenu(menuName = "Dice/Effects/...")]`, not by branching in the states.
- Effect assets and face assets are created in the Editor (`Assets/Faces/` holds the faces). `value` meaning is per effect (e.g. `MultiplyStatusEffectSO` treats it as a multiplier).

### Entities and statuses
- `Entity` holds HP, block and a `Dictionary<StatusType, int>` of statuses (`StatusType` enum is in `Entity.cs`). `TakeDamage` is absorbed by Block first, then Fortify (a status that never resets at turn start), then HP. Damage over time uses `LoseHP` and bypasses both. Every state change must raise `OnHealthChanged`, which is how `UIManager` refreshes.
- Block resets at the start of the owner's own turn (`OnTurnStart`), not the end.
- Status rules are hardcoded in `Entity`: Burn deals its stacks as damage at `OnTurnEnd`, bypassing block, and decays by 2 unless Burn was added since the last tick; Poison also ticks at `OnTurnEnd`, bypasses block and never decays; FireShield applies Burn to the attacker when hit (only if `TakeDamage` gets a `source`) and is cleared at turn start. Effects that should trigger FireShield must pass `source` to `TakeDamage`.
- Adding a status means touching `StatusType`, `Entity` turn hooks and `UIManager.BuildStatusText` (which lists statuses explicitly).
- Enums used in assets (`StatusType`, `FaceTag`, `Rarity`, `ActionType`) are serialized as numbers: only add new values at the end, never insert or reorder.

### UI
- `UIManager` is a Battle-scene singleton (`UIManager.Instance`, cleared in `OnDestroy`); states null-check it before calling `UpdateDiceText` / `UpdateEnemyIntentText` / `ShowBattleResult`. Entity and UI references are assigned in the Inspector.

### Enemy
- `EnemyBrain` picks a random `EnemyIntent { intentName, intentType, EffectSO, value }` from its list at the start of the player's turn (Slay the Spire style telegraphing); `EnemyTurnState` executes it.
