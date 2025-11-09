using StrategyGame.ConsoleGame.Game;
using StrategyGame.ConsoleGame.UI.CustomConsole;

namespace StrategyGame.ConsoleGame.UI.Windows;

/// <summary>
/// Базовое консольное окно с рамкой, опциональным заголовком и текстом
/// </summary>
public class ConsoleWindow<TResult>
{
    protected readonly Coordinate position;
    protected readonly int width;
    protected readonly int height;
    protected readonly string? title;
    protected readonly string? message;

    /// <summary>
    /// Custom constructor: explicit width/height and explicit position
    /// </summary>
    public ConsoleWindow(string message, string? title, int width, int height,
        Coordinate position)
    {
        this.message = message;
        this.title = title;

        int consoleWidth = GameConsole.WindowWidth;
        int consoleHeight = GameConsole.WindowHeight;

        this.width = Math.Clamp(width, 1, consoleWidth);
        this.height = Math.Clamp(height, 1, consoleHeight);

        int x = Math.Clamp(position.X, 0, Math.Max(0, consoleWidth - this.width));
        int y = Math.Clamp(position.Y, 0, Math.Max(0, consoleHeight - this.height));
        this.position = new Coordinate(x, y);
    }

    /// <summary>
    /// Auto constructor: computes size based on message and positions via
    /// WindowPosition / WindowSize.
    /// Note: derived types that need additional sizing should compute size
    /// themselves and call the custom ctor to avoid virtual calls from base
    /// ctor.
    /// </summary>
    public ConsoleWindow(
        string message,
        string? title = null,
        WindowPosition windowPosition = WindowPosition.Center,
        WindowSize windowSize = WindowSize.Auto)
    {
        this.message = message;
        this.title = title;

        int consoleWidth = GameConsole.WindowWidth;
        int consoleHeight = GameConsole.WindowHeight;
        int effWidth;
        int effHeight;

        if (windowSize == WindowSize.FullScreen)
        {
            effWidth = consoleWidth;
            effHeight = consoleHeight;
        }
        else // Auto sizing by message
        {
            string[] lines = string.IsNullOrEmpty(message)
                ? Array.Empty<string>()
                : message.Split('\n');

            int maxLineLen = 0;
            foreach (var l in lines)
                if (l.Length > maxLineLen)
                    maxLineLen = l.Length;

            // Add padding for borders/margins
            effWidth = Math.Clamp(maxLineLen + 4, 10, consoleWidth);
            effHeight = Math.Clamp(lines.Length + 2, 3, consoleHeight);
        }

        // determine position from enum
        Coordinate effPosition = windowPosition switch
        {
            WindowPosition.Center => new Coordinate(
                Math.Max(0, (consoleWidth - effWidth) / 2),
                Math.Max(0, (consoleHeight - effHeight) / 2)),
            WindowPosition.Left => new Coordinate(
                0,
                Math.Max(0, (consoleHeight - effHeight) / 2)),
            WindowPosition.Right => new Coordinate(
                Math.Max(0, consoleWidth - effWidth),
                Math.Max(0, (consoleHeight - effHeight) / 2)),
            WindowPosition.Top => new Coordinate(
                Math.Max(0, (consoleWidth - effWidth) / 2),
                0),
            WindowPosition.Bottom => new Coordinate(
                Math.Max(0, (consoleWidth - effWidth) / 2),
                Math.Max(0, consoleHeight - effHeight)),
            _ => new Coordinate(0, 0)
        };

        position = effPosition;
        width = effWidth;
        height = effHeight;
    }

    /// <summary>
    /// Template Show: calls derived ShowInternal and guarantees ClearScreen()
    /// runs before returning.
    /// </summary>
    public TResult Show()
    {
        TResult result = ShowInternal();
        // restore console (buffer) after window closes
        ClearScreen();
        return result;
    }

    /// <summary>
    /// Each derived window implements its interactive logic and returns its
    /// typed result.
    /// Default implementation: draw window and wait for any key, return default(TResult).
    /// </summary>
    protected virtual TResult ShowInternal()
    {
        Draw();
        ReadKey(true);
        return default!;
    }

    /// <summary>
    /// Clear only area covered by window in buffer (not whole screen)
    /// </summary>
    protected void ClearScreen()
    {
        // clear window area
        for (int y = position.Y; y < position.Y + height && y < GameConsole.WindowHeight; y++)
        for (int x = position.X; x < position.X + width && x < GameConsole.WindowWidth; x++)
        {
            GameConsole.SetCursorPosition(x, y);
            GameConsole.Write(' ');
        }

        GameConsole.Flush();
    }

    protected ConsoleKeyInfo ReadKey(bool intercept = false) => GameConsole.ReadKey(intercept);

    /// <summary>
    /// Рисует окно: рамка, заголовок и текст
    /// </summary>
    public virtual void Draw()
    {
        var originalForegroundColor = GameConsole.ForegroundColor;
        var originalBackgroundColor = GameConsole.BackgroundColor;

        GameConsole.ForegroundColor = ConsoleColor.Gray;

        // рамка
        for (int i = 0; i < height; i++)
        {
            GameConsole.SetCursorPosition(position.X, position.Y + i);
            for (int j = 0; j < width; j++)
            {
                char c =
                    i == 0 && j == 0 ? '┌' :
                    i == 0 && j == width - 1 ? '┐' :
                    i == height - 1 && j == 0 ? '└' :
                    i == height - 1 && j == width - 1 ? '┘' :
                    i == 0 || i == height - 1 ? '─' :
                    j == 0 || j == width - 1 ? '│' : ' ';
                GameConsole.Write(c);
            }
        }

        // заголовок
        if (!string.IsNullOrEmpty(title))
        {
            GameConsole.SetCursorPosition(position.X + 2, position.Y);
            GameConsole.Write($"[{title}]");
        }

        // текст
        if (!string.IsNullOrEmpty(message))
        {
            string[] lines = message.Split('\n');
            for (int i = 0; i < lines.Length && i < height - 2; i++)
            {
                GameConsole.SetCursorPosition(position.X + 2, position.Y + 1 + i);
                GameConsole.Write(lines[i]);
            }
        }

        GameConsole.ForegroundColor = originalForegroundColor;
        GameConsole.BackgroundColor = originalBackgroundColor;

        GameConsole.Flush();
    }
}