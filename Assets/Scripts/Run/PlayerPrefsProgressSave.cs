using System;
using UnityEngine;

public class PlayerPrefsProgressSave : ProgressSave
{
    public const string DefaultKey = "PlayerProgress_v1";
    public const string DefaultLegacyBoneKey = "SavedBoneCount";

    private const string LegacyGoldKey = "GoldIsSaved";
    private const string LegacyBoardPrefix = "GearMatrix_";
    private const int LegacyBoardRows = 4;
    private const int LegacyBoardColumns = 9;

    private readonly string key;
    private readonly string legacyBoneKey;

    public PlayerPrefsProgressSave(string key = DefaultKey, string legacyBoneKey = DefaultLegacyBoneKey)
    {
        this.key = key;
        this.legacyBoneKey = legacyBoneKey;
    }

    public PlayerProgress Load()
    {
        if (!PlayerPrefs.HasKey(key)) return MigrateLegacyBones();

        try
        {
            PlayerProgress loaded = JsonUtility.FromJson<PlayerProgress>(PlayerPrefs.GetString(key, ""));
            return loaded ?? new PlayerProgress();
        }
        catch (Exception)
        {
            // Broad on purpose, like PlayerPrefsInventorySave: JsonUtility's failure type is not guaranteed.
            Debug.LogWarning("PlayerPrefsProgressSave: saved progress was unreadable; starting fresh.");
            return new PlayerProgress();
        }
    }

    public void Save(PlayerProgress progress)
    {
        PlayerPrefs.SetString(key, JsonUtility.ToJson(progress));
        PlayerPrefs.Save();
    }

    public void Clear()
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
    private PlayerProgress MigrateLegacyBones()
    {
        PlayerProgress progress = new PlayerProgress();
        if (PlayerPrefs.HasKey(legacyBoneKey))
        {
            progress.bones = PlayerPrefs.GetInt(legacyBoneKey, 0);
            PlayerPrefs.DeleteKey(legacyBoneKey);
            Save(progress);
        }
        return progress;
    }
}
