using UnityEngine;
using UnityEngine.EventSystems;

public class UIGearSlot : MonoBehaviour, IDropHandler
{
    public bool isFull = false;
    public bool isStagingSlot = false;

    public void OnDrop(PointerEventData eventData)
    {
        if (eventData.pointerDrag == null || isFull) return;

        GameObject draggedObject = eventData.pointerDrag;
        UIDragHandler uiDragHandler = draggedObject.GetComponent<UIDragHandler>();

        if (!uiDragHandler.startedOnBoard)
        {
            if (InventoryManager.instance != null &&
                !InventoryManager.instance.TryConsume(uiDragHandler.gearNum))
            {
                return;
            }
        }

        isFull = true;
        uiDragHandler.isConnected = true;

        if (isStagingSlot)
        {
            GameObject stagedCopy = Instantiate(draggedObject, transform.position, transform.rotation, transform);
            stagedCopy.tag = "Gears";
            stagedCopy.transform.localPosition = Vector3.zero;

            UIDragHandler copyHandler = stagedCopy.GetComponent<UIDragHandler>();
            if (copyHandler != null)
            {
                copyHandler.isConnected = false;
            }

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
            return;
        }

        GearBox thisGearBox = gameObject.GetComponent<GearBox>();
        GearManager.instance.SetGear(thisGearBox.GetXIndex(), thisGearBox.GetYIndex(), uiDragHandler.gearNum);
        GearManager.instance.SpawnSingleGear(thisGearBox.GetXIndex(), thisGearBox.GetYIndex(), uiDragHandler.gearNum);
    }

    public void ClearSlot()
    {
        isFull = false;

        if (isStagingSlot) return;

        GearBox thisGearBox = GetComponent<GearBox>();
        if (thisGearBox != null)
        {
            GearManager.instance.SetGear(thisGearBox.GetXIndex(), thisGearBox.GetYIndex(), 0);
        }
    }
}
