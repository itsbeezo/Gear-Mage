# Gear Inventory System — Design Spec

Date: 2026-09-15
Status: Approved for implementation planning

## 1. Problem

The player currently has effectively infinite supply of every gear kind — nothing
tracks how many of a given gear type they own, so anything can be placed on the
board with no cost. `InventoryManager.cs` and `InventoryGears.cs` already exist
as empty stub scripts, and a scene object called `InventoryBox` exists as a
placeholder UI area, but neither has any real logic yet.

We're building a real inventory: each gear kind (including the Clicker) has a
capped stock count. Placing a gear from inventory onto the board consumes one
unit; picking an already-placed gear back up off the board refunds one unit.
Stock persists across sessions (same mechanism `CurrencyManager` already uses
for gold), independent of the run-scoped/persistent currency split that's
planned as separate future work.

## 2. Goals

- Per-gear-kind stock, capped individually (default 2 starting / 5 max; Clicker
  is special-cased to 1 starting / 2 max).
- Placing a gear from inventory decrements its stock by one; placement is
  rejected if stock is 0.
- Picking an already-placed gear off the board (not just relocating it to
  another slot) refunds one unit to its stock.
- Stock persists between sessions via `PlayerPrefs`, matching the pattern
  `CurrencyManager` already uses for gold.
- Each gear kind's UI icon shows a live "current/max" badge and disappears
  entirely when depleted, reappearing when restocked.
- Gear kind *identity* (id, display name, icon) is factored out into its own
  reusable list (`GearCatalog`), separate from *stock counts*
  (`InventoryManager`), so a future Shop system can read the same catalog
  without depending on the inventory system or duplicating the gear-kind list.

## 3. Non-goals (explicitly out of scope for this spec)

- The Shop UI itself (buying gears, spending currency to restock, random shop
  rolls). This spec only makes sure `InventoryManager` exposes the hooks
  (`IncreaseMaxStock`, and stock is mutable) a future Shop can call into.
- The gold → run-scoped-gold + persistent-bones currency split. Explicitly
  deferred to its own separate spec, per prior discussion. Inventory
  persistence is built independently of the currency code, using the same
  `PlayerPrefs` *pattern*, not a shared implementation.
- Any automated test framework — this project has none today; verification
  here is manual Play-mode testing.

## 4. Architecture

Three small components, each with one job:

```
GearCatalog (identity)        InventoryManager (stock)         InventoryGears (per-icon UI)
  id -> {name, icon}    <----   id -> {current, max}    <---->   polls stock, updates badge/visibility
                                  persists via PlayerPrefs
                                  TryConsume / Refund / IncreaseMaxStock
                                       ^                    ^
                                       |                    |
                              UIGearSlot.OnDrop      UIDragHandler.OnEndDrag
                              (consume on placement)  (refund on removal)
```

`GearCatalog` is pure reference data with no state that changes at runtime — a
future Shop reads it the same way `InventoryGears` does, without needing to
know anything about current stock counts. `InventoryManager` is the only piece
that owns mutable state (the counts) and the only piece that persists.

## 5. Components

### 5.1 `GearCatalog` (new script)

```csharp
[System.Serializable]
public class GearDefinition
{
    public int id;
    public string displayName;
    public Sprite icon;
}

public class GearCatalog : MonoBehaviour
{
    public static GearCatalog instance { get; private set; }
    [SerializeField] private List<GearDefinition> definitions;
    private Dictionary<int, GearDefinition> lookup;

    private void Awake()
    {
        instance = this;
        lookup = new Dictionary<int, GearDefinition>();
        foreach (var def in definitions) lookup[def.id] = def;
    }

    public GearDefinition GetDefinition(int id) =>
        lookup.TryGetValue(id, out var def) ? def : null;

    public IReadOnlyList<GearDefinition> GetAll() => definitions;
}
```

`id` reuses the numbering `UIDragHandler` already assigns via its tag/name
matching, so no new identity scheme is introduced. Based on that existing
code:

| id | Kind | id | Kind |
|----|------|----|------|
| 1 | Clicker | 7 | Melee |
| 2 | Archer | 9 | Tank |
| 3 | Booster 1x | 10 | GAttack |
| 4 | Booster 2x | 11 | AttackSpeed |
| 5 | Booster 8x | 12 | HP |
| 6 | Booster 4x | | |

