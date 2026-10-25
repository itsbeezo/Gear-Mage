using UnityEngine;
using UnityEngine.SceneManagement;

// The run module [R17]: owns the current RunData. Ending a run (defeat or taking the winnings)
// reloads Main.unity, which always starts in the main lobby [R18].
public class RunManager : MonoBehaviour
{
    public static RunManager instance { get; private set; }

    private const string MainSceneName = "Main";

    public RunData Current { get; private set; }

    private void Awake()
    {
        instance = this;
        // Created at scene load, not on Start, so the Play tab can arrange the first board [S11].
        Current = new RunData();
    }

    public void StartRun(int world)
    {
        Current.Begin(world);
    }

    public void EndRun()
    {
        SceneManager.LoadScene(MainSceneName);
    }
}
