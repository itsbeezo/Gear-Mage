using UnityEngine;
using UnityEngine.SceneManagement;

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

    private void Awake()
    {
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
    public void SetState(State newState)
    {
        if (state == newState) return;

        state = newState;
        if (UIManager.instance != null) UIManager.instance.ApplyState(state);
    }
    // Parameterless wrappers so buttons can target them from the Inspector OnClick list (enums aren't allowed there)
    public void SetStateBeforeStart() { SetState(State.BeforeStart); }
    public void SetStateUpgradeMenu() { SetState(State.UpgradeMenu); }
    public void SetStatePShopMenu() { SetState(State.PShopMenu); }
    public void SetStateNormal() { SetState(State.Normal); }
    public void SetStateWaveVictory() { SetState(State.WaveVictory); }
    public void SetStateEndGame()
    {
        // Uncomment this when you have an actually level system and doesnt end after 3 waves
        // CurrencyManager.instance.ClearGoldSave();
        SetState(State.EndGame);
    }
    public int GetCurrentWave()
    {
        return wave;
    }
    public void NextWave()
    {
        wave += 1;
    }
    public void StartNextWave()
    {
        UnitManager.instance.OnNextWave();
        SetStateNormal();
    }
    public void ResetScene()
    {
        SceneManager.LoadScene(0);
    }

    public void NextLevel()
    {
        SceneManager.LoadScene(0);//Temporary
        //I plan To Add PlayerPrefs for gear matrix so the gars pass on to the next level
    }

}
