using NUnit.Framework;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEngine;

public class GearManager : MonoBehaviour
{


    // Gears list them later


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

    private void Start()
    {
        instance = this;
        DrawGears();
    }
    private GameObject GetGear(int i)
    {
        return GearList[i];
    }
    public void SetGear(int i, int j, int g)
    {
        GearMatrix[i, j] = g;
    }// override object.Equals
    public override bool Equals(object obj)
    {
        //
        // See the full list of guidelines at
        //   http://go.microsoft.com/fwlink/?LinkID=85237
        // and also the guidance for operator== at
        //   http://go.microsoft.com/fwlink/?LinkId=85238
        //
        
        if (obj == null || GetType() != obj.GetType())
        {
            return false;
        }
        
        // TODO: write your implementation of Equals() here
        throw new System.NotImplementedException();
        return base.Equals (obj);
    }
    
    // override object.GetHashCode
    public override int GetHashCode()
    {
        // TODO: write your implementation of GetHashCode() here
        throw new System.NotImplementedException();
        return base.GetHashCode();
    }
    public void DrawGears()
    {
        for(int i = 0; i < GearMatrix.GetLength(0); i++)
        {
            for(int j = 0; j < GearMatrix.GetLength(1); j++)
            {
                if (GearMatrix[i, j] != 0)
                    switch(i)
                    {
                        case 0:
                        {
                            Instantiate(GetGear(GearMatrix[i, j]), GearBoxList1[j].transform.position, GearBoxList1[j].transform.rotation);
                            break;
                        }
                        case 1:
                        {
                            Instantiate(GetGear(GearMatrix[i, j]), GearBoxList2[j].transform.position, GearBoxList2[j].transform.rotation);
                            break;
                        }
                        case 2:
                        {
                            Instantiate(GetGear(GearMatrix[i, j]), GearBoxList3[j].transform.position, GearBoxList3[j].transform.rotation);
                            break;
                        }
                        case 3:
                        {
                            Instantiate(GetGear(GearMatrix[i, j]), GearBoxList4[j].transform.position, GearBoxList4[j].transform.rotation);
                            break;
                        }
                    }
                if (GearMatrix[i, j] != 0)
                    AddStats(GearMatrix[i,j]);
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

        Instantiate(GetGear(gearNum), spawnPos, GetGear(gearNum).transform.rotation);
        AddStats(gearNum);
    }

}
