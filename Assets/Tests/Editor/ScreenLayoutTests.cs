using NUnit.Framework;

public class ScreenLayoutTests
{
    // Expected = the names of the flags that are true, in declaration order.
    [TestCase(GameManager.State.LobbyPlay, "lobbyTabs startButton gearBoxRaycast")]
    [TestCase(GameManager.State.LobbyUpgrade, "lobbyTabs upgradeMenu")]
    [TestCase(GameManager.State.LobbyPermShop, "lobbyTabs permShopMenu")]
    [TestCase(GameManager.State.PreLevel, "startButton gearBoxRaycast")]
    [TestCase(GameManager.State.Wave, "levelCamera")]
    [TestCase(GameManager.State.WaveShop, "waveShop levelCamera levelRaycast")]
    [TestCase(GameManager.State.LevelVictory, "levelVictory levelCamera")]
    [TestCase(GameManager.State.LevelShop, "levelShop gearBoxRaycast")]
    [TestCase(GameManager.State.Defeat, "defeat levelCamera")]
    public void For_EachState_ShowsExactlyItsScreens(GameManager.State state, string expectedOnFlags)
    {
        Assert.AreEqual(expectedOnFlags, ScreenLayout.For(state).ToString());
    }
}
