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

    public int gearNum = 0;
    private UIGearSlot currentSlot;
    public bool isConnected = false;
    public bool attemptButFull = false;

    private GameObject gearFallArea;

    private void Awake()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        col2D = GetComponent<Collider2D>();
        mainCamera = Camera.main;
        gearFallArea = GameObject.Find("GearFallArea");
        GearBoxCamera = GameObject.Find("GearBoxCamera").GetComponent<Camera>();
    }

    public void OnBeginDrag(PointerEventData eventData)
    {
        originalPosition = transform.position;
        originalParent = transform.parent;

        Vector3 mouseWorldPos = GearBoxCamera.ScreenToWorldPoint(eventData.position);
        mouseWorldPos.z = 0f;
        offset = transform.position - mouseWorldPos;

        Collider2D[] hits = Physics2D.OverlapPointAll(transform.position);
        foreach (Collider2D hit in hits)
        {
            UIGearSlot slot = hit.GetComponent<UIGearSlot>();
            if (slot != null)
            {
                currentSlot = slot;
                break;
            }
        }

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

        if (gameObject.CompareTag("Drag1"))
        {
            gearNum = 1;
        } 
        else if (gameObject.CompareTag("Archer"))
        {
            gearNum = 2;
        } 
        else if (gameObject.CompareTag("Drag3"))
        {
            gearNum = 3;
        }
        else if (gameObject.CompareTag("Drag4"))
        {
            gearNum = 4;
        }
        else if (gameObject.CompareTag("Drag5"))
        {
            gearNum = 5;
        }
        else if (gameObject.CompareTag("Drag6"))
        {
            gearNum = 6;
        }
        else if (gameObject.CompareTag("Melee"))
        {
            gearNum = 7;
        }
        else if (gameObject.CompareTag("Drag8"))
        {
            gearNum = 8;
        }
        else if (gameObject.CompareTag("Tank"))
        {
            gearNum = 9;
        }
        else if (gameObject.CompareTag("Drag10"))
        {
            gearNum = 10;
        }
        else if (gameObject.CompareTag("Drag11"))
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
            else if (gameObject.name.Contains("GBooster"))
            {
                gearNum = 3;
            }
            else if (gameObject.name.Contains("2x"))
            {
                gearNum = 4;
            }
            else if (gameObject.name.Contains("4x"))
            {
                gearNum = 6;
            }
            else if (gameObject.name.Contains("8x"))
            {
                gearNum = 5;
            }
            else if (gameObject.name.Contains("HP"))
            {
                gearNum = 12;
            }
            else if (gameObject.name.Contains("Melee"))
            {
                gearNum = 7;
            }
            else if (gameObject.name.Contains("Tank"))
            {
                gearNum = 9;
            }
            else if (gameObject.name.Contains("Speed"))
            {
                gearNum = 11;
            }
            else if (gameObject.name.Contains("Archer"))
            {
                gearNum = 2;
            }

                
        }
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        bool gearUnderneath = false;
        Collider2D[] hits = Physics2D.OverlapPointAll(transform.position);

        foreach (Collider2D hit in hits)
        {
            // Ignores this dragged object, but detects ANY other gear with a UIDragHandler
            if (hit.gameObject != gameObject && hit.GetComponent<UIDragHandler>() != null)
            {
                gearUnderneath = true;
                break;
            }
        }

        if ((gameObject.CompareTag("Gears") || gameObject.CompareTag("Clicker")) && isConnected)
        {
            currentSlot.ClearSlot();
            Destroy(gameObject);
            Debug.Log("reached " + isConnected);
        }
        else if ((gameObject.CompareTag("Gears") || gameObject.CompareTag("Clicker")) && !isConnected)
        {
            if (gearUnderneath)
            {
                StartCoroutine(ReSlot());
            }
            else
            {
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

        // 1. Re-occupy currentSlot and spawn gear back at original location
        GearBox box = currentSlot.GetComponent<GearBox>();
        if (box != null)
        {
            currentSlot.isFull = true;
            GearManager.instance.SetGear(box.GetXIndex(), box.GetYIndex(), gearNum);
            GearManager.instance.SpawnSingleGear(box.GetXIndex(), box.GetYIndex(), gearNum);
        }

        yield return null;

        // 2. Destroy the dragged temporary object
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
        else if (gearFallArea == null)
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
