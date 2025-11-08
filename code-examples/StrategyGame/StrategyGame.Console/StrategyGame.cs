using StrategyGame.ConsoleGame.UI;
using StrategyGame.ConsoleGame.UI.CustomConsole;
using System.Text;
using System.IO;
using System.Linq;

namespace StrategyGame.ConsoleGame;

public class StrategyGame()
{
    private MapCell[,] map;
    private Player player;

    // TODO туман войны

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

            // Вычислить смещение для клавиш движения
            Coordinate shift = new Coordinate(0, 0);
            bool isMoveKey = true;

            switch (key)
            {
                case ConsoleKey.W:
                case ConsoleKey.UpArrow:
                    shift = new Coordinate(-1, 0); break;
                case ConsoleKey.S:
                case ConsoleKey.DownArrow:
                    shift = new Coordinate(1, 0); break;
                case ConsoleKey.D:
                case ConsoleKey.RightArrow:
                    shift = new Coordinate(0, 1); break;
                case ConsoleKey.A:
                case ConsoleKey.LeftArrow:
                    shift = new Coordinate(0, -1); break;
                default:
                    isMoveKey = false; break;
            }

            if (!isMoveKey)
                continue;

            if (CanMove(player.position, shift))
            {
                player.Move(shift);
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
        // Отрисовать карту в буфер
        StringBuilder sb = new StringBuilder();
        sb.Clear();
        for (int i = 0; i < map.GetLength(0); i++)
        {
            for (int j = 0; j < map.GetLength(1); j++)
                sb.Append(map[i, j].ToChar());
            sb.AppendLine();
        }

        GameConsole.ForegroundColor = ConsoleColor.Gray;
        GameConsole.SetCursorPosition(0, 0);
        GameConsole.Write(sb.ToString());

        // Отрисовать игрока
        GameConsole.ForegroundColor = ConsoleColor.Red;
        GameConsole.SetCursorPosition(player.Y, player.X);
        GameConsole.Write('@');

        GameConsole.Flush();
    }

    private static void ClearScreen()
    {
        GameConsole.Clear();
        GameConsole.Flush();
    }
}
