using NUnit.Framework;

public class PlayerProgressTests
{
    [Test]
    public void AddBones_IgnoresZeroAndNegative()
    {
        PlayerProgress progress = new PlayerProgress();

        progress.AddBones(3);
        progress.AddBones(0);
        progress.AddBones(-2);

        Assert.AreEqual(3, progress.bones);
    }

    [Test]
    public void TrySpendBones_MoreThanOwned_FailsAndKeepsBones()
    {
        PlayerProgress progress = new PlayerProgress { bones = 2 };

        Assert.IsFalse(progress.TrySpendBones(3));
        Assert.AreEqual(2, progress.bones);
    }

    [Test]
    public void UnlockWorld_NeverLowers_AndCapsAtWorldCount()
    {
        PlayerProgress progress = new PlayerProgress();

        progress.UnlockWorld(2);
        progress.UnlockWorld(1);
        Assert.AreEqual(2, progress.highestUnlockedWorld);

        progress.UnlockWorld(9);
        Assert.AreEqual(PlayerProgress.WorldCount, progress.highestUnlockedWorld);
    }

    [Test]
    public void IsWorldUnlocked_ByDefaultOnlyWorldOne()
    {
        PlayerProgress progress = new PlayerProgress();

        Assert.IsFalse(progress.IsWorldUnlocked(0));
        Assert.IsTrue(progress.IsWorldUnlocked(1));
        Assert.IsFalse(progress.IsWorldUnlocked(2));
    }
}
