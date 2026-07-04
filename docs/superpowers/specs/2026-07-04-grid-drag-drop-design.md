# Grid Drag-and-Drop Design

## Overview

Players build their gear machine by dragging items (Gears, the Clicker, and later
Abilities) from an inventory panel onto a peg grid, and rearranging them within the
machine. This document specifies the slot/grid system that makes that placement
possible. It does not change the existing gear pulse/rotation logic
(`GearRotate`, `ClickerRotate`), which continues to operate on whatever is placed.

## Screen Layout (context)

Portrait mobile layout, three zones on one screen:
- **Machine** — large grid, where gears are placed to build the production chain
- **Inventory** — long, narrow strip below the Machine, holds unplaced items
- **Abilities** — smaller strip, separate zone, deferred for later work

Each zone has its own grid of pegs. Pegs are hard slots — items snap their center
exactly to a peg's center. Gears are sized so that adjacent gears' hitboxes touch,
so grid spacing is effectively fixed once gear size is fixed.

## Components

### `GridSlot`
One per peg. Responsibilities:
- Stores its own grid coordinate (row/column) within its zone
- Stores which zone it belongs to (Machine / Inventory / Abilities)
- Tracks occupancy: empty, or a reference to the occupying item
- Carries a trigger collider on a dedicated physics layer (`SlotDetection`),
  used only for hover/visual feedback — not for drop resolution (see Data Flow)
- Carries an "allowed types" flag, defaulted to "anything," as a seam for later
  zone-specific restrictions (e.g. if Abilities slots should only accept Ability
  items). Not enforced yet.

### `GridManager`
One per scene. Responsibilities:
- Discovers all `GridSlot`s in the scene at startup
- Single source of truth for occupancy — no other system queries or mutates
  slot state directly
- Resolves drops: given a screen/world drop position, finds the nearest slot
  within snap range
- Owns swap logic: if the target slot is occupied, the occupying item is moved
  to the dragged item's original slot
- Enforces one active drag/drop resolution at a time (rejects starting a new
  drag while another is still resolving)

### `Draggable`
Lives on any placeable item (Gear, Clicker, later Ability). Responsibilities:
- Handles touch/mouse drag lifecycle: pick up, follow input, release
- Remembers its originating slot when a drag begins
- On release, queries `GridManager` for the nearest valid slot to the drop
  point (position-based, not collision-based — a fast drag can skip past a
  slot's collider in a single frame, so collision-on-drop is not reliable)

## Data Flow

1. Player touches an item in a slot. `Draggable` begins the drag and records
   the originating slot. The item follows the input position.
2. The originating slot is not cleared in `GridManager` yet — nothing is
   committed until drop.
3. On release, `Draggable` asks `GridManager` for the nearest slot to the
   release point within snap range.
4. `GridManager` resolves:
   - **Target slot empty** → item moves there; original slot is cleared.
   - **Target slot occupied** → swap: the occupying item is moved to the
     dragged item's original slot; the dragged item takes the target slot.
   - **No slot within snap range** → item returns to its original slot.

## Decisions Made

- Snap-to-slot is hard (center-to-center), not free placement.
- Occupied-slot drop behavior is **swap**, not block and not displace-to-inventory.
- The Clicker is a player-placeable item on the grid, same as Gears. Its exact
  placement rules are still open (see below) but the grid system treats it as
  a generic placeable, so this is a small future change, not a redesign.
- Drop resolution is position-based (nearest slot to drop point), not
  collision-based, due to fast-drag reliability.
- Invalid drop (no slot in range) always falls back to the original slot —
  a single fallback rule covers every invalid-drop case.
- Slot hover/visual feedback uses a dedicated physics layer (`SlotDetection`)
  kept separate from the gear-to-gear pulse trigger colliders, to avoid the
  slot detection colliders interfering with existing pulse/neighbor logic.

## Open Questions / Seams for Later

- Clicker placement rules (can it move freely, is it restricted to certain
  slots, is it still fixed for now) — intentionally left open.
- Abilities zone behavior and any type restrictions on slots — deferred.
- Visual/animation treatment for swap (instant snap vs. a short move
  animation) — not decided, functionally either works with this design.

## Testing Approach

Manual/in-editor verification (no backend, no automated test suite for this
feature yet):
- Snap-to-slot works independently in each of the three zones
- Swap works in both directions: inventory → occupied machine slot, and
  machine slot → occupied machine slot
- Dropping outside any slot's range returns the item to its original slot
- Existing pulse behavior (Enter-based trigger) still works correctly when
  gears are placed at runtime via drag, not just when positioned at
  design-time in the scene
