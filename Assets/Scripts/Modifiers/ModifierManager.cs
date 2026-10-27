using UnityEngine;

// The unit-stat module [R16]: gear board bonuses + in-run (gold) upgrades + permanent (bone) upgrades.
// UnitBase and BaseBase ask only this. Enemies get no player bonuses.
public class ModifierManager : MonoBehaviour
{
    public static ModifierManager instance { get; private set; }

    [Header("Upgrade value per level")]
    [Tooltip("What one in-run (gold) upgrade level adds.")]
    public StatBlock runUpgradePerLevel;
    [Tooltip("What one permanent (bone) upgrade level adds.")]
    public StatBlock permanentUpgradePerLevel;

    private void Awake()
    {
        instance = this;
    }

    // The bonus a newly spawned unit adds to its own base stats.
    public StatBlock BonusFor(bool playerTeam)
    {
        return playerTeam ? PlayerBonus() : new StatBlock();
    }

    public float PlayerBaseMaxHp(float baseMaxHp)
    {
        return baseMaxHp + PlayerBonus().baseMaxHp;
    }

    public StatBlock PlayerBonus()
    {
        StatBlock runBonus = RunManager.instance.Current.runUpgrades.Times(runUpgradePerLevel);
        StatBlock permanentBonus = RunManager.instance.Progress.permanentUpgrades.Times(permanentUpgradePerLevel);
        return GearManager.instance.BoardTotals.Plus(runBonus).Plus(permanentBonus);
    }
}
