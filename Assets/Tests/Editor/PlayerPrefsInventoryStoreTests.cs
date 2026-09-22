using NUnit.Framework;
using UnityEngine;

public class PlayerPrefsInventoryStoreTests
{
    private const string TestKey = "GearInventory_TEST_9001";

    [SetUp]
    public void SetUp()
    {
        PlayerPrefs.DeleteKey(TestKey);
    }

    [TearDown]
    public void TearDown()
    {
        PlayerPrefs.DeleteKey(TestKey);
    }

    [Test]
    public void Load_WhenNothingSaved_ReturnsEmptyData()
    {
        var store = new PlayerPrefsInventoryStore(TestKey);

        PlayerInventoryData data = store.Load();

        Assert.AreEqual(0, data.permanentExtras.Count);
        Assert.AreEqual(0, data.consumables.Count);
    }

    [Test]
    public void SaveThenLoad_RoundTrips()
    {
        var store = new PlayerPrefsInventoryStore(TestKey);
        var data = new PlayerInventoryData();
        data.AddPermanentExtra(5, 2);
        data.AddConsumable(7, 3);

        store.Save(data);
        PlayerInventoryData loaded = new PlayerPrefsInventoryStore(TestKey).Load();

        Assert.AreEqual(2, loaded.GetPermanentExtra(5));
        Assert.AreEqual(3, loaded.GetConsumableCount(7));
    }

    [Test]
    public void Load_CorruptJson_ReturnsEmptyData()
    {
        PlayerPrefs.SetString(TestKey, "{ this is not json");
        var store = new PlayerPrefsInventoryStore(TestKey);

        PlayerInventoryData data = store.Load();

        Assert.AreEqual(0, data.permanentExtras.Count);
    }

    [Test]
    public void Clear_RemovesSavedData()
    {
        var store = new PlayerPrefsInventoryStore(TestKey);
        var data = new PlayerInventoryData();
        data.AddPermanentExtra(5, 2);
        store.Save(data);

        store.Clear();

        Assert.AreEqual(0, store.Load().GetPermanentExtra(5));
    }
}
