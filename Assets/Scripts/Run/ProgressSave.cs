// Seam for permanent progress, same shape as InventorySave: PlayerPrefs in the game, in-memory in tests.
public interface ProgressSave
{
    PlayerProgress Load();
    void Save(PlayerProgress progress);
}
