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
                // Out of stock - reject the drop. isConnected stays false,
                // so OnEndDrag's existing catch-all branch snaps the dragged
                // object back to its original position - no new bounce-back
                // code needed here.
                return;
            }
        }

        isFull = true;
        uiDragHandler.isConnected = true;

        if (isStagingSlot)
        {
            // Staging never touches the board's gear grid - just park the
            // actual Shell object here so it can be dragged again later.
            uiDragHandler.landedInStagingSlot = true;
            draggedObject.transform.SetParent(transform);
            draggedObject.transform.localPosition = Vector3.zero;
            return;
        }

        uiDragHandler.landedInStagingSlot = false;

        GearBox thisGearBox = gameObject.GetComponent<GearBox>();
        GearManager.instance.SetGear(thisGearBox.GetXIndex(), thisGearBox.GetYIndex(), uiDragHandler.gearNum);
        GearManager.instance.SpawnSingleGear(thisGearBox.GetXIndex(), thisGearBox.GetYIndex(), uiDragHandler.gearNum);
    }

    public void ClearSlot()
    {
        isFull = false;

        GearBox thisGearBox = GetComponent<GearBox>();
        if (thisGearBox != null)
        {
            GearManager.instance.SetGear(thisGearBox.GetXIndex(), thisGearBox.GetYIndex(), 0);
        }
    }

}
