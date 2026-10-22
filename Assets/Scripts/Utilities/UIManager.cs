using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.Serialization;
using UnityEngine.UI;

// The screen module [S3]: ApplyState(state) sets every button, group, camera, and raycaster
// for that state. GameManager calls it once per state change. Buttons are wired in the
// Inspector OnClick list to GameManager; nothing here adds listeners.
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
        ScreenLayout layout = ScreenLayout.For(state);

        SetShown(playTabButton, layout.lobbyTabs);
        SetShown(upgradeTabButton, layout.lobbyTabs);
        SetShown(permShopTabButton, layout.lobbyTabs);
        SetShown(startButton, layout.startButton);
        SetShown(continueButton, layout.continueButton);
        SetShown(upgradeMenuGroup, layout.upgradeMenu);
        SetShown(pShopGroup, layout.permShopMenu);
        SetShown(waveShopGroup, layout.waveShop);
        SetShown(levelVictoryGroup, layout.levelVictory);
        SetShown(defeatGroup, layout.defeat);

        if (ShopManager.instance != null)
        {
            ShopManager.instance.SetShopOpen(layout.levelShop);
        }

        if (levelRaycaster == null || gearBoxRaycaster == null)
        {
            CacheRaycasters();
        }

        LevelCamera.enabled = layout.levelCamera;
        GearBoxCamera.enabled = !layout.levelCamera;
        if (levelRaycaster != null) levelRaycaster.enabled = layout.levelRaycast;
        if (gearBoxRaycaster != null) gearBoxRaycaster.enabled = layout.gearBoxRaycast;
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
