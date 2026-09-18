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

    public int gearNum = 0;
    private UIGearSlot currentSlot;
    public bool isConnected = false;
    public bool attemptButFull = false;
    public bool startedOnBoard = false;

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

    public void OnBeginDrag(PointerEventData eventData)
    {
        originalPosition = transform.position;
        originalParent = transform.parent;

        currentSlot = null;

        Vector3 mouseWorldPos = GearBoxCamera.ScreenToWorldPoint(eventData.position);
        mouseWorldPos.z = 0f;
        offset = transform.position - mouseWorldPos;

        Collider2D[] hits = Physics2D.OverlapPointAll(transform.position);
        foreach (Collider2D hit in hits)
        {
            UIGearSlot slot = hit.GetComponent<UIGearSlot>();
            if (slot != null && slot.isFull)
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
        Vector3 mouseWorldPos = GearBoxCamera.ScreenToWorldPoint(eventData.position);
        mouseWorldPos.z = 0f;
        transform.position = mouseWorldPos + offset;

        if (gameObject.CompareTag("ClickerShell"))
        {
            gearNum = 1;
        }
        else if (gameObject.CompareTag("Melee"))
        {
            gearNum = 2;
        }
        else if (gameObject.CompareTag("Archer"))
        {
            gearNum = 3;
        }
        else if (gameObject.CompareTag("Tank"))
        {
            gearNum = 4;
        }
        else if (gameObject.CompareTag("Booster1"))
        {
            gearNum = 5;
        }
        else if (gameObject.CompareTag("Booster2"))
        {
            gearNum = 6;
        }
        else if (gameObject.CompareTag("Booster4"))
        {
            gearNum = 7;
        }
        else if (gameObject.CompareTag("Booster8"))
        {
            gearNum = 8;
        }
        else if (gameObject.CompareTag("Hp"))
        {
            gearNum = 9;
        }
        else if (gameObject.CompareTag("Attack"))
        {
            gearNum = 10;
        }
        else if (gameObject.CompareTag("AttackSpeed"))
        {
            gearNum = 11;
        }
        else if (gameObject.CompareTag("Gears") || gameObject.CompareTag("Clicker"))
        {
            if (gameObject.name.Contains("Clicker"))
            {
                gearNum = 1;
            }
            else if (gameObject.name.Contains("GAttack"))
            {
                gearNum = 10;
            }
            else if (gameObject.name.Contains("1x"))
            {
                gearNum = 5;
            }
            else if (gameObject.name.Contains("2x"))
            {
                gearNum = 6;
            }
            else if (gameObject.name.Contains("4x"))
            {
                gearNum = 7;
            }
            else if (gameObject.name.Contains("8x"))
            {
                gearNum = 8;
            }
            else if (gameObject.name.Contains("HP"))
            {
                gearNum = 9;
            }
            else if (gameObject.name.Contains("Melee"))
            {
                gearNum = 2;
            }
            else if (gameObject.name.Contains("Tank"))
            {
                gearNum = 4;
            }
            else if (gameObject.name.Contains("Speed"))
            {
                gearNum = 11;
            }
            else if (gameObject.name.Contains("Archer"))
            {
                gearNum = 3;
            }
        }
    }

    public void OnEndDrag(PointerEventData eventData)
    {
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
        Vector3 dropWorldPos = GearBoxCamera.ScreenToWorldPoint(eventData.position);
        dropWorldPos.z = originalPosition.z;
        bool isCloseToOriginal = Vector2.Distance(originalPosition, dropWorldPos) <= maxReslotDistance;

        if ((gameObject.CompareTag("Gears") || gameObject.CompareTag("Clicker")) && isConnected)
        {
            currentSlot.ClearSlot();
            Destroy(gameObject);
            Debug.Log("reached " + isConnected);
        }
        else if ((gameObject.CompareTag("Gears") || gameObject.CompareTag("Clicker")) && !isConnected)
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

                currentSlot.ClearSlot();
                StartCoroutine(GearAnimation());
                Debug.Log("reachedfalseoutcome " + isConnected);
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

    public IEnumerator ReSlot()
    {
        yield return null;

        if (currentSlot != null)
        {
            if (currentSlot.isStagingSlot)
            {
                GameObject stagedCopy = Instantiate(gameObject, currentSlot.transform.position, currentSlot.transform.rotation, currentSlot.transform);
                stagedCopy.tag = "Gears";
                stagedCopy.transform.localPosition = Vector3.zero;

                UIDragHandler copyHandler = stagedCopy.GetComponent<UIDragHandler>();
                if (copyHandler != null)
                {
                    copyHandler.isConnected = false;
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
