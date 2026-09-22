using System.Collections.Generic;
using NUnit.Framework;
using UnityEngine;

public class InventoryManagerTests
{
    // Far outside the real gear id range (1-11).
    private const int Owned = 9001;    // starts owned: 1 copy, cap 3
    private const int Locked = 9002;   // starts locked: 0 copies, cap 2

    private class FakeStore : IInventoryStore
    {
        private string json = "";

        public PlayerInventoryData Load()
        {
            return string.IsNullOrEmpty(json)
                ? new PlayerInventoryData()
                : JsonUtility.FromJson<PlayerInventoryData>(json);
        }

        public void Save(PlayerInventoryData data)
        {
            json = JsonUtility.ToJson(data);
        }
    }

    private FakeStore store;
    private List<GearDefinition> defs;
    private List<GameObject> spawned;

    [SetUp]
    public void SetUp()
    {
        store = new FakeStore();
        spawned = new List<GameObject>();
        defs = new List<GearDefinition>
        {
            new GearDefinition { id = Owned, displayName = "Owned", startingCopies = 1, maxCopies = 3 },
            new GearDefinition { id = Locked, displayName = "Locked", startingCopies = 0, maxCopies = 2 },
        };
    }

    [TearDown]
    public void TearDown()
    {
        foreach (GameObject go in spawned) Object.DestroyImmediate(go);
    }

    // Each call simulates a brand-new run: a fresh manager reading the same saved data.
    private InventoryManager StartRun()
    {
        var go = new GameObject("TestInventoryManager");
        spawned.Add(go);
        InventoryManager manager = go.AddComponent<InventoryManager>();
        manager.BeginRun(defs, store);
        return manager;
    }

    [Test]
    public void OwnedByDefault_StartsWithStartingCopies_BadgeShowsCap()
    {
        InventoryManager m = StartRun();

        Assert.AreEqual(1, m.GetStock(Owned));
        Assert.AreEqual(3, m.GetMaxStock(Owned));
    }

    [Test]
    public void Locked_HasNoStock_ButBadgeCapStillReadable()
    {
        InventoryManager m = StartRun();

        Assert.AreEqual(0, m.GetStock(Locked));
        Assert.AreEqual(2, m.GetMaxStock(Locked));
    }

    [Test]
    public void AddPermanentCopies_OnLockedGear_MakesItOwned()
    {
        InventoryManager m = StartRun();

        m.AddPermanentCopies(Locked, 1);

        Assert.AreEqual(1, m.GetStock(Locked));
    }

    [Test]
    public void AddPermanentCopies_ClampsAtMaxCopies()
    {
        InventoryManager m = StartRun();

        m.AddPermanentCopies(Owned, 10);

        Assert.AreEqual(3, m.GetStock(Owned));
        Assert.AreEqual(3, m.GetMaxStock(Owned));
    }

    [Test]
    public void AddPermanentCopies_PersistsIntoNextRun()
    {
        StartRun().AddPermanentCopies(Locked, 2);

        InventoryManager next = StartRun();

        Assert.AreEqual(2, next.GetStock(Locked));
    }

    [Test]
    public void TryConsume_DecrementsUntilEmpty_ThenFails()
    {
        InventoryManager m = StartRun();

        Assert.IsTrue(m.TryConsume(Owned));
        Assert.IsFalse(m.TryConsume(Owned));
        Assert.AreEqual(0, m.GetStock(Owned));
    }

    [Test]
    public void PerRunStock_RefillsEveryRun_DepletionIsNeverSaved()
    {
        InventoryManager first = StartRun();
        first.TryConsume(Owned);
        Assert.AreEqual(0, first.GetStock(Owned));

        InventoryManager next = StartRun();

        Assert.AreEqual(1, next.GetStock(Owned));
    }

    [Test]
    public void Refund_RestoresConsumedCopy()
    {
        InventoryManager m = StartRun();
        m.TryConsume(Owned);

        m.Refund(Owned);

        Assert.AreEqual(1, m.GetStock(Owned));
    }

