using UnityEditor;
using UnityEngine;

public static class DebugInventoryTools
{
    private const int PurchasableGearId = 3;   // Archer (locked by default)
    private const int RunGrantGearId = 2;      // Melee
    private const int ConsumableGearId = 6;    // Booster2x

    [MenuItem("Gear Mage/Debug/Buy 1 Archer (id 3)")]
    private static void BuyPermanentCopy()
    {
        InventoryManager.instance.AddPermanentCopies(PurchasableGearId, 1);
    }

    [MenuItem("Gear Mage/Debug/Grant Run Gear: Melee (id 2)")]
    private static void GrantRunGear()
    {
        InventoryManager.instance.GrantRunGear(RunGrantGearId);
    }

    [MenuItem("Gear Mage/Debug/Grant Consumable: Booster2x (id 6)")]
    private static void GrantConsumable()
    {
        InventoryManager.instance.GrantConsumable(ConsumableGearId);
    }

    // One validator per menu item: [MenuItem] cannot be stacked on a single method.
    [MenuItem("Gear Mage/Debug/Buy 1 Archer (id 3)", true)]
    private static bool ValidateBuyPermanentCopy()
    {
        return InPlayModeWithInventory();
    }

    [MenuItem("Gear Mage/Debug/Grant Run Gear: Melee (id 2)", true)]
    private static bool ValidateGrantRunGear()
    {
        return InPlayModeWithInventory();
    }

    [MenuItem("Gear Mage/Debug/Grant Consumable: Booster2x (id 6)", true)]
    private static bool ValidateGrantConsumable()
    {
        return InPlayModeWithInventory();
    }

    private static bool InPlayModeWithInventory()
    {
        return Application.isPlaying && InventoryManager.instance != null;
    }
}
