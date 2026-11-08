using UnityEngine;

// The gear board module [review candidate 3]: place a gear, remove a gear.
// BoardGrid (below) holds the ids; this class spawns board gears and keeps slots in step with it.
// Stat bonuses are not totalled here: they travel along the gear chain (GearRotate.CalculateChainBonus).
// The board lives as long as the scene, like RunData, so nothing is saved.
public class GearManager : MonoBehaviour
{
    public static GearManager instance { get; private set; }

    [Header("Board")]
    [Tooltip("UIElements/GearBoxAll. Spawned board gears are parented here so they move with the gearbox.")]
    [SerializeField] private Transform slotRoot;

    private readonly BoardGrid grid = new BoardGrid();

    private void Awake()
    {
        instance = this;
        if (slotRoot == null) Debug.LogError("GearManager: Slot Root is not set (expected UIElements/GearBoxAll).", this);
    }

    // Whether PlaceGear would succeed. Callers that spend gold or stock check this first,
    // so a failed placement never costs anything.
    public bool CanPlace(GearBox box, int id)
    {
        if (box == null || !grid.IsFree(box.GetXIndex(), box.GetYIndex())) return false;
        if (id <= 0 || GearCatalog.instance.GetPrefab(id) == null)
        {
            Debug.LogWarning("GearManager: no board prefab in the catalog for gear id " + id);
            return false;
        }
        return true;
    }

    // False, and nothing changes, if CanPlace is false.
    public bool PlaceGear(GearBox box, int id)
    {
        if (!CanPlace(box, id) || !grid.TryPlace(box.GetXIndex(), box.GetYIndex(), id)) return false;

        Spawn(GearCatalog.instance.GetPrefab(id), box, id);
        return true;
    }

    // Clears the cell and returns the id that was there. The caller owns the dragged GameObject.
    public int RemoveGear(GearBox box)
    {
        if (box == null) return 0;

        int id = grid.Remove(box.GetXIndex(), box.GetYIndex());
        SetSlotFull(box, false);
        return id;
    }

    private void Spawn(GameObject prefab, GearBox box, int id)
    {
        GameObject gear = Instantiate(prefab, box.transform.position, prefab.transform.rotation);
        gear.transform.SetParent(slotRoot, true);
        gear.GetComponent<GearBase>().AssignId(id);
        SetSlotFull(box, true);
    }

    private static void SetSlotFull(GearBox box, bool full)
    {
        if (box.TryGetComponent(out UIGearSlot slot)) slot.isFull = full;
    }
}

// The gear board's cells [review candidate 3]: which gear id sits where. Plain C#, so EditMode
// tests can construct it.
// Row = GearBox x index (0-3), column = GearBox y index (0-8).
public class BoardGrid
{
    public const int Rows = 4;
    public const int Columns = 9;
    public const int CellCount = Rows * Columns;

    private readonly int[] cells = new int[CellCount];

    private static bool IsInside(int row, int column)
    {
        return row >= 0 && row < Rows && column >= 0 && column < Columns;
    }

    // Inside the board and empty.
    public bool IsFree(int row, int column)
    {
        return IsInside(row, column) && cells[Index(row, column)] == 0;
    }

    private static int Index(int row, int column)
    {
        return row * Columns + column;
    }

    public int Get(int row, int column)
    {
        return IsInside(row, column) ? cells[Index(row, column)] : 0;
    }

    // False, and nothing changes, for an id of 0 or less, a cell outside the board, or a taken cell.
    public bool TryPlace(int row, int column, int id)
    {
        if (id <= 0 || !IsFree(row, column)) return false;

        cells[Index(row, column)] = id;
        return true;
    }

    // Empties the cell and returns the id that was there (0 if empty or outside the board).
    public int Remove(int row, int column)
    {
        if (!IsInside(row, column)) return 0;

        int index = Index(row, column);
        int id = cells[index];
        cells[index] = 0;
        return id;
    }
}
