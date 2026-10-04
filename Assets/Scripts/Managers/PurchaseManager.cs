using UnityEngine;

public class PurchaseManager : MonoBehaviour
{
    public static PurchaseManager instance { get; private set; }

    private void Awake()
    {
        instance = this;
    }

    // Spends gold for one gear. Called when a shop offer is dropped on the gearbox.
    // Returns false and spends nothing if the id is unknown, the shop isn't open,
    // or the player can't afford it. Placing the gear itself is handled by UIGearSlot.
    public bool TryBuyGear(int id)
    {
        if (!TryGetDefinition(id, out GearDefinition def)) return false;

        if (GameManager.instance == null || GameManager.instance.GetState() != GameManager.State.WaveVictory) return false;
        if (CurrencyManager.instance == null) return false;

        if (!CurrencyManager.instance.TrySpendGold(def.cost))
        {
            Debug.Log("Not enough gold for " + def.displayName + " (costs " + def.cost + ")");
            return false;
        }

        Debug.Log("Bought " + def.displayName + " for " + def.cost + " gold");
        return true;
    }

    public void SellGear(int id, int sellValue)
    {
        CurrencyManager.instance.RefundGold(sellValue);
        Debug.Log("Sold gear for " + sellValue + " Gold");
    }

    private bool TryGetDefinition(int id, out GearDefinition def)
    {
        def = GearCatalog.instance != null ? GearCatalog.instance.GetDefinition(id) : null;
        if (def == null)
        {
            Debug.Log("Id provided for gear is invalid " + id);
            return false;
        }
        return true;
    }
}
