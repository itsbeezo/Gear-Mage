using UnityEngine;

public class GearBase : MonoBehaviour
{
    [SerializeField] private float hpBonus;
    [SerializeField] private float attackBonus;
    [SerializeField] private float spawnSpeedBonus;
    [SerializeField] private float moveSpeedBonus;
    [SerializeField] private float attackSpeedBonus;

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
