namespace StrategyGame.ConsoleGame.UI.CustomConsole;

public static class GameConsole
{
    public static ConsoleBuffer Buffer { get; private set; }

    static GameConsole()
    {
        // initialize buffer with current console size
        Buffer = new ConsoleBuffer();
    }

    public static ConsoleKeyInfo ReadKey(bool intercept = false)
    {
        return Buffer.ReadKey(intercept);
    }

    public static int WindowWidth => Buffer.WindowWidth;
    public static int WindowHeight => Buffer.WindowHeight;
}
