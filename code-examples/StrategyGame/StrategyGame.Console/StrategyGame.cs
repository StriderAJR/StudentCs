using StrategyGame.ConsoleGame.UI;
using StrategyGame.ConsoleGame.UI.CustomConsole;
using System.Text;
using System.IO;
using System.Linq;
using System;

namespace StrategyGame.ConsoleGame;

public class StrategyGame()
{
    private MapCell[,] map;
    private Player player;

    // TODO туман войны

    // UI layout settings
    private readonly int sidePanelWidth = 30;
    private readonly int bottomPanelHeight = 5;

    // game state for bottom panel
    private int day = 1;
    private int week = 1;
    private int wood = 0;
    private int stone = 0;
    private int gold = 0;

    private readonly Random rng = new();

    public void Start()
    {
        // Выбрать и загрузить карту (может вернуть позицию игрока, если в карте есть '@')
        if (!ChooseAndLoadMap(out Coordinate? initialPlayerPos))
            return; // карт нет — пользователю показано сообщение

        // Запросить имя и тип игрока и создать игрока (если в карте нет игрока — поставить на первую свободную клетку)
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

        // Если в папке нет файлов карт — проинформировать пользователя и прервать запуск
        if (files.Length == 0)
        {
            string msg = "В папке 'maps' не найдено ни одной карты.\n" +
                         "Поместите файлы карт в папку и запустите программу снова.\n" +
                         "Нажмите любую клавишу для выхода.";
            // показываем информационное окно; результат не требуется
            new ConsoleWindow<int>(msg, "Нет карт").Show();
            return false;
        }

        // Показать список файлов карт для выбора
        string[] menuItems = files.Select(x => Path.GetFileName(x)).ToArray();

        int selected = new MenuWindow("Выберите карту:", menuItems, buttonPosition: ButtonPosition.CenterVertically).Show();

        // Загрузить выбранный файл (по индексу)
        string chosenPath = files[selected];
        map = LoadMapFromFile(chosenPath, out playerPos);

        return true;
    }

    private void AskPlayerInfoAndCreatePlayer(Coordinate? initialPos)
    {
        string playerName = new InputWindow("Введите имя игрока").Show();

        PlayerType playerType = (PlayerType)new MenuWindow("Выберите тип игрока",
            Enum.GetNames(typeof(PlayerType))).Show();

        // Если в карте не указана позиция игрока — поставить его на первую свободную клетку
        if (initialPos == null)
        {
            var found = FindFirstEmptyCell();
            initialPos = found ?? new Coordinate(1, 1);
        }

        player = new Player(playerName, playerType, initialPos.Value);
    }

    private Coordinate? FindFirstEmptyCell()
    {
        for (int i = 0; i < map.GetLength(0); i++)
        {
            for (int j = 0; j < map.GetLength(1); j++)
            {
                if (map[i, j] == MapCell.Empty)
                    return new Coordinate(i, j);
            }
        }

        return null;
    }

