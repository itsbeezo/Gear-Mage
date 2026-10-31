using UnityEngine;

// One drop target: a gearbox cell (has a GearBox) or a level-shop tray slot (no GearBox; see ShopManager).
public class UIGearSlot : MonoBehaviour
{
    public bool isFull = false;

    private GearBox box;

    public bool IsTraySlot
    {
        get { return box == null; }
    }

    private void Awake()
    {
        box = GetComponent<GearBox>();
    }

    // False, and nothing is spent or placed, if this slot can't take the dragged gear right now.
    public bool TryPlaceGear(UIDragHandler dragged)
    {
        if (isFull) return false;
        return IsTraySlot ? TryStage(dragged) : TryPlaceOnBoard(dragged);
    }

    // The old slot of a board gear that moved or left the board.
    public void ClearSlot()
    {
        isFull = false;
        if (box != null) GearManager.instance.RemoveGear(box);
    }

    // Tray: optional storage for level-shop offers, bought on drop [R20].
    private bool TryStage(UIDragHandler dragged)
    {
        if (!dragged.IsShopOffer || ShopManager.instance == null) return false;
        if (GameManager.instance.GetState() != GameManager.State.LevelShop) return false;

        return ShopManager.instance.TryBuyIntoTray(this, dragged.gearNum);
    }

    private bool TryPlaceOnBoard(UIDragHandler dragged)
    {
        int id = dragged.gearNum;
        if (!GearManager.instance.CanPlace(box, id)) return false;

        if (dragged.IsShopOffer)
        {
            // Offers from either shop are bought by dropping them into the gearbox.
            if (!CurrencyManager.instance.TryBuyGear(id)) return false;
            TakeAsRunGear(id);
        }
        else if (dragged.IsTrayGear)
        {
            TakeAsRunGear(id);
            ShopManager.instance.Unstage(dragged.TraySlot);
        }
        else if (!dragged.startedOnBoard)
        {
            if (InventoryManager.instance != null && !InventoryManager.instance.TryConsume(id)) return false;
        }

        return GearManager.instance.PlaceGear(box, id);
    }

    // A bought gear is a run gear [R1]: removing it from the board returns it to tray stock for this run.
    private static void TakeAsRunGear(int id)
    {
        if (InventoryManager.instance == null) return;
        InventoryManager.instance.GrantRunGear(id);
        InventoryManager.instance.TryConsume(id);
    }
}
