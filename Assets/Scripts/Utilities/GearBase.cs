using UnityEngine;

public class GearBase : MonoBehaviour
{
    [SerializeField] private float hpBonus;
    [SerializeField] private float attackBonus;
    [SerializeField] private float spawnSpeedBonus;
    [SerializeField] private float moveSpeedBonus;
    [SerializeField] private float attackSpeedBonus;

    // Set when this gear is owned: GearManager stamps board gears, ShopManager stamps level-shop
    // tray gears [G1]. 0 = not owned (prefab asset, shop offer, stock shell such as HPShell,
    // which also carries a GearBase).
    public int Id { get; private set; }

    public void AssignId(int id)
    {
        Id = id;
    }

    // What this gear adds to the board totals while it sits on the board [R16].
    public StatBlock Bonus()
    {
        StatBlock bonus = new StatBlock();
        bonus.maxHp = hpBonus;
        bonus.attack = attackBonus;
        bonus.spawnSpeed = spawnSpeedBonus;
        bonus.moveSpeed = moveSpeedBonus;
        bonus.attackSpeed = attackSpeedBonus;
        return bonus;
    }
}
