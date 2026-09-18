using TMPro;
using UnityEngine;

public class InventoryGears : MonoBehaviour
{
    public int gearId;

    [SerializeField] private SpriteRenderer icon;
    [SerializeField] private Collider2D dragCollider;
    [SerializeField] private TextMeshProUGUI stockBadge;

    private void Update()
    {
        if (InventoryManager.instance == null) return;

        int current = InventoryManager.instance.GetStock(gearId);
        int max = InventoryManager.instance.GetMaxStock(gearId);
        bool visible = ShouldBeVisible(current);

        if (icon != null) icon.enabled = visible;
        if (dragCollider != null) dragCollider.enabled = visible;

        if (stockBadge != null)
        {
            stockBadge.gameObject.SetActive(visible);
            if (visible) stockBadge.text = FormatBadge(current, max);
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
