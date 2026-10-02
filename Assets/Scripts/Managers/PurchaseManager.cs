using UnityEngine;



public class PurchaseManager : MonoBehaviour
{

    public int cost;
    private string gearName;
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
        string gearName = def.displayName;
        return true;
    } 

    public void PurchaseGearGold(int id, Vector3 spawnposition)
    {
        if (!TryGetDefinition(id, out GearDefinition def)) return;
        GameObject gearPrefab = GearManager.instance.GetGear(id);
        GameObject spawned = Instantiate(gearPrefab, spawnposition, gearPrefab.transform.rotation);
        


        CurrencyManager.instance.SubtractGold(def.cost);
        Debug.Log("bought Gaer ID: " + id + " Display Name: " + gearName + " for " + def.cost);
        
        return;
    }

    public void SellGear(int id, int sellValue)
    {
        CurrencyManager.instance.RefundGold(sellValue);
        Debug.Log("Sold gear for " + sellValue + " Gold");
    }

    public void TestPurchase()
    {
        PurchaseGearGold(4, transform.position);
    }

}
