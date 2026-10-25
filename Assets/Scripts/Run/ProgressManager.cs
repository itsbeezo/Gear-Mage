using UnityEngine;

// Owns PlayerProgress for the scene and saves it on every change, so nothing waits for quit [W8].
public class ProgressManager : MonoBehaviour
{
    public static ProgressManager instance { get; private set; }

    public PlayerProgress Progress { get; private set; }

    private ProgressSave save;

    private void Awake()
    {
        instance = this;
        PlayerPrefsProgressSave.DeleteLegacyRunKeys();
        Begin(new PlayerPrefsProgressSave());
    }

    // Separated from Awake so tests can use an in-memory save.
    public void Begin(ProgressSave progressSave)
    {
        save = progressSave;
        Progress = save.Load();
    }

    public void AddBones(int amount)
    {
        Progress.AddBones(amount);
        save.Save(Progress);
    }

    public bool TrySpendBones(int amount)
    {
        if (!Progress.TrySpendBones(amount)) return false;
        save.Save(Progress);
        return true;
    }

    public bool IsWorldUnlocked(int world)
    {
        return Progress.IsWorldUnlocked(world);
    }

    public void UnlockWorld(int world)
    {
        Progress.UnlockWorld(world);
        save.Save(Progress);
    }
}
