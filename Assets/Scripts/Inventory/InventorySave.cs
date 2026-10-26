public interface InventorySave
{
    PlayerInventoryData Load();
    void Save(PlayerInventoryData data);
}
