using System.Text;

namespace StrategyGame.ConsoleGame;

static class Programm
{
    private static char[,] map = GenerateMap();
    private static int playerX = 1, playerY = 1;

    private static void Main()
    {
        // отключаем отображение курсора
        Console.CursorVisible = false;

        while (true)
        {
            PrintMap();

            ConsoleKey input = Console.ReadKey().Key;
            switch(input)
            {
                case ConsoleKey.W: 
                case ConsoleKey.UpArrow:
                    MovePlayer(-1, 0); break;
                case ConsoleKey.S: 
                case ConsoleKey.DownArrow: 
                    MovePlayer(1, 0); break;
                case ConsoleKey.D: 
                case ConsoleKey.RightArrow:
                    MovePlayer(0, 1); break;
                case ConsoleKey.A: 
                case ConsoleKey.LeftArrow:
                    MovePlayer(0, -1); break;
            }
        }
    }

    private static char[,] GenerateMap()
    {
        char[,] map = new char[10,10];
        for (int i = 0; i < 10; i++)
            for (int j = 0; j < 10; j++)
                map[i,j] = (i == 0 || i == 9 || j == 0 || j == 9) ? '#' : ' ';

        return map;
    }

    static void MovePlayer(int shiftX, int shiftY)
    {
        if (map[playerX + shiftX, playerY + shiftY] != '#')
        {
            playerY += shiftY;
            playerX += shiftX;
        }
    }

    private static void PrintMap()
    {
        // draw map
        
        // Simple way to print map, but with flickering:
        // Console.ForegroundColor = ConsoleColor.Gray;
        //for (int i = 0; i < map.GetLength(0); i++)
        //{
        //    for(int j = 0; j < map.GetLength(1); j++)
        //        Console.Write(string.Join("", map[i,j]));
        //    Console.WriteLine();
        //}

        // More complex, but without flickering (redraw over old drawing):
        StringBuilder sb = new StringBuilder();
        sb.Clear();
        for (int i = 0; i < map.GetLength(0); i++)
        {
            for (int j = 0; j < map.GetLength(1); j++)
                sb.Append(map[i, j]);
            sb.AppendLine();
        }
        Console.ForegroundColor = ConsoleColor.Gray;
        Console.SetCursorPosition(0, 0);
        Console.Write(sb.ToString());

        // draw player
        Console.ForegroundColor = ConsoleColor.Red;
        Console.SetCursorPosition(playerY, playerX);
        Console.Write('@');
    }
}
