using NUnit.Framework;
using UnityEngine;

public class PlayerPrefsProgressSaveTests
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

    private static PlayerPrefsProgressSave NewTestSave()
    {
        return new PlayerPrefsProgressSave(TestKey, TestLegacyBoneKey);
    }

    [Test]
    public void Load_WhenNothingSaved_ReturnsFreshProgress()
    {
        PlayerProgress progress = NewTestSave().Load();

        Assert.AreEqual(0, progress.bones);
        Assert.AreEqual(1, progress.highestUnlockedWorld);
    }

    [Test]
    public void SaveThenLoad_RoundTrips()
    {
        PlayerPrefsProgressSave save = NewTestSave();
        save.Save(new PlayerProgress { bones = 7, highestUnlockedWorld = 2 });

        PlayerProgress loaded = NewTestSave().Load();

        Assert.AreEqual(7, loaded.bones);
        Assert.AreEqual(2, loaded.highestUnlockedWorld);
    }

    [Test]
    public void Load_LegacyBoneKey_IsMigratedOnceAndDeleted()
    {
        PlayerPrefs.SetInt(TestLegacyBoneKey, 42);

        PlayerProgress loaded = NewTestSave().Load();

        Assert.AreEqual(42, loaded.bones);
        Assert.IsFalse(PlayerPrefs.HasKey(TestLegacyBoneKey));
        Assert.AreEqual(42, NewTestSave().Load().bones);
    }

    [Test]
    public void Load_UnreadableJson_ReturnsFreshProgress()
    {
        PlayerPrefs.SetString(TestKey, "{not json");

        PlayerProgress loaded = NewTestSave().Load();

        Assert.AreEqual(0, loaded.bones);
    }

    [Test]
    public void Clear_RemovesSavedProgress()
    {
        PlayerPrefsProgressSave save = NewTestSave();
        save.Save(new PlayerProgress { bones = 5 });

        save.Clear();

        Assert.AreEqual(0, NewTestSave().Load().bones);
    }
}
