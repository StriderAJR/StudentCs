using System;

namespace StrategyGame.ConsoleGame.UI;

/// <summary>
/// Окно с полем ввода текста и кнопкой Ok (Enter)
/// </summary>
public class InputWindow : ConsoleWindow<string>
{
    private string input = string.Empty;

    /// <summary>
    /// Custom constructor: explicit size and position
    /// </summary>
    public InputWindow(string message, string? title, int width, int height, Coordinate position)
        : base(message, title, width, height, position)
    {
    }

    /// <summary>
    /// Auto constructor: computes size (message + optional separator + input line) and then calls base custom ctor.
    /// </summary>
    public InputWindow(
        string message, string? title = null, WindowPosition windowPosition = WindowPosition.Center,
        WindowSize windowSize = WindowSize.Auto)
        : this(message, title, CalculateAutoParams(message, windowPosition, windowSize))
    {
    }

    // Private forwarding ctor that accepts computed params.
    private InputWindow(string message, string? title, (int width, int height, Coordinate position) autoParams)
        : base(message, title, autoParams.width, autoParams.height, autoParams.position)
    {
    }

    private static (int width, int height, Coordinate position) CalculateAutoParams(string message, WindowPosition windowPosition, WindowSize windowSize)
    {
        int consoleWidth = Console.WindowWidth;
        int consoleHeight = Console.WindowHeight;

        if (windowSize == WindowSize.FullScreen)
            return (consoleWidth, consoleHeight, new Coordinate(0, 0));

        string[] messageLines = string.IsNullOrEmpty(message) ? Array.Empty<string>() : message.Split('\n');
        int maxLineLen = 0;
        foreach (var l in messageLines)
            if (l.Length > maxLineLen) maxLineLen = l.Length;

        int effWidth = Math.Clamp(maxLineLen + 4, 10, consoleWidth);

        // Add one separator row after message only when a message exists,
        // plus one row for the actual input field.
        int separator = messageLines.Length > 0 ? 1 : 0;
        int inputFieldHeight = 1;

        int interiorRows = messageLines.Length + separator + inputFieldHeight;
        int effHeight = Math.Clamp(interiorRows + 2, 3, consoleHeight); // +2 for top/bottom borders

        Coordinate effPosition = windowPosition switch
        {
            WindowPosition.Center => new Coordinate(Math.Max(0, (consoleWidth - effWidth) / 2), Math.Max(0, (consoleHeight - effHeight) / 2)),
            WindowPosition.Left => new Coordinate(0, Math.Max(0, (consoleHeight - effHeight) / 2)),
            WindowPosition.Right => new Coordinate(Math.Max(0, consoleWidth - effWidth), Math.Max(0, (consoleHeight - effHeight) / 2)),
            WindowPosition.Top => new Coordinate(Math.Max(0, (consoleWidth - effWidth) / 2), 0),
            WindowPosition.Bottom => new Coordinate(Math.Max(0, (consoleWidth - effWidth) / 2), Math.Max(0, consoleHeight - effHeight)),
            _ => new Coordinate(0, 0)
        };

        return (effWidth, effHeight, effPosition);
    }

    /// <summary>
    /// Interactive logic moved here; base.Show() will call this then ClearScreen().
    /// </summary>
    protected override string ShowInternal()
    {
        bool finished = false;

        // compute input Y relative to message and optional separator
        string[] messageLines = string.IsNullOrEmpty(message) ? Array.Empty<string>() : message.Split('\n');
        int messageCount = messageLines.Length;
        int separator = messageCount > 0 ? 1 : 0;
        int inputY = position.Y + 1 + messageCount + separator; // top border + message lines + optional separator

        while (!finished)
        {
            base.Draw();

            // поле ввода
            int inputX = position.X + 2;
            Console.SetCursorPosition(inputX, inputY);
            Console.Write(new string(' ', width - 4));
            Console.SetCursorPosition(inputX, inputY);
            Console.Write(input);

            Console.SetCursorPosition(inputX + input.Length, inputY);

            ConsoleKeyInfo key = Console.ReadKey(true);

            if (key.Key == ConsoleKey.Enter)
            {
                finished = true;
            }
            else if (key.Key == ConsoleKey.Backspace)
            {
                if (input.Length > 0)
                    input = input[..^1];
            }
            else if (!char.IsControl(key.KeyChar))
            {
                input += key.KeyChar;
            }
        }

        return input;
    }
}
