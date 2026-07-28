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
    private float playerSpawnStep;
    [SerializeField] private float enemySpawnRate;
    private float enemySpawnStep;
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
        
    }
}
