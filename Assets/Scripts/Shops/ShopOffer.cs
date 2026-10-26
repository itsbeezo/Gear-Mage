using UnityEngine;

public class ShopOffer : MonoBehaviour
{
    // Shop gears draw on this sorting layer, above the slot frames on the same layer.
    public const string SortingLayerName = "WaveShop";

    // Added to each sprite's authored order so the gear always draws above the slot frames.
    // Keep the slot frames below this value.
    private const int OfferSortBoost = 20;

    public int gearID;
    public SpawnSlot homeSlot;


    // make this a wave slot var and make another method for level slot
    public void Setup(int id, SpawnSlot slot)
    {
        gearID = id;
        homeSlot = slot;

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
