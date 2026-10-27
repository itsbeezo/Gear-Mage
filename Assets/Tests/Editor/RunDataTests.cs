using NUnit.Framework;

public class RunDataTests
{
    [Test]
    public void New_StartsAtWorldOneLevelOneWaveOne_WithNoGold_NotStarted()
    {
        RunData run = new RunData();

        Assert.AreEqual(1, run.world);
        Assert.AreEqual(1, run.level);
        Assert.AreEqual(1, run.wave);
        Assert.AreEqual(0, run.gold);
        Assert.IsFalse(run.started);
        Assert.IsFalse(run.endless);
    }

    [Test]
    public void Begin_SetsWorld_AndResetsLevelWaveEndless()
    {
        RunData run = new RunData { level = 3, wave = 4, endless = true };

        run.Begin(2);

        Assert.AreEqual(2, run.world);
        Assert.AreEqual(1, run.level);
        Assert.AreEqual(1, run.wave);
        Assert.IsFalse(run.endless);
        Assert.IsTrue(run.started);
    }

    [Test]
    public void AddGold_IgnoresZeroAndNegative()
    {
        RunData run = new RunData();

        run.AddGold(5);
        run.AddGold(0);
        run.AddGold(-3);

        Assert.AreEqual(5, run.gold);
    }

    [Test]
    public void TrySpendGold_MoreThanOwned_FailsAndKeepsGold()
    {
        RunData run = new RunData { gold = 4 };

        Assert.IsFalse(run.TrySpendGold(5));
        Assert.AreEqual(4, run.gold);
    }

    [Test]
    public void TrySpendGold_ExactAmount_SpendsAll()
    {
        RunData run = new RunData { gold = 4 };

        Assert.IsTrue(run.TrySpendGold(4));
        Assert.AreEqual(0, run.gold);
    }

    [Test]
    public void AddRunGear_Accumulates_UnknownIdIsZero()
    {
        RunData run = new RunData();

        run.AddRunGear(2, 1);
        run.AddRunGear(2, 2);
        run.AddRunGear(3, 0);

        Assert.AreEqual(3, run.GetRunGearCount(2));
        Assert.AreEqual(0, run.GetRunGearCount(3));
    }

    [Test]
    public void New_HasNoRunUpgrades()
    {
        RunData run = new RunData();

        Assert.AreEqual(0, run.runUpgrades.attack);
    }
}
