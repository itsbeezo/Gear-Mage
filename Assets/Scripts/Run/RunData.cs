using System.Collections.Generic;

// Everything one run owns [R17]. Plain C#, memory only [R8, R19]: RunManager creates it when
// Main.unity loads and drops it when the run ends and the scene reloads [R18].
// Phase 4 adds the board and in-run upgrades.
public class RunData
{
    public int world = 1;
    public int level = 1;
    public int wave = 1;
    public bool endless;
    public bool started;
    public int gold;
    public readonly List<GearCount> runGears = new List<GearCount>();

    public void Begin(int startWorld)
    {
        world = startWorld;
        level = 1;
        wave = 1;
        endless = false;
        started = true;
    }

    public void AddGold(int amount)
    {
        if (amount > 0) gold += amount;
    }

    public bool TrySpendGold(int amount)
    {
        if (amount < 0 || amount > gold) return false;
        gold -= amount;
        return true;
    }

    public int GetRunGearCount(int id)
    {
        foreach (GearCount entry in runGears)
        {
            if (entry.id == id) return entry.count;
        }
        return 0;
    }

    public void AddRunGear(int id, int count)
    {
        if (count <= 0) return;

        foreach (GearCount entry in runGears)
        {
            if (entry.id == id)
            {
                entry.count += count;
                return;
            }
        }
        runGears.Add(new GearCount { id = id, count = count });
    }
}
