using System.Runtime.InteropServices;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class UIManager : MonoBehaviour
{
    public static UIManager instance { get; private set; }
    //List of UI Elements
    [SerializeField] private Button startButton;
    [SerializeField] private Button upgradeButton;
    [SerializeField] private Button playButton;
    [SerializeField] private Button pShopButton;
    [SerializeField] private GameObject upgradeMenuGroup;
    [SerializeField] private GameObject pShopGroup;
    [SerializeField] private GameObject waveShopGroup;
    [SerializeField] private Button nextWaveButton;
    [SerializeField] private GameObject levelVictoryGroup;
    [SerializeField] private Button restartButton;
    //Camera Variables (temp?)
    public Camera LevelCamera;
    public Camera GearBoxCamera;
    public float smoothspeed;
    private Vector3 targetPos, newpos;
    public Vector3 minPos, maxPos;

    [SerializeField] private GameObject GearMage;

    private void Start()
    {
        instance = this;
        //Camera Stuff (temp?)
        LevelCamera.enabled = false;
        GearBoxCamera.enabled = true;

        nextWaveButton.onClick.AddListener(OnNextWavePressed);
    }
    private void FixedUpdate()
    {
        if(GameManager.instance.GetState() == GameManager.State.BeforeStart)
        {
            startButton.onClick.RemoveAllListeners();
            startButton.onClick.AddListener(GameManager.instance.SetStateNormal);
            startButton.onClick.AddListener(SwitchCamera);
            // startButton.onClick.AddListener(SpawnMage);

            upgradeButton.onClick.RemoveAllListeners();
            upgradeButton.onClick.AddListener(GameManager.instance.SetStateUpgradeMenu);

            playButton.onClick.RemoveAllListeners();
            playButton.onClick.AddListener(GameManager.instance.SetStateBeforeStart);

            pShopButton.onClick.RemoveAllListeners();
            pShopButton.onClick.AddListener(GameManager.instance.SetStatePShopMenu);

            HideUIElement(pShopGroup);
            HideUIElement(upgradeMenuGroup);
        }

        if(GameManager.instance.GetState() == GameManager.State.Normal)
        {
            HideUIElement(startButton.gameObject);
            HideUIElement(upgradeButton.gameObject);
            HideUIElement(playButton.gameObject);
            HideUIElement(pShopButton.gameObject);
            HideUIElement(waveShopGroup);
            HideUIElement(levelVictoryGroup);

            GearBoxCamera.GetComponent<Physics2DRaycaster>().enabled = false;
            Debug.Log("GearBox Camera Raycase off");
            LevelCamera.GetComponent<Physics2DRaycaster>().enabled = false;
            Debug.Log("Level Camera Raycast off");

            UnitManager.instance.totalUnitsalive = 0;
            Debug.Log("units spawned reset");
        }   

        if(GameManager.instance.GetState() == GameManager.State.PShopMenu)
        {
            ShowUIElement(pShopGroup);
            HideUIElement(upgradeMenuGroup);
        }

        if(GameManager.instance.GetState() == GameManager.State.UpgradeMenu)
        {
            ShowUIElement(upgradeMenuGroup);
            HideUIElement(pShopGroup);
        } 

        if(GameManager.instance.GetState() == GameManager.State.WaveVictory)
        {
            ShowUIElement(waveShopGroup);
            GearBoxCamera.GetComponent<Physics2DRaycaster>().enabled = false;
            Debug.Log("GearBox Camera Raycase off");
            LevelCamera.GetComponent<Physics2DRaycaster>().enabled = true;
            Debug.Log("Level Camera Raycast on");
            
            nextWaveButton.onClick.RemoveAllListeners();
            nextWaveButton.onClick.AddListener(GameManager.instance.SetStateNormal);
            nextWaveButton.onClick.AddListener(UnitManager.instance.OnNextWave);

        }

        if(GameManager.instance.GetState() == GameManager.State.EndGame)
        {
            ShowUIElement(levelVictoryGroup);

            restartButton.onClick.RemoveAllListeners();
            restartButton.onClick.AddListener(GameManager.instance.ResetScene);
        }
    }
    private void OnNextWavePressed()
    {
        Debug.Log("Next wave started");
        GameManager.instance.SetStateNormal();
        UnitManager.instance.OnNextWave();
    }

    public void ShowUIElement(GameObject elementToShow)
    {
        elementToShow.SetActive(true);
    }
    public void HideUIElement(GameObject elementToHide)
    {
        elementToHide.SetActive(false);
    }
    //Camera Functions (temp?) || I think this is helpful and shouldn't be removed for now - Antonio
    public void SwitchCamera()
    {
        if (LevelCamera.enabled == true)
        {
            LevelCamera.enabled = false;
            GearBoxCamera.enabled = true;
        }
        else if (GearBoxCamera.enabled == true)
        {
            GearBoxCamera.enabled = false;
            LevelCamera.enabled = true;
        }
    }

    // private void SpawnMage()
    // {
    //     GearMage.enabled = true;
    // }
}