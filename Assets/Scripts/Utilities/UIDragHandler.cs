using System.Collections;
using UnityEngine;
using UnityEngine.EventSystems;

public class UIDragHandler : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler
{
    private SpriteRenderer spriteRenderer;
    private Collider2D col2D;
    private Vector3 originalPosition;
    private Transform originalParent;
    private Camera mainCamera;
    private Vector3 offset;
    private Camera GearBoxCamera;

    [Header("Drop Settings")]
    [Tooltip("Maximum allowed distance from originalPosition to trigger ReSlot.")]
    [SerializeField] private float maxReslotDistance = 1.5f;

    // Resolved once per drag in OnBeginDrag [G1]; UIGearSlot reads it on drop.
    public int gearNum { get; private set; }
    public bool IsShopOffer { get; private set; }
    private bool isBoardGear;
    private UIGearSlot currentSlot;
    public bool attemptButFull = false;
    public bool startedOnBoard = false;

    // The camera that actually received the pointer, so drags track the cursor
    // in whichever view the gear was clicked in (level or gear box).
    private Camera InputCamera(PointerEventData eventData)
    {
        return eventData.pressEventCamera != null ? eventData.pressEventCamera : GearBoxCamera;
    }

    private GameObject gearFallArea;

    private void Awake()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        col2D = GetComponent<Collider2D>();
        mainCamera = Camera.main;
        gearFallArea = GameObject.Find("GearFallArea");

