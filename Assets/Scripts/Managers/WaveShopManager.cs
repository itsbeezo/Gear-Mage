using System.Collections.Generic;
using UnityEngine;

public class WaveShopManager : MonoBehaviour
{
    public static WaveShopManager instance { get; private set; }

    [SerializeField] private List<SpawnSlot> spawnSlots;
    [SerializeField] private List<SpawnSlot> storeSlots;

    // Parameterless so it can be wired to a button's OnClick in the Inspector.
    // Spawns the test gear (id 4, Tank) into the first free slot.
    public void SpawnTestOffer()
    {
        ReRoll();
    }

    public void ReRoll()
    {
        SpawnSlot slot = FindFreeSlot();

        foreach (var s in spawnSlots)
        {
            if (!s.isOccupied) SpawnOffer(s, PickRandomID());
            else
            {
                // Destroy(gameobject, 2f) --Makes the destroy wait 2 seconds, good for death animations)
                Destroy(s.currentOffer.gameObject);
                s.Clear();
                SpawnOffer(s, PickRandomID());
            }
        }

        foreach (var t in storeSlots)
        {
            if (!t.isOccupied) SpawnOffer(t, PickRandomID());
            else
            {
                Destroy(t.currentOffer.gameObject);
                t.Clear();
                SpawnOffer(t, PickRandomID());
            }
        }
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
    public ShopOffer SpawnOffer(SpawnSlot slot, int gearId)
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

    private SpawnSlot FindFreeSlot()
    {
        foreach (SpawnSlot slot in spawnSlots)
        {
            if (!slot.isOccupied) return slot;
        }
        return null;
    }
}
