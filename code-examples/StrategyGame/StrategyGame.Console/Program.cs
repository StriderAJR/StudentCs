using StrategyGame.ConsoleGame.UI;
using StrategyGame.ConsoleGame.UI.CustomConsole;
using StrategyGame.Logic;
using StrategyGame.Logic.Models;
using System.Text;

namespace StrategyGame.ConsoleGame;

static class Program
{
    public static void Start(StrategyGameState game)
    {
        string playerName = new InputWindow("Введите имя игрока").Show();

        PlayerType playerType = (PlayerType) new MenuWindow("Выберите тип игрока",
            Enum.GetNames(typeof(PlayerType))).Show();

        game.StartNewGame(playerName, playerType);
        Player player = game.Player!;

        ClearScreen();
        while (true)
        {
            PrintMap(game);

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

    private static void PrintMap(StrategyGameState game)
    {
        MapCell[,] map = game.Map;
        Player player = game.Player!;

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

    private static void Main()
    {
        // отключаем отображение курсора via shared buffer
        GameConsole.Buffer.CursorVisible = false;

        // ограничить размер буфера, чтобы не появлялись полосы прокрутки
        Console.BufferHeight = Console.WindowHeight;
        Console.BufferWidth = Console.WindowWidth;

        MenuWindow mainMenu = new MenuWindow("Do you want to start game?", new[] { "Ok", "Cancel" }, "Menu");
        int menuButtonIndex = mainMenu.Show();
        if (menuButtonIndex == 0)
        {
            // OK - start game
            StrategyGameState game = new StrategyGameState((uint)(Console.WindowHeight - 1), (uint)Console.WindowWidth);
            Start(game);
        }

        // else - exit
    }
}
