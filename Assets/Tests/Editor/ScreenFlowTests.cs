using NUnit.Framework;

public class ScreenFlowTests
{
    [TestCase(GameManager.State.LobbyPlay, ScreenEvent.PlayTab, false, GameManager.State.LobbyPlay)]
    [TestCase(GameManager.State.LobbyPlay, ScreenEvent.UpgradeTab, false, GameManager.State.LobbyUpgrade)]
    [TestCase(GameManager.State.LobbyPlay, ScreenEvent.PermShopTab, false, GameManager.State.LobbyPermShop)]
    [TestCase(GameManager.State.LobbyUpgrade, ScreenEvent.PlayTab, false, GameManager.State.LobbyPlay)]
    [TestCase(GameManager.State.LobbyPermShop, ScreenEvent.UpgradeTab, false, GameManager.State.LobbyUpgrade)]
    [TestCase(GameManager.State.LobbyPlay, ScreenEvent.Start, false, GameManager.State.Wave)]
    [TestCase(GameManager.State.LobbyUpgrade, ScreenEvent.Start, false, GameManager.State.LobbyUpgrade)]
    [TestCase(GameManager.State.PreLevel, ScreenEvent.Start, false, GameManager.State.Wave)]
    [TestCase(GameManager.State.PreLevel, ScreenEvent.PlayTab, false, GameManager.State.PreLevel)]
    [TestCase(GameManager.State.Wave, ScreenEvent.EnemyBaseDestroyed, false, GameManager.State.WaveShop)]
    [TestCase(GameManager.State.Wave, ScreenEvent.EnemyBaseDestroyed, true, GameManager.State.LevelVictory)]
    [TestCase(GameManager.State.Wave, ScreenEvent.PlayerBaseDestroyed, false, GameManager.State.Defeat)]
    [TestCase(GameManager.State.Wave, ScreenEvent.Start, false, GameManager.State.Wave)]
    [TestCase(GameManager.State.WaveShop, ScreenEvent.NextWave, false, GameManager.State.Wave)]
    [TestCase(GameManager.State.WaveShop, ScreenEvent.ContinueToShop, false, GameManager.State.WaveShop)]
    [TestCase(GameManager.State.LevelVictory, ScreenEvent.ContinueToShop, true, GameManager.State.LevelShop)]
    [TestCase(GameManager.State.LevelShop, ScreenEvent.LeaveLevelShop, false, GameManager.State.PreLevel)]
    [TestCase(GameManager.State.Defeat, ScreenEvent.Start, false, GameManager.State.Defeat)]
    [TestCase(GameManager.State.Defeat, ScreenEvent.NextWave, false, GameManager.State.Defeat)]
    public void Next_FollowsTheStateTable(GameManager.State current, ScreenEvent screenEvent, bool lastWaveOfLevel, GameManager.State expected)
    {
        Assert.AreEqual(expected, ScreenFlow.Next(current, screenEvent, lastWaveOfLevel));
    }
}
