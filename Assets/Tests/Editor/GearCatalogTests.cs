using System.Collections.Generic;
using NUnit.Framework;
using UnityEngine;

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
