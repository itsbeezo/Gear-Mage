using System;
using UnityEngine;

public class PlayerPrefsInventoryStore : IInventoryStore
{
    public const string DefaultKey = "GearInventory_v1";

    private const string LegacyStockPrefix = "GearStock_";
    private const string LegacyMaxPrefix = "GearStockMax_";
    private const int LegacyHighestId = 11;

    private readonly string key;

    public PlayerPrefsInventoryStore(string key = DefaultKey)
    {
        this.key = key;
    }

    public PlayerInventoryData Load()
    {
        if (!PlayerPrefs.HasKey(key))
        {
            if (key == DefaultKey) DeleteLegacyStockKeys();
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
        if (key == DefaultKey) DeleteLegacyStockKeys();
        PlayerPrefs.Save();
    }

    private static void DeleteLegacyStockKeys()
    {
        for (int id = 1; id <= LegacyHighestId; id++)
        {
            PlayerPrefs.DeleteKey(LegacyStockPrefix + id);
            PlayerPrefs.DeleteKey(LegacyMaxPrefix + id);
        }
    }
}
