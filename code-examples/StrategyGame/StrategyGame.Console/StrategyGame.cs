using StrategyGame.ConsoleGame.UI;
using StrategyGame.ConsoleGame.UI.CustomConsole;
using System.Text;

namespace StrategyGame.ConsoleGame;

public class StrategyGame(uint width, uint height)
{
    public readonly uint Width = width;
    public readonly uint Height = height;
    private MapCell[,] map;
    private Player player;

    // TODO туман войны
    // TODO загружать карту из файла

    public void Start()
    {

        string playerName = new InputWindow("Введите имя игрока").Show();

        PlayerType playerType = (PlayerType) new MenuWindow("Выберите тип игрока",
            Enum.GetNames(typeof(PlayerType))).Show();

        map = GenerateMap(Height, Width);
        player = new Player(playerName, playerType, new Coordinate(1, 1));

        ClearScreen();
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
