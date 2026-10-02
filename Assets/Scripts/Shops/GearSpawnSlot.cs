using UnityEngine;

public class GearSpawnSlot : MonoBehaviour
{

    public bool isOccupied;
    public ShopOffer currentPrice;

    public void Occupy(ShopOffer price)
    {
        isOccupied = true;
        currentPrice = price;
    }

    public void clear()
    {
        isOccupied = false;
        currentPrice = null;
    }

}
