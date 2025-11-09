using StrategyGame.ConsoleGame.Game;
using StrategyGame.ConsoleGame.UI;
using StrategyGame.ConsoleGame.UI.CustomConsole;

namespace StrategyGame.ConsoleGame;

public class StrategyGame()
{
    private Map map;
    private Player player;

    private readonly int sidePanelWidth = 30;
    private readonly int bottomPanelHeight = 5;

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
        string[] files = Directory.Exists(mapsDir)
            ? Directory.GetFiles(mapsDir).Where(f => !string.IsNullOrEmpty(f)).ToArray()
            : Array.Empty<string>();

        if (files.Length == 0)
        {
            string msg = "В папке 'maps' не найдено ни одной карты.\n" +
                         "Поместите файлы карт в папку и запустите программу снова.\n" +
                         "Нажмите любую клавишу для выхода.";
            new ConsoleWindow<int>(msg, "Нет карт").Show();
            return false;
        }

        string[] menuItems = files.Select(Path.GetFileName).ToArray();
        int selected = new MenuWindow("Выберите карту:", menuItems, buttonPosition: ButtonPosition.CenterVertically).Show();

        map = new Map();
        map.LoadFromFile(files[selected], out playerPos);

        return true;
    }

    private void AskPlayerInfoAndCreatePlayer(Coordinate? initialPos)
    {
        string playerName = new InputWindow("Введите имя игрока").Show();
        PlayerType playerType = (PlayerType)new MenuWindow("Выберите тип игрока",
            Enum.GetNames(typeof(PlayerType))).Show();

        if (initialPos == null)
        {
            var found = map.FindFirstEmptyCell();
            initialPos = found ?? new Coordinate(1, 1);
        }

        player = new Player(playerName, playerType, initialPos.Value, PlayerColor.Red);
        map.RevealAround(player.position);
    }

    private void RunGameLoop()
    {
        while (true)
        {
            PrintMap();

            ConsoleKey key = GameConsole.ReadKey().Key;
            Coordinate shift = new(0, 0);
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
                    int sel = new MenuWindow("Меню", new[] { "Продолжить", "Выйти" }, "Меню", buttonPosition: ButtonPosition.Horizontal).Show();
                    if (sel == 1)
                        return;
                    break;
            }

            if (hasMovement && map.CanMove(player.position, shift))
            {
                player.Move(shift);
                map.RevealAround(player.position);
                TryCaptureBuilding();
            }
        }
    }

    private void TryCaptureBuilding()
    {
        var building = map.GetBuildingAt(player.position);
        if (building != null && !building.IsCaptured)
            building.Capture(player);
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

        wood += rng.Next(0, 2);
        stone += rng.Next(0, 2);
        gold += rng.Next(0, 1);
    }

    private void CollectWeeklyIncome()
    {
        foreach (var b in map.Buildings)
        {
            if (b.Owner == player)
            {
                switch (b.Type)
                {
                    case MapCell.Wood:
                        wood += b.IncomePerWeek;
                        break;
                    case MapCell.Stone:
                        stone += b.IncomePerWeek;
                        break;
                    case MapCell.Gold:
                        gold += b.IncomePerWeek;
                        break;
                }
            }
        }
    }

    private void PrintMap()
    {
        int totalW = GameConsole.WindowWidth;
        int totalH = GameConsole.WindowHeight;

        int sideW = Math.Min(sidePanelWidth, Math.Max(0, totalW - 10));
        int bottomH = Math.Min(bottomPanelHeight, Math.Max(0, totalH - 4));

        int mapW = Math.Max(3, totalW - sideW);
        int mapH = Math.Max(3, totalH - bottomH);

        DrawFrame(0, 0, mapW, mapH, "Карта");
        DrawFrame(mapW, 0, sideW, totalH - bottomH, "Панель");
        DrawFrame(0, mapH, totalW, bottomH, "Инфо");

        map.Draw(0, 0, mapW, mapH, player.position);
        DrawSidePanel(mapW, 0, sideW, totalH - bottomH);
        DrawBottomPanel(0, mapH, totalW, bottomH);

        GameConsole.Flush();
    }

    private void DrawFrame(int x, int y, int width, int height, string? title = null)
    {
        if (width <= 0 || height <= 0)
            return;

        var prevColor = GameConsole.ForegroundColor;
        GameConsole.ForegroundColor = ConsoleColor.Gray;

        for (int row = 0; row < height; row++)
        {
            GameConsole.SetCursorPosition(x, y + row);
            if (row == 0)
            {
                GameConsole.Write('┌');
                GameConsole.Write(new string('─', Math.Max(0, width - 2)));
                if (width > 1) GameConsole.Write('┐');
            }
            else if (row == height - 1)
            {
                GameConsole.Write('└');
                GameConsole.Write(new string('─', Math.Max(0, width - 2)));
                if (width > 1) GameConsole.Write('┘');
            }
            else
            {
                GameConsole.Write('│');
                GameConsole.Write(new string(' ', Math.Max(0, width - 2)));
                GameConsole.Write('│');
            }
        }

        if (!string.IsNullOrEmpty(title) && width >= 6)
        {
            GameConsole.SetCursorPosition(x + 2, y);
            GameConsole.Write($"[{title}]");
        }

        GameConsole.ForegroundColor = prevColor;
    }

    private void DrawSidePanel(int x, int y, int width, int height)
    {
        int innerX = x + 1;
        int innerY = y + 1;
        int innerW = Math.Max(0, width - 2);
        int innerH = Math.Max(0, height - 2);

        string[] buttons = new[] { "[I] Информация", "[E] Завершить день", "[M] Меню" };
        for (int i = 0; i < buttons.Length; i++)
        {
            string text = buttons[i];
            if (text.Length > innerW) text = text[..innerW];
            int posX = innerX + Math.Max(0, (innerW - text.Length) / 2);
            int posY = innerY + 1 + i * 2;
            if (posY < innerY + innerH)
            {
                GameConsole.SetCursorPosition(posX, posY);
                GameConsole.ForegroundColor = ConsoleColor.Yellow;
                GameConsole.Write(text);
            }
        }
    }

    private void DrawBottomPanel(int x, int y, int width, int height)
    {
        int innerX = x + 1;
        int innerY = y + 1;
        int innerW = Math.Max(0, width - 2);

        string line1 = $"День: {day}   Неделя: {week}";
        string line2 = $"Дерево: {wood}  Камень: {stone}  Золото: {gold}";

        GameConsole.SetCursorPosition(innerX, innerY);
        GameConsole.ForegroundColor = ConsoleColor.Cyan;
        GameConsole.Write(line1.Length > innerW ? line1[..innerW] : line1);

        GameConsole.SetCursorPosition(innerX, innerY + 1);
        GameConsole.ForegroundColor = ConsoleColor.Green;
        GameConsole.Write(line2.Length > innerW ? line2[..innerW] : line2);

        GameConsole.ForegroundColor = ConsoleColor.Gray;
    }

    private void ShowPlayerInfo()
    {
        string msg = $"Имя: {player.Name}\nТип: {player.Type}\nHP: {player.Health}\nПозиция: ({player.X},{player.Y})\n" +
                     $"Ресурсы - Д:{wood} К:{stone} З:{gold}\n\nНажмите любую клавишу...";
        new ConsoleWindow<int>(msg, "Информация об игроке").Show();
    }

    private static void ClearScreen()
    {
        GameConsole.Clear();
        GameConsole.Flush();
    }
}
