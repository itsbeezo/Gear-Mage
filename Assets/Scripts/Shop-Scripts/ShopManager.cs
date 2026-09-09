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

    [SerializeField] private SortingGroup gearBoxGroupToggle;

    [SerializeField] private GameObject returnButton;

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
            shopGroup.SetActive(false);
            gearBoxToggle.gameObject.SetActive(false);
            contButton.gameObject.SetActive(true);
            returnButton.gameObject.SetActive(false);
        }
    }

    public void ToggleGearbox()
    {
        if (gearBoxGroupToggle.sortingOrder < 10)
        {
            gearBoxGroupToggle.sortingOrder = 10;
        }
        else
        {
            gearBoxGroupToggle.sortingOrder = -9;
        }
    }

    public void OpenShop()
    {
        if (shopGroup != null)
        {
            shopGroup.SetActive(true);
            gearBoxToggle.gameObject.SetActive(true);
            returnButton.gameObject.SetActive(true);
        }
    }

}
