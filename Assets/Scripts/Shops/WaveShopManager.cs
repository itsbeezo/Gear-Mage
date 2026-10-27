using System.Collections.Generic;
using UnityEngine;

public class WaveShopManager : MonoBehaviour
{
    public static WaveShopManager instance { get; private set; }

    [Header("Offer slots")]
    [Tooltip("Wave shop slots, shown between waves.")]
    [SerializeField] private List<SpawnSlot> spawnSlots;
    [Tooltip("Level shop slots, shown after the last wave.")]
    [SerializeField] private List<SpawnSlot> storeSlots;

    private readonly List<int> candidateIds = new List<int>();

    private void Awake()
    {
        instance = this;
    }

    // Wave shop "Re Roll" button (Inspector OnClick). GameManager also calls it on entering WaveShop [R4].
    public void RollWaveShop()
    {
        Roll(spawnSlots);
    }

    // Level shop "Spawn Gears" button (Inspector OnClick). GameManager also calls it on entering LevelShop.
    public void RollLevelShop()
    {
        Roll(storeSlots);
    }

    public int PickRandomID()
    {
        candidateIds.Clear();
        foreach (GearDefinition def in GearCatalog.instance.GetAll())
        {
            if (def.id != 0) candidateIds.Add(def.id);
        }

        if (candidateIds.Count == 0) return -1;
        return candidateIds[Random.Range(0, candidateIds.Count)];
    }

    // Creates a gear offer centred on the slot. The offer is parented to the slot,
    // so it hides and moves with the slot's shop group.
    public ShopOffer SpawnOffer(SpawnSlot slot, int gearId)
    {
        GameObject prefab = GearCatalog.instance.GetPrefab(gearId);
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

    private void Roll(List<SpawnSlot> slots)
    {
        foreach (SpawnSlot slot in slots)
        {
            if (slot.isOccupied)
            {
                if (slot.currentOffer != null)
                {
                    // Destroy(gameobject, 2f) --Makes the destroy wait 2 seconds, good for death animations)
                    Destroy(slot.currentOffer.gameObject);
                }
                slot.Clear();
            }

            int id = PickRandomID();
            if (id < 0) return;
            SpawnOffer(slot, id);
        }
    }
}
