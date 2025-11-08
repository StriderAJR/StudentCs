using StrategyGame.ConsoleGame.Windows;
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
        // InputWindow nameInput = new InputWindow("Введите имя игрока");
        // string playerName = nameInput.Show();

        // Если объект одноразовый, то можно сделать вот так хитро:
        string playerName = new InputWindow("Введите имя игрока").Show();

        // за счет того, что у нас индексы пунктов меню совпадают с индексами в enum, то можно тоже сделать хитро:
        PlayerType playerType = (PlayerType) new MenuWindow("Выберите тип игрока", Enum.GetNames(typeof(PlayerType))).Show();
        
        map = GenerateMap(Height, Width);
        player = new Player(playerName, playerType, new Coordinate(1, 1));

        ClearScreen();
        while (true)
        {
            PrintMap();

            ConsoleKey input = Console.ReadKey().Key;
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
        // draw map
        StringBuilder sb = new StringBuilder();
        sb.Clear();
        for (int i = 0; i < map.GetLength(0); i++)
        {
            for (int j = 0; j < map.GetLength(1); j++)
                sb.Append(map[i, j].ToChar());
            sb.AppendLine();
        }
        Console.ForegroundColor = ConsoleColor.Gray;
        Console.SetCursorPosition(0, 0);
        Console.Write(sb.ToString());

        // draw player
        Console.ForegroundColor = ConsoleColor.Red;
        Console.SetCursorPosition(player.Y, player.X);
        Console.Write('@');
    }

    private static void ClearScreen()
    {
        StringBuilder sb = new StringBuilder();
        for(int i = 0; i <= Console.WindowHeight; i++) {
            sb.AppendLine(new string(' ', Console.WindowWidth));
        }
        Console.SetCursorPosition(0,0);
        Console.Write(sb.ToString());
    }
}
