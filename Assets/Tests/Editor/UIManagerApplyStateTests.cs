using System.Collections.Generic;
using NUnit.Framework;
using UnityEditor;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class UIManagerApplyStateTests
{
    private readonly List<GameObject> spawned = new List<GameObject>();
    private UIManager ui;
    private Button playTab;
    private Button startButton;
    private GameObject upgradeMenu;
    private GameObject waveShop;
    private GameObject levelVictory;
    private GameObject defeat;
    private Camera levelCamera;
    private Camera gearBoxCamera;

    [SetUp]
    public void SetUp()
    {
        ui = Make("UIManager").AddComponent<UIManager>();
        playTab = Make("PlayTab").AddComponent<Button>();
        Button upgradeTab = Make("UpgradeTab").AddComponent<Button>();
        Button permShopTab = Make("PermShopTab").AddComponent<Button>();
        startButton = Make("Start").AddComponent<Button>();
        upgradeMenu = Make("UpgradeMenu");
        GameObject permShopMenu = Make("PermShopMenu");
        waveShop = Make("WaveShop");
        levelVictory = Make("LevelVictory");
        defeat = Make("Defeat");
        levelCamera = MakeCamera("LevelCamera");
        gearBoxCamera = MakeCamera("GearBoxCamera");

        // Private [SerializeField]s are set the way the Inspector does it.
        SerializedObject so = new SerializedObject(ui);
        so.FindProperty("playTabButton").objectReferenceValue = playTab;
        so.FindProperty("upgradeTabButton").objectReferenceValue = upgradeTab;
        so.FindProperty("permShopTabButton").objectReferenceValue = permShopTab;
        so.FindProperty("startButton").objectReferenceValue = startButton;
        so.FindProperty("upgradeMenuGroup").objectReferenceValue = upgradeMenu;
        so.FindProperty("pShopGroup").objectReferenceValue = permShopMenu;
        so.FindProperty("waveShopGroup").objectReferenceValue = waveShop;
        so.FindProperty("levelVictoryGroup").objectReferenceValue = levelVictory;
        so.FindProperty("defeatGroup").objectReferenceValue = defeat;
        so.FindProperty("LevelCamera").objectReferenceValue = levelCamera;
        so.FindProperty("GearBoxCamera").objectReferenceValue = gearBoxCamera;
        so.ApplyModifiedPropertiesWithoutUndo();
    }

    [TearDown]
    public void TearDown()
    {
        foreach (GameObject go in spawned) Object.DestroyImmediate(go);
        spawned.Clear();
    }

    private GameObject Make(string name)
    {
        GameObject go = new GameObject(name);
        spawned.Add(go);
        return go;
    }

    private Camera MakeCamera(string name)
    {
        GameObject go = Make(name);
        Camera cam = go.AddComponent<Camera>();
        go.AddComponent<Physics2DRaycaster>();
        return cam;
    }

    [Test]
    public void ApplyState_Wave_HidesLobbyAndUsesLevelCameraWithNoRaycasts()
    {
        ui.ApplyState(GameManager.State.Wave);

        Assert.IsFalse(playTab.gameObject.activeSelf);
        Assert.IsFalse(startButton.gameObject.activeSelf);
        Assert.IsTrue(levelCamera.enabled);
        Assert.IsFalse(gearBoxCamera.enabled);
        Assert.IsFalse(levelCamera.GetComponent<Physics2DRaycaster>().enabled);
        Assert.IsFalse(gearBoxCamera.GetComponent<Physics2DRaycaster>().enabled);
    }

    [Test]
    public void ApplyState_WaveShop_ShowsWaveShopAndEnablesLevelRaycast()
    {
        ui.ApplyState(GameManager.State.WaveShop);

        Assert.IsTrue(waveShop.activeSelf);
        Assert.IsTrue(levelCamera.GetComponent<Physics2DRaycaster>().enabled);
    }

    [Test]
    public void ApplyState_Defeat_ShowsDefeatNotVictory()
    {
        ui.ApplyState(GameManager.State.Defeat);

        Assert.IsTrue(defeat.activeSelf);
        Assert.IsFalse(levelVictory.activeSelf);
    }

    [Test]
    public void ApplyState_LobbyUpgrade_ShowsTabsAndUpgradeMenuOnGearBoxCamera()
    {
        ui.ApplyState(GameManager.State.LobbyUpgrade);

        Assert.IsTrue(playTab.gameObject.activeSelf);
        Assert.IsTrue(upgradeMenu.activeSelf);
        Assert.IsFalse(startButton.gameObject.activeSelf);
        Assert.IsTrue(gearBoxCamera.enabled);
        Assert.IsFalse(levelCamera.enabled);
    }
}
