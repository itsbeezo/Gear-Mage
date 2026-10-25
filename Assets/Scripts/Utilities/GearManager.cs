using System.Collections.Generic;
using UnityEngine;

public class GearManager : MonoBehaviour
{
    public static GearManager instance { get; private set; }

    private int[,] GearMatrix = { {0, 0, 0, 0, 0, 0, 0, 0, 0},
                                  {0, 0, 0, 0, 0, 0, 0, 0, 0},
                                  {0, 0, 0, 0, 0, 0, 0, 0, 0},
                                  {0, 0, 0, 0, 0, 0, 0, 0, 0} };

    [SerializeField] private List<GameObject> GearList;
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
    public List<GameObject> AllGears => GearList;

    private void Awake()
    {
        instance = this;
    }

    public GameObject GetGear(int i)
    {
        return GearList[i];
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

        gearInstance.transform.SetParent(ShopManager.instance.gearBox.transform, true);

        AddStats(gearNum);
    }

}