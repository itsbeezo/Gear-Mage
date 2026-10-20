using UnityEngine;
using UnityEngine.EventSystems;

public class UIManager : MonoBehaviour
{
    public static UIManager instance { get; private set; }
    //Screens shown/hidden per GameManager.State
    [Header("Before Start")]
    [SerializeField] private GameObject startButton;
    [SerializeField] private GameObject upgradeButton;
    [SerializeField] private GameObject playButton;
    [SerializeField] private GameObject pShopButton;
    [SerializeField] private GameObject upgradeMenuGroup;
    [SerializeField] private GameObject pShopGroup;
    [Header("Between Waves / Levels")]
    [SerializeField] private GameObject waveShopGroup;
    [SerializeField] private GameObject levelVictoryGroup;
    [SerializeField] private GameObject victoryContinueButton;
    [Header("Cameras")]
    public Camera LevelCamera;
    public Camera GearBoxCamera;

    private Physics2DRaycaster levelRaycaster;
    private Physics2DRaycaster gearBoxRaycaster;

    private void Awake()
    {
        instance = this;
        levelRaycaster = LevelCamera.GetComponent<Physics2DRaycaster>();
        gearBoxRaycaster = GearBoxCamera.GetComponent<Physics2DRaycaster>();
    }
    private void Start()
    {
        ApplyState(GameManager.instance.GetState());

    }

    // Called by GameManager.SetState; every state sets every screen explicitly so nothing depends on the previous one
    public void ApplyState(GameManager.State state)
    {
        bool inLobby = state == GameManager.State.BeforeStart
            || state == GameManager.State.UpgradeMenu
            || state == GameManager.State.PShopMenu;

        startButton.SetActive(inLobby);
        upgradeButton.SetActive(inLobby);
        playButton.SetActive(inLobby);
        pShopButton.SetActive(inLobby);
        upgradeMenuGroup.SetActive(state == GameManager.State.UpgradeMenu);
        pShopGroup.SetActive(state == GameManager.State.PShopMenu);
        waveShopGroup.SetActive(state == GameManager.State.WaveVictory);
        levelVictoryGroup.SetActive(state == GameManager.State.EndGame);
        victoryContinueButton.SetActive(state == GameManager.State.EndGame);

        // Lobby looks at the gearbox; everything after Start looks at the level
        LevelCamera.enabled = !inLobby;
        GearBoxCamera.enabled = inLobby;

        // Gears can be dragged in the lobby (gearbox) and in the wave shop (level); not during play or on the victory screen
        gearBoxRaycaster.enabled = inLobby;
        levelRaycaster.enabled = state == GameManager.State.WaveVictory;
    }

    // Victory screen's Continue button (Inspector OnClick)
    public void OpenShop()
    {
        ShopManager.instance.OpenShop();
        victoryContinueButton.SetActive(false);
        LevelCamera.enabled = false;
        GearBoxCamera.enabled = true;
    }

    public void SwitchCamera()
    {
        if (LevelCamera.enabled == true)
        {
            
        }
    }

}
