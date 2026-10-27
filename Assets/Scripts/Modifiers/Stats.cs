using System;
using UnityEngine;

// The unit-stat types and rules [R16]. Plain C#, so EditMode tests can construct them.
// ModifierManager adds them up; UnitBase and BaseBase apply the result.

// A set of stat bonuses: one gear's bonus, one upgrade level's value, or the board's total.
[Serializable]
public struct StatBlock
{
    public const float MinAttackRate = 1f;

    public float maxHp;
    public float attack;
    [Tooltip("Physics ticks taken off a unit's attack rate (higher = faster attacks).")]
    public float attackSpeed;
    public float moveSpeed;
    [Tooltip("Not applied to units yet; kept so gear data is not lost.")]
    public float spawnSpeed;
    [Tooltip("Player base max HP.")]
    public float baseMaxHp;

    public StatBlock Plus(StatBlock other)
    {
        StatBlock sum = this;
        sum.maxHp += other.maxHp;
        sum.attack += other.attack;
        sum.attackSpeed += other.attackSpeed;
        sum.moveSpeed += other.moveSpeed;
        sum.spawnSpeed += other.spawnSpeed;
        sum.baseMaxHp += other.baseMaxHp;
        return sum;
    }

    // A unit's attack rate (physics ticks between attacks) after this bonus. Never below MinAttackRate.
    public float AttackRate(float baseRate)
    {
        return Mathf.Max(MinAttackRate, baseRate - attackSpeed);
    }
}

// How many levels of each upgrade were bought. RunData holds the in-run (gold) levels,
// PlayerProgress the permanent (bone) levels. The shop items that raise them come later.
[Serializable]
public class UpgradeLevels
{
    public int maxHp;
    public int attack;
    public int attackSpeed;
    public int moveSpeed;
    public int baseMaxHp;

    public StatBlock Times(StatBlock perLevel)
    {
        StatBlock result = new StatBlock();
        result.maxHp = maxHp * perLevel.maxHp;
        result.attack = attack * perLevel.attack;
        result.attackSpeed = attackSpeed * perLevel.attackSpeed;
        result.moveSpeed = moveSpeed * perLevel.moveSpeed;
        result.baseMaxHp = baseMaxHp * perLevel.baseMaxHp;
        return result;
    }
}
