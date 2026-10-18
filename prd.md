# PRD: Assets/Scripts folder reorganization

- **Date:** 2026-10-18
- **Branch:** `Architecture-fixes` (local only, no push)
- **Project:** `~/Documents/GitHub/Gear-Mage` (Unity 6000.4.9f1). Note: `~/Gear-Mage` is an older separate copy, not this project.
- **Status:** Done 2026-10-18. Moves committed (aa1f2ca..fae2de2, folder metas 778b364), static checks pass, no compile errors, EditMode 59/59, no new Missing Scripts, Play Mode smoke test passed. Not pushed.

## Baseline (step 1, recorded 2026-10-18, before any move)

- **EditMode tests:** 59 tests, all 59 passed, reported by the user from the Unity Test Runner. After the moves,
  expect 59/59 again.
- **Scenes / Console:** checked statically before moving (GUID snapshot): `Main.unity` and 10 `_Recovery/*.unity`
  scenes already had one `m_Script` GUID (`15e75e0019702776f98bf01f7e36d92a`) that resolves to no script, i.e. a
  pre-existing Missing Script. Live Editor.log had 0 `error CS` lines before the reimport.

## Result (2026-10-18)

- Commits: aa1f2ca Inventory, fd86612 Shops + LevelShop rename, 0caa7a6 Abilities, f799df5 Units, 6f3a714 Bases,
  d075ba5 Modifiers, fae2de2 Utilities, 778b364 Unity-generated folder metas. All 54 path changes are R100 renames.
- Static: all 46 pre-move GUIDs present once, same file names (allowing the 3 planned renames), same 296 referencing
  files; only new GUIDs are the 3 new folders; no orphan `.meta`/`.cs`; tree matches the target layout;
  `Utilities/` kept the old `Managers/` GUID `c1ad5b36…`.
- Compile: live Editor.log shows `Tundra build success` for the reimport and 0 `error CS` lines.
- User: EditMode 59/59 passed; only the pre-existing Missing Script in `Main.unity`; Play Mode smoke test works.

## Goal

Replace the catch-all `Managers/` folder and the loose files in the `Scripts/` root with folders grouped by
game concept, so a reader can find a class by what it does rather than by whether its name ends in "Manager".
No behavior changes.

## Why this is low-risk (verified facts)

- One asmdef, `Assets/Scripts/GearMage.Runtime.asmdef`, covers all of `Scripts/` and its subfolders. Moving files
  between subfolders does not change which assembly they compile into. Tests (`GearMage.Tests.Editor`) reference
  that assembly by name.
- Every script is in the global namespace, so there are no `using` lines or namespaces to update.
- No code loads a script by path (`Resources.Load`/`AssetDatabase` paths only point at prefabs under
  `Assets/Prefabs/`).
- Prefabs and scenes reference scripts by the GUID in the script's `.meta`. Moving the `.cs` **together with its
  `.meta`** (via `git mv`) keeps every prefab/scene reference intact. `.meta` GUIDs are never hand-edited.

## Decisions (from grilling)