    private void RunGameLoop()
    {
        while (true)
        {
            PrintMap();

            ConsoleKey key = GameConsole.ReadKey().Key;

            // Обработать команды и движение
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
                    int sel = new MenuWindow("Меню", new[] { "Продолжить", "Выйти" }, "Меню", buttonPosition: ButtonPosition.Horizontal).Show();
                    if (sel == 1)
                        return; // выход из игры
                    break;
                default:
                    // нераспознанная клавиша
                    break;
            }

            if (hasMovement)
            {
                if (CanMove(player.position, shift))
                {
                    player.Move(shift);
                }
            }
        }
    }

    private bool CanMove(Coordinate playerPos, Coordinate shift)
    {
        int targetRow = playerPos.X + shift.X;
        int targetCol = playerPos.Y + shift.Y;

        // Проверка границ карты
        if (targetRow < 0 || targetRow >= map.GetLength(0) ||
            targetCol < 0 || targetCol >= map.GetLength(1))
            return false;

        // Проверка столкновения со стеной
        if (map[targetRow, targetCol] == MapCell.Wall)
            return false;

        return true;
    }

    private MapCell[,] LoadMapFromFile(string path, out Coordinate? playerPos)
    {
        playerPos = null;
        string[] lines = File.ReadAllLines(path);
        int fileH = lines.Length;
        int fileW = lines.Any() ? lines.Max(l => l.Length) : 0;

        MapCell[,] result = new MapCell[(uint)fileH, (uint)fileW];

        for (int i = 0; i < result.GetLength(0); i++)
        {
            string line = lines[i];
            for (int j = 0; j < result.GetLength(1); j++)
            {
                char c = j < line.Length ? line[j] : ' ';
                switch (c)
                {
                    case '#':
                        result[i, j] = MapCell.Wall; break;
                    case 'G':
                        result[i, j] = MapCell.Gold; break;
                    case 'W':
                        result[i, j] = MapCell.Wood; break;
                    case 'S':
                        result[i, j] = MapCell.Stone; break;
                    case '@':
                        // Маркер игрока '@' — оставить клетку пустой, но запомнить позицию
                        result[i, j] = MapCell.Empty;
                        playerPos = new Coordinate(i, j);
                        break;
                    default:
                        result[i, j] = MapCell.Empty; break;
                }
            }
        }

        return result;
    }

    private void PrintMap()
    {
        // Вычислить размеры панелей
        int totalW = GameConsole.WindowWidth;
        int totalH = GameConsole.WindowHeight;

        int sideW = Math.Min(sidePanelWidth, Math.Max(0, totalW - 10));
        int bottomH = Math.Min(bottomPanelHeight, Math.Max(0, totalH - 4));

        int mapW = Math.Max(3, totalW - sideW);
        int mapH = Math.Max(3, totalH - bottomH);

        // Обрамления и контент
        DrawFrame(0, 0, mapW, mapH, "Карта");
        DrawFrame(mapW, 0, sideW, totalH - bottomH, "Панель");
        DrawFrame(0, mapH, totalW, bottomH, "Инфо");

        DrawMapPanel(0, 0, mapW, mapH);
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
                if (width == 1)
                    GameConsole.Write('┌');
                else
                {
                    GameConsole.Write('┌');
                    GameConsole.Write(new string('─', width - 2));
                    if (width > 1) GameConsole.Write('┐');
                }
            }
            else if (row == height - 1)
            {
                if (width == 1)
                    GameConsole.Write('└');
                else
                {
                    GameConsole.Write('└');
                    GameConsole.Write(new string('─', width - 2));
                    if (width > 1) GameConsole.Write('┘');
                }
            }
            else
            {
                if (width == 1)
                    GameConsole.Write('│');
                else
                {
                    GameConsole.Write('│');
                    GameConsole.Write(new string(' ', width - 2));
                    GameConsole.Write('│');
                }
            }
        }

        // Заголовок
        if (!string.IsNullOrEmpty(title) && width >= 6)
        {
            GameConsole.SetCursorPosition(x + 2, y);
            GameConsole.Write($"[{title}]");
        }

        GameConsole.ForegroundColor = prevColor;
    }

    private void DrawMapPanel(int x, int y, int width, int height)
    {
        int innerX = x + 1;
        int innerY = y + 1;
        int innerW = Math.Max(0, width - 2);
        int innerH = Math.Max(0, height - 2);

        if (map == null || innerW <= 0 || innerH <= 0)
            return;

        int mapRows = map.GetLength(0);
        int mapCols = map.GetLength(1);

        // центрируем вид на игроке
        int top = player.X - innerH / 2;
        int left = player.Y - innerW / 2;

        top = Math.Clamp(top, 0, Math.Max(0, mapRows - innerH));
        left = Math.Clamp(left, 0, Math.Max(0, mapCols - innerW));

        for (int row = 0; row < innerH; row++)
        {
            StringBuilder sb = new StringBuilder(innerW);
            for (int col = 0; col < innerW; col++)
            {
                int mr = top + row;
                int mc = left + col;
                char ch = ' ';
                if (mr >= 0 && mr < mapRows && mc >= 0 && mc < mapCols)
                    ch = map[mr, mc].ToChar();
                sb.Append(ch);
            }

            GameConsole.SetCursorPosition(innerX, innerY + row);
            GameConsole.ForegroundColor = ConsoleColor.Gray;
            GameConsole.Write(sb.ToString());
        }

        // Отрисовать игрока
        int screenRow = player.X - top;
        int screenCol = player.Y - left;
        if (screenRow >= 0 && screenRow < innerH && screenCol >= 0 && screenCol < innerW)
        {
            GameConsole.ForegroundColor = ConsoleColor.Red;
            GameConsole.SetCursorPosition(innerX + screenCol, innerY + screenRow);
            GameConsole.Write('@');
        }

        GameConsole.ForegroundColor = ConsoleColor.Gray;
    }

    private void DrawSidePanel(int x, int y, int width, int height)
    {
        int innerX = x + 1;
        int innerY = y + 1;
        int innerW = Math.Max(0, width - 2);
        int innerH = Math.Max(0, height - 2);

        if (innerW <= 0 || innerH <= 0)
            return;

        // Include hotkeys inside the button text
        string[] buttons = new[] { "[I] Информация об игроке", "[E] Завершить день", "[M] Меню" };

        int startY = innerY + 1;
        for (int i = 0; i < buttons.Length; i++)
        {
            string text = buttons[i];
            if (text.Length > innerW)
                text = text.Substring(0, innerW);

            int posX = innerX + Math.Max(0, (innerW - text.Length) / 2);
            int posY = startY + i * 2;
            if (posY >= innerY && posY < innerY + innerH)
            {
                GameConsole.SetCursorPosition(posX, posY);
                GameConsole.ForegroundColor = ConsoleColor.Yellow;
                GameConsole.Write(text);
            }
        }

        // Previously there were hotkey hints at the bottom; remove them because hotkeys are shown in buttons
        // GameConsole.SetCursorPosition(innerX, innerY + innerH - 1);
        // GameConsole.ForegroundColor = ConsoleColor.Gray;
        // GameConsole.Write(hints);
    }

    private void DrawBottomPanel(int x, int y, int width, int height)
    {
        int innerX = x + 1;
        int innerY = y + 1;
        int innerW = Math.Max(0, width - 2);
        int innerH = Math.Max(0, height - 2);

        if (innerW <= 0 || innerH <= 0)
            return;

        // Первая строка: День / Неделя
        string line1 = $"День: {day}   Неделя: {week}";
        if (line1.Length > innerW) line1 = line1.Substring(0, innerW);
        GameConsole.SetCursorPosition(innerX, innerY);
        GameConsole.ForegroundColor = ConsoleColor.Cyan;
        GameConsole.Write(line1);

        // Вторая строка: ресурсы
        string line2 = $"Дерево: {wood}  Камень: {stone}  Золото: {gold}";
        if (line2.Length > innerW) line2 = line2.Substring(0, innerW);
        GameConsole.SetCursorPosition(innerX, innerY + 1);
        GameConsole.ForegroundColor = ConsoleColor.Green;
        GameConsole.Write(line2);

        GameConsole.ForegroundColor = ConsoleColor.Gray;
    }

    private void EndDay()
    {
        day++;
        if (day > 7)
        {
            day = 1;
            week++;
        }

        // случайный сбор ресурсов за день
        wood += rng.Next(0, 3);
        stone += rng.Next(0, 2);
        gold += rng.Next(0, 2);
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
