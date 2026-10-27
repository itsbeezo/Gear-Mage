using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.UI;

// The level shop panel. UIManager.ApplyState opens/closes it; it never touches GameManager's screens [S2].
// It also owns the level-shop tray [G2, G3, R20]: gears bought in the level shop wait there until the
// player puts them on the board or sells them back. Continue stays greyed out while the tray holds any.
public class ShopManager : MonoBehaviour
{
    public static ShopManager instance { get; private set; }

    private static readonly Vector3 BoardGearBoxPosition = new Vector3(-2.02f, 2.9f, 11f);
    private static readonly Vector3 ShopGearBoxPosition = new Vector3(-2.02f, 4.31f, 11f);

    public GameObject gearBox;

    [SerializeField] private GameObject shopGroup;

    [SerializeField] private Button gearBoxToggle;

    [SerializeField] private SortingGroup shopGroupLayer;

    [SerializeField] private GameObject returnButton;

    [SerializeField] private GameObject inventoryGears;

    [Header("Level-shop tray")]
    [Tooltip("Tray slots under GearBoxAll (so they move with the gearbox): copies of a GearBox slot with the GearBox component removed.")]
    [SerializeField] private List<UIGearSlot> traySlots = new List<UIGearSlot>();

    public bool IsTrayEmpty
    {
        get
        {
            foreach (UIGearSlot slot in traySlots)
            {
                if (slot != null && slot.isFull) return false;
            }
            return true;
        }
    }

    private void Awake()
    {
        instance = this;
    }

    // Level shop "Return" button (Inspector OnClick). Leaving the shop is a screen change, so GameManager decides.
    public void BackToGame()
    {
        GameManager.instance.LeaveLevelShop();
    }

    // Gearbox toggle button (Inspector OnClick).
    public void ToggleGearbox()
    {
        int oldOrder = shopGroupLayer.sortingOrder;
        int newOrder = (oldOrder > 9) ? 8 : 12;

        shopGroupLayer.sortingOrder = newOrder;
    }

    // Called only by UIManager.ApplyState.
    public void SetShopOpen(bool open)
    {
        if (shopGroup == null) return;

        inventoryGears.SetActive(!open);
        shopGroup.SetActive(open);
        gearBoxToggle.gameObject.SetActive(open);
        returnButton.SetActive(open);
        gearBox.transform.position = open ? ShopGearBoxPosition : BoardGearBoxPosition;
    }

    // UIGearSlot, when a level-shop offer is dropped on a tray slot. False, and nothing is spent,
    // if the slot is not an empty tray slot, the gear has no board prefab, or the player can't afford it.
    public bool TryBuyIntoTray(UIGearSlot slot, int id)
    {
        if (!traySlots.Contains(slot) || slot.isFull) return false;

        GameObject prefab = GearCatalog.instance.GetPrefab(id);
        if (prefab == null)
        {
            Debug.LogWarning("ShopManager: no board prefab in the catalog for gear id " + id);
            return false;
        }

        if (!CurrencyManager.instance.TryBuyGear(id)) return false;

        SpawnTrayGear(prefab, slot, id);
        slot.isFull = true;
        UIManager.instance.RefreshContinueButton();
        return true;
    }

    // The tray gear left its slot (placed on the board or sold). The caller owns its GameObject.
    public void Unstage(UIGearSlot slot)
    {
        if (!traySlots.Contains(slot)) return;

        slot.isFull = false;
        UIManager.instance.RefreshContinueButton();
    }

    // Sell back for the catalog's sellValue [G3]. False, and nothing changes, if the gear is not sellable.
    public bool TrySell(UIGearSlot slot, int id)
    {
        GearDefinition def = GearCatalog.instance.GetDefinition(id);
        if (def == null || !def.Sellable) return false;

        CurrencyManager.instance.RefundGold(def.sellValue);
        Unstage(slot);
        return true;
    }

    // A tray gear is owned (GearBase.Id stamped) and parented to its tray slot; UIDragHandler reads both.
    // Display only until placed: no rotation, production, or neighbour logic (same as ShopOffer).
    private static void SpawnTrayGear(GameObject prefab, UIGearSlot slot, int id)
    {
        GameObject gear = Instantiate(prefab, slot.transform.position, prefab.transform.rotation);
        gear.transform.SetParent(slot.transform, true);
        gear.GetComponent<GearBase>().AssignId(id);

        if (gear.TryGetComponent(out GearRotate rotate)) rotate.enabled = false;
    }
}