        GameObject cameraObj = GameObject.Find("GearBoxCamera");
        if (cameraObj != null)
        {
            GearBoxCamera = cameraObj.GetComponent<Camera>();
        }
    }

    // Which gear this is and where it came from. Order matters: shop offers and shells can also
    // carry a GearBase, but only GearManager stamps a non-zero id onto board gears.
    private void ResolveIdentity()
    {
        if (TryGetComponent(out ShopOffer offer))
        {
            IsShopOffer = true;
            isBoardGear = false;
            gearNum = offer.gearID;
            return;
        }

        IsShopOffer = false;

        if (TryGetComponent(out InventoryGears shell))
        {
            isBoardGear = false;
            gearNum = shell.gearId;
            return;
        }

        if (TryGetComponent(out GearBase gear) && gear.Id > 0)
        {
            isBoardGear = true;
            gearNum = gear.Id;
            return;
        }

        isBoardGear = false;
        gearNum = 0;
    }

    public void OnBeginDrag(PointerEventData eventData)
    {
        ResolveIdentity();
        originalPosition = transform.position;
        originalParent = transform.parent;

        currentSlot = null;

        Vector3 mouseWorldPos = InputCamera(eventData).ScreenToWorldPoint(eventData.position);
        mouseWorldPos.z = 0f;
        offset = transform.position - mouseWorldPos;

        Collider2D[] hits = Physics2D.OverlapPointAll(transform.position);
        foreach (Collider2D hit in hits)
        {
            UIGearSlot slot = hit.GetComponent<UIGearSlot>();
            if (slot != null && slot.isFull && isBoardGear)
            {
                currentSlot = slot;
                break;
            }
        }

        startedOnBoard = (currentSlot != null);

        if (spriteRenderer != null)
        {
            Color color = spriteRenderer.color;
            color.a = 0.6f;
            spriteRenderer.color = color;
        }

        if (col2D != null)
        {
            col2D.enabled = false;
        }
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

        UIGearSlot targetSlot = FindDropTarget();

        if (targetSlot != null && targetSlot.TryPlaceGear(gameObject, this))
        {
            if (currentSlot != null)
            {
                currentSlot.ClearSlot();
            }

            if (isBoardGear)
            {
                Destroy(gameObject);
            }
            else
            {
                if (spriteRenderer != null)
                {
                    Color color = spriteRenderer.color;
                    color.a = 1.0f;
                    spriteRenderer.color = color;
                }

                if (col2D != null)
                {
                    col2D.enabled = true;
                }

                transform.position = originalPosition;
            }

            return;
        }

        bool gearUnderneath = false;
        Collider2D[] hits = Physics2D.OverlapPointAll(transform.position);

        foreach (Collider2D hit in hits)
        {
            // Ignores this gear, but detects gears below
            if (hit.gameObject != gameObject && hit.GetComponent<UIDragHandler>() != null)
            {
                gearUnderneath = true;
                break;
            }
        }

        // Calculate distance from drop location to original position in World Space
        Vector3 dropWorldPos = InputCamera(eventData).ScreenToWorldPoint(eventData.position);
        dropWorldPos.z = originalPosition.z;
        bool isCloseToOriginal = Vector2.Distance(originalPosition, dropWorldPos) <= maxReslotDistance;

        if (isBoardGear)
        {
            // Triggers ReSlot if dropped over another gear OR dropped close to its starting point
            if (gearUnderneath || isCloseToOriginal)
            {
                StartCoroutine(ReSlot());
            }
            else
            {
                if (InventoryManager.instance != null)
                {
                    InventoryManager.instance.Refund(gearNum);
                }

                if (currentSlot != null)
                {
                    currentSlot.ClearSlot();
                }

                StartCoroutine(GearAnimation());
            }
        }
        else
        {
            if (spriteRenderer != null)
            {
                Color color = spriteRenderer.color;
                color.a = 1.0f;
                spriteRenderer.color = color;
            }

            if (col2D != null)
            {
                col2D.enabled = true;
            }

            transform.position = originalPosition;
        }
    }

    // A gear slot under the drop point that can take a gear right now, or null.
    private UIGearSlot FindDropTarget()
    {
        Collider2D[] dropHits = Physics2D.OverlapPointAll(transform.position);
        foreach (Collider2D hit in dropHits)
        {
            UIGearSlot slot = hit.GetComponent<UIGearSlot>();
            if (slot != null && !slot.isFull && slot != currentSlot)
            {
                return slot;
            }
        }
        return null;
    }

    // A shop offer is consumed only when it's bought into the gearbox. Otherwise it
    // returns to its spawn slot, uncharged. It is never refunded to stock or destroyed.
    private void EndShopOfferDrag()
    {
        UIGearSlot targetSlot = FindDropTarget();

        if (targetSlot != null && targetSlot.TryPlaceGear(gameObject, this))
        {
            GetComponent<ShopOffer>().homeSlot.Clear();
            Destroy(gameObject);
            return;
        }

        ReturnToOrigin();
    }

    private void ReturnToOrigin()
    {
        if (spriteRenderer != null)
        {
            Color color = spriteRenderer.color;
            color.a = 1.0f;
            spriteRenderer.color = color;
        }

        if (col2D != null)
        {
            col2D.enabled = true;
        }

        transform.position = originalPosition;
    }

    public IEnumerator ReSlot()
    {
        yield return null;

        if (currentSlot != null)
        {
            if (currentSlot.isStagingSlot)
            {
                GameObject stagedCopy = Instantiate(gameObject, currentSlot.transform.position, currentSlot.transform.rotation);
                stagedCopy.tag = "Gears";
                stagedCopy.transform.SetParent(currentSlot.transform, true);

                SpriteRenderer copySprite = stagedCopy.GetComponent<SpriteRenderer>();
                if (copySprite != null)
                {
                    Color color = copySprite.color;
                    color.a = 1.0f;
                    copySprite.color = color;
                }

                Collider2D copyCollider = stagedCopy.GetComponent<Collider2D>();
                if (copyCollider != null)
                {
                    copyCollider.enabled = true;
                }

                InventoryGears copyInventoryGears = stagedCopy.GetComponent<InventoryGears>();
                if (copyInventoryGears != null)
                {
                    copyInventoryGears.suppressVisibilityControl = true;
                }

                currentSlot.isFull = true;
            }
            else
            {
                GearBox box = currentSlot.GetComponent<GearBox>();
                if (box != null)
                {
                    currentSlot.isFull = true;
                    GearManager.instance.SetGear(box.GetXIndex(), box.GetYIndex(), gearNum);
                    GearManager.instance.SpawnSingleGear(box.GetXIndex(), box.GetYIndex(), gearNum);
                }
            }
        }

        yield return null;

        Destroy(gameObject);
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
