using NUnit.Framework;

public class InventoryGearsTests
{
    [Test]
    public void ShouldBeVisible_PositiveStock_ReturnsTrue()
    {
        Assert.IsTrue(InventoryGears.ShouldBeVisible(1));
    }

    [Test]
    public void ShouldBeVisible_ZeroStock_ReturnsFalse()
    {
        Assert.IsFalse(InventoryGears.ShouldBeVisible(0));
    }

    [Test]
    public void FormatBadge_FormatsAsCurrentSlashMax()
    {
        Assert.AreEqual("2/5", InventoryGears.FormatBadge(2, 5));
    }

    [Test]
    public void FormatBadge_ZeroCurrent_StillFormats()
    {
        Assert.AreEqual("0/5", InventoryGears.FormatBadge(0, 5));
    }
}
