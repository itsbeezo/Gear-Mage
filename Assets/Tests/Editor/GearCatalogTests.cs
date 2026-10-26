using System.Collections.Generic;
using NUnit.Framework;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GearCatalogTests
{
    private GameObject go;
    private GearCatalog catalog;

    [SetUp]
    public void SetUp()
    {
        go = new GameObject("TestGearCatalog");
        catalog = go.AddComponent<GearCatalog>();
    }

    [TearDown]
    public void TearDown()
    {
        Object.DestroyImmediate(go);
    }

    [Test]
    public void GetDefinition_ReturnsMatchingEntry()
    {
        catalog.definitions = new List<GearDefinition>
        {
            new GearDefinition { id = 2, displayName = "Melee" },
            new GearDefinition { id = 4, displayName = "Tank" },
        };
        catalog.Initialize();

        var result = catalog.GetDefinition(2);

        Assert.IsNotNull(result);
        Assert.AreEqual("Melee", result.displayName);
    }

    [Test]
    public void GetDefinition_UnknownId_ReturnsNull()
    {
        catalog.definitions = new List<GearDefinition>
        {
            new GearDefinition { id = 2, displayName = "Melee" },
        };
        catalog.Initialize();

        var result = catalog.GetDefinition(999);

        Assert.IsNull(result);
    }

    [Test]
    public void GetAll_ReturnsAllDefinitions()
    {
        catalog.definitions = new List<GearDefinition>
        {
            new GearDefinition { id = 1 },
            new GearDefinition { id = 2 },
        };
        catalog.Initialize();

        Assert.AreEqual(2, catalog.GetAll().Count);
    }

    [Test]
    public void GetPrefab_ReturnsTheDefinitionsPrefab()
    {
        GameObject prefab = new GameObject("FakeGearPrefab");
        try
        {
            catalog.definitions = new List<GearDefinition>
            {
                new GearDefinition { id = 2, prefab = prefab, type = GearType.Unit },
            };
            catalog.Initialize();

            Assert.AreSame(prefab, catalog.GetPrefab(2));
        }
        finally
        {
            Object.DestroyImmediate(prefab);
        }
    }

    [Test]
    public void GetPrefab_UnknownId_ReturnsNull()
    {
        catalog.definitions = new List<GearDefinition>();
        catalog.Initialize();

        Assert.IsTrue(catalog.GetPrefab(999) == null);
    }
}

// Pins the real catalog in Main.unity: ids unique, every id has a board prefab of the right type,
// every tray shell's id exists. Opens Main.unity additively if it is not already open.
public class MainSceneCatalogTests
{
    private const string MainScenePath = "Assets/Scenes/Main.unity";
    private const string ShellFolder = "Assets/Prefabs/Gears/Shells";

    private Scene scene;
    private bool openedHere;
    private GearCatalog catalog;

    [OneTimeSetUp]
    public void OpenMainScene()
    {
        scene = SceneManager.GetSceneByPath(MainScenePath);
        openedHere = !scene.isLoaded;
        if (openedHere)
        {
            scene = EditorSceneManager.OpenScene(MainScenePath, OpenSceneMode.Additive);
        }

        foreach (GameObject root in scene.GetRootGameObjects())
        {
            catalog = root.GetComponentInChildren<GearCatalog>(true);
            if (catalog != null) break;
        }
        Assert.IsTrue(catalog != null, "No GearCatalog in " + MainScenePath);
        catalog.Initialize();
    }

    [OneTimeTearDown]
    public void CloseMainScene()
    {
        if (openedHere) EditorSceneManager.CloseScene(scene, true);
    }

    [Test]
    public void Catalog_IdsAreUniqueAndPositive_AndEachHasABoardPrefab()
    {
        HashSet<int> seen = new HashSet<int>();
        foreach (GearDefinition def in catalog.GetAll())
        {
            Assert.Greater(def.id, 0, def.displayName);
            Assert.IsTrue(seen.Add(def.id), "Duplicate id " + def.id);
            Assert.IsTrue(def.prefab != null, "No prefab for id " + def.id);
            Assert.IsTrue(def.prefab.GetComponent<GearBase>() != null, "Prefab for id " + def.id + " has no GearBase");
        }
    }

    [Test]
    public void Catalog_TypeMatchesThePrefabsProduction()
    {
        foreach (GearDefinition def in catalog.GetAll())
        {
            Assert.AreEqual(ExpectedType(def.prefab), def.type, "id " + def.id + " (" + def.displayName + ")");
        }
    }

    [Test]
    public void EveryTrayShell_HasAnIdInTheCatalog()
    {
        foreach (string guid in AssetDatabase.FindAssets("t:Prefab", new[] { ShellFolder }))
        {
            string path = AssetDatabase.GUIDToAssetPath(guid);
            GameObject shell = AssetDatabase.LoadAssetAtPath<GameObject>(path);
            InventoryGears inventory = shell.GetComponent<InventoryGears>();
            if (inventory == null) continue; // e.g. Badge.prefab

            Assert.IsNotNull(catalog.GetDefinition(inventory.gearId), path + " has gearId " + inventory.gearId + ", not in the catalog");
        }
    }

    private static GearType ExpectedType(GameObject prefab)
    {
        if (prefab.GetComponentInChildren<ClickerRotate>(true) != null) return GearType.Clicker;

        GearRotate rotate = prefab.GetComponentInChildren<GearRotate>(true);
        if (rotate == null) return GearType.StatBuff;

        switch (rotate.productionType)
        {
            case GearRotate.GearProductionType.Melee:
            case GearRotate.GearProductionType.Archer:
            case GearRotate.GearProductionType.Tank:
                return GearType.Unit;
            case GearRotate.GearProductionType.Booster:
                return GearType.Booster;
            default:
                return GearType.StatBuff;
        }
    }
}
