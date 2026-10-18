using System;
using System.Collections.Generic;

[Serializable]
public class GearCount
{
    public int id;
    public int count;
}

[Serializable]
public class PlayerInventoryData
{
    public List<GearCount> permanentExtras = new List<GearCount>();
    public List<GearCount> consumables = new List<GearCount>();

    public int GetPermanentExtra(int id)
    {
        return Get(permanentExtras, id);
    }

    // No cap check here - the caller (InventoryManager) clamps against
    // GearDefinition.maxCopies before calling this.
    public void AddPermanentExtra(int id, int delta)
    {
        Add(permanentExtras, id, delta);
    }

    public int GetConsumableCount(int id)
    {
        return Get(consumables, id);
    }

    public void AddConsumable(int id, int delta)
    {
        Add(consumables, id, delta);
    }

    private static int Get(List<GearCount> list, int id)
    {
        foreach (GearCount entry in list)
        {
            if (entry.id == id) return entry.count;
        }
        return 0;
    }

    private static void Add(List<GearCount> list, int id, int delta)
    {
        foreach (GearCount entry in list)
        {
            if (entry.id == id)
            {
                entry.count = Math.Max(0, entry.count + delta);
                return;
            }
        }
        list.Add(new GearCount { id = id, count = Math.Max(0, delta) });
    }
}
