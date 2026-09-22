using System.Collections.Generic;
using UnityEngine;

public class InventoryManager : MonoBehaviour
{
    public static InventoryManager instance { get; private set; }

    private class GearState
    {
        public GearDefinition definition;
        public int runGranted;
        public int runUsed;
        public int permanentUsed;
        public int consumableUsed;
    }

    private readonly Dictionary<int, GearState> states = new Dictionary<int, GearState>();
    private PlayerInventoryData data = new PlayerInventoryData();
    private IInventoryStore store;
    private bool runActive;

    private void Awake()
    {
        instance = this;
    }

    // Start (not Awake) so GearCatalog.Awake has already run regardless of scene order.
    private void Start()
    {
        if (runActive) return;

        GearCatalog catalog = GearCatalog.instance;
        if (catalog == null)
        {
            Debug.LogError("InventoryManager: no GearCatalog in the scene, so no run can start.");
            return;
        }

        BeginRun(catalog.GetAll(), new PlayerPrefsInventoryStore());
    }

    // Separated from Start so tests can start a run without Unity's lifecycle.
    public void BeginRun(IReadOnlyList<GearDefinition> definitions, IInventoryStore inventoryStore)
    {
        store = inventoryStore;
        data = store.Load();

        states.Clear();
        foreach (GearDefinition definition in definitions)
        {
            if (states.ContainsKey(definition.id))
            {
                Debug.LogWarning("InventoryManager.BeginRun: duplicate gear id " + definition.id + " in the catalog - the last definition for this id wins.");
            }

            states[definition.id] = new GearState { definition = definition };
        }

        runActive = true;
    }

    public int GetStock(int id)
    {
        GearState state;
        if (!states.TryGetValue(id, out state)) return 0;

        int runAvailable = Mathf.Max(0, state.runGranted - state.runUsed);
        int permanentAvailable = Mathf.Max(0, PermanentOwned(state) - state.permanentUsed);

        return runAvailable + permanentAvailable + data.GetConsumableCount(id);
    }

    // Badge denominator: the gear's cap, not what's currently owned - see Design
    // Decisions "Badge denominator" (D5). Active run/consumable extras stack on top.
    public int GetMaxStock(int id)
    {
        GearState state;
        if (!states.TryGetValue(id, out state)) return 0;

        return state.definition.maxCopies
             + state.runGranted
             + data.GetConsumableCount(id)
             + state.consumableUsed;
    }

    // Spend order: run-only copies first, then permanent, then saved consumables.
    public bool TryConsume(int id)
    {
        GearState state;
        if (!states.TryGetValue(id, out state)) return false;

        if (state.runGranted - state.runUsed > 0)
        {
            state.runUsed++;
            return true;
        }

        if (PermanentOwned(state) - state.permanentUsed > 0)
        {
            state.permanentUsed++;
            return true;
        }

        if (data.GetConsumableCount(id) > 0)
        {
            data.AddConsumable(id, -1);
            state.consumableUsed++;
            store.Save(data);
            return true;
        }

        return false;
    }

    // Reverse of the spend order, so picking the SAME just-placed copy back up restores
    // the exact prior state. This is a global per-gear priority order, not tied to which
    // pool actually paid for the specific instance being picked up - if a gear has copies
    // from multiple pools active at once (e.g. both a run grant and a saved consumable),
    // picking up a DIFFERENT placed copy than the one just spent can refund the wrong
    // pool. Not reachable today: nothing grants run copies or consumables outside the
    // Editor debug menu (Assets/Tests/Editor/DebugInventoryTools.cs). Revisit if
    // GrantRunGear/GrantConsumable get real gameplay callers (drops, wave shop, etc.).
    public void Refund(int id)
    {
        GearState state;
        if (!states.TryGetValue(id, out state)) return;

        if (state.consumableUsed > 0)
        {
            state.consumableUsed--;
            data.AddConsumable(id, 1);
            store.Save(data);
            return;
        }

        if (state.permanentUsed > 0)
        {
            state.permanentUsed--;
            return;
        }

        if (state.runUsed > 0)
        {
            state.runUsed--;
        }
    }

    // The single purchase call: buying a first copy of a locked gear (0 owned) and
    // buying an Nth copy of an already-owned gear are the same operation. Clamps so
    // owned copies never exceed the catalog's cap; a purchase past the cap is a no-op.
    public void AddPermanentCopies(int id, int delta)
    {
        GearState state;
        if (!states.TryGetValue(id, out state))
        {
            Debug.LogWarning("InventoryManager.AddPermanentCopies: unknown gear id " + id);
            return;
        }

        int maxExtra = Mathf.Max(0, state.definition.maxCopies - state.definition.startingCopies);
        int currentExtra = data.GetPermanentExtra(id);
        int allowedDelta = Mathf.Clamp(currentExtra + delta, 0, maxExtra) - currentExtra;

        if (allowedDelta == 0) return;

        data.AddPermanentExtra(id, allowedDelta);
        store.Save(data);
    }

    // Memory only: gone when the run (scene) ends. count is expected positive - a
    // negative count would drive runGranted negative and could make GetStock/GetMaxStock
    // misreport, so it's rejected rather than silently corrupting state.
    public void GrantRunGear(int id, int count = 1)
    {
        if (count <= 0) return;

        GearState state;
        if (!states.TryGetValue(id, out state))
        {
            Debug.LogWarning("InventoryManager.GrantRunGear: unknown gear id " + id);
            return;
        }

        state.runGranted += count;
    }

    // Persisted until spent. Not capped - see Design Decisions D10. count is expected
    // positive, same reasoning as GrantRunGear (PlayerInventoryData.AddConsumable already
    // clamps at 0 on its own, but rejecting here keeps both grant methods symmetric).
    public void GrantConsumable(int id, int count = 1)
    {
        if (count <= 0) return;

        if (!states.ContainsKey(id))
        {
            Debug.LogWarning("InventoryManager.GrantConsumable: unknown gear id " + id);
            return;
        }

        data.AddConsumable(id, count);
        store.Save(data);
    }

    private int PermanentOwned(GearState state)
    {
        int owned = state.definition.startingCopies + data.GetPermanentExtra(state.definition.id);
        return Mathf.Min(owned, state.definition.maxCopies);
    }
}
