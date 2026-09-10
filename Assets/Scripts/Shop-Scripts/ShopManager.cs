using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class ShopManager : MonoBehaviour
{
    [SerializeField] private Button contButton;

    [SerializeField] private GameObject gearBox;

    [SerializeField] private GameObject shopGroup;

    [SerializeField] private Button gearBoxToggle;

    [SerializeField] private SortingGroup shopGroupLayer;

    [SerializeField] private GameObject returnButton;

    [SerializeField] private GameObject inventoryGears;

    private CameraTrans cameraScript;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        cameraScript = GameObject.Find("LevelCamera").GetComponent<CameraTrans>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void BackToGame()
    {
        if (shopGroup != null)
        {
            inventoryGears.SetActive(true);// Might never make this true again after first placement
            shopGroup.SetActive(false);
            gearBoxToggle.gameObject.SetActive(false);
            contButton.gameObject.SetActive(true);
            returnButton.gameObject.SetActive(false);
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
        }
    }

}
