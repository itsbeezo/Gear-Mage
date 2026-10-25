using System;

// Permanent data that survives runs [R7, R14, R19]. Saved through ProgressSave.
// Phase 4 adds permanent upgrade levels.
[Serializable]
public class PlayerProgress
{
    public const int WorldCount = 3;

    public int bones;
    public int highestUnlockedWorld = 1;

    public bool IsWorldUnlocked(int world)
    {
        return world >= 1 && world <= highestUnlockedWorld;
    }

    public void UnlockWorld(int world)
    {
        int clamped = Math.Min(world, WorldCount);
        if (clamped > highestUnlockedWorld) highestUnlockedWorld = clamped;
    }

    public void AddBones(int amount)
    {
        if (amount > 0) bones += amount;
    }

    public bool TrySpendBones(int amount)
    {
        if (amount < 0 || amount > bones) return false;
        bones -= amount;
        return true;
    }
}
