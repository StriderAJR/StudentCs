namespace StrategyGame.ConsoleGame.UI.CustomConsole;

public sealed class ConsoleBufferSnapshot(
    int width, int height, char[,] chars, 
    ConsoleColor[,] foregroundColors, ConsoleColor[,] backgroundColors)
{
    public int width = width;
    public int height = height;

    public char[,] chars = chars;
    public ConsoleColor[,] ForegroundColors = foregroundColors;
    public ConsoleColor[,] BackgroundColors = backgroundColors;

    public int CursorLeft;
    public int CursorTop;
    public ConsoleColor ForegroundColor;
    public ConsoleColor BackgroundColor;

    public ConsoleBufferSnapshot(int width, int height)
        : this(width, height, new char[height, width], new ConsoleColor[height, width], new ConsoleColor[height, width])
    {
    }
}
