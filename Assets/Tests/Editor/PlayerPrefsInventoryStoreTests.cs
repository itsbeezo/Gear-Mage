using NUnit.Framework;
using UnityEngine;

public class PlayerPrefsInventoryStoreTests
{
    private const string TestKey = "GearInventory_TEST_9001";
    private const string TestLegacyStockPrefix = "GearStockTEST_9001_";
    private const string TestLegacyMaxPrefix = "GearStockMaxTEST_9001_";

    [SetUp]
    public void SetUp()
    {
        CleanUp();
    }

    [TearDown]
    public void TearDown()
    {
        CleanUp();
    }

    private static void CleanUp()
    {
        PlayerPrefs.DeleteKey(TestKey);
        for (int id = 1; id <= 11; id++)
        {
            PlayerPrefs.DeleteKey(TestLegacyStockPrefix + id);
            PlayerPrefs.DeleteKey(TestLegacyMaxPrefix + id);
        }
    }

    private static PlayerPrefsInventoryStore NewTestStore()
    {
        return new PlayerPrefsInventoryStore(TestKey, TestLegacyStockPrefix, TestLegacyMaxPrefix);
    }

    [Test]
    public void Load_WhenNothingSaved_ReturnsEmptyData()
    {
        var store = NewTestStore();

        PlayerInventoryData data = store.Load();

        Assert.AreEqual(0, data.permanentExtras.Count);
        Assert.AreEqual(0, data.consumables.Count);
    }

    [Test]
    public void SaveThenLoad_RoundTrips()
    {
        var store = NewTestStore();
        var data = new PlayerInventoryData();
        data.AddPermanentExtra(5, 2);
        data.AddConsumable(7, 3);

        store.Save(data);
        PlayerInventoryData loaded = NewTestStore().Load();

        Assert.AreEqual(2, loaded.GetPermanentExtra(5));
        Assert.AreEqual(3, loaded.GetConsumableCount(7));
    }

    [Test]
    public void Load_CorruptJson_ReturnsEmptyData()
    {
        PlayerPrefs.SetString(TestKey, "{ this is not json");
        var store = NewTestStore();

        PlayerInventoryData data = store.Load();

        Assert.AreEqual(0, data.permanentExtras.Count);
    }

    [Test]
    public void Clear_RemovesSavedData()
    {
        var store = NewTestStore();
        var data = new PlayerInventoryData();
        data.AddPermanentExtra(5, 2);
        store.Save(data);

        store.Clear();

        Assert.AreEqual(0, store.Load().GetPermanentExtra(5));
    }

    [Test]
    public void Load_WhenKeyMissing_CleansUpLegacyKeys()
    {
        PlayerPrefs.SetInt(TestLegacyStockPrefix + "1", 2);
        PlayerPrefs.SetInt(TestLegacyMaxPrefix + "1", 5);
        var store = NewTestStore();

        store.Load();

        Assert.IsFalse(PlayerPrefs.HasKey(TestLegacyStockPrefix + "1"));
        Assert.IsFalse(PlayerPrefs.HasKey(TestLegacyMaxPrefix + "1"));
    }

    [Test]
    public void Clear_AlsoCleansUpLegacyKeys()
    {
        PlayerPrefs.SetInt(TestLegacyStockPrefix + "1", 2);
        var store = NewTestStore();

        store.Clear();

        Assert.IsFalse(PlayerPrefs.HasKey(TestLegacyStockPrefix + "1"));
    }
}
