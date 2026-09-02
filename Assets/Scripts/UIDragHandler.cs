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

    private void Awake()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        col2D = GetComponent<Collider2D>();
        mainCamera = Camera.main;
    }

    public void OnBeginDrag(PointerEventData eventData)
    {
        originalPosition = transform.position;
        originalParent = transform.parent;

        Vector3 mouseWorldPos = mainCamera.ScreenToWorldPoint(eventData.position);
        mouseWorldPos.z = 0f;
        offset = transform.position - mouseWorldPos;

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
    }

    public void OnEndDrag(PointerEventData eventData)
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
