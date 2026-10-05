using System.Collections.Generic;
using UnityEngine;

public class WaveShopManager : MonoBehaviour
{
    [SerializeField] private List<GearSpawnSlot> spawnSlots;

    // Parameterless so it can be wired to a button's OnClick in the Inspector.
    // Spawns the test gear (id 4, Tank) into the first free slot.
    public void SpawnTestOffer()
    {
        
        GearSpawnSlot slot = FindFreeSlot();
        if (slot == null)
        {
            Debug.Log("No free wave shop slot for a test offer.");
            return;
        }

        SpawnOffer(slot, PickRandomID());
        
    }

    public int PickRandomID()
    {
        List<int> ids = new List<int>();
        foreach (GearDefinition def in GearCatalog.instance.GetAll())
        {
            if (def.id != 0) ids.Add(def.id);
        }

        if (ids.Count == 0) return -1;
        return ids[Random.Range(0, ids.Count)];

    }

    // Creates a gear offer centred on the slot. The offer is parented to the slot,
    // so it hides and moves with the slot's shop group.
    public ShopOffer SpawnOffer(GearSpawnSlot slot, int gearId)
    {
        GameObject prefab = GearManager.instance.GetGear(gearId);
        GameObject gear = Instantiate(prefab, slot.SpawnPosition, prefab.transform.rotation);
        // worldPositionStays keeps the gear's on-screen size even if the slot is scaled.
        gear.transform.SetParent(slot.transform, true);

        ShopOffer offer = gear.GetComponent<ShopOffer>();
        if (offer == null)
        {
            offer = gear.AddComponent<ShopOffer>();
        }

        offer.Setup(gearId, slot);
        slot.Occupy(offer);
        return offer;
    }

    private GearSpawnSlot FindFreeSlot()
    {
        foreach (GearSpawnSlot slot in spawnSlots)
        {
            if (!slot.isOccupied) return slot;
        }
        return null;
    }
}
