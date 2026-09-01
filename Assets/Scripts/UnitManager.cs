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
    [SerializeField] private float playerArcherSpawnRate;
    private float playerSpawnStep;
    private float playerTankStep;
    private float playerArcherStep;
    [SerializeField] private float enemySpawnRate;
    [SerializeField] private float enemyTankSpawnRate;
    [SerializeField] private float enemyArcherSpawnRate;
    private float enemySpawnStep;
    private float enemyTankStep;
    private float enemyArcherStep;
    private bool unitsCanMove = true;
    private void Start()
    {
        instance = this;
    }
    private void FixedUpdate()
    {
        // playerSpawnStep/playerTankStep are no longer ticked here - they're driven
        // entirely by GearMelee/GearTank production gears via AddPlayerSpawnStep/
        // AddPlayerTankStep (see GearRotate.ApplyProductionStep). Enemy steps still
        // run on the fixed timer since there's no enemy-side gear production yet.
        if (GameManager.instance.GetState() == GameManager.State.Normal)
        {
            enemySpawnStep += 1;
            enemyTankStep += 1;
            playerArcherStep += 1;
            enemyArcherStep += 1;
        }

        // if(playerSpawnStep >= (playerSpawnRate - GearManager.instance.GetSpawnSpeedMod()))
        // {
        //     SpawnUnit(0, playerSpawnPoint.transform.position);
        //     playerSpawnStep = 0;
        // }

        if(enemySpawnStep >= enemySpawnRate)
        {
            Instantiate(UnitList[1], enemySpawnPoint.transform.position, Quaternion.identity);
            enemySpawnStep = 0;
        }

        // if(playerTankStep >= (playerTankSpawnRate - GearManager.instance.GetSpawnSpeedMod()))
        // {
        //     Instantiate(UnitList[2], playerSpawnPoint.transform.position, Quaternion.identity);
        //     playerTankStep = 0;
        // }

        if(enemyTankStep >= enemyTankSpawnRate)
        {
            Instantiate(UnitList[3], enemySpawnPoint.transform.position,Quaternion.identity);
            enemyTankStep = 0;
        }

        if(playerArcherStep >= (playerArcherSpawnRate - GearManager.instance.GetSpawnSpeedMod()))
        {
            Instantiate(UnitList[4], playerSpawnPoint.transform.position , Quaternion.identity);
            playerArcherStep = 0;
        }

        if(enemyArcherStep >= enemyArcherSpawnRate)
        {
            Instantiate(UnitList[5], enemySpawnPoint.transform.position, Quaternion.identity);
            enemyArcherStep = 0;
        }
    }
    public void SpawnUnit(int unitIndex,  Vector3 spawnPosition)
    {
        Instantiate(UnitList[unitIndex], spawnPosition, Quaternion.identity);
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