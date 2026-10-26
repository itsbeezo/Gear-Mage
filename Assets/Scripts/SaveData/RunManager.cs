using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;

// The run module [R17]: owns the current RunData. Ending a run (defeat or taking the winnings)
// reloads Main.unity, which always starts in the main lobby [R18].
public class RunManager : MonoBehaviour
{
    public static RunManager instance { get; private set; }

    private const string MainSceneName = "Main";

    public RunData Current { get; private set; }

    [SerializeField] private TextMeshProUGUI WaveAndLevelDisplay;

    private void Awake()
    {
        instance = this;
        // Created at scene load, not on Start, so the Play tab can arrange the first board [S11].
        Current = new RunData();
    }

    private void Update()
    {
        WaveAndLevelDisplay.text = "Wave: " + Current.level + "-" + Current.wave;
    }

    public void StartRun(int world)
    {
        Current.Begin(world);
    }

    public void GetWaveAndLevel()
    {
        // Wave = Current.wave;
        // Level = Current.level;
        return;
    }

    public void EndRun()
    {
        SceneManager.LoadScene(MainSceneName);
    }
}
