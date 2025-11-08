using StrategyGame.ConsoleGame.UI;
using StrategyGame.ConsoleGame.UI.CustomConsole;

namespace StrategyGame.ConsoleGame;

static class Program
{
    private static void Main()
    {
        // отключаем отображение курсора via shared buffer
        GameConsole.Buffer.CursorVisible = false;

        // ограничить размер буфера, чтобы не появлялись полосы прокрутки
        Console.BufferHeight = Console.WindowHeight;
        Console.BufferWidth = Console.WindowWidth;

        MenuWindow mainMenu = new MenuWindow("Do you want to start game?", ["Ok", "Cancel"], "Menu", 
            buttonPosition: ButtonPosition.Horizontal);
        int menuButtonIndex = mainMenu.Show();
        if (menuButtonIndex == 0)
        {
            // OK - start game
            StrategyGame game = new StrategyGame((uint)(Console.WindowHeight - 1), (uint)Console.WindowWidth);
            game.Start();
        }

        // else - exit
    }
}
