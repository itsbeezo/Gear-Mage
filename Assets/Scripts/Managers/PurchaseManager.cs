using UnityEngine;



public class PurchaseManager : MonoBehaviour
{

    public int cost;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    private bool TryGetDefinition(int id, out GearDefinition def)
    {
        def = GearCatalog.instance != null ? GearCatalog.instance.GetDefinition(id) : null;
        if (def == null)
        {
            Debug.Log("Id provided for gear is invalid " + id);
            return false;
        }
        name = def.displayName;
        return true;
    }

    public void PurchaseGearGold(int id)
    {
        id = 4;
        if (!TryGetDefinition(id, out GearDefinition def)) return;
        
        
        cost = 0;
        CurrencyManager.instance.SubtractGold(cost);
        Debug.Log("bought Gaer ID: " + id + " Display Name: " + name + " for 10 Gold");
        return;
    }

    public void SellGear(int id, int sellValue)
    {
        CurrencyManager.instance.RefundGold(sellValue);
        Debug.Log("Sold gear for " + sellValue + " Gold");
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
