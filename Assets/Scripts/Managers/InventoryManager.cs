using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class GearStockEntry
{
    public int id;
    public int startingStock;
    public int maxStock;
}

public class InventoryManager : MonoBehaviour
{
    public static InventoryManager instance { get; private set; }

    public List<GearStockEntry> startingEntries = new List<GearStockEntry>();

    private class GearStock
    {
        public int current;
        public int max;
    }

    private Dictionary<int, GearStock> stock;

    private const string STOCK_KEY_PREFIX = "GearStock_";
    private const string MAX_KEY_PREFIX = "GearStockMax_";

    private void Awake()
    {
        instance = this;
        Initialize();
    }

    // Separated from Awake so tests can build stock without going through
    // Unity's scene-load lifecycle.
    public void Initialize()
    {
        stock = new Dictionary<int, GearStock>();
        foreach (var entry in startingEntries)
        {
            int max = PlayerPrefs.GetInt(MAX_KEY_PREFIX + entry.id, entry.maxStock);
            int current = PlayerPrefs.GetInt(STOCK_KEY_PREFIX + entry.id, entry.startingStock);
            stock[entry.id] = new GearStock { current = current, max = max };
        }
    }

    public int GetStock(int id)
    {
        return stock != null && stock.TryGetValue(id, out var s) ? s.current : 0;
    }

    public int GetMaxStock(int id)
    {
        return stock != null && stock.TryGetValue(id, out var s) ? s.max : 0;
    }

    public bool TryConsume(int id)
    {
        if (stock == null || !stock.TryGetValue(id, out var s) || s.current <= 0) return false;
        s.current -= 1;
        SaveStock(id);
        return true;
    }

    public void Refund(int id)
    {
        if (stock == null || !stock.TryGetValue(id, out var s)) return;
        s.current = Mathf.Min(s.current + 1, s.max);
        SaveStock(id);
    }

    // Not called by anything yet - the hook point for a future Shop
    // "upgrade capacity" purchase.
    public void IncreaseMaxStock(int id, int amount)
    {
        if (stock == null || !stock.TryGetValue(id, out var s)) return;
        s.max += amount;
        SaveStock(id);
    }

    private void SaveStock(int id)
    {
        var s = stock[id];
        PlayerPrefs.SetInt(STOCK_KEY_PREFIX + id, s.current);
        PlayerPrefs.SetInt(MAX_KEY_PREFIX + id, s.max);
        PlayerPrefs.Save();
    }

    private void OnApplicationQuit()
    {
        SaveAll();
    }

    private void OnApplicationPause(bool paused)
    {
        if (paused) SaveAll();
    }

    private void SaveAll()
    {
        if (stock == null) return;
        foreach (var id in stock.Keys)
        {
            SaveStock(id);
        }
    }
}
