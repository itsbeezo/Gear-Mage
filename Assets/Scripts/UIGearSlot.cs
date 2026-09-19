using UnityEngine;

public class UIGearSlot : MonoBehaviour
{
    public bool isFull = false;
    public bool isStagingSlot = false;

    public bool TryPlaceGear(GameObject draggedObject, UIDragHandler uiDragHandler)
    {
        if (isFull) return false;

        if (!uiDragHandler.startedOnBoard)
        {
            if (InventoryManager.instance != null &&
                !InventoryManager.instance.TryConsume(uiDragHandler.gearNum))
            {
                return false;
            }
        }

        isFull = true;

        if (isStagingSlot)
        {
            GameObject stagedCopy = Instantiate(draggedObject, transform.position, transform.rotation);
            stagedCopy.tag = "Gears";
            stagedCopy.transform.SetParent(transform, true);

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
        }
        else
        {
            GearBox thisGearBox = gameObject.GetComponent<GearBox>();
            GearManager.instance.SetGear(thisGearBox.GetXIndex(), thisGearBox.GetYIndex(), uiDragHandler.gearNum);
            GearManager.instance.SpawnSingleGear(thisGearBox.GetXIndex(), thisGearBox.GetYIndex(), uiDragHandler.gearNum);
        }

        return true;
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
