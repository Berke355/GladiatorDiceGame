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
- There is no CLI build/test setup. Code is compiled and run from the Unity Editor (`Assets/Scenes/SampleScene.unity` is the only scene).
- The Editor is reachable through **MCP for Unity** (`com.coplaydev.unity-mcp`, pinned in `Packages/manifest.json`; set up via `Window → MCP for Unity`). Its tools only work while the Editor is open with the bridge running. Use them to read the Console after script changes and to inspect the scene; without them, ask the user to paste compile errors.
- When creating or moving files under `Assets/`, Unity generates `.meta` files on import. Never hand-write or delete `.meta` files without their asset; moving a script must move its `.meta` too, or scene/asset references break.

## Architecture (`Assets/Scripts`)

### Battle state machine
- `BattleManager` (MonoBehaviour on the scene) owns the current `BattleState`, the player and enemy `Entity`, the enemy's `EnemyBrain`, the six `currentDiceFaces` and the `currentRolledFace` shared between states.
- `States/` contain plain C# classes (not MonoBehaviours) deriving from `BattleState` (`Enter`/`Execute`/`Exit`), constructed with the manager: `new PlayerRollState(battleManager)`.
- Turn flow: `PlayerRollState` (player `OnTurnStart`, enemy picks and shows its intent, waits for roll) → `PlayerActionSelectState` (waits for an action, runs the rolled face's effects, player `OnTurnEnd`) → `EnemyTurnState` (enemy `OnTurnStart`, executes its intent, `OnTurnEnd`) → back to `PlayerRollState`. If the enemy dies the loop simply stops (no win/lose state yet).
- Input is event-driven: UI buttons call `BattleManager.OnRolled()` and `BattleManager.OnActionSelected(int)`, which forward only if the current state is the right type. The int is cast to `ActionType` (0 Attack, 1 Defense, 2 Magic), so button wiring depends on enum order.

### Data-driven effects (ScriptableObjects)
- `DieFaceSO` — one die face: `faceName`, `description`, `baseValue`, and a list of `ActionEffect { ActionType, EffectSO }`. A face can respond to several actions; choosing an action runs every effect tagged with it, all using the face's `baseValue`.
- `Effects/EffectSO` — abstract strategy: `Execute(Entity source, Entity target, int value)`. Every effect is source/target-agnostic so the same asset works for the player's die faces and for `EnemyIntent`s. New mechanics are added as a new `EffectSO` subclass with `[CreateAssetMenu(menuName = "Dice/Effects/...")]`, not by branching in the states.
- Effect assets and face assets are created in the Editor (`Assets/Faces/` holds the faces). `value` meaning is per effect (e.g. `MultiplyStatusEffectSO` treats it as a multiplier).

### Entities and statuses
- `Entity` holds HP, block and a `Dictionary<StatusType, int>` of statuses (`StatusType` enum is in `Entity.cs`). Damage hits block first. Every state change must raise `OnHealthChanged`, which is how `UIManager` refreshes.
- Block resets at the start of the owner's own turn (`OnTurnStart`), not the end.
- Status rules are hardcoded in `Entity`: Burn deals its stacks as damage at `OnTurnEnd` and decays by 2 unless Burn was added that turn; FireShield applies Burn to the attacker when hit (only if `TakeDamage` gets a `source`) and is cleared at turn start. Effects that should trigger FireShield must pass `source` to `TakeDamage`.
- Adding a status means touching `StatusType`, `Entity` turn hooks and the status text in `UIManager` (which lists statuses explicitly).

### UI
- `UIManager` is a scene singleton (`UIManager.Instance`); states null-check it before calling `UpdateDiceText` / `UpdateEnemyIntentText`. Entity references are assigned in the Inspector.

### Enemy
- `EnemyBrain` picks a random `EnemyIntent { intentName, intentType, EffectSO, value }` from its list at the start of the player's turn (Slay the Spire style telegraphing); `EnemyTurnState` executes it.
