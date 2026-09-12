using System;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{
    public static GameManager instance { get; private set; }
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
    public void SetStateWaveVictory()
    {
        state = State.WaveVictory;
    }
    public void SetStateEndGame()
    {
        state = State.EndGame;
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
}