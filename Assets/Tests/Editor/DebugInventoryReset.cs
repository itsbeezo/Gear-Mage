using UnityEditor;
using UnityEngine;

public static class DebugInventoryReset
{
    private const string STOCK_KEY_PREFIX = "GearStock_";
    private const string MAX_KEY_PREFIX = "GearStockMax_";
    private const int LOWEST_GEAR_ID = 1;
    private const int HIGHEST_GEAR_ID = 11;

    [MenuItem("Gear Mage/Reset Gear Inventory Stock")]
    private static void ResetGearInventoryStock()
    {
        for (int id = LOWEST_GEAR_ID; id <= HIGHEST_GEAR_ID; id++)
        {
            PlayerPrefs.DeleteKey(STOCK_KEY_PREFIX + id);
            PlayerPrefs.DeleteKey(MAX_KEY_PREFIX + id);
        }

        PlayerPrefs.Save();
        Debug.Log("Cleared saved gear inventory stock (ids 1-11). Next Play session will start from GearCatalog's starting/max values again.");
    }
}
