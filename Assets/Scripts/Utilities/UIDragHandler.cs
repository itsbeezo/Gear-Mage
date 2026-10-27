using System.Collections;
using UnityEngine;
using UnityEngine.EventSystems;

public class UIDragHandler : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler
{
    private SpriteRenderer spriteRenderer;
    private Collider2D col2D;
    private Vector3 originalPosition;
    private Vector3 offset;
    private Camera GearBoxCamera;
    private GameObject gearFallArea;

    [Header("Drop Settings")]
    [Tooltip("Maximum distance from the start point that still counts as 'dropped back where it was'.")]
    [SerializeField] private float maxReslotDistance = 1.5f;

    // Resolved once per drag in OnBeginDrag [G1]; UIGearSlot reads these on drop.
    public int gearNum { get; private set; }
    public bool IsShopOffer { get; private set; }
    public bool IsTrayGear { get; private set; }
    public UIGearSlot TraySlot { get; private set; }
    public bool startedOnBoard { get; private set; }
    private bool isBoardGear;
    private UIGearSlot currentSlot;

    private void Awake()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        col2D = GetComponent<Collider2D>();
        gearFallArea = GameObject.Find("GearFallArea");

        GameObject cameraObj = GameObject.Find("GearBoxCamera");
        if (cameraObj != null)
        {
            GearBoxCamera = cameraObj.GetComponent<Camera>();
        }
    }

    // The camera that actually received the pointer, so drags track the cursor
    // in whichever view the gear was clicked in (level or gear box).
    private Camera InputCamera(PointerEventData eventData)
    {
        return eventData.pressEventCamera != null ? eventData.pressEventCamera : GearBoxCamera;
    }

    public void OnBeginDrag(PointerEventData eventData)
    {
        ResolveIdentity();
        originalPosition = transform.position;
        currentSlot = isBoardGear ? BoardSlotUnderneath() : null;
        startedOnBoard = currentSlot != null;

        Vector3 mouseWorldPos = InputCamera(eventData).ScreenToWorldPoint(eventData.position);
        mouseWorldPos.z = 0f;
        offset = transform.position - mouseWorldPos;

        SetDragLook(true);
    }

    public void OnDrag(PointerEventData eventData)
    {
        Vector3 mouseWorldPos = InputCamera(eventData).ScreenToWorldPoint(eventData.position);
        mouseWorldPos.z = 0f;
        transform.position = mouseWorldPos + offset;
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        if (IsShopOffer)
        {
            EndShopOfferDrag();
            return;
        }

        if (IsTrayGear)
        {
            EndTrayGearDrag(eventData);
            return;
        }

        UIGearSlot targetSlot = FindDropTarget();
        if (targetSlot != null && targetSlot.TryPlaceGear(this))
        {
            if (currentSlot != null) currentSlot.ClearSlot();

            // A board gear was spawned again in its new slot; a stock shell stays in the tray (its stock dropped).
            if (isBoardGear) Destroy(gameObject);
            else ReturnToOrigin();
            return;
        }

        if (!isBoardGear || GearUnderneath() || IsCloseToOriginal(eventData))
        {
            // The board still holds this gear's id, so snapping back needs no new spawn and no stat change.
            ReturnToOrigin();
            return;
        }

        // Dropped away from the board: back to tray stock.
        if (InventoryManager.instance != null) InventoryManager.instance.Refund(gearNum);
        if (currentSlot != null) currentSlot.ClearSlot();
        StartCoroutine(GearAnimation());
    }

    // Which gear this is and where it came from. Order matters: shop offers and stock shells can also
    // carry a GearBase. Only owned gears have a non-zero GearBase.Id: GearManager stamps board gears,
    // ShopManager stamps level-shop tray gears, which sit under their tray slot.
    private void ResolveIdentity()
    {
        IsShopOffer = false;
        IsTrayGear = false;
        TraySlot = null;
        isBoardGear = false;
        gearNum = 0;

        if (TryGetComponent(out ShopOffer offer))
        {
            IsShopOffer = true;
            gearNum = offer.gearID;
        }
        else if (TryGetComponent(out InventoryGears shell))
        {
            gearNum = shell.gearId;
        }
        else if (TryGetComponent(out GearBase gear) && gear.Id > 0)
        {
            gearNum = gear.Id;
            TraySlot = ParentTraySlot();
            IsTrayGear = TraySlot != null;
            isBoardGear = !IsTrayGear;
        }
    }

    private UIGearSlot ParentTraySlot()
    {
        Transform parent = transform.parent;
        if (parent != null && parent.TryGetComponent(out UIGearSlot slot) && slot.IsTraySlot) return slot;
        return null;
    }

    // The full gearbox slot this board gear sits on, or null.
    private UIGearSlot BoardSlotUnderneath()
    {
        Collider2D[] hits = Physics2D.OverlapPointAll(transform.position);
        foreach (Collider2D hit in hits)
        {
            if (hit.TryGetComponent(out UIGearSlot slot) && slot.isFull && !slot.IsTraySlot) return slot;
        }
        return null;
    }

    // A gear slot under the drop point that can take a gear right now, or null.
    private UIGearSlot FindDropTarget()
    {
        Collider2D[] dropHits = Physics2D.OverlapPointAll(transform.position);
        foreach (Collider2D hit in dropHits)
        {
            if (hit.TryGetComponent(out UIGearSlot slot) && !slot.isFull && slot != currentSlot) return slot;
        }
        return null;
    }

    private bool GearUnderneath()
    {
        Collider2D[] hits = Physics2D.OverlapPointAll(transform.position);
        foreach (Collider2D hit in hits)
        {
            // Ignores this gear, but detects gears below.
            if (hit.gameObject != gameObject && hit.TryGetComponent(out UIDragHandler _)) return true;
        }
        return false;
    }

    private bool IsCloseToOriginal(PointerEventData eventData)
    {
        Vector3 dropWorldPos = InputCamera(eventData).ScreenToWorldPoint(eventData.position);
        dropWorldPos.z = originalPosition.z;
        return Vector2.Distance(originalPosition, dropWorldPos) <= maxReslotDistance;
    }

    // A shop offer is consumed only when it's bought (gearbox in the wave shop, tray in the level shop).
    // Otherwise it returns to its spawn slot, uncharged.
    private void EndShopOfferDrag()
    {
        UIGearSlot targetSlot = FindDropTarget();
        if (targetSlot != null && targetSlot.TryPlaceGear(this))
        {
            GetComponent<ShopOffer>().homeSlot.Clear();
            Destroy(gameObject);
            return;
        }

        ReturnToOrigin();
    }

    // A tray gear goes onto the board, or is sold if dropped away from everything [G3].
    private void EndTrayGearDrag(PointerEventData eventData)
    {
        UIGearSlot targetSlot = FindDropTarget();
        if (targetSlot != null && targetSlot.TryPlaceGear(this))
        {
            Destroy(gameObject);
            return;
        }

        bool awayFromEverything = targetSlot == null && !GearUnderneath() && !IsCloseToOriginal(eventData);
        if (awayFromEverything && ShopManager.instance.TrySell(TraySlot, gearNum))
        {
            StartCoroutine(GearAnimation());
            return;
        }

        ReturnToOrigin();
    }

    private void ReturnToOrigin()
    {
        SetDragLook(false);
        transform.position = originalPosition;
    }

    private void SetDragLook(bool dragging)
    {
        if (spriteRenderer != null)
        {
            Color color = spriteRenderer.color;
            color.a = dragging ? 0.6f : 1.0f;
            spriteRenderer.color = color;
        }

        if (col2D != null)
        {
            col2D.enabled = !dragging;
        }
    }

    IEnumerator GearAnimation()
    {
        if (gearFallArea != null)
        {
            Vector2 targetPos = gearFallArea.transform.position;

            while (Vector2.Distance(gameObject.transform.position, targetPos) > 0.1f)
            {
                gameObject.transform.position = Vector2.MoveTowards(gameObject.transform.position, targetPos, 8f * Time.deltaTime);

                if ((Vector2.Distance(gameObject.transform.position, targetPos) < 1.25f))
                    gameObject.transform.localScale = Vector2.MoveTowards(gameObject.transform.localScale, new Vector2(0, 0), 1.00001f * Time.deltaTime);

                yield return null;
            }
        }
        else
        {
            while (gameObject.transform.localScale != Vector3.zero)
            {
                gameObject.transform.localScale = Vector2.MoveTowards(gameObject.transform.localScale, new Vector2(0, 0), 1.1f * Time.deltaTime);
                yield return null;
            }
        }

        Destroy(gameObject);
    }
}
