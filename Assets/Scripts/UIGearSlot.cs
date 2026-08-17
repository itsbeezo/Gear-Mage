using UnityEngine;
using UnityEngine.EventSystems;

public class UIGearSlot : MonoBehaviour, IDropHandler
{
    public void OnDrop(PointerEventData eventData)
    {
        if (eventData.pointerDrag != null)
        {
            GameObject draggedObject = eventData.pointerDrag;

            //This is a temporary solution, I plan on changing the logic here shortly. 
            draggedObject.transform.SetParent(transform);
            draggedObject.GetComponent<RectTransform>().anchoredPosition = Vector2.zero;
        }
    }

}
