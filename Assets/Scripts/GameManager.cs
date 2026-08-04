using System;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{
    public static GameManager instance { get; private set; }
    [SerializeField] private Button button;
    [SerializeField] private Button buttonShop;
    [SerializeField] private TextMeshProUGUI buttonTM;
    public enum State
    {
        BeforeStart,
        Normal,
        EndGame,
    }
    private State state;

    private void Start()
    {
        instance = this;
        button.onClick.RemoveAllListeners();
        button.onClick.AddListener(SetStateNormal);
        state = State.BeforeStart;
    }
    public void FixedUpdate()
    {
        if (state == State.Normal)
            HideButton();
        if (state == State.EndGame)
        {
            buttonTM.text = "Restart";
            button.onClick.RemoveAllListeners();
            button.onClick.AddListener(ResetScene);
            ShowButton();
        }
    }
    public State GetState()
    {
        return state;
    }
    public void SetStateNormal()
    {
        state = State.Normal;
    }
    public void SetStateEndGame()
    {
        state = State.EndGame;
    }
    private void ShowButton()
    {
        button.gameObject.SetActive(true);
        buttonShop.gameObject.SetActive(true);
    }
    private void HideButton()
    {
        button.gameObject.SetActive(false);
        buttonShop.gameObject.SetActive(false);
    }
    private void ResetScene()
    {
        SceneManager.LoadScene(0);
    }

    public void ShopScene()
    {
        SceneManager.LoadScene("Shop-Scene");
    }

}
