using UnityEngine;
using System.Collections.Generic;

public class WaveShopManager : MonoBehaviour
{

    [SerializeField] private List<GearSpawnSlot> spawnSlots;

    private GearSpawnSlot FindFreeSlot()
    {
        foreach (GearSpawnSlot slot in spawnSlots)
        {
            if (!slot.isOccupied) return slot;
        }
        return null;
    }

}
