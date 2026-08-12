using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class UnitManager : MonoBehaviour
{
    public static UnitManager instance { get; private set; }
    [SerializeField] List<UnitBase> UnitList;
    [SerializeField] private GameObject playerSpawnPoint;
    [SerializeField] private GameObject enemySpawnPoint;
    [SerializeField] private float playerSpawnRate;
    [SerializeField] private float playerTankSpawnRate;
    private float playerSpawnStep;
    private float playerTankStep;
    [SerializeField] private float enemySpawnRate;
    [SerializeField] private float enemyTankSpawnRate;
    private float enemySpawnStep;
    private float enemyTankStep;
    private bool unitsCanMove = true;
    private void Start()
    {
        instance = this;
    }
    private void FixedUpdate()
    {
        if (GameManager.instance.GetState() == GameManager.State.Normal)
        {
            playerSpawnStep += 1;
            enemySpawnStep += 1;
            playerTankStep += 1;
            enemyTankStep += 1;
        }

        if(playerSpawnStep >= (playerSpawnRate - GearManager.instance.GetSpawnSpeedMod()))
        {
            Instantiate(UnitList[0], playerSpawnPoint.transform.position, Quaternion.identity);
            playerSpawnStep = 0;
        }

        if(enemySpawnStep >= enemySpawnRate)
        {
            Instantiate(UnitList[1], enemySpawnPoint.transform.position, Quaternion.identity);
            enemySpawnStep = 0;
        }

        if(playerTankStep >= (playerTankSpawnRate - GearManager.instance.GetSpawnSpeedMod()))
        {
            Instantiate(UnitList[2], playerSpawnPoint.transform.position, Quaternion.identity);
            playerTankStep = 0;
        }

        if(enemyTankStep >= enemyTankSpawnRate)
        {
            Instantiate(UnitList[3], enemySpawnPoint.transform.position,Quaternion.identity);
            enemyTankStep = 0;
        }
    }
    public bool GetUnitsCanMove()
    {
        return unitsCanMove;
    }
    public void SetUnitsCanMove(bool newSet)
    {
        unitsCanMove = newSet;
    }
}
