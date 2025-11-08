namespace StrategyGame.ConsoleGame.UI.CustomConsole;

public sealed record ConsoleBufferSnapshot(
    int Width,
    int Height,
    char[,] Chars,
    ConsoleColor[,] ForegroundColors,
    ConsoleColor[,] BackgroundColors)
{
    public int CursorLeft;
    public int CursorTop;
    public ConsoleColor ForegroundColor;
    public ConsoleColor BackgroundColor;

    public ConsoleBufferSnapshot(int width, int height)
        : this(width, height, new char[height, width], new ConsoleColor[height, width],
            new ConsoleColor[height, width])
    {
    }
}
