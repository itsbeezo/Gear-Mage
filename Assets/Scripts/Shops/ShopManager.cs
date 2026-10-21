using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.UI;

// The level shop panel. UIManager.ApplyState opens/closes it; it never touches GameManager's screens [S2].
public class ShopManager : MonoBehaviour
{
    public static ShopManager instance { get; private set; }

    private static readonly Vector3 BoardGearBoxPosition = new Vector3(-2.02f, 2.9f, 11f);
    private static readonly Vector3 ShopGearBoxPosition = new Vector3(-2.02f, 4.31f, 11f);

    public GameObject gearBox;

    [SerializeField] private GameObject shopGroup;

    [SerializeField] private Button gearBoxToggle;

    [SerializeField] private SortingGroup shopGroupLayer;

    [SerializeField] private GameObject returnButton;

    [SerializeField] private GameObject inventoryGears;

    public void Awake()
    {
        if (instance != null && instance != this)
        {
            Destroy(gameObject);
            return;
        }

        instance = this;
        DontDestroyOnLoad(gameObject);
    }

    // Level shop "Return" button (Inspector OnClick). Leaving the shop is a screen change, so GameManager decides.
    public void BackToGame()
    {
        GameManager.instance.LeaveLevelShop();
    }

    // Gearbox toggle button (Inspector OnClick).
    public void ToggleGearbox()
    {
        int oldOrder = shopGroupLayer.sortingOrder;
        int newOrder = (oldOrder > 9) ? 8 : 12;

        shopGroupLayer.sortingOrder = newOrder;
    }

    // Called only by UIManager.ApplyState.
    public void SetShopOpen(bool open)
    {
        if (shopGroup == null) return;

        inventoryGears.SetActive(!open);
        shopGroup.SetActive(open);
        gearBoxToggle.gameObject.SetActive(open);
        returnButton.SetActive(open);
        gearBox.transform.position = open ? ShopGearBoxPosition : BoardGearBoxPosition;
    }
}
