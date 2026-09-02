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

    public int gearNum = 0;
    private UIGearSlot currentSlot;
    public bool isConnected = false;

    private GameObject gearFallArea;

    private void Awake()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        col2D = GetComponent<Collider2D>();
        mainCamera = Camera.main;
        gearFallArea = GameObject.Find("GearFallArea");
    }

    public void OnBeginDrag(PointerEventData eventData)
    {
        originalPosition = transform.position;
        originalParent = transform.parent;

        Vector3 mouseWorldPos = mainCamera.ScreenToWorldPoint(eventData.position);
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
        Vector3 mouseWorldPos = mainCamera.ScreenToWorldPoint(eventData.position);
        mouseWorldPos.z = 0f;
        transform.position = mouseWorldPos + offset;

        if (gameObject.CompareTag("Drag1"))
        {
            gearNum = 1;
        } 
        else if (gameObject.CompareTag("Drag2"))
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
        else if (gameObject.CompareTag("Archer"))
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
                gearNum = 9;
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
                gearNum = 5;
            }
            else if (gameObject.name.Contains("8x"))
            {
                gearNum = 6;
            }
            else if (gameObject.name.Contains("HP"))
            {
                gearNum = 11;
            }
            else if (gameObject.name.Contains("Melee"))
            {
                gearNum = 7;
            }
            else if (gameObject.name.Contains("Tank"))
            {
                gearNum = 8;
            }
            else if (gameObject.name.Contains("Speed"))
            {
                gearNum = 10;
            }

            currentSlot.ClearSlot();
        }
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        if ((gameObject.CompareTag("Gears") || gameObject.CompareTag("Clicker")) && isConnected)
        {
            Destroy(gameObject);
            Debug.Log("reached " + isConnected);
        }
        else if ((gameObject.CompareTag("Gears") || gameObject.CompareTag("Clicker")) && !isConnected)
        {
            StartCoroutine(GearAnimation());
            Debug.Log("reachedfalseoutcome " + isConnected);
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

        // if (GearRotate.instance != null)
        // {
        //     GearRotate.instance.GetRotationSpeed(gearNum);
        // }
        // else
        // {
        //     Debug.LogWarning("GearRotate.instance is null; cannot call GetRotationSpeed");
        // }

            transform.position = originalPosition;
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
