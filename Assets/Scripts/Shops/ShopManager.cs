using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.UI;

public class ShopManager : MonoBehaviour
{
    public static ShopManager instance { get; private set; }

    public GameObject gearBox;

    [SerializeField] private GameObject shopGroup;

    [SerializeField] private Button gearBoxToggle;

    [SerializeField] private SortingGroup shopGroupLayer;

    [SerializeField] private GameObject returnButton;

    [SerializeField] private GameObject inventoryGears;
    // Update is called once per frame
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

    public void BackToGame()
    {
        if (shopGroup != null)
        {
            inventoryGears.SetActive(true);// Might never make this true again after first placement
            shopGroup.SetActive(false);
            gearBoxToggle.gameObject.SetActive(false);
            returnButton.gameObject.SetActive(false);
            gearBox.transform.position = new Vector3(-2.02f, 2.9f, 11);
        }
    }

    public void ToggleGearbox()
    {
        int oldOrder = shopGroupLayer.sortingOrder;
        int newOrder = (oldOrder > 9) ? 8 : 12;

        shopGroupLayer.sortingOrder = newOrder;
    }

    public void OpenShop()
    {
        if (shopGroup != null)
        {
            inventoryGears.SetActive(false);
            shopGroup.SetActive(true);
            gearBoxToggle.gameObject.SetActive(true);
            returnButton.gameObject.SetActive(true);
            gearBox.transform.position = new Vector3(-2.02f, 4.31f,11);
        }
    }

}
