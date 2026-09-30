using NUnit.Framework;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEngine;

public class GearManager : MonoBehaviour
{
    public static GearManager instance { get; private set; }

    private static bool isFirstLoadInSequence = true;

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

    private void Start()
    {
        instance = this;

        // If it's the start of a new sequence, clear saved prefs, otherwise reload saved gears
        if (isFirstLoadInSequence)
        {
            isFirstLoadInSequence = false;
            ClearGearSave();
        }
        else
        {
            LoadGearMatrix();
        }

        DrawGears();
    }

    private GameObject GetGear(int i)
    {
        return GearList[i];
    }

    public void SetGear(int i, int j, int g)
    {
        GearMatrix[i, j] = g;
        SaveGearMatrix();
    }

    public void SaveGearMatrix()
    {
        for (int i = 0; i < GearMatrix.GetLength(0); i++)
        {
            for (int j = 0; j < GearMatrix.GetLength(1); j++)
            {
                PlayerPrefs.SetInt($"GearMatrix_{i}_{j}", GearMatrix[i, j]);
            }
        }
        PlayerPrefs.Save();
    }

    public void LoadGearMatrix()
    {
        for (int i = 0; i < GearMatrix.GetLength(0); i++)
        {
            for (int j = 0; j < GearMatrix.GetLength(1); j++)
            {
                GearMatrix[i, j] = PlayerPrefs.GetInt($"GearMatrix_{i}_{j}", 0);
            }
        }
    }

    public void ClearGearSave()
    {
        for (int i = 0; i < GearMatrix.GetLength(0); i++)
        {
            for (int j = 0; j < GearMatrix.GetLength(1); j++)
            {
                PlayerPrefs.DeleteKey($"GearMatrix_{i}_{j}");
            }
        }
        PlayerPrefs.Save();
    }

    // override object.Equals
    public override bool Equals(object obj)
    {
        if (obj == null || GetType() != obj.GetType())
        {
            return false;
        }

        throw new System.NotImplementedException();
    }

    // override object.GetHashCode
    public override int GetHashCode()
    {
        throw new System.NotImplementedException();
    }

    public void DrawGears()
    {
        for (int i = 0; i < GearMatrix.GetLength(0); i++)
        {
            for (int j = 0; j < GearMatrix.GetLength(1); j++)
            {
                int gearNum = GearMatrix[i, j];
                if (gearNum != 0)
                {
                    GearBox targetSlotBox = null;

                    switch (i)
                    {
                        case 0: targetSlotBox = GearBoxList1[j]; break;
                        case 1: targetSlotBox = GearBoxList2[j]; break;
                        case 2: targetSlotBox = GearBoxList3[j]; break;
                        case 3: targetSlotBox = GearBoxList4[j]; break;
                    }

                    if (targetSlotBox != null)
                    {
                        GameObject gearInstance = Instantiate(GetGear(gearNum), targetSlotBox.transform.position, GetGear(gearNum).transform.rotation);

                        if (ShopManager.instance != null && ShopManager.instance.gearBox != null)
                        {
                            gearInstance.transform.SetParent(ShopManager.instance.gearBox.transform, true);
                        }

                        UIGearSlot slotScript = targetSlotBox.GetComponent<UIGearSlot>();
                        if (slotScript != null)
                        {
                            slotScript.isFull = true;
                        }

                        //Recalculate stats
                        AddStats(gearNum);
                    }
                }
            }
        }
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