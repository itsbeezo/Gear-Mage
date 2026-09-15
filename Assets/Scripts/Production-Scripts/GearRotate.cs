using System.Collections.Generic;
using UnityEngine;

public class GearRotate : MonoBehaviour
{
    // Which player-unit spawn step this gear feeds when its tick completes.
    // None = a stat/booster gear that only rotates; no spawn-step output.

    public GearRotate instance;

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
    private float pulseCount = 0f;

    [Header("Unit Production")]
    [SerializeField]
    public GearProductionType productionType = GearProductionType.None;
    // Player spawn-step contribution added each time this gear's tick completes.
    // UnitManager.playerSpawnRate / playerTankSpawnRate were rebalanced around
    // the board's current gear count (1 Melee, 2 Tank) assuming this value - see UnitManager.cs.
    public float productionStepAmount = 0.4f;
    // Only meaningful when productionType == Booster. Multiplies a neighboring
    // producer's effective RotationsToComplete (see pulse()) once per pulse.
    // 1 = no effect, so non-booster gears are inert here by default.
    [SerializeField]
    public float boostMultiplier = 1f;

    private List<GearRotate> neighbors = new List<GearRotate>();

    // TEMP diagnostics - remove once the booster effect is confirmed working.
    // Select this gear in Play mode and watch these in the Inspector.
    [Header("DEBUG - booster diagnostics (temp)")]
    [SerializeField] private int debugNeighborCount;
    [SerializeField] private float debugChainBoostMultiplier;
    [SerializeField] private float debugEffectiveRotationsToComplete;

    private void Awake()
    {
        GetRotationSpeed();
    }

    private void Update()
    {
        // TEMP - keeps the DEBUG fields live at all times (including the lobby,
        // pre-Start), independent of PropagatePulse's Normal-state gate, so you can
        // select a gear and read values without starting a run and clicking through it.
        debugNeighborCount = neighbors.Count;
        debugChainBoostMultiplier = CalculateChainBoostMultiplier(new HashSet<GearRotate>());
        debugEffectiveRotationsToComplete = RotationsToComplete > 0
            ? Mathf.Max(0.0001f, RotationsToComplete * debugChainBoostMultiplier)
            : 0f;
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

    public void GetRotationSpeed()
    {
        // if (UnitManager.instance == null) return;
        // if (GameManager.instance == null || GameManager.instance.GetState() != GameManager.State.Normal) return;

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


    public void Pulse()
    {
        float chainBoostMultiplier = CalculateChainBoostMultiplier(new HashSet<GearRotate>());
        PropagatePulse(new HashSet<GearRotate>(), chainBoostMultiplier);
    }

    // Pure - no rotation, no tickProgress, no side effects. Walks the whole
    // connected component once (visited-set guards against cycles/revisits,
    // same pattern as PropagatePulse) and multiplies together every Booster's
    // boostMultiplier found anywhere in the chain. Callable independently of
    // clicking/pulsing later - e.g. by a placement-screen UI wanting to show a
    // gear's current live boost without needing a pulse event.
    private float CalculateChainBoostMultiplier(HashSet<GearRotate> visited)
    {
        if (visited.Contains(this)) return 1f;
        visited.Add(this);

        float multiplier = productionType == GearProductionType.Booster ? boostMultiplier : 1f;
        foreach (var neighbor in neighbors)
        {
            multiplier *= neighbor.CalculateChainBoostMultiplier(visited);
        }
        return multiplier;
    }

    private void PropagatePulse(HashSet<GearRotate> hasPulsed, float chainBoostMultiplier)
    {
        if (UnitManager.instance == null) return;
        if (GameManager.instance == null || GameManager.instance.GetState() != GameManager.State.Normal) return;

        if (hasPulsed.Contains(this))
        {
            // Already pulsed earlier in this same chain (reachable again via a
            // mutual neighbor link) - just stop here so we don't recurse forever.
            return;
        }
        hasPulsed.Add(this);

        transform.Rotate(0, 0, -rotationStep);

        pulseCount += 1;

        // Uses the chain-wide multiplier computed once in Pulse(), not a local
        // neighbor scan - every gear in the connected chain sees the same value,
        // so a booster anywhere in the chain affects every producer in it.
        // Skipped entirely when RotationsToComplete is 0 (None/Booster/Stats
        // gears), so a Booster's own 0 threshold never gets floored to 1 here.
        float effectiveRotationsToComplete = RotationsToComplete;
        if (RotationsToComplete > 0)
        {
            effectiveRotationsToComplete = Mathf.Max(0.0001f, RotationsToComplete * chainBoostMultiplier);
        }
        debugNeighborCount = neighbors.Count;
        debugChainBoostMultiplier = chainBoostMultiplier;
        debugEffectiveRotationsToComplete = effectiveRotationsToComplete;


        while (effectiveRotationsToComplete > 0 && pulseCount >= effectiveRotationsToComplete)
        {
            pulseCount -= effectiveRotationsToComplete;
            unitsSpawned += 1;
            ApplyProductionStep();
        }

        tickProgress = effectiveRotationsToComplete > 0 ? (float)pulseCount / effectiveRotationsToComplete : 0f;
        
        foreach (var neighbor in neighbors)
        {
            neighbor.PropagatePulse(hasPulsed, chainBoostMultiplier);
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
            case GearProductionType.Booster:
                // Intentionally a no-op: boosters never reach this method at all,
                // since RotationsToComplete stays 0 for them (no case in
                // GetRotationSpeed()), so pulseCount can never reach it. Their
                // actual effect is applied passively in pulse(), on each
                // neighboring producer gear's own effectiveRotationsToComplete.
                break;
        }
    }
}
