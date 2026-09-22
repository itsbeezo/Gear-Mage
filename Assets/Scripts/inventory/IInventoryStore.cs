public interface IInventoryStore
{
    PlayerInventoryData Load();
    void Save(PlayerInventoryData data);
}
