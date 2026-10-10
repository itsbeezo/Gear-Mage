using UnityEngine;

public class SpawnSlot : MonoBehaviour
{
    // Where a new offer is centred. Leave empty to use this object's own position.
    [SerializeField] private Transform spawnPoint;

    public bool isOccupied;
    public ShopOffer currentOffer;

    public Vector3 SpawnPosition => spawnPoint != null ? spawnPoint.position : transform.position;

    public void Occupy(ShopOffer offer)
    {
        isOccupied = true;
        currentOffer = offer;
    }

    public void Clear()
    {
        isOccupied = false;
        currentOffer = null;
    }
}
