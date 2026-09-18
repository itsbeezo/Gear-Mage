using System.Collections.Generic;
using NUnit.Framework;
using UnityEngine;

public class InventoryManagerTests
{
    // Deliberately far outside the real gear id range (1-11) so these tests
    // can never collide with real saved player data in PlayerPrefs.
    private const int TestId = 9001;

    private GameObject go;
    private InventoryManager manager;

    [SetUp]
    public void SetUp()
    {
        PlayerPrefs.DeleteKey("GearStock_" + TestId);
        PlayerPrefs.DeleteKey("GearStockMax_" + TestId);

        go = new GameObject("TestInventoryManager");
        manager = go.AddComponent<InventoryManager>();
        manager.startingEntries = new List<GearStockEntry>
        {
            new GearStockEntry { id = TestId, startingStock = 2, maxStock = 5 }
        };
        manager.Initialize();
    }

    [TearDown]
    public void TearDown()
    {
        PlayerPrefs.DeleteKey("GearStock_" + TestId);
        PlayerPrefs.DeleteKey("GearStockMax_" + TestId);
        Object.DestroyImmediate(go);
    }

    [Test]
    public void GetStock_ReturnsStartingStock()
    {
        Assert.AreEqual(2, manager.GetStock(TestId));
        Assert.AreEqual(5, manager.GetMaxStock(TestId));
    }

    [Test]
    public void TryConsume_DecrementsStock_ReturnsTrue()
    {
        bool result = manager.TryConsume(TestId);

        Assert.IsTrue(result);
        Assert.AreEqual(1, manager.GetStock(TestId));
    }

    [Test]
    public void TryConsume_AtZeroStock_ReturnsFalse_DoesNotGoNegative()
    {
        manager.TryConsume(TestId);
        manager.TryConsume(TestId);

        bool result = manager.TryConsume(TestId);

        Assert.IsFalse(result);
        Assert.AreEqual(0, manager.GetStock(TestId));
    }

    [Test]
    public void Refund_IncrementsStock()
    {
        manager.TryConsume(TestId);

        manager.Refund(TestId);

        Assert.AreEqual(2, manager.GetStock(TestId));
    }

    [Test]
    public void Refund_ClampsAtMax_DoesNotExceedMaxStock()
    {
        manager.Refund(TestId);
        manager.Refund(TestId);
        manager.Refund(TestId);
        manager.Refund(TestId);

        Assert.AreEqual(5, manager.GetStock(TestId));
    }

    [Test]
    public void IncreaseMaxStock_RaisesMax()
    {
        manager.IncreaseMaxStock(TestId, 3);

        Assert.AreEqual(8, manager.GetMaxStock(TestId));
    }

    [Test]
    public void TryConsume_UnknownId_ReturnsFalse()
    {
        bool result = manager.TryConsume(424242);

        Assert.IsFalse(result);
    }

    [Test]
    public void Stock_PersistsAcrossReinitialize()
    {
        manager.TryConsume(TestId);

        // Simulates a fresh load (new scene/session) reading the same saved data.
        var go2 = new GameObject("TestInventoryManager2");
        var manager2 = go2.AddComponent<InventoryManager>();
        manager2.startingEntries = new List<GearStockEntry>
        {
            new GearStockEntry { id = TestId, startingStock = 2, maxStock = 5 }
        };
        manager2.Initialize();

        Assert.AreEqual(1, manager2.GetStock(TestId));

        Object.DestroyImmediate(go2);
    }
}