(id 8 is unused/skipped in the existing scheme — not a problem, ids don't need
to be contiguous. These values should be double-checked against each Shell
prefab's actual tag at implementation time rather than assumed.)

`definitions` is configured by hand in the Inspector — one entry per row
above — matching how `GearManager.GearList`/`UnitManager.UnitList` are already
configured in this project.

### 5.2 `InventoryManager` (fills in the existing empty stub)

```csharp
[System.Serializable]
public class GearStockEntry
{
    public int id;
    public int startingStock;
    public int maxStock;
}

public class InventoryManager : MonoBehaviour
{
    public static InventoryManager instance { get; private set; }
    [SerializeField] private List<GearStockEntry> startingEntries;

    private class GearStock { public int current; public int max; }
    private Dictionary<int, GearStock> stock;

    private const string STOCK_KEY_PREFIX = "GearStock_";
    private const string MAX_KEY_PREFIX = "GearStockMax_";

    private void Awake()
    {
        instance = this;
        LoadOrSeedStock();
    }

    private void LoadOrSeedStock()
    {
        stock = new Dictionary<int, GearStock>();
        foreach (var entry in startingEntries)
        {
            int max = PlayerPrefs.GetInt(MAX_KEY_PREFIX + entry.id, entry.maxStock);
            int current = PlayerPrefs.GetInt(STOCK_KEY_PREFIX + entry.id, entry.startingStock);
            stock[entry.id] = new GearStock { current = current, max = max };
        }
    }

    public int GetStock(int id) => stock.TryGetValue(id, out var s) ? s.current : 0;
    public int GetMaxStock(int id) => stock.TryGetValue(id, out var s) ? s.max : 0;

    public bool TryConsume(int id)
    {
        if (!stock.TryGetValue(id, out var s) || s.current <= 0) return false;
        s.current -= 1;
        SaveStock(id);
        return true;
    }

    public void Refund(int id)
    {
        if (!stock.TryGetValue(id, out var s)) return;
        s.current = Mathf.Min(s.current + 1, s.max);
        SaveStock(id);
    }

    // Not called by anything yet - the hook point for a future Shop
    // "upgrade capacity" purchase.
    public void IncreaseMaxStock(int id, int amount)
    {
        if (!stock.TryGetValue(id, out var s)) return;
        s.max += amount;
        SaveStock(id);
    }

    private void SaveStock(int id)
    {
        var s = stock[id];
        PlayerPrefs.SetInt(STOCK_KEY_PREFIX + id, s.current);
        PlayerPrefs.SetInt(MAX_KEY_PREFIX + id, s.max);
        PlayerPrefs.Save();
    }

    private void OnApplicationQuit() => SaveAll();
    private void OnApplicationPause(bool paused) { if (paused) SaveAll(); }

    private void SaveAll()
    {
        foreach (var id in stock.Keys) SaveStock(id);
    }
}
```

This mirrors `CurrencyManager`'s save-on-every-change-plus-quit/pause pattern.
Unlike `CurrencyManager`, `InventoryManager` does **not** need
`DontDestroyOnLoad` — it reloads from `PlayerPrefs` on `Awake` each time a
scene loads, which is already correct and matches how `GameManager` itself is
scene-local and rebuilt on reload.

### 5.3 `InventoryGears` (fills in the existing empty stub)

Lives on each per-kind Shell icon (one per `GearCatalog` entry) sitting in
`InventoryBox`.

```csharp
public class InventoryGears : MonoBehaviour
{
    [SerializeField] private int gearId;
    [SerializeField] private TextMeshProUGUI stockBadge;

    private void Update()
    {
        if (InventoryManager.instance == null) return;

        int current = InventoryManager.instance.GetStock(gearId);
        int max = InventoryManager.instance.GetMaxStock(gearId);

        gameObject.SetActive(current > 0);
        if (current > 0)
            stockBadge.text = $"{current}/{max}";
    }
}
```

Polling rather than push/event-driven, consistent with this codebase's
established style (no C# events anywhere) and the same lightweight pattern
used for `GearRotate`'s live debug fields. At ~11 icons this is trivially
cheap per frame.

## 6. Hooking into the existing drag/drop flow

### 6.1 `UIDragHandler` — new field

```csharp
public bool startedOnBoard;
```

Public, matching the existing `gearNum`/`isConnected` fields on this same
class — `UIGearSlot.OnDrop` (a different class) needs to read it, so it can't
be `private`.

Set in `OnBeginDrag`, right where `currentSlot` is already resolved via
`Physics2D.OverlapPointAll`:

```csharp
startedOnBoard = (currentSlot != null);
```

`true` means the drag began on an already-placed board gear (a relocation);
`false` means it began on an inventory icon (a fresh placement).

### 6.2 `UIGearSlot.OnDrop` — consume on fresh placement, reject if empty

```csharp
public void OnDrop(PointerEventData eventData)
{
    if (eventData.pointerDrag == null || isFull) return;

    GameObject draggedObject = eventData.pointerDrag;
    UIDragHandler uiDragHandler = draggedObject.GetComponent<UIDragHandler>();

    if (!uiDragHandler.startedOnBoard)
    {
        if (InventoryManager.instance != null &&
            !InventoryManager.instance.TryConsume(uiDragHandler.gearNum))
        {
            return; // out of stock - reject the drop, isConnected stays false
        }
    }

    isFull = true;
    uiDragHandler.isConnected = true;

    GearBox thisGearBox = gameObject.GetComponent<GearBox>();
    GearManager.instance.SetGear(thisGearBox.GetXIndex(), thisGearBox.GetYIndex(), uiDragHandler.gearNum);
    GearManager.instance.SpawnSingleGear(thisGearBox.GetXIndex(), thisGearBox.GetYIndex(), uiDragHandler.gearNum);
}
```

Rejecting simply means returning without placing anything. Since
`isConnected` never gets set, `OnEndDrag`'s existing catch-all branch already
snaps the dragged object back to `originalPosition` — no new bounce-back
animation needs to be written.

### 6.3 `UIDragHandler.OnEndDrag` — refund on removal from the board

In the existing branch that already handles "board gear picked up and not
successfully re-placed anywhere":

```csharp
else if ((gameObject.CompareTag("Gears") || gameObject.CompareTag("Clicker")) && !isConnected)
{
    if (gearUnderneath)
    {
        StartCoroutine(ReSlot());
    }
    else
    {
        if (InventoryManager.instance != null)
            InventoryManager.instance.Refund(gearNum);

        currentSlot.ClearSlot();
        StartCoroutine(GearAnimation());
    }
}
```

This branch is reached only for objects tagged `Gears`/`Clicker` that started
already placed on the board, so no additional `startedOnBoard` check is
needed here.

## 7. Scene / prefab work

`InventoryBox` in the current scene is leftover scaffolding, not a working
setup — 9 of its 10 children are stray `GearBox` placeholder instances, and
only 1 (`TankShell`) is a real gear-kind icon. Implementation includes
rebuilding it: one Shell instance per `GearCatalog` entry, each carrying a new
`InventoryGears` component and a small TextMeshPro child for the stock badge.

Existing Shell prefabs to reuse: `ArcherShell`, `MeleeShell`, `TankShell`,
`Booster1x/2x/4x/8x`, `AttackShell`, `AttackSpeedShell`. Two are missing and
need to be created: `ClickerShell`, `HPShell`.

Component wiring (adding `InventoryGears`, setting `gearId`, adding a
`TextMeshProUGUI` child and referencing it) can be done via direct prefab
editing. Visual layout — badge sizing/positioning, arranging 11 icons within
`InventoryBox` — should be done in the Editor with visual feedback rather than
by hand-editing transform numbers blind; this will be called out explicitly
once the scripts exist.

## 8. Error handling / edge cases

- `TryConsume` on an unknown id (not in `startingEntries`) returns `false`
  safely rather than throwing — treated as "out of stock."
- `Refund` on an unknown id is a no-op.
- `InventoryManager.instance`/`GearCatalog.instance` null-checked at every
  call site, matching the existing codebase convention (`UnitManager.instance
  == null` guards used throughout `GearRotate.cs`).
- Stock is clamped to `[0, max]` at both `TryConsume` and `Refund` — refunding
  can never push stock above the current max, even if max was recently
  reduced (not currently possible, but safe by construction).

## 9. Testing (manual, Play mode)

- Place a gear from inventory → stock decrements, badge updates immediately.
- Deplete a kind to 0 → its icon disappears from `InventoryBox`.
- Attempt to drag a depleted kind's icon (if somehow still visible/mid-drag) →
  placement is rejected, icon snaps back, no gear spawned, no further
  decrement.
- Pick an already-placed gear back up off the board and drop it somewhere
  invalid → stock refunds by 1, badge updates, icon reappears if it had been
  hidden at 0.
- Relocate an already-placed gear to a different board slot → stock
  unchanged (no consume, no refund).
- Quit and relaunch (or reload the scene) → stock values persisted correctly
  via `PlayerPrefs`.
- Clicker specifically: starts at 1/2, behaves identically to other kinds
  through consume/refund.

## 10. Follow-up work (not in this spec)

- Shop UI reading `GearCatalog` to populate random/rotating stock for sale,
  and calling `InventoryManager.IncreaseMaxStock` on capacity purchases.
- The gold/bones currency split (separate spec).
