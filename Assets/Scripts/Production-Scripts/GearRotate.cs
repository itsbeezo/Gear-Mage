using System.Collections.Generic;
using UnityEngine;

public class GearRotate : MonoBehaviour
{
    // Which player-unit spawn step this gear feeds when its tick completes.
    // None = a stat/booster gear that only rotates; no spawn-step output.
    public enum GearProductionType
    {
        None,
        Melee,
        Tank
    }

    public GameObject Clicker;

    public GameObject Gear;

    private float tickTimer = 0;

    private float gearspeed = 3f;


    [Header("Gear Settings")]
    [SerializeField]
    public float RotationSpeed = 50f;
    public float rotationStep = 90f;

    [Header("Production Settings")]
    [SerializeField]
    private int unitsSpawned;


    public float tickProgress = 0f;

    [Header("Unit Production")]
    [SerializeField]
    public GearProductionType productionType = GearProductionType.None;
    // Player spawn-step contribution added each time this gear's tick completes.
    // UnitManager.playerSpawnRate / playerTankSpawnRate were rebalanced around
    // the board's current gear count (1 Melee, 2 Tank) assuming this value - see UnitManager.cs.
    public float productionStepAmount = 0.4f;

    private List<GearRotate> neighbors = new List<GearRotate>();

    // Update is called once per frame
    void Update()
    {

        // if (GearRotation())
        // {
            // gearspeed = 2f;
            tickTimer += Time.deltaTime;

            tickProgress = Mathf.Clamp01(tickTimer / gearspeed);
        
            if (tickTimer >= gearspeed)
            {
            tickTimer = 0;
            unitsSpawned += 1;
            ApplyProductionStep();
            }
        // }
        //if (Input.GetKeyDown(KeyCode.E))
        //{
        //    Debug.Log("E was pressed");
        //    GearRotation();
        //}
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        GearRotate neighbor = collision.gameObject.GetComponent<GearRotate>();
        if (neighbor != null && !neighbors.Contains(neighbor))
        {
            neighbors.Add(neighbor);
            //Debug.Log(gameObject.name + " added neighbor " + neighbor.gameObject.name + " | neighbor count now: " + neighbors.Count);
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        GearRotate neighbor = collision.gameObject.GetComponent<GearRotate>();
        if (neighbor != null)
        {
            neighbors.Remove(neighbor);
        }
    }

    public void pulse(HashSet<GearRotate> hasPulsed)
    {
        //Debug.Log(gameObject.name + " is pulsing. Neighbor count: " + neighbors.Count); 
        if (hasPulsed.Contains(this)) return;
        hasPulsed.Add(this);

        transform.Rotate(0,0, -rotationStep);

            foreach (var neighbor in neighbors)
            {
                neighbor.pulse(hasPulsed);
            }
    }

    public void GearRotation()
    {
        Gear.transform.Rotate(0, 0, -90);
        //Gear.transform.Rotate(0, 0, -45);
    }

    private void ApplyProductionStep()
    {
        if (UnitManager.instance == null) return;
        // Match the same "only while playing" gating UnitManager used to apply to its
        // own step timer, so gears can't pre-fill spawn progress before Start / after EndGame.
        if (GameManager.instance == null || GameManager.instance.GetState() != GameManager.State.Normal) return;

        switch (productionType)
        {
            case GearProductionType.Melee:
                UnitManager.instance.AddPlayerSpawnStep(productionStepAmount);
                break;
            case GearProductionType.Tank:
                UnitManager.instance.AddPlayerTankStep(productionStepAmount);
                break;
        }
    }
}
