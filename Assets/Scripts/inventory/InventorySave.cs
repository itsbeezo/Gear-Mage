public interface InventorySave
{
    PlayerInventoryData Load();
    void Save(PlayerInventoryData data);
}




// Stuff for me to look at:
// The wiring, since you feel disconnected. This is what I read in the code.
// - Gold in. UnitBase calls CurrencyManager.AddGold(...) when units die.
// - Wave end. BaseBase.DestroySelf calls GameManager.SetStateWaveVictory, and UIManager.FixedUpdate shows WaveShop while that state holds.
// - Placing. UIGearSlot.TryPlaceGear calls InventoryManager.TryConsume, then GearManager.SetGear and SpawnSingleGear.
// - Showing stock. InventoryGears reads InventoryManager every frame.
// - Persistence. InventoryManager uses the save interface through BeginRun.
