using System;

namespace StrategyGame.ConsoleGame.UI;

/// <summary>
/// Базовое консольное окно с рамкой, опциональным заголовком и текстом
/// </summary>
public abstract class ConsoleWindow<TResult>
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

        int consoleWidth = Console.WindowWidth;
        int consoleHeight = Console.WindowHeight;

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

        int consoleWidth = Console.WindowWidth;
        int consoleHeight = Console.WindowHeight;
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

        this.position = effPosition;
        this.width = effWidth;
        this.height = effHeight;
    }

    /// <summary>
    /// Template Show: calls derived ShowInternal and guarantees ClearScreen()
    /// runs before returning.
    /// </summary>
    public TResult Show()
    {
        TResult result = ShowInternal();
        ClearScreen();
        return result;
    }

    /// <summary>
    /// Each derived window implements its interactive logic and returns its
    /// typed result.
    /// </summary>
    protected abstract TResult ShowInternal();

    /// <summary>
    /// Очищает весь видимый экран, заполняя каждую строку пробелами через
    /// Console.Write() (не использует Console.Clear()).
    /// Восстанавливает цвета и видимость курсора, в конце устанавливает
    /// курсор в верхний левый угол (0,0).
    /// </summary>
    private void ClearScreen()
    {
        var originalFg = Console.ForegroundColor;
        var originalBg = Console.BackgroundColor;
        bool originalCursorVisible = Console.CursorVisible;

        try
        {
            Console.CursorVisible = false;

            int w = Math.Max(Console.WindowWidth, 1);
            int h = Math.Max(Console.WindowHeight, 1);

            string blankLine = new string(' ', w);

            for (int row = 0; row < h; row++)
            {
                Console.SetCursorPosition(0, row);
                Console.Write(blankLine);
            }

            // position cursor like Console.Clear()
            Console.SetCursorPosition(0, 0);
        }
        finally
        {
            Console.ForegroundColor = originalFg;
            Console.BackgroundColor = originalBg;
            Console.CursorVisible = originalCursorVisible;
        }
    }

    /// <summary>
    /// Hook kept for future use but not called from the base ctor to avoid
    /// virtual calls during construction.
    /// Derived types should calculate auto sizes themselves and call the
    /// custom ctor.
    /// </summary>
    protected virtual void AdjustAutoSize(ref int width, ref int height,
        int consoleWidth, int consoleHeight)
    {
        // no-op by default
    }

    /// <summary>
    /// Рисует окно: рамку, заголовок и текст
    /// </summary>
    protected virtual void Draw()
    {
        Console.ForegroundColor = ConsoleColor.Gray;

        // рамка
        for (int i = 0; i < height; i++)
        {
            Console.SetCursorPosition(position.X, position.Y + i);
            for (int j = 0; j < width; j++)
            {
                char c =
                    i == 0 && j == 0 ? '┌' :
                    i == 0 && j == width - 1 ? '┐' :
                    i == height - 1 && j == 0 ? '└' :
                    i == height - 1 && j == width - 1 ? '┘' :
                    i == 0 || i == height - 1 ? '─' :
                    j == 0 || j == width - 1 ? '│' : ' ';
                Console.Write(c);
            }
        }

        // заголовок
        if (!string.IsNullOrEmpty(title))
        {
            Console.SetCursorPosition(position.X + 2, position.Y);
            Console.Write($"[{title}]");
        }

        // текст
        if (!string.IsNullOrEmpty(message))
        {
            string[] lines = message.Split('\n');
            for (int i = 0; i < lines.Length && i < height - 2; i++)
            {
                Console.SetCursorPosition(position.X + 2, position.Y + 1 + i);
                Console.Write(lines[i]);
            }
        }

        Console.ResetColor();
    }
}