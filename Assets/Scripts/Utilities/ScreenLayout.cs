using System.Collections.Generic;

// What one screen state shows [S3]. UIManager.ApplyState applies it; For() is the table.
// Camera rule: levelCamera true = level camera on, gearbox camera off; false = the reverse.
public readonly struct ScreenLayout
{
    public readonly bool lobbyTabs;
    public readonly bool startButton;
    public readonly bool upgradeMenu;
    public readonly bool permShopMenu;
    public readonly bool waveShop;
    public readonly bool levelVictory;
    public readonly bool levelShop;
    public readonly bool defeat;
    public readonly bool levelCamera;
    public readonly bool gearBoxRaycast;
    public readonly bool levelRaycast;

    public ScreenLayout(
        bool lobbyTabs = false,
        bool startButton = false,
        bool upgradeMenu = false,
        bool permShopMenu = false,
        bool waveShop = false,
        bool levelVictory = false,
        bool levelShop = false,
        bool defeat = false,
        bool levelCamera = false,
        bool gearBoxRaycast = false,
        bool levelRaycast = false)
    {
        this.lobbyTabs = lobbyTabs;
        this.startButton = startButton;
        this.upgradeMenu = upgradeMenu;
        this.permShopMenu = permShopMenu;
        this.waveShop = waveShop;
        this.levelVictory = levelVictory;
        this.levelShop = levelShop;
        this.defeat = defeat;
        this.levelCamera = levelCamera;
        this.gearBoxRaycast = gearBoxRaycast;
        this.levelRaycast = levelRaycast;
    }

    public static ScreenLayout For(GameManager.State state)
    {
        switch (state)
        {
            case GameManager.State.LobbyPlay: return new ScreenLayout(lobbyTabs: true, startButton: true, gearBoxRaycast: true);
            case GameManager.State.LobbyUpgrade: return new ScreenLayout(lobbyTabs: true, upgradeMenu: true);
            case GameManager.State.LobbyPermShop: return new ScreenLayout(lobbyTabs: true, permShopMenu: true);
            case GameManager.State.PreLevel: return new ScreenLayout(startButton: true, gearBoxRaycast: true);
            case GameManager.State.Wave: return new ScreenLayout(levelCamera: true);
            case GameManager.State.WaveShop: return new ScreenLayout(waveShop: true, levelCamera: true, levelRaycast: true);
            case GameManager.State.LevelVictory: return new ScreenLayout(levelVictory: true, levelCamera: true);
            case GameManager.State.LevelShop: return new ScreenLayout(levelShop: true, gearBoxRaycast: true);
            case GameManager.State.Defeat: return new ScreenLayout(defeat: true, levelCamera: true);
            default: return new ScreenLayout();
        }
    }

    // Test and debug aid only; allocates, so never call it per frame.
    public override string ToString()
    {
        List<string> on = new List<string>();
        if (lobbyTabs) on.Add("lobbyTabs");
        if (startButton) on.Add("startButton");
        if (upgradeMenu) on.Add("upgradeMenu");
        if (permShopMenu) on.Add("permShopMenu");
        if (waveShop) on.Add("waveShop");
        if (levelVictory) on.Add("levelVictory");
        if (levelShop) on.Add("levelShop");
        if (defeat) on.Add("defeat");
        if (levelCamera) on.Add("levelCamera");
        if (gearBoxRaycast) on.Add("gearBoxRaycast");
        if (levelRaycast) on.Add("levelRaycast");
        return string.Join(" ", on);
    }
}
