using NUnit.Framework;
using UnityEngine;

public class PlayerInventoryDataTests
{
    [Test]
    public void AddPermanentExtra_Accumulates_AndClampsAtZero()
    {
        var data = new PlayerInventoryData();

        data.AddPermanentExtra(5, 2);
        data.AddPermanentExtra(5, -5);

        Assert.AreEqual(0, data.GetPermanentExtra(5));
    }

    [Test]
    public void AddConsumable_Accumulates_AndClampsAtZero()
    {
        var data = new PlayerInventoryData();

        data.AddConsumable(7, 2);
        data.AddConsumable(7, -5);

        Assert.AreEqual(0, data.GetConsumableCount(7));
    }

    [Test]
    public void UnknownIds_ReadAsZero()
    {
        var data = new PlayerInventoryData();

        Assert.AreEqual(0, data.GetPermanentExtra(99));
        Assert.AreEqual(0, data.GetConsumableCount(99));
    }

    [Test]
    public void PermanentExtra_AndConsumable_AreIndependentPerGear()
    {
        var data = new PlayerInventoryData();

        data.AddPermanentExtra(5, 2);
        data.AddConsumable(5, 3);

        Assert.AreEqual(2, data.GetPermanentExtra(5));
        Assert.AreEqual(3, data.GetConsumableCount(5));
    }

    [Test]
    public void JsonRoundTrip_PreservesEverything()
    {
        var data = new PlayerInventoryData();
        data.AddPermanentExtra(5, 2);
        data.AddConsumable(7, 3);

        var copy = JsonUtility.FromJson<PlayerInventoryData>(JsonUtility.ToJson(data));

        Assert.AreEqual(2, copy.GetPermanentExtra(5));
        Assert.AreEqual(3, copy.GetConsumableCount(7));
    }
}
