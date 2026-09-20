using TMPro;
using UnityEngine;

public class InventoryGears : MonoBehaviour
{
    public int gearId;
    public bool suppressVisibilityControl = false;

    [SerializeField] private SpriteRenderer icon;
    [SerializeField] private Collider2D dragCollider;
    [SerializeField] private TextMeshPro stockBadge;

    private SpriteRenderer[] allRenderers;
    private string lastLoggedBadgeState;

    private void Awake()
    {
        allRenderers = GetComponentsInChildren<SpriteRenderer>(true);

        // Force the badge active at spawn regardless of its prefab-authored
        // default - Update() is the sole authority over its visibility from
        // here on. Guards against the default accidentally getting baked to
        // inactive on the shared Badge prefab (e.g. an "Apply to Prefab" on
        // its Active-state override while testing in Play mode).
        if (stockBadge != null)
        {
            stockBadge.gameObject.SetActive(true);
        }
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
                stockBadge.ForceMeshUpdate();
            }
            else
            {
                stockBadge.gameObject.SetActive(visible);
                if (visible)
                {
                    stockBadge.text = FormatBadge(current, max);
                    stockBadge.ForceMeshUpdate();
                }
            }

            Renderer meshRenderer = stockBadge.GetComponent<Renderer>();
            string debugState = $"{gameObject.name}|gearId={gearId}|current={current}|max={max}|visible={visible}|badgeActiveSelf={stockBadge.gameObject.activeSelf}|badgeEnabled={stockBadge.enabled}|badgeTextNow=\"{stockBadge.text}\"|meshRendererEnabled={(meshRenderer != null ? meshRenderer.enabled.ToString() : "NULL")}";
            if (debugState != lastLoggedBadgeState)
            {
                Debug.Log("[BadgeDebug] " + debugState);
                lastLoggedBadgeState = debugState;
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
