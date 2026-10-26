using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

// The run module [R17]. Owns both lifetimes:
// - Current (RunData): memory only; EndRun reloads Main.unity, which drops it and starts in the lobby [R18, R19].
// - Progress (PlayerProgress): saved after every change, so nothing waits for quit [W8].
public class RunManager : MonoBehaviour
{
    public static RunManager instance { get; private set; }

    private const string MainSceneName = "Main";

    [SerializeField] private TextMeshProUGUI WaveAndLevelDisplay;

    public RunData Current { get; private set; }
    public PlayerProgress Progress { get; private set; }

    private void Awake()
    {
        instance = this;
        // Created at scene load, not on Start, so the Play tab can arrange the first board [S11].
        Current = new RunData();
        PlayerProgress.DeleteLegacyRunKeys();
        Progress = PlayerProgress.Load();
    }

    private void Start()
    {
        UpdateWaveDisplay();
    }

    // Called whenever wave or level changes (StartRun here; GameManager after it changes the wave).
    public void UpdateWaveDisplay()
    {
        if (WaveAndLevelDisplay != null)
            WaveAndLevelDisplay.text = "Wave: " + Current.level + "-" + Current.wave;
    }

    public void StartRun(int world)
    {
        Current.Begin(world);
        UpdateWaveDisplay();
    }

    public void EndRun()
    {
        SceneManager.LoadScene(MainSceneName);
    }

    public void AddBones(int amount)
    {
        Progress.AddBones(amount);
        Progress.Save();
    }

    public bool TrySpendBones(int amount)
    {
        if (!Progress.TrySpendBones(amount)) return false;
        Progress.Save();
        return true;
    }

    public void UnlockWorld(int world)
    {
        Progress.UnlockWorld(world);
        Progress.Save();
    }
}
