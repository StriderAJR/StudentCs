using StrategyGame.ConsoleGame.UI;
using StrategyGame.ConsoleGame.UI.CustomConsole;
using System.Text;
using System.IO;
using System.Linq;

namespace StrategyGame.ConsoleGame;

public class StrategyGame(uint width, uint height)
{
    public readonly uint Width = width;
    public readonly uint Height = height;
    private MapCell[,] map;
    private Player player;

    // TODO туман войны

    public void Start()
    {
        // Choose and load map (may return player position if map contains '@')
        if (!ChooseAndLoadMap(out Coordinate? initialPlayerPos))
            return; // no maps available — user was informed

        // Ask user for player name/type and create player (place on first free cell if needed)
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

        // If there are no map files — inform user and abort
        if (files.Length == 0)
        {
            string msg = "В папке 'maps' не найдено ни одной карты.\n" +
                         "Поместите файлы карт в папку и запустите программу снова.\n" +
                         "Нажмите любую клавишу для выхода.";
            // use ConsoleWindow to show informational message; no result needed
            new ConsoleWindow<int>(msg, "Нет карт").Show();
            return false;
        }

        // show file names + an option to generate default
        string[] menuItems = new string[files.Length + 1];
        menuItems[0] = "Сгенерировать карту по умолчанию";
        for (int i = 0; i < files.Length; i++)
            menuItems[i + 1] = Path.GetFileName(files[i]);

        int selected = new MenuWindow("Выберите карту:", menuItems, buttonPosition: ButtonPosition.CenterVertically).Show();

        if (selected == 0)
        {
            // generate default
            map = GenerateMap(Height, Width);
        }
        else
        {
            // load selected file (index-1)
            string chosenPath = files[selected - 1];
            map = LoadMapFromFile(chosenPath, out playerPos);
        }

        return true;
    }

    private void AskPlayerInfoAndCreatePlayer(Coordinate? initialPos)
    {
        string playerName = new InputWindow("Введите имя игрока").Show();

        PlayerType playerType = (PlayerType)new MenuWindow("Выберите тип игрока",
            Enum.GetNames(typeof(PlayerType))).Show();

        // If map didn't contain player, place first free cell
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

            ConsoleKey input = GameConsole.ReadKey().Key;
            switch (input)
            {
                case ConsoleKey.W:
                case ConsoleKey.UpArrow:
                    player.Move(new Coordinate(-1, 0)); break;
                case ConsoleKey.S:
                case ConsoleKey.DownArrow:
                    player.Move(new Coordinate(1, 0)); break;
                case ConsoleKey.D:
                case ConsoleKey.RightArrow:
                    player.Move(new Coordinate(0, 1)); break;
                case ConsoleKey.A:
                case ConsoleKey.LeftArrow:
                    player.Move(new Coordinate(0, -1)); break;
            }
        }
    }

    private MapCell[,] GenerateMap(uint height, uint width)
    {
        MapCell[,] map = new MapCell[height, width];
        for (int i = 0; i < height; i++)
            for (int j = 0; j < width; j++)
                map[i, j] = (i == 0 || i == height-1 || j == 0 || j == width-1)
                    ? MapCell.Wall
                    : MapCell.Empty;

        return map;
    }

    private MapCell[,] LoadMapFromFile(string path, out Coordinate? playerPos)
    {
        playerPos = null;
        // initialize with default generated map (so borders are walls)
        MapCell[,] result = GenerateMap(Height, Width);

        string[] lines = File.ReadAllLines(path);
        int fileH = lines.Length;
        int fileW = lines.Any() ? lines.Max(l => l.Length) : 0;

        int maxH = Math.Min((int)Height, fileH);
        int maxW = Math.Min((int)Width, fileW);

        for (int i = 0; i < maxH; i++)
        {
            string line = lines[i];
            for (int j = 0; j < maxW; j++)
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
                        // player marker -> leave cell empty but remember position
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
        ConsoleBuffer buffer = GameConsole.Buffer;

        // draw map into buffer
        StringBuilder sb = new StringBuilder();
        sb.Clear();
        for (int i = 0; i < map.GetLength(0); i++)
        {
            for (int j = 0; j < map.GetLength(1); j++)
                sb.Append(map[i, j].ToChar());
            sb.AppendLine();
        }

        buffer.ForegroundColor = ConsoleColor.Gray;
        buffer.SetCursorPosition(0, 0);
        buffer.Write(sb.ToString());

        // draw player
        buffer.ForegroundColor = ConsoleColor.Red;
        buffer.SetCursorPosition((int)player.Y, (int)player.X);
        buffer.Write('@');

        buffer.Flush();
    }

    private static void ClearScreen()
    {
        ConsoleBuffer buffer = GameConsole.Buffer;
        buffer.Clear();
        buffer.Flush();
    }
}
