using UnityEditor;
using UnityEngine;

public static class DebugInventoryReset
{
    [MenuItem("Gear Mage/Reset Gear Inventory Data")]
    private static void ResetGearInventoryData()
    {
        new PlayerPrefsInventoryStore().Clear();
        Debug.Log("Cleared saved gear inventory data (purchased copies, consumables). Next run starts from the catalog's default-owned gears.");
    }

    [MenuItem("Gear Mage/Reset Gear Inventory Data", true)]
    private static bool ValidateResetGearInventoryData()
    {
        return !Application.isPlaying;
    }
}
