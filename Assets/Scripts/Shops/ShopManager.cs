using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

// The level shop panel. UIManager.ApplyState opens/closes it; it never touches GameManager's screens [S2].
// In the shop the gearbox waits off the top and the tray off the bottom; their toggles slide them in and out.
// The tray [G2, G3, R20] is optional storage: offers dropped into the gearbox are bought onto the board,
// offers dropped into the tray are bought and wait there until the player puts them on the board or sells them.
// The tray also shows in PreLevel, where Continue stays greyed out while it holds any gear.
public class ShopManager : MonoBehaviour
{
    public static ShopManager instance { get; private set; }

    private static readonly Vector3 BoardGearBoxPosition = new Vector3(-2.02f, 2.9f, 11f);
    private static readonly Vector3 ShopGearBoxPosition = new Vector3(-2.02f, 4.31f, 11f);

    public GameObject gearBox;

    [SerializeField] private GameObject shopGroup;

    [SerializeField] private Button gearBoxToggle;

    [SerializeField] private GameObject returnButton;

    [SerializeField] private GameObject inventoryGears;

    [Header("Slide in/out")]
    [Tooltip("How far the gearbox (up) and the tray (down) move to leave the frame.")]
    [SerializeField] private float slideDistance = 10f;
    [SerializeField] private float slideSeconds = 0.35f;

    [Header("Level-shop tray")]
    [Tooltip("The Tray object. Where it sits in the scene is its shown position.")]
    [SerializeField] private GameObject tray;
    [Tooltip("The bottom indicator that slides the tray in and out (Inspector OnClick: ToggleTray).")]
    [SerializeField] private Button trayToggle;
    [Tooltip("Tray slots under Tray: copies of a GearBox slot with the GearBox component removed.")]
    [SerializeField] private List<UIGearSlot> traySlots = new List<UIGearSlot>();

    private bool gearBoxShown;
    private bool trayShown;
    private Vector3 trayShownPosition;
    private Coroutine gearBoxSlide;
    private Coroutine traySlide;

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

    private Vector3 GearBoxHiddenPosition
    {
        get { return ShopGearBoxPosition + Vector3.up * slideDistance; }
    }

    private Vector3 TrayHiddenPosition
    {
        get { return trayShownPosition + Vector3.down * slideDistance; }
    }

    private void Awake()
    {
        instance = this;
        if (tray != null) trayShownPosition = tray.transform.position;
    }

    // Level shop "Return" button (Inspector OnClick). Leaving the shop is a screen change, so GameManager decides.
    public void BackToGame()
    {
        GameManager.instance.LeaveLevelShop();
    }

    // Gearbox toggle button, the top arrow (Inspector OnClick).
    public void ToggleGearbox()
    {
        gearBoxShown = !gearBoxShown;
        SlideTo(gearBox.transform, ref gearBoxSlide, gearBoxShown ? ShopGearBoxPosition : GearBoxHiddenPosition);
    }

    // Tray toggle button, the bottom indicator (Inspector OnClick). The tray is active only while shown or sliding.
    public void ToggleTray()
    {
        if (tray == null) return;

        trayShown = !trayShown;
        if (trayShown) tray.SetActive(true);
        SlideTo(tray.transform, ref traySlide, trayShown ? trayShownPosition : TrayHiddenPosition, !trayShown ? tray : null);
    }

    // Called only by UIManager.ApplyState. Screen changes snap into place; only the toggles slide.
    public void ApplyState(GameManager.State state)
    {
        if (shopGroup == null) return;

        bool open = state == GameManager.State.LevelShop;
        inventoryGears.SetActive(!open);
        shopGroup.SetActive(open);
        gearBoxToggle.gameObject.SetActive(open);
        if (trayToggle != null) trayToggle.gameObject.SetActive(open);
        returnButton.SetActive(open);

        gearBoxShown = false;
        SnapTo(gearBox.transform, ref gearBoxSlide, open ? GearBoxHiddenPosition : BoardGearBoxPosition);

        if (tray == null) return;
        // Hidden means off and below the frame, so a click always slides it up first.
        trayShown = state == GameManager.State.PreLevel;
        tray.SetActive(trayShown);
        SnapTo(tray.transform, ref traySlide, trayShown ? trayShownPosition : TrayHiddenPosition);
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

    private void SnapTo(Transform target, ref Coroutine running, Vector3 position)
    {
        if (running != null) StopCoroutine(running);
        running = null;
        target.position = position;
    }

    // A new slide starts from wherever the last one left off, so toggling mid-slide reverses smoothly.
    // deactivateAtEnd (the tray sliding out) is switched off once it has left the frame.
    private void SlideTo(Transform target, ref Coroutine running, Vector3 position, GameObject deactivateAtEnd = null)
    {
        if (running != null) StopCoroutine(running);
        running = StartCoroutine(Slide(target, position, deactivateAtEnd));
    }

    private IEnumerator Slide(Transform target, Vector3 to, GameObject deactivateAtEnd)
    {
        Vector3 from = target.position;
        for (float t = 0f; t < slideSeconds; t += Time.unscaledDeltaTime)
        {
            target.position = Vector3.Lerp(from, to, Mathf.SmoothStep(0f, 1f, t / slideSeconds));
            yield return null;
        }
        target.position = to;
        if (deactivateAtEnd != null) deactivateAtEnd.SetActive(false);
    }
}
