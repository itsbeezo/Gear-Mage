using UnityEngine;
using UnityEngine.EventSystems;

public class UIGearSlot : MonoBehaviour, IDropHandler
{
    public void OnDrop(PointerEventData eventData)
    {
        if (eventData.pointerDrag != null)
        {
            GameObject draggedObject = eventData.pointerDrag;

            UIDragHandler uiDragHandler = draggedObject.GetComponent<UIDragHandler>();

            GearBox thisGearBox = gameObject.GetComponent<GearBox>();

            GearManager.instance.SetGear(thisGearBox.GetXIndex(), thisGearBox.GetYIndex(), uiDragHandler.gearNum);

            GearManager.instance.SpawnSingleGear(thisGearBox.GetXIndex(), thisGearBox.GetYIndex(), uiDragHandler.gearNum); 
        }
    }

}
