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
    }
    public void SetStateUpgradeMenu()
    {
        state = State.UpgradeMenu;
    }
    public void SetStatePShopMenu()
    {
        state = State.PShopMenu;
    }
    public void SetStateNormal()
    {
        state = State.Normal;
    }
    public void SetStateEndGame()
    {
        state = State.EndGame;
    }
    private void ResetScene()
    {
        SceneManager.LoadScene(0);
    }
    public void ShopScene()
    {
        shopManager.OpenShop();
        buttonShop.gameObject.SetActive(false);
    }
}