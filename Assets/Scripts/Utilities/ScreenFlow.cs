// Something the player pressed or the game reported. GameManager turns each into a state change.
public enum ScreenEvent
{
    PlayTab,
    UpgradeTab,
    PermShopTab,
    Start,
    NextWave,
    ContinueToShop,
    LeaveLevelShop,
    EnemyBaseDestroyed,
    PlayerBaseDestroyed,
}

// The screen-state table [S4, S9, S10]. Pure, so every row is an EditMode test.
// An event that does not apply to the current state returns the current state (ignored).
public static class ScreenFlow
{
    public static GameManager.State Next(GameManager.State current, ScreenEvent screenEvent, bool lastWaveOfLevel)
    {
        switch (current)
        {
            case GameManager.State.LobbyPlay:
            case GameManager.State.LobbyUpgrade:
            case GameManager.State.LobbyPermShop:
                if (screenEvent == ScreenEvent.PlayTab) return GameManager.State.LobbyPlay;
                if (screenEvent == ScreenEvent.UpgradeTab) return GameManager.State.LobbyUpgrade;
                if (screenEvent == ScreenEvent.PermShopTab) return GameManager.State.LobbyPermShop;
                if (screenEvent == ScreenEvent.Start && current == GameManager.State.LobbyPlay) return GameManager.State.Wave;
                return current;

            case GameManager.State.PreLevel:
                return screenEvent == ScreenEvent.Start ? GameManager.State.Wave : current;

            case GameManager.State.Wave:
                if (screenEvent == ScreenEvent.PlayerBaseDestroyed) return GameManager.State.Defeat;
                if (screenEvent == ScreenEvent.EnemyBaseDestroyed)
                {
                    return lastWaveOfLevel ? GameManager.State.LevelVictory : GameManager.State.WaveShop;
                }
                return current;

            case GameManager.State.WaveShop:
                return screenEvent == ScreenEvent.NextWave ? GameManager.State.Wave : current;

            case GameManager.State.LevelVictory:
                return screenEvent == ScreenEvent.ContinueToShop ? GameManager.State.LevelShop : current;

            case GameManager.State.LevelShop:
                return screenEvent == ScreenEvent.LeaveLevelShop ? GameManager.State.PreLevel : current;

            default:
                return current;
        }
    }
}
