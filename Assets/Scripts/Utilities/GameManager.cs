using UnityEngine;
using UnityEngine.SceneManagement;

// Owns the screen state. Every screen button's Inspector OnClick calls one method below,
// named after the button; the method does the button's work and calls SetState [S1].
// SetState calls UIManager.ApplyState once per change [S3].
public class GameManager : MonoBehaviour
{
    public static GameManager instance { get; private set; }

    public enum State
    {
        LobbyPlay,
        LobbyUpgrade,
        LobbyPermShop,
        PreLevel,
        Wave,
        WaveShop,
        LevelVictory,
        LevelShop,
        Defeat,
    }

    private const string MainSceneName = "Main";

    [Header("Run")]
    [Tooltip("Waves per level [S6]. Moves to LevelDefinition in Phase 5.")]
    public int wavesPerLevel = 5;

    [Header("Rewards (temporary)")]
    [Tooltip("Today's bones per wave won. Replaced by LevelDefinition payouts in Phase 5.")]
    public int bonesPerWaveWon = 10;

    private State state = State.LobbyPlay;
    private int wave = 1;

    private void Awake()
    {
        instance = this;
    }

    private void Start()
    {
        UIManager.instance.ApplyState(state);
    }

    public State GetState()
    {
        return state;
    }

    public int GetCurrentWave()
    {
        return wave;
    }

    // ---- Inspector OnClick targets, one per button. Each does its own work, then SetState. ----

    public void PlayTabButton()
    {
        SetState(State.LobbyPlay);
    }

    public void UpgradeTabButton()
    {
        SetState(State.LobbyUpgrade);
    }

    public void PermShopTabButton()
    {
        SetState(State.LobbyPermShop);
    }

    // Lobby Play tab: first wave of a new run.
    public void StartButton()
    {
        wave = 1;
        SetState(State.Wave);
    }

    // Pre-level: first wave of the next level.
    public void ContinueButton()
    {
        wave = 1;
        UnitManager.instance.OnNextWave();
        SetState(State.Wave);
    }

    public void NextWaveButton()
    {
        wave++;
        UnitManager.instance.OnNextWave();
        SetState(State.Wave);
    }

    public void ContinueToShopButton()
    {
        WaveShopManager.instance.RollLevelShop();
        SetState(State.LevelShop);
    }

    public void EndRunButton()
    {
        ReloadMain();
    }

    public void DefeatContinueButton()
    {
        ReloadMain();
    }

    public void StuffButton()
    {
        Debug.Log("Button was pressed and stuff happened");
    }

    // ---- Direct calls from other modules ----

    // ShopManager.BackToGame (the level shop's Return button keeps its shop target [S2]).
    public void LeaveLevelShop()
    {
        SetState(State.PreLevel);
    }

    // BaseBase.DestroySelf. Not a button, so it checks the state: a base only counts during a wave.
    public void BaseDestroyed(bool enemyBase)
    {
        if (state != State.Wave) return;

        if (!enemyBase)
        {
            SetState(State.Defeat);
        }
        else if (wave >= wavesPerLevel)
        {
            SetState(State.LevelVictory);
        }
        else
        {
            CurrencyManager.instance.AddBone(bonesPerWaveWon);
            WaveShopManager.instance.RollWaveShop();
            SetState(State.WaveShop);
        }
    }

    private void SetState(State next)
    {
        state = next;
        UIManager.instance.ApplyState(state);
    }

    private void ReloadMain()
    {
        SceneManager.LoadScene(MainSceneName);
    }
}
