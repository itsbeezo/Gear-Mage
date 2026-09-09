using UnityEngine;
using UnityEngine.EventSystems;

public class UIGearSlot : MonoBehaviour, IDropHandler
{
    //public PointerEventData LastPointerData;
    //public UIDragHandler lastGear;
    public bool isFull = false;
    public void OnDrop(PointerEventData eventData)
    {
        if (eventData.pointerDrag != null && !isFull)
        {
            //LastPointerData = eventData;

            isFull = true;

            GameObject draggedObject = eventData.pointerDrag;

            UIDragHandler uiDragHandler = draggedObject.GetComponent<UIDragHandler>();

            //lastGear = uiDragHandler;

            uiDragHandler.isConnected = true;
            Debug.Log(uiDragHandler.isConnected);

            GearBox thisGearBox = gameObject.GetComponent<GearBox>();

            GearManager.instance.SetGear(thisGearBox.GetXIndex(), thisGearBox.GetYIndex(), uiDragHandler.gearNum);

            GearManager.instance.SpawnSingleGear(thisGearBox.GetXIndex(), thisGearBox.GetYIndex(), uiDragHandler.gearNum); 
        } 
    }

    //public void OnDrop2(PointerEventData eventData, UIDragHandler uiDragHandler)
    //{
    //    if (eventData.pointerDrag != null && !isFull)
    //    {
    //        isFull = true;

    //        uiDragHandler.isConnected = true;
    //        Debug.Log(uiDragHandler.isConnected);

    //        GearBox thisGearBox = gameObject.GetComponent<GearBox>();

    //        GearManager.instance.SetGear(thisGearBox.GetXIndex(), thisGearBox.GetYIndex(), uiDragHandler.gearNum);

    //        GearManager.instance.SpawnSingleGear(thisGearBox.GetXIndex(), thisGearBox.GetYIndex(), uiDragHandler.gearNum);
    //    }
    //}

    //public void PassEventData()
    //{
    //    OnDrop2(LastPointerData, lastGear);
    //}

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
