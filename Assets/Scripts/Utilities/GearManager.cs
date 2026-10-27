using System.Collections.Generic;
using UnityEngine;

public class GearManager : MonoBehaviour
{
    public static GearManager instance { get; private set; }

    private int[,] GearMatrix = { {0, 0, 0, 0, 0, 0, 0, 0, 0},
                                  {0, 0, 0, 0, 0, 0, 0, 0, 0},
                                  {0, 0, 0, 0, 0, 0, 0, 0, 0},
                                  {0, 0, 0, 0, 0, 0, 0, 0, 0} };

    [SerializeField] private List<GearBox> GearBoxList1;
    [SerializeField] private List<GearBox> GearBoxList2;
    [SerializeField] private List<GearBox> GearBoxList3;
    [SerializeField] private List<GearBox> GearBoxList4;
    private GearBase currentGear;
    private float HPMod;
    private float attackMod;
    private float spawnSpeedMod;
    private float moveSpeedMod;
    private float attackSpeedMod;

    private void Awake()
    {
        instance = this;
    }

    public GameObject GetGear(int i)
    {
        return GearCatalog.instance.GetPrefab(i);
    }

    public void SetGear(int i, int j, int g)
    {
        GearMatrix[i, j] = g;
    }

    private void AddStats(int gear)
    {
        currentGear = GetGear(gear).GetComponent<GearBase>();
        HPMod += currentGear.GetHPBonus();
        attackMod += currentGear.GetAttackBonus();
        spawnSpeedMod += currentGear.GetSpawnSpeedBonus();
        moveSpeedMod += currentGear.GetMoveSpeedBonus();
        attackSpeedMod += currentGear.GetAttackSpeedBonus();
    }

    public float GetHPMod()
    {
        return HPMod;
    }

    public float GetAttackMod()
    {
        return attackMod;
    }

    public float GetSpawnSpeedMod()
    {
        return spawnSpeedMod;
    }

    public float GetMoveSpeedMod()
    {
        return moveSpeedMod;
    }

    public float GetAttackSpeedMod()
    {
        return attackSpeedMod;
    }

    public void SpawnSingleGear(int i, int j, int gearNum)
    {
        if (gearNum <= 0) return;

        Vector3 spawnPos = Vector3.zero;

        switch (i)
        {
            case 0:
                spawnPos = GearBoxList1[j].transform.position;
                break;
            case 1:
                spawnPos = GearBoxList2[j].transform.position;
                break;
            case 2:
                spawnPos = GearBoxList3[j].transform.position;
                break;
            case 3:
                spawnPos = GearBoxList4[j].transform.position;
                break;
        }

        GameObject gearInstance = Instantiate(GetGear(gearNum), spawnPos, GetGear(gearNum).transform.rotation);
        gearInstance.GetComponent<GearBase>().AssignId(gearNum);

        gearInstance.transform.SetParent(ShopManager.instance.gearBox.transform, true);

        AddStats(gearNum);
    }

}

// The gear board's cells [review candidate 3]: which gear id sits where. Plain C#, so EditMode
// tests can construct it. Stat totals are always recomputed from the cells, so removing a gear lowers them.
// Row = GearBox x index (0-3), column = GearBox y index (0-8).
public class BoardGrid
{
    public const int Rows = 4;
    public const int Columns = 9;
    public const int CellCount = Rows * Columns;

    private readonly int[] cells = new int[CellCount];

    public static bool IsInside(int row, int column)
    {
        return row >= 0 && row < Rows && column >= 0 && column < Columns;
    }

    public static int Index(int row, int column)
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
        if (id <= 0 || !IsInside(row, column)) return false;

        int index = Index(row, column);
        if (cells[index] != 0) return false;

        cells[index] = id;
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

    public StatBlock Totals(IReadOnlyDictionary<int, StatBlock> bonusById)
    {
        StatBlock total = new StatBlock();
        foreach (int id in cells)
        {
            if (id != 0 && bonusById.TryGetValue(id, out StatBlock bonus))
            {
                total = total.Plus(bonus);
            }
        }
        return total;
    }
}