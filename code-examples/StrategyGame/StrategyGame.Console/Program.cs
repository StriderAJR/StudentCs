using StrategyGame.ConsoleGame.UI;
using StrategyGame.ConsoleGame.UI.CustomConsole;
using System.Runtime.InteropServices;
using System.Threading;

namespace StrategyGame.ConsoleGame;

static class Program
{
    private static void Main()
    {
        SetupConsoleWindow();

        MenuWindow mainMenu = new MenuWindow("Do you want to start game?", new[] { "Ok", "Cancel" }, "Menu",
            buttonPosition: ButtonPosition.Horizontal);
        int menuButtonIndex = mainMenu.Show();
        if (menuButtonIndex == 0)
        {
            // OK - старт игры
            StrategyGame game = new StrategyGame();
            game.Start();
        }

        // иначе - выход
    }

    private static void SetupConsoleWindow()
    {
        // Попытаться развернуть окно консоли на весь экран (Windows)
        if (OperatingSystem.IsWindows())
        {
            try
            {
                NativeWin.Maximize();
                // дать ОС немного времени, чтобы изменить размер окна
                Thread.Sleep(80);
            }
            catch
            {
                // игнорировать ошибки
            }

            try
            {
                // попытаться установить максимальные размеры буфера/окна
                int w = Console.LargestWindowWidth;
                int h = Console.LargestWindowHeight;
                Console.SetBufferSize(w, h);
                Console.SetWindowSize(Math.Min(Console.WindowWidth, w), Math.Min(Console.WindowHeight, h));
            }
            catch
            {
                // некоторые консоли (IDE) не позволяют менять размер
            }
        }

        // ограничить размер буфера, чтобы не появлялись полосы прокрутки
        try
        {
            Console.BufferHeight = Console.WindowHeight;
            Console.BufferWidth = Console.WindowWidth;
        }
        catch
        {
            // игнорировать, если не поддерживается
        }

        // Гарантировать, что внутренний буфер GameConsole соответствует реальному размеру консоли
        GameConsole.Buffer.Init(Console.WindowWidth, Console.WindowHeight);
        // отключаем отображение курсора через общий буфер
        GameConsole.Buffer.CursorVisible = false;
    }

    private static class NativeWin
    {
        [DllImport("kernel32.dll", SetLastError = true)]
        internal static extern IntPtr GetConsoleWindow();

        [DllImport("user32.dll", SetLastError = true)]
        internal static extern bool ShowWindow(IntPtr hWnd, int nCmdShow);

        public const int SW_MAXIMIZE = 3;

        public static void Maximize()
        {
            var h = GetConsoleWindow();
            if (h != IntPtr.Zero)
                ShowWindow(h, SW_MAXIMIZE);
        }
    }
}
