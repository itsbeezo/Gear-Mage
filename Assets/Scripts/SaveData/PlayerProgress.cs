using System;
using UnityEngine;

// Permanent data that survives runs [R7, R14, R19], and its storage: JSON under one PlayerPrefs key.
// RunManager loads it on scene load and saves it after every change [W8].
[Serializable]
public class PlayerProgress
{
    public const int WorldCount = 3;
    public const string DefaultKey = "PlayerProgress_v1";
    public const string DefaultLegacyBoneKey = "SavedBoneCount";

    private const string LegacyGoldKey = "GoldIsSaved";
    private const string LegacyBoardPrefix = "GearMatrix_";
    private const int LegacyBoardRows = 4;
    private const int LegacyBoardColumns = 9;

    public int bones;
    public int highestUnlockedWorld = 1;
    public UpgradeLevels permanentUpgrades = new UpgradeLevels();

    public bool IsWorldUnlocked(int world)
    {
        return world >= 1 && world <= highestUnlockedWorld;
    }

    public void UnlockWorld(int world)
    {
        int clamped = Math.Min(world, WorldCount);
        if (clamped > highestUnlockedWorld) highestUnlockedWorld = clamped;
    }

    public void AddBones(int amount)
    {
        if (amount > 0) bones += amount;
    }

    public bool TrySpendBones(int amount)
    {
        if (amount < 0 || amount > bones) return false;
        bones -= amount;
        return true;
    }

    // ---- Storage. The key parameters exist so tests can use their own keys. ----

    public static PlayerProgress Load(string key = DefaultKey, string legacyBoneKey = DefaultLegacyBoneKey)
    {
        if (!PlayerPrefs.HasKey(key)) return MigrateLegacyBones(key, legacyBoneKey);

        try
        {
            PlayerProgress loaded = JsonUtility.FromJson<PlayerProgress>(PlayerPrefs.GetString(key, ""));
            return loaded ?? new PlayerProgress();
        }
        catch (Exception)
        {
            // Broad on purpose, like PlayerPrefsInventorySave: JsonUtility's failure type is not guaranteed.
            Debug.LogWarning("PlayerProgress: saved progress was unreadable; starting fresh.");
            return new PlayerProgress();
        }
    }

    public void Save(string key = DefaultKey)
    {
        PlayerPrefs.SetString(key, JsonUtility.ToJson(this));
        PlayerPrefs.Save();
    }

    public static void Clear(string key = DefaultKey, string legacyBoneKey = DefaultLegacyBoneKey)
    {
        PlayerPrefs.DeleteKey(key);
        PlayerPrefs.DeleteKey(legacyBoneKey);
        PlayerPrefs.Save();
    }

    // Run data is memory-only now [R19]: delete the gold and board keys older builds wrote.
    public static void DeleteLegacyRunKeys()
    {
        PlayerPrefs.DeleteKey(LegacyGoldKey);
        for (int row = 0; row < LegacyBoardRows; row++)
        {
            for (int column = 0; column < LegacyBoardColumns; column++)
            {
                PlayerPrefs.DeleteKey(LegacyBoardPrefix + row + "_" + column);
            }
        }
        PlayerPrefs.Save();
    }

    // Bones used to live in their own int key; carry them over once.
    private static PlayerProgress MigrateLegacyBones(string key, string legacyBoneKey)
    {
        PlayerProgress progress = new PlayerProgress();
        if (PlayerPrefs.HasKey(legacyBoneKey))
        {
            progress.bones = PlayerPrefs.GetInt(legacyBoneKey, 0);
            PlayerPrefs.DeleteKey(legacyBoneKey);
            progress.Save(key);
        }
        return progress;
    }
}
