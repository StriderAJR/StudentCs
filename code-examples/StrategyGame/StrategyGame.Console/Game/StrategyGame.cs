using StrategyGame.ConsoleGame.UI;
using StrategyGame.ConsoleGame.UI.CustomConsole;
using StrategyGame.ConsoleGame.UI.Panels;
using StrategyGame.ConsoleGame.UI.Windows;

namespace StrategyGame.ConsoleGame.Game;

public class StrategyGame
{
    private Map map;
    private Player player;

    // Панели UI
    private SidePanel sidePanel;
    private BottomPanel bottomPanel;

    // Настройки интерфейса
    private readonly int sidePanelWidth = 30;
    private readonly int bottomPanelHeight = 5;

    // Состояние игры
    private int day = 1;
    private int week = 1;
    private int wood = 0;
    private int stone = 0;
    private int gold = 0;

    private readonly Random rng = new();

    public void Start()
    {
        if (!ChooseAndLoadMap(out Coordinate? initialPlayerPos))
            return;

        AskPlayerInfoAndCreatePlayer(initialPlayerPos);

        ClearScreen();
        RunGameLoop();
    }

    private bool ChooseAndLoadMap(out Coordinate? playerPos)
    {
        playerPos = null;

        string mapsDir = Path.Combine(AppContext.BaseDirectory, "maps");
        string[] files = Array.Empty<string>();
        if (Directory.Exists(mapsDir))
        {
            files = Directory.GetFiles(mapsDir)
                .Where(f => !string.IsNullOrEmpty(f))
                .ToArray();
        }

        if (files.Length == 0)
        {
            string msg = "В папке 'maps' не найдено ни одной карты.\n" +
                         "Поместите файлы карт в папку и запустите программу снова.\n" +
                         "Нажмите любую клавишу для выхода.";
            new ConsoleWindow<int>(msg, "Нет карт").Show();
            return false;
        }

        string[] menuItems = files.Select(x => Path.GetFileName(x)).ToArray();

        int selected = new MenuWindow("Выберите карту:", menuItems,
            buttonPosition: ButtonPosition.CenterVertically).Show();

        string chosenPath = files[selected];
        map = new Map();
        map.LoadFromFile(chosenPath, out playerPos);

        return true;
    }

    private void AskPlayerInfoAndCreatePlayer(Coordinate? initialPos)
    {
        string playerName = new InputWindow("Введите имя игрока").Show();

        PlayerType playerType = (PlayerType)new MenuWindow(
            "Выберите тип игрока",
            Enum.GetNames(typeof(PlayerType))
        ).Show();

        if (initialPos == null)
        {
            var found = map.FindFirstEmptyCell();
            initialPos = found ?? new Coordinate(1, 1);
        }

        player = new Player(playerName, playerType, initialPos.Value, PlayerColor.Red);
        map.RevealAround(initialPos.Value, 3);
    }

    private void RunGameLoop()
    {
        while (true)
        {
            Draw();

            ConsoleKey key = GameConsole.ReadKey().Key;
            Coordinate shift = new Coordinate(0, 0);
            bool hasMovement = false;

            switch (key)
            {
                case ConsoleKey.W:
                case ConsoleKey.UpArrow:
                    shift = new Coordinate(-1, 0); hasMovement = true; break;
                case ConsoleKey.S:
                case ConsoleKey.DownArrow:
                    shift = new Coordinate(1, 0); hasMovement = true; break;
                case ConsoleKey.D:
                case ConsoleKey.RightArrow:
                    shift = new Coordinate(0, 1); hasMovement = true; break;
                case ConsoleKey.A:
                case ConsoleKey.LeftArrow:
                    shift = new Coordinate(0, -1); hasMovement = true; break;
                case ConsoleKey.I:
                    ShowPlayerInfo(); break;
                case ConsoleKey.E:
                    EndDay(); break;
                case ConsoleKey.M:
                    int sel = new MenuWindow("Меню", new[] { "Продолжить", "Выйти" },
                        "Меню", buttonPosition: ButtonPosition.Horizontal).Show();
                    if (sel == 1)
                        return;
                    break;
            }

            if (hasMovement && map.CanMove(player.position, shift))
            {
                player.Move(shift);
                map.RevealAround(player.position, 3);
                map.TryCaptureBuilding(player.position, player);
            }
        }
    }

    private void EndDay()
    {
        day++;
        if (day > 7)
        {
            day = 1;
            week++;
            CollectWeeklyIncome();
        }

        wood += rng.Next(0, 3);
        stone += rng.Next(0, 2);
        gold += rng.Next(0, 2);
    }

    private void CollectWeeklyIncome()
    {
        foreach (var b in map.Buildings)
        {
            if (b.Owner == player)
            {
                switch (b.Type)
                {
                    case MapCell.Wood: wood += b.IncomePerWeek; break;
                    case MapCell.Stone: stone += b.IncomePerWeek; break;
                    case MapCell.Gold: gold += b.IncomePerWeek; break;
                }
            }
        }
    }

    private (int wood, int stone, int gold) CalculateWeeklyIncome()
    {
        int woodInc = 0, stoneInc = 0, goldInc = 0;

        foreach (var b in map.Buildings)
        {
            if (b.Owner == player)
            {
                switch (b.Type)
                {
                    case MapCell.Wood:
                        woodInc += b.IncomePerWeek; break;
                    case MapCell.Stone:
                        stoneInc += b.IncomePerWeek; break;
                    case MapCell.Gold:
                        goldInc += b.IncomePerWeek; break;
                }
            }
        }

        return (woodInc, stoneInc, goldInc);
    }

    private void Draw()
    {
        int totalW = GameConsole.WindowWidth;
        int totalH = GameConsole.WindowHeight;

        int sideW = Math.Min(sidePanelWidth, Math.Max(0, totalW - 10));
        int bottomH = Math.Min(bottomPanelHeight, Math.Max(0, totalH - 4));

        int mapW = Math.Max(3, totalW - sideW);
        int mapH = Math.Max(3, totalH - bottomH);

        map.DrawFrame(0, 0, mapW, mapH, "Карта");
        map.DrawVisible(0, 0, mapW, mapH, player);

        sidePanel = new SidePanel(mapW, 0, sideW, totalH - bottomH);
        bottomPanel = new BottomPanel(0, mapH, totalW, bottomH, GetPanelData);

        sidePanel.Draw("Панель");
        bottomPanel.Draw("Инфо");

        GameConsole.Flush();
    }

    private PanelData GetPanelData()
    {
        var (wInc, sInc, gInc) = CalculateWeeklyIncome();
        return new PanelData(day, week, wood, stone, gold, wInc, sInc, gInc);
    }

    private void ShowPlayerInfo()
    {
        string msg = $"Имя: {player.Name}\nТип: {player.Type}\nHP: {player.Health}\n" +
                     $"Позиция: ({player.X},{player.Y})\n" +
                     $"Ресурсы - Д:{wood} К:{stone} З:{gold}\n\nНажмите любую клавишу...";
        new ConsoleWindow<int>(msg, "Информация об игроке").Show();
    }

    private static void ClearScreen()
    {
        GameConsole.Clear();
        GameConsole.Flush();
    }
}
