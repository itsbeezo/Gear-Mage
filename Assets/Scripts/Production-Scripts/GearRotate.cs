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
        Archer,
        Tank,
        Booster,
        Stats
    }

    public GameObject Clicker;

    public GameObject Gear;

    [Header("Gear Settings")]
    [SerializeField]
    public float RotationSpeed = 50f;
    public float rotationStep = 90f;

    [Header("Production Settings")]
    [SerializeField]
    private int unitsSpawned;
    public int RotationsToComplete;
    public float tickProgress = 0f;
    // Counts actual pulse() calls (clicker/neighbor spins), not elapsed time -
    // replaces the old tickTimer, which was Time.deltaTime-based and got left
    // running unconditionally in Update() even after pulse-based fill was added.
    private int pulseCount = 0;

    [Header("Unit Production")]
    [SerializeField]
    public GearProductionType productionType = GearProductionType.None;
    // Player spawn-step contribution added each time this gear's tick completes.
    // UnitManager.playerSpawnRate / playerTankSpawnRate were rebalanced around
    // the board's current gear count (1 Melee, 2 Tank) assuming this value - see UnitManager.cs.
    public float productionStepAmount = 0.4f;

    private List<GearRotate> neighbors = new List<GearRotate>();

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

    // private void CreateCounter()
    // {
    //     switch (neighbors.Count <= 0)
    //     {
    //         case true:
    //             counter = 10;
    //             break;
    //         case false when neighbors.Count >= 1:
    //             foreach(var neighbor in neighbors)
    //             {
    //                 counter -= 1;
    //             }   
    //             break;
    //        case false when (counter <= 1):
    //             counter = 1;
    //             break;            
    //     }
    //}

    public void GetRotationSpeed()
    {
        if (UnitManager.instance == null) return;
        if (GameManager.instance == null || GameManager.instance.GetState() != GameManager.State.Normal) return;

        switch (productionType)
        {
            case GearProductionType.Melee:
                this.RotationsToComplete = 7;
                break;
            case GearProductionType.Archer:
                this.RotationsToComplete = 5;
                break;
            case GearProductionType.Tank:
                this.RotationsToComplete = 9;
                break;
        }
    }


    public void pulse(HashSet<GearRotate> hasPulsed)
    {
        if (UnitManager.instance == null) return;
        if (GameManager.instance == null || GameManager.instance.GetState() != GameManager.State.Normal) return;
        //Debug.Log(gameObject.name + " is pulsing. Neighbor count: " + neighbors.Count);
            if (hasPulsed.Contains(this))
            {
                // Already pulsed earlier in this same chain (reachable again via a
                // mutual neighbor link) - just stop here so we don't recurse forever.
                // Production logic used to live in this branch, which meant a gear
                // with no neighbors (never revisited) could never reach it at all.
                return;
            }
            hasPulsed.Add(this);

            transform.Rotate(0,0, -rotationStep);
            Debug.Log("Pulse method"); // TEMP - remove after diagnosing double-spin

            GetRotationSpeed();
            pulseCount += 1;
            tickProgress = RotationsToComplete > 0 ? (float)pulseCount / RotationsToComplete : 0f;

            if (RotationsToComplete > 0 && pulseCount >= RotationsToComplete)
            {
                pulseCount = 0;
                unitsSpawned += 1;
                ApplyProductionStep();
            }

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
                UnitManager.instance.SpawnUnit(0, UnitManager.instance.GetPlayerSpawnPoint().transform.position);
                break;
            case GearProductionType.Tank:
                UnitManager.instance.SpawnUnit(2, UnitManager.instance.GetPlayerSpawnPoint().transform.position);
                break;
            case GearProductionType.Archer:
                UnitManager.instance.SpawnUnit(4, UnitManager.instance.GetPlayerSpawnPoint().transform.position);
                break;
        }
    }
}
