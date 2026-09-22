using System;
using UnityEngine;

public class PlayerPrefsInventoryStore : IInventoryStore
{
    public const string DefaultKey = "GearInventory_v1";

    private const string DefaultLegacyStockPrefix = "GearStock_";
    private const string DefaultLegacyMaxPrefix = "GearStockMax_";
    private const int LegacyHighestId = 11;

    private readonly string key;
    private readonly string legacyStockPrefix;
    private readonly string legacyMaxPrefix;

    public PlayerPrefsInventoryStore(
        string key = DefaultKey,
        string legacyStockPrefix = DefaultLegacyStockPrefix,
        string legacyMaxPrefix = DefaultLegacyMaxPrefix)
    {
        this.key = key;
        this.legacyStockPrefix = legacyStockPrefix;
        this.legacyMaxPrefix = legacyMaxPrefix;
    }

    public PlayerInventoryData Load()
    {
        if (!PlayerPrefs.HasKey(key))
        {
            DeleteLegacyStockKeys();
            return new PlayerInventoryData();
        }

        try
        {
            PlayerInventoryData loaded = JsonUtility.FromJson<PlayerInventoryData>(PlayerPrefs.GetString(key, ""));
            return loaded ?? new PlayerInventoryData();
        }
        catch (Exception)
        {
            // Catches broadly, not just ArgumentException: Unity's JsonUtility's exact
            // failure mode on malformed input isn't guaranteed, so this must be robust
            // to whatever it actually throws, not just the most likely case.
            Debug.LogWarning("PlayerPrefsInventoryStore: saved inventory data was unreadable; starting empty.");
            return new PlayerInventoryData();
        }
    }

    public void Save(PlayerInventoryData data)
    {
        PlayerPrefs.SetString(key, JsonUtility.ToJson(data));
        PlayerPrefs.Save();
    }

    public void Clear()
    {
        PlayerPrefs.DeleteKey(key);
        DeleteLegacyStockKeys();
        PlayerPrefs.Save();
    }

    private void DeleteLegacyStockKeys()
    {
        for (int id = 1; id <= LegacyHighestId; id++)
        {
            PlayerPrefs.DeleteKey(legacyStockPrefix + id);
            PlayerPrefs.DeleteKey(legacyMaxPrefix + id);
        }

        PlayerPrefs.Save();
    }
}
