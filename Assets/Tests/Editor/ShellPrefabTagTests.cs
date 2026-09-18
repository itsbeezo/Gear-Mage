using NUnit.Framework;
using UnityEditor;
using UnityEngine;

public class ShellPrefabTagTests
{
    [TestCase("Assets/Prefabs/Gears/Shells/ClickerShell.prefab", "ClickerShell")]
    [TestCase("Assets/Prefabs/Gears/Shells/MeleeShell.prefab", "Melee")]
    [TestCase("Assets/Prefabs/Gears/Shells/ArcherShell.prefab", "Archer")]
    [TestCase("Assets/Prefabs/Gears/Shells/TankShell.prefab", "Tank")]
    [TestCase("Assets/Prefabs/Gears/Shells/Booster1x.prefab", "Booster1")]
    [TestCase("Assets/Prefabs/Gears/Shells/Booster2x.prefab", "Booster2")]
    [TestCase("Assets/Prefabs/Gears/Shells/Booster4x.prefab", "Booster4")]
    [TestCase("Assets/Prefabs/Gears/Shells/Booster8x.prefab", "Booster8")]
    [TestCase("Assets/Prefabs/Gears/Shells/HPShell.prefab", "Hp")]
    [TestCase("Assets/Prefabs/Gears/Shells/AttackShell.prefab", "Attack")]
    [TestCase("Assets/Prefabs/Gears/Shells/AttackSpeedShell.prefab", "AttackSpeed")]
    public void ShellPrefab_HasExpectedTag(string prefabPath, string expectedTag)
    {
        var prefab = AssetDatabase.LoadAssetAtPath<GameObject>(prefabPath);

        Assert.IsNotNull(prefab, $"Prefab not found at {prefabPath}");
        Assert.AreEqual(expectedTag, prefab.tag);
    }
}
