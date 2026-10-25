using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.Serialization;
using UnityEngine.UI;
using State = GameManager.State;

// The screen module [S3]: ApplyState(state) sets every button, group, camera, and raycaster
// for that state, one line per object. GameManager calls it once per state change. Buttons
// are wired in the Inspector OnClick list to GameManager; nothing here adds listeners.
public class UIManager : MonoBehaviour
{
    public static UIManager instance { get; private set; }

    [Header("Main lobby")]
    [FormerlySerializedAs("playButton")]
    [SerializeField] private Button playTabButton;
    [FormerlySerializedAs("upgradeButton")]
    [SerializeField] private Button upgradeTabButton;
    [FormerlySerializedAs("pShopButton")]
    [SerializeField] private Button permShopTabButton;
    [SerializeField] private Button startButton;
    [SerializeField] private GameObject upgradeMenuGroup;
    [SerializeField] private GameObject pShopGroup;
    [SerializeField] private Button Stuff;
    [Header("Run screens")]
    [SerializeField] private GameObject waveShopGroup;
    [SerializeField] private GameObject levelVictoryGroup;
    [SerializeField] private GameObject defeatGroup;
    [SerializeField] private Button continueButton;


    //Camera Variables (temp?)
    public Camera LevelCamera;
    public Camera GearBoxCamera;
    public float smoothspeed;
    public Vector3 minPos, maxPos;

    private Physics2DRaycaster levelRaycaster;
    private Physics2DRaycaster gearBoxRaycaster;

    private void Awake()
    {
        instance = this;
        CacheRaycasters();
    }

    public void ApplyState(GameManager.State state)
    {
        // Screens and buttons: one line each, shown in the states named on that line.
        bool lobby = state is State.LobbyPlay or State.LobbyUpgrade or State.LobbyPermShop;
        SetShown(playTabButton, lobby);
        SetShown(upgradeTabButton, lobby);
        SetShown(permShopTabButton, lobby);
        SetShown(startButton, state == State.LobbyPlay);
        SetShown(upgradeMenuGroup, state == State.LobbyUpgrade);
        SetShown(pShopGroup, state == State.LobbyPermShop);
        SetShown(continueButton, state == State.PreLevel);
        SetShown(waveShopGroup, state == State.WaveShop);
        SetShown(levelVictoryGroup, state == State.LevelVictory);
        SetShown(defeatGroup, state == State.Defeat);

        if (ShopManager.instance != null)
        {
            ShopManager.instance.SetShopOpen(state == State.LevelShop);
        }

        // Cameras: the level camera during and right after a wave, the gearbox camera otherwise.
        bool levelView = state is State.Wave or State.WaveShop or State.LevelVictory or State.Defeat;
        LevelCamera.enabled = levelView;
        GearBoxCamera.enabled = !levelView;

        if (levelRaycaster == null || gearBoxRaycaster == null)
        {
            CacheRaycasters();
        }
        if (levelRaycaster != null) levelRaycaster.enabled = state == State.WaveShop;
        if (gearBoxRaycaster != null) gearBoxRaycaster.enabled = state is State.LobbyPlay or State.PreLevel or State.LevelShop;
    }

    //Camera Functions (temp?) || I think this is helpful and shouldn't be removed for now - Antonio
    public void SwitchCamera()
    {
        if (LevelCamera.enabled == true)
        {
            LevelCamera.enabled = false;
            GearBoxCamera.enabled = true;
            Debug.Log("Gear Camera Enabled");
        }
        else if (GearBoxCamera.enabled == true)
        {
            GearBoxCamera.enabled = false;
            LevelCamera.enabled = true;
            Debug.Log("Level Camera Enabled");
        }
    }

    private void CacheRaycasters()
    {
        if (LevelCamera != null) LevelCamera.TryGetComponent(out levelRaycaster);
        if (GearBoxCamera != null) GearBoxCamera.TryGetComponent(out gearBoxRaycaster);
    }

    private static void SetShown(Component target, bool shown)
    {
        if (target != null) target.gameObject.SetActive(shown);
    }

    private static void SetShown(GameObject target, bool shown)
    {
        if (target != null) target.SetActive(shown);
    }
}
