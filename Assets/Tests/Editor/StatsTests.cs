using NUnit.Framework;

// The stat rules in Stats.cs [R16].
public class StatsTests
{
    private static StatBlock Block(float maxHp = 0, float attack = 0, float attackSpeed = 0, float moveSpeed = 0, float baseMaxHp = 0)
    {
        return new StatBlock { maxHp = maxHp, attack = attack, attackSpeed = attackSpeed, moveSpeed = moveSpeed, baseMaxHp = baseMaxHp };
    }

    [Test]
    public void StatBlock_Plus_AddsEveryField()
    {
        StatBlock sum = Block(1, 2, 3, 4, 5).Plus(Block(10, 20, 30, 40, 50));

        Assert.AreEqual(11f, sum.maxHp);
        Assert.AreEqual(22f, sum.attack);
        Assert.AreEqual(33f, sum.attackSpeed);
        Assert.AreEqual(44f, sum.moveSpeed);
        Assert.AreEqual(55f, sum.baseMaxHp);
    }

    [Test]
    public void UpgradeLevels_Times_ScalesPerLevelValues()
    {
        UpgradeLevels levels = new UpgradeLevels { maxHp = 3, attack = 2 };

        StatBlock result = levels.Times(Block(maxHp: 5, attack: 1, moveSpeed: 9));

        Assert.AreEqual(15f, result.maxHp);
        Assert.AreEqual(2f, result.attack);
        Assert.AreEqual(0f, result.moveSpeed);
    }

    [Test]
    public void AttackRate_TakesAttackSpeedOffTheBaseRate()
    {
        Assert.AreEqual(25f, Block(attackSpeed: 15).AttackRate(40f));
    }

    [Test]
    public void AttackRate_NeverBelowMinimum()
    {
        Assert.AreEqual(StatBlock.MinAttackRate, Block(attackSpeed: 50).AttackRate(5f));
    }
}
