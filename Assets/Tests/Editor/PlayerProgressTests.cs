using NUnit.Framework;
using UnityEngine;

// PlayerProgress rules (bones, world unlocks).
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

// PlayerProgress storage (Load/Save/Clear and the legacy bone migration) against PlayerPrefs, with test-only keys.
public class PlayerProgressStorageTests
{
    private const string TestKey = "PlayerProgress_TEST_9001";
    private const string TestLegacyBoneKey = "SavedBoneCount_TEST_9001";

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
        PlayerPrefs.DeleteKey(TestLegacyBoneKey);
    }

    private static PlayerProgress LoadTest()
    {
        return PlayerProgress.Load(TestKey, TestLegacyBoneKey);
    }

    [Test]
    public void Load_WhenNothingSaved_ReturnsFreshProgress()
    {
        PlayerProgress progress = LoadTest();

        Assert.AreEqual(0, progress.bones);
        Assert.AreEqual(1, progress.highestUnlockedWorld);
    }

    [Test]
    public void SaveThenLoad_RoundTrips()
    {
        new PlayerProgress { bones = 7, highestUnlockedWorld = 2 }.Save(TestKey);

        PlayerProgress loaded = LoadTest();

        Assert.AreEqual(7, loaded.bones);
        Assert.AreEqual(2, loaded.highestUnlockedWorld);
    }

    [Test]
    public void Load_LegacyBoneKey_IsMigratedOnceAndDeleted()
    {
        PlayerPrefs.SetInt(TestLegacyBoneKey, 42);

        PlayerProgress loaded = LoadTest();

        Assert.AreEqual(42, loaded.bones);
        Assert.IsFalse(PlayerPrefs.HasKey(TestLegacyBoneKey));
        Assert.AreEqual(42, LoadTest().bones);
    }

    [Test]
    public void Load_UnreadableJson_ReturnsFreshProgress()
    {
        PlayerPrefs.SetString(TestKey, "{not json");

        PlayerProgress loaded = LoadTest();

        Assert.AreEqual(0, loaded.bones);
    }

    [Test]
    public void Clear_RemovesSavedProgress()
    {
        new PlayerProgress { bones = 5 }.Save(TestKey);

        PlayerProgress.Clear(TestKey, TestLegacyBoneKey);

        Assert.AreEqual(0, LoadTest().bones);
    }

    [Test]
    public void SaveThenLoad_RoundTripsPermanentUpgrades()
    {
        PlayerProgress progress = new PlayerProgress();
        progress.permanentUpgrades.attack = 3;
        progress.Save(TestKey);

        Assert.AreEqual(3, LoadTest().permanentUpgrades.attack);
    }
}
