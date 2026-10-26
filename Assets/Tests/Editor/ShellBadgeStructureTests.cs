using System.Linq;
using NUnit.Framework;
using UnityEditor;
using UnityEngine;

// Tray shell prefab structure: one stock badge each, and the gear id each one carries.
public class ShellBadgeStructureTests
{
    private static readonly string[] ShellPaths =
    {
        "Assets/Prefabs/Gears/Shells/ClickerShell.prefab",
        "Assets/Prefabs/Gears/Shells/MeleeShell.prefab",
        "Assets/Prefabs/Gears/Shells/ArcherShell.prefab",
        "Assets/Prefabs/Gears/Shells/TankShell.prefab",
        "Assets/Prefabs/Gears/Shells/Booster1x.prefab",
        "Assets/Prefabs/Gears/Shells/Booster2x.prefab",
        "Assets/Prefabs/Gears/Shells/Booster4x.prefab",
        "Assets/Prefabs/Gears/Shells/Booster8x.prefab",
        "Assets/Prefabs/Gears/Shells/HPShell.prefab",
        "Assets/Prefabs/Gears/Shells/AttackShell.prefab",
        "Assets/Prefabs/Gears/Shells/AttackSpeedShell.prefab",
    };

    // InventoryGears finds its badge with GetComponentInChildren, so each shell must
    // carry exactly one TextMeshPro in its own hierarchy (the nested Badge instance).
    [TestCaseSource(nameof(ShellPaths))]
    public void Shell_HasExactlyOneBadgeInItsOwnHierarchy(string prefabPath)
    {
        var prefab = AssetDatabase.LoadAssetAtPath<GameObject>(prefabPath);
        Assert.IsNotNull(prefab, $"Prefab not found at {prefabPath}");

        int badgeCount = prefab.GetComponentsInChildren<Component>(true)
            .Count(c => c != null && c.GetType().Name == "TextMeshPro");

        Assert.AreEqual(1, badgeCount, $"{prefabPath} should contain exactly one TextMeshPro badge");
    }

    // Pins each tray shell's gear id. Tags no longer carry meaning [G1].
    [TestCase("Assets/Prefabs/Gears/Shells/ClickerShell.prefab", 1)]
    [TestCase("Assets/Prefabs/Gears/Shells/MeleeShell.prefab", 2)]
    [TestCase("Assets/Prefabs/Gears/Shells/ArcherShell.prefab", 3)]
    [TestCase("Assets/Prefabs/Gears/Shells/TankShell.prefab", 4)]
    [TestCase("Assets/Prefabs/Gears/Shells/Booster1x.prefab", 5)]
    [TestCase("Assets/Prefabs/Gears/Shells/Booster2x.prefab", 6)]
    [TestCase("Assets/Prefabs/Gears/Shells/Booster4x.prefab", 7)]
    [TestCase("Assets/Prefabs/Gears/Shells/Booster8x.prefab", 8)]
    [TestCase("Assets/Prefabs/Gears/Shells/HPShell.prefab", 9)]
    [TestCase("Assets/Prefabs/Gears/Shells/AttackShell.prefab", 10)]
    [TestCase("Assets/Prefabs/Gears/Shells/AttackSpeedShell.prefab", 11)]
    public void ShellPrefab_HasExpectedGearId(string prefabPath, int expectedId)
    {
        GameObject prefab = AssetDatabase.LoadAssetAtPath<GameObject>(prefabPath);
        Assert.IsNotNull(prefab, "Prefab not found at " + prefabPath);

        InventoryGears inventory = prefab.GetComponent<InventoryGears>();
        Assert.IsNotNull(inventory, prefabPath + " has no InventoryGears");
        Assert.AreEqual(expectedId, inventory.gearId);
    }
}