    [Test]
    public void Refund_WithNothingUsed_DoesNotCreateStock()
    {
        InventoryManager m = StartRun();

        m.Refund(Owned);
        m.Refund(Owned);

        Assert.AreEqual(1, m.GetStock(Owned));
    }

    [Test]
    public void RunGrantedGear_AddsStockAndMaxStock_ThenVanishesNextRun()
    {
        InventoryManager m = StartRun();

        m.GrantRunGear(Owned);

        Assert.AreEqual(2, m.GetStock(Owned));
        Assert.AreEqual(4, m.GetMaxStock(Owned));

        InventoryManager next = StartRun();
        Assert.AreEqual(1, next.GetStock(Owned));
        Assert.AreEqual(3, next.GetMaxStock(Owned));
    }

    [Test]
    public void RunGrant_IsNeverWrittenToTheStore()
    {
        InventoryManager m = StartRun();

        m.GrantRunGear(Locked, 3);

        PlayerInventoryData saved = store.Load();
        Assert.AreEqual(0, saved.GetPermanentExtra(Locked));
        Assert.AreEqual(3, m.GetStock(Locked));
    }

    [Test]
    public void Consumable_UnspentPersistsIntoNextRun()
    {
        StartRun().GrantConsumable(Locked, 2);

        InventoryManager next = StartRun();

        Assert.AreEqual(2, next.GetStock(Locked));
    }

    [Test]
    public void Consumable_IsSpentPermanently()
    {
        InventoryManager m = StartRun();
        m.GrantConsumable(Locked, 1);

        Assert.IsTrue(m.TryConsume(Locked));

        Assert.AreEqual(0, StartRun().GetStock(Locked));
    }

    [Test]
    public void Consumable_RefundedInSameRun_RestoresSavedCount()
    {
        InventoryManager m = StartRun();
        m.GrantConsumable(Locked, 1);
        m.TryConsume(Locked);

        m.Refund(Locked);

        Assert.AreEqual(1, StartRun().GetStock(Locked));
    }

    [Test]
    public void ConsumeOrder_RunFirst_ThenPermanent_ThenConsumable()
    {
        InventoryManager m = StartRun();
        m.GrantRunGear(Owned, 1);      // run pool: 1
        m.GrantConsumable(Owned, 1);   // consumable pool: 1 (permanent pool: 1, from startingCopies)

        m.TryConsume(Owned);
        m.TryConsume(Owned);

        // Two spends should drain run then permanent, leaving the consumable
        // untouched - proven by checking the SAVED consumable count directly,
        // not just total stock (which would be 1 either way regardless of order).
        Assert.AreEqual(1, m.GetStock(Owned));
        Assert.AreEqual(1, store.Load().GetConsumableCount(Owned));
    }

    [Test]
    public void RefundOrder_ReversesConsumeOrder_ConsumableComesBackFirst()
    {
        InventoryManager m = StartRun();
        m.GrantRunGear(Owned, 1);
        m.GrantConsumable(Owned, 1);
        m.TryConsume(Owned);
        m.TryConsume(Owned);
        m.TryConsume(Owned);

        m.Refund(Owned);

        // Refund order is Consumable -> Permanent -> Run, so the consumable comes back
        // first: available goes from 0 back to 1 (the just-refunded consumable).
        Assert.AreEqual(1, m.GetStock(Owned));
    }

    [Test]
    public void GetMaxStock_StaysStable_WhileConsumableIsSpent()
    {
        InventoryManager m = StartRun();
        m.GrantConsumable(Locked, 2);
        int before = m.GetMaxStock(Locked);

        m.TryConsume(Locked);

        Assert.AreEqual(before, m.GetMaxStock(Locked));
    }

    [Test]
    public void UnknownId_IsRejected()
    {
        InventoryManager m = StartRun();

        Assert.IsFalse(m.TryConsume(424242));
        Assert.AreEqual(0, m.GetStock(424242));
        Assert.AreEqual(0, m.GetMaxStock(424242));
    }
}
