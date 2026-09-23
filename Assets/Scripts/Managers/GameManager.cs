using System;
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
    public GameObject victoryScreen;
    public enum State
    {
        BeforeStart,
        UpgradeMenu,
        WaveVictory,
        PShopMenu,
        Normal,
        EndGame,
    }
    private State state;
    private int wave = 1;

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
        Debug.Log(state);
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
    public void SetStateWaveVictory()
    {
        state = State.WaveVictory;
    }
    public void SetStateEndGame()
    {
        state = State.EndGame;

        victoryScreen.gameObject.SetActive(true);
        //buttonShop.gameObject.SetActive(true);
        //UIManager.instance.SwitchCamera();
    }
    public int GetCurrentWave()
    {
        return wave;
    }
    public void NextWave()
    {
        wave += 1;
    }
    public void ResetScene()
    {
        SceneManager.LoadScene(0);
    }
    public void ShopScene()
    {
        shopManager.OpenShop();
        buttonShop.gameObject.SetActive(false);
    }

    public void NextLevel()
    {
        SceneManager.LoadScene(0);//Temporary 
        //I plan To Add PlayerPrefs for gear matrix so the gars pass on to the next level
    }

}