using UnityEngine;
using UnityEngine.SceneManagement;

// Owns the screen state. Every screen button's Inspector OnClick calls one method below,
// named after the button [S1]. Each state change calls UIManager.ApplyState once [S3].
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

    // ---- Inspector OnClick targets, one per button ----

    public void PlayTabButton()
    {
        Fire(ScreenEvent.PlayTab);
    }

    public void UpgradeTabButton()
    {
        Fire(ScreenEvent.UpgradeTab);
    }

    public void PermShopTabButton()
    {
        Fire(ScreenEvent.PermShopTab);
    }

    public void StartButton()
    {
        Fire(ScreenEvent.Start);
    }

    public void ContinueButton()
    {
        Fire(ScreenEvent.Start);
    }

    public void NextWaveButton()
    {
        Fire(ScreenEvent.NextWave);
    }

    public void ContinueToShopButton()
    {
        Fire(ScreenEvent.ContinueToShop);
    }

    public void EndRunButton()
    {
        ReloadMain();
    }

    public void DefeatContinueButton()
    {
        ReloadMain();
    }

    // ---- Direct calls from other modules ----

    // ShopManager.BackToGame (the level shop's Return button keeps its shop target [S2]).
    public void LeaveLevelShop()
    {
        Fire(ScreenEvent.LeaveLevelShop);
    }

    // BaseBase.DestroySelf.
    public void BaseDestroyed(bool enemyBase)
    {
        Fire(enemyBase ? ScreenEvent.EnemyBaseDestroyed : ScreenEvent.PlayerBaseDestroyed);
    }

    private void Fire(ScreenEvent screenEvent)
    {
        State next = ScreenFlow.Next(state, screenEvent, wave >= wavesPerLevel);
        if (next == state) return;

        State previous = state;
        state = next;
        RunEnterEffects(previous, next);
        UIManager.instance.ApplyState(next);
    }

    private void RunEnterEffects(State previous, State next)
    {
        switch (next)
        {
            case State.Wave:
                wave = previous == State.WaveShop ? wave + 1 : 1;
                if (previous != State.LobbyPlay)
                {
                    UnitManager.instance.OnNextWave();
                }
                break;
            case State.WaveShop:
                CurrencyManager.instance.AddBone(bonesPerWaveWon);
                WaveShopManager.instance.RollWaveShop();
                break;
            case State.LevelShop:
                WaveShopManager.instance.RollLevelShop();
                break;
        }
    }

    private void ReloadMain()
    {
        SceneManager.LoadScene(MainSceneName);
    }
}
