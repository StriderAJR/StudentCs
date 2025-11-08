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
        string[] menuItems = files.Select(x => Path.GetFileName(x)).ToArray();

        int selected = new MenuWindow("Выберите карту:", menuItems, buttonPosition: ButtonPosition.CenterVertically).Show();

        // load selected file (index-1)
        string chosenPath = files[selected];
        map = LoadMapFromFile(chosenPath, out playerPos);

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
        // draw map into buffer
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

        // draw player
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