| # | Decision |
|---|---|
| Q1 | Rename existing `inventory/` to `Inventory/` (two-step `git mv` so the case change sticks); move `InventoryManager.cs` in. |
| Q2 | New folder is named `Modifiers/` (not "Modifies"). |
| Q3 | `PlayerBase.cs` goes in `Bases/` with `BaseBase.cs` and `EnemyBase.cs` (it is the twin of `EnemyBase`, the player's home base). `ProjectileBase.cs` goes in `Units/`. |
| Q4 | `GearBase.cs` stays with gear logic in `Utilities/`, next to `GearManager` (its only reader; it sits on every gear prefab). It does **not** go in `Modifiers/`, which is only stubs today. `GearBox.cs` also goes to `Utilities/`. |
| Q5 | `TempButton.cs` (root), `Misc/`, and `Production-Scripts/` stay where they are. Existing `Shops/` files `ShopOffer.cs` and `SpawnSlot.cs` stay. |
| Q5b | Rename `Shops/LevelShopManger.cs` to `Shops/LevelShop.cs` so the file name matches the class `LevelShop` (Unity requires this for MonoBehaviours). No new logic. |
| Q6 | Tests: the editor is open (project lock), so no batch mode. Take a baseline with the user before moving anything, then compare after. See Verification. |
| Q7 | Use `git mv`, one commit per folder group, on `Architecture-fixes`, no push. |
| Q8 | This PRD lives at the repo root and is committed. No HTML architecture report this session; structural problems are listed below instead. |

## Target layout

```
Assets/Scripts/
  GearMage.Runtime.asmdef      (unchanged)
  TempButton.cs                (unchanged)
  Abilities/      AbilityManager.cs
  Bases/          BaseBase.cs, EnemyBase.cs, PlayerBase.cs
  Inventory/      InventoryManager.cs + existing: InventoryGears, InventorySave, PlayerInventoryData, PlayerPrefsInventorySave
  Misc/           (unchanged) LidOpen.cs, TriggerProbe.cs
  Modifiers/      ModifierManager.cs, Stats.cs
  Production-Scripts/ (unchanged) ClickerRotate.cs, GearRotate.cs
  Shops/          CurrencyManager.cs, PurchaseManager.cs, ShopManager.cs, WaveShopManager.cs
                  + existing: ShopOffer.cs, SpawnSlot.cs, LevelShop.cs (renamed)
  Units/          UnitManager.cs, ProjectileBase.cs + existing UnitArcher/Base/Enemy/Player/Soldier/Tank
  Utilities/      (renamed from Managers/) GameManager.cs, GearManager.cs, GearCatalog.cs, UIManager.cs,
                  GearBase.cs, GearBox.cs, UIDragHandler.cs, UIGearSlot.cs
```

## Move table

| File | From | To |
|---|---|---|
| InventoryManager.cs | Managers/ | Inventory/ |
| CurrencyManager.cs | Managers/ | Shops/ |
| PurchaseManager.cs | Managers/ | Shops/ |
| ShopManager.cs | Managers/ | Shops/ |
| WaveShopManager.cs | Managers/ | Shops/ |
| LevelShopManger.cs | Shops/ | Shops/LevelShop.cs (rename) |
| AbilityManager.cs | Managers/ | Abilities/ |
| UnitManager.cs | Managers/ | Units/ |
| ProjectileBase.cs | root | Units/ |
| BaseBase.cs | root | Bases/ |
| EnemyBase.cs | root | Bases/ |
| PlayerBase.cs | root | Bases/ |
| ModifierManager.cs | Managers/ | Modifiers/ |
| Stats.cs | root | Modifiers/ |
| GearBase.cs | root | Utilities/ |
| GearBox.cs | root | Utilities/ |
| UIDragHandler.cs | root | Utilities/ |
| UIGearSlot.cs | root | Utilities/ |
| GameManager.cs, GearManager.cs, GearCatalog.cs, UIManager.cs | Managers/ | Utilities/ (folder rename) |

Every move includes the file's `.meta`.

## Implementation steps

0. Commit this PRD.
1. **Baseline** (user, in the editor): Test Runner, EditMode, Run All; record pass/fail counts. Open the main
   scenes and note any existing "Missing Script" warnings. Note current Console errors.
2. Pre-move snapshot (me): record the GUID of every script `.meta` under `Scripts/`, and every prefab/scene/asset
   file that references each GUID.
3. Moves, one commit each, in this order:
   1. `inventory/` to `Inventory/`, + InventoryManager
   2. Shops (4 managers + LevelShop rename)
   3. Abilities
   4. Units (UnitManager, ProjectileBase)
   5. Bases
   6. Modifiers
   7. Root UI/gear files into `Managers/`, then `Managers/` to `Utilities/` (folder `.meta` renamed with it to keep its GUID)
4. New folders (`Abilities/`, `Bases/`, `Modifiers/`) get their folder `.meta` from Unity when the editor
   refreshes; commit those generated files. Do not hand-write GUIDs.
5. Verify (below), then report.

## Verification

- **Static (me):** every script GUID from step 2 still exists exactly once and points at the same file name; the
  set of prefab/scene files referencing each GUID is unchanged; no `.cs` without a `.meta` or orphaned `.meta`;
  `Assets/Scripts/Managers` no longer exists; the tree matches the target layout.
- **Compile (me):** after the editor reimports, `~/.config/unity3d/Editor.log` shows no new `error CS` lines.
- **Tests (user):** EditMode Run All matches the baseline (same counts, no new failures).
- **Scenes (user):** opening the main scenes shows no new "Missing (Mono Script)" components; a quick Play Mode
  run (click Clicker, gear pulses, units spawn, shop opens) still works. Gameplay is only confirmed by Play Mode,
  not by reading code.
- Run the `verifying-before-completion` skill before claiming any of this is done.

## Out of scope / follow-ups (recorded, not done)

- **Possible `Gears/` folder:** `GearBase`, `GearBox`, `GearManager`, `GearCatalog`, and maybe `GearRotate`/
  `ClickerRotate` are all gear logic; `Utilities/` is a loose fit for them. If modifiers become a real system,
  revisit whether `GearBase` (stat bonuses) belongs with it.
- **`LevelShop` is an empty stub**, attached to nothing. Decide what it does (vs. `ShopManager`/`WaveShopManager`)
  or delete it.
- **`ModifierManager` and `Stats` are empty stubs.** `Modifiers/` is a placeholder until they are designed.
- **`TempButton` is an empty class.**
- Structural problems noticed (no changes made):
  - `GameManager`, `PlayerBase`, and `EnemyBase` assign their singleton `instance` in `Start`, not `Awake`, so other
    scripts reading them in `Awake`/`Start` can see null. `PlayerBase`/`EnemyBase` use a public settable field
    rather than `{ get; private set; }`.
  - `UnitBase` calls `GetComponent<Rigidbody2D>()` repeatedly in its movement loop, and the "move toward base"
    velocity formula is duplicated four times; `ProjectileBase` and `BaseBase` also call `GetComponent` per use.
  - `UnitManager` calls `GetComponent<BaseBase>()` on the base singletons each reset.
  - `BaseBase` reaches into `GameManager`, `CurrencyManager`, and (commented out) `WaveShopManager` directly on
    death: base destruction, wave progression, and rewards are coupled in one method.
- **Pre-existing Missing Script** in `Main.unity` (and the `_Recovery` scenes): `m_Script` GUID
  `15e75e0019702776f98bf01f7e36d92a` points at no script. Find the GameObject and remove or re-link the component.
- **`Assets/_Recovery/`** holds ~20 auto-recovered scenes that reference most scripts; consider deleting them.
- **Editor.log location:** Unity Hub runs as a Flatpak, so the live log is
  `~/.var/app/com.unity.UnityHub/config/unity3d/Editor.log`; `~/.config/unity3d/Editor.log` is stale (last written
  2026-09-30). Add this to the `gear-mage-unity` skill's facts. `[Licensing::Client] Error: Code 404` lines are
  pre-existing noise in that log.
- The `gear-mage-unity` skill's project facts are stale: the project path is no longer a symlink, there are now 2
  asmdefs, and EditMode tests exist. Update the skill separately.
