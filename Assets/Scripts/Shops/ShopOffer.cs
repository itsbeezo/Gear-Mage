using UnityEngine;

public class ShopOffer : MonoBehaviour
{
    // Shop gears draw on this sorting layer, above the slot frames on the same layer.
    public const string SortingLayerName = "WaveShop";

    // Added to each sprite's authored order so the gear always draws above the slot frames.
    // Keep the slot frames below this value.
    private const int OfferSortBoost = 20;

    public int gearID;
    public GearSpawnSlot homeSlot;

    public void Setup(int id, GearSpawnSlot slot)
    {
        gearID = id;
        homeSlot = slot;

        // UIDragHandler and UIGearSlot read the gear id from here.
        UIDragHandler drag = GetComponent<UIDragHandler>();
        if (drag != null) drag.gearNum = id;

        // An offer is display-only until bought: no rotation, production, or neighbour logic.
        GearRotate rotate = GetComponent<GearRotate>();
        if (rotate != null) rotate.enabled = false;

        foreach (SpriteRenderer sr in GetComponentsInChildren<SpriteRenderer>(true))
        {
            sr.sortingLayerName = SortingLayerName;
            sr.sortingOrder += OfferSortBoost;
        }
    }
}
