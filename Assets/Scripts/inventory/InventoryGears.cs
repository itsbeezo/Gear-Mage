using TMPro;
using UnityEngine;

public class InventoryGears : MonoBehaviour
{
    public int gearId;
    public bool suppressVisibilityControl = false;

    [SerializeField] private Collider2D dragCollider;
    private TextMeshPro stockBadge;

    private SpriteRenderer[] allRenderers;

    private const string STOCK_KEY_PREFIX = "GearStock_";
    private const string MAX_KEY_PREFIX = "GearStockMax_";
    private const int LOWEST_GEAR_ID = 1;
    private const int HIGHEST_GEAR_ID = 11;

    private static void ResetGearInventoryStock()
    {
        for (int id = LOWEST_GEAR_ID; id <= HIGHEST_GEAR_ID; id++)
        {
            PlayerPrefs.DeleteKey(STOCK_KEY_PREFIX + id);
            PlayerPrefs.DeleteKey(MAX_KEY_PREFIX + id);
        }

        PlayerPrefs.Save();
        Debug.Log("Cleared saved gear inventory stock (ids 1-11). Next Play session will start from GearCatalog's starting/max values again.");
    }

    private void Awake()
    {
        ResetGearInventoryStock();
        allRenderers = GetComponentsInChildren<SpriteRenderer>(true);

        // Resolve the badge from THIS instance's own hierarchy. Never serialize a
        // reference to it: a field pointing at the Badge *prefab asset* (instead of the
        // nested child) makes every shell write to the shared asset and never to the
        // badge on screen. Cloned shells run their own Awake, so each finds its own.
        stockBadge = GetComponentInChildren<TextMeshPro>(true);
    }

    private void Update()
    {
        if (InventoryManager.instance == null) return;

        int current = InventoryManager.instance.GetStock(gearId);
        int max = InventoryManager.instance.GetMaxStock(gearId);
        bool visible = ShouldBeVisible(current);

        if (!suppressVisibilityControl)
        {
            foreach (SpriteRenderer renderer in allRenderers)
            {
                if (renderer != null) renderer.enabled = visible;
            }
            if (dragCollider != null) dragCollider.enabled = visible;
        }

        if (stockBadge != null)
        {
            if (suppressVisibilityControl)
            {
                stockBadge.gameObject.SetActive(true);
                stockBadge.text = FormatBadge(current, max);
            }
            else
            {
                stockBadge.gameObject.SetActive(visible);
                if (visible) stockBadge.text = FormatBadge(current, max);
            }
        }
    }

    public static bool ShouldBeVisible(int current)
    {
        return current > 0;
    }

    public static string FormatBadge(int current, int max)
    {
        return $"{current}/{max}";
    }
}
