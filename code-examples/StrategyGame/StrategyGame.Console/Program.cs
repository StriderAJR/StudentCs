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

        int selectedBtnIndex = 0;
        string[] buttons = ["Ok", "Cancel"];
        bool shouldContinue = true;
        while (shouldContinue)
        {
            DrawWindow(5, 5, 40, 10, "Menu", "Do you want to start game?", buttons, selectedBtnIndex);

            ConsoleKey input = Console.ReadKey().Key;
            switch (input)
            {
                case ConsoleKey.Enter:
                    // some button selected
                    shouldContinue = false;
                    break;
                case ConsoleKey.UpArrow:
                case ConsoleKey.LeftArrow:
                    selectedBtnIndex = selectedBtnIndex - 1 >= 0
                        ? selectedBtnIndex - 1
                        : buttons.Length - 1;
                    break;

                case ConsoleKey.DownArrow:
                case ConsoleKey.RightArrow:
                    selectedBtnIndex = selectedBtnIndex + 1 < buttons.Length
                        ? selectedBtnIndex + 1
                        : 0;
                    break;
            }
        }

        if (selectedBtnIndex == 0)
            // OK - start game
            StartGame();

        // else - exit
    }

    private static void StartGame()
    {
        while (true)
        {
            PrintMap();

            ConsoleKey input = Console.ReadKey().Key;
            switch (input)
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
        char[,] map = new char[10, 10];
        for (int i = 0; i < 10; i++)
            for (int j = 0; j < 10; j++)
                map[i, j] = (i == 0 || i == 9 || j == 0 || j == 9) ? '#' : ' ';

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

    static void DrawWindow(
        int x, int y,
        int width, int height,
        string title, string text,
        string[] buttons, int selectedBtnIndex)
    {
        Console.ForegroundColor = ConsoleColor.Gray;

        // borders
        for (int i = 0; i < height; i++)
        {
            Console.SetCursorPosition(x, y + i);
            for (int j = 0; j < width; j++)
            {
                char c =
                    (i == 0 && j == 0) ? '┌' :
                    (i == 0 && j == width - 1) ? '┐' :
                    (i == height - 1 && j == 0) ? '└' :
                    (i == height - 1 && j == width - 1) ? '┘' :
                    (i == 0 || i == height - 1) ? '─' :
                    (j == 0 || j == width - 1) ? '│' : ' ';
                Console.Write(c);
            }
        }

        // header
        Console.SetCursorPosition(x + 2, y);
        Console.Write($"[{title}]");

        // text
        string[] lines = text.Split('\n');
        for (int i = 0; i < lines.Length && i < height - 6; i++)
        {
            Console.SetCursorPosition(x + 2, y + 2 + i);
            Console.Write(lines[i]);
        }

        // buttons
        if (buttons == null || buttons.Length == 0)
            return;

        int baseY = y + height - 3;

        // if buttons count = 1 - draw in the window center
        // if buttons count = 2 - draw on the left and right
        // uf buttons count > 2 - draw buttons vertically in the window center 

        if (buttons.Length == 1)
        {
            string b = $"[ {buttons[0]} ]";
            int bx = x + width / 2 - b.Length / 2;
            Console.SetCursorPosition(bx, baseY);
            if (selectedBtnIndex == 0)
            {
                Console.BackgroundColor = ConsoleColor.Gray;
                Console.ForegroundColor = ConsoleColor.Black;
            }
            Console.Write(b);
            Console.ResetColor();
        }
        else if (buttons.Length == 2)
        {
            string b1 = $"[ {buttons[0]} ]";
            string b2 = $"[ {buttons[1]} ]";
            int b1x = x + 4;
            int b2x = x + width - b2.Length - 4;

            Console.SetCursorPosition(b1x, baseY);
            if (selectedBtnIndex == 0)
            {
                Console.BackgroundColor = ConsoleColor.Gray;
                Console.ForegroundColor = ConsoleColor.Black;
            }
            Console.Write(b1);
            Console.ResetColor();

            Console.SetCursorPosition(b2x, baseY);
            if (selectedBtnIndex == 1)
            {
                Console.BackgroundColor = ConsoleColor.Gray;
                Console.ForegroundColor = ConsoleColor.Black;
            }
            Console.Write(b2);
            Console.ResetColor();
        }
        else // more than 2 buttons
        {
            int totalHeight = buttons.Length * 2 - 1;
            int startY = y + (height - totalHeight) / 2;
            for (int i = 0; i < buttons.Length; i++)
            {
                string b = $"[ {buttons[i]} ]";
                int bx = x + width / 2 - b.Length / 2;
                Console.SetCursorPosition(bx, startY + i * 2);
                if (selectedBtnIndex == i)
                {
                    Console.BackgroundColor = ConsoleColor.Gray;
                    Console.ForegroundColor = ConsoleColor.Black;
                }
                Console.Write(b);
                Console.ResetColor();
            }
        }
    }
}
