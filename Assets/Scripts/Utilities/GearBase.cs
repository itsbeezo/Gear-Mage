using UnityEngine;

public class GearBase : MonoBehaviour
{
    [SerializeField] private float hpBonus;
    [SerializeField] private float attackBonus;
    [SerializeField] private float spawnSpeedBonus;
    [SerializeField] private float moveSpeedBonus;
    [SerializeField] private float attackSpeedBonus;

    // Set by GearManager when it spawns this gear onto the board [G1]. 0 = not a board gear
    // (prefab asset, shop offer, tray shell such as HPShell, which also carries a GearBase).
    public int Id { get; private set; }

    public void AssignId(int id)
    {
        Id = id;
    }

    public float GetHPBonus()
    {
        return hpBonus;
    }
    public float GetAttackBonus()
    {
        return attackBonus;
    }
    public float GetSpawnSpeedBonus()
    {
        return spawnSpeedBonus;
    }
    public float GetMoveSpeedBonus()
    {
        return moveSpeedBonus;
    }
    public float GetAttackSpeedBonus()
    {
        return attackSpeedBonus;
    }
}
