using System;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{
    public static GameManager instance { get; private set; }
    [SerializeField] private Button buttonShop;
    public ShopManager shopManager;
    public GameObject startButton;
    public GameObject playButton;
    public GameObject upgradeButton;
    public GameObject permShopButton;
    public enum State
    {
        BeforeStart,
        UpgradeMenu,
        PShopMenu,
        Normal,
        EndGame,
    }
    private State state;

    private void Start()
    {
        //shopManager = GameObject.Find("ShopManager").GetComponent<ShopManager>();
        instance = this;
        state = State.BeforeStart;
    }
    private void Update()
    {
        if(Input.GetKeyDown(KeyCode.P))
        {
            ResetScene();
        }
    }
    public State GetState()
    {
        return state;
    }
    public void SetStateBeforeStart()
    {
        state = State.BeforeStart;
        buttonShop.gameObject.SetActive(true);
    }
    public void SetStateUpgradeMenu()
    {
        state = State.UpgradeMenu;
        buttonShop.gameObject.SetActive(false);
    }
    public void SetStatePShopMenu()
    {
        state = State.PShopMenu;
        buttonShop.gameObject.SetActive(false);
        Debug.Log("Shop Menu");
    }
    public void SetStateNormal()
    {
        state = State.Normal;
        buttonShop.gameObject.SetActive(false);
    }
    public void SetStateEndGame()
    {
        state = State.EndGame;
        buttonShop.gameObject.SetActive(true);
        UIManager.instance.SwitchCamera();
    }
    private void ResetScene()
    {
        SceneManager.LoadScene(0);
    }
    public void ShopScene()
    {
        shopManager.OpenShop();
        buttonShop.gameObject.SetActive(false);
        startButton.gameObject.SetActive(false);
        playButton.gameObject.SetActive(false);
        permShopButton.gameObject.SetActive(false);
        upgradeButton.gameObject.SetActive(false);

    }

}