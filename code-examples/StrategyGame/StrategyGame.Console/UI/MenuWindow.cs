using System;

namespace StrategyGame.ConsoleGame.UI;

public class MenuWindow : ConsoleWindow<int>
{
    private readonly string[] items;
    private int selectedItemIndex = 0;

    /// <summary>
    /// Custom constructor: explicit width/height and position
    /// </summary>
    public MenuWindow(string message, string[] items, string? title, int width, int height, Coordinate position)
        : base(message, title, width, height, position)
    {
        this.items = items ?? Array.Empty<string>();
    }

    /// <summary>
    /// Auto constructor: computes size based on message + menu items and then calls base custom ctor.
    /// This avoids calling virtual methods from the base constructor.
    /// Now optimized to call CalculateAutoParams only once by forwarding a tuple to a private ctor.
    /// </summary>
    public MenuWindow(string message, string[] items, string? title = null,
        WindowPosition windowPosition = WindowPosition.Center, WindowSize windowSize = WindowSize.Auto)
        : this(message, items, title, CalculateAutoParams(message, items, windowPosition, windowSize))
    {
    }

    // Private helper constructor that accepts the precomputed tuple (width, height, position)
    private MenuWindow(string message, string[] items, string? title,
        (int width, int height, Coordinate position) autoParams)
        : base(message, title, autoParams.width, autoParams.height, autoParams.position)
    {
        this.items = items ?? Array.Empty<string>();
    }

    // Helper to compute required width/height/position for menu auto mode.
    private static (int width, int height, Coordinate position) CalculateAutoParams(
        string message, string[] items, WindowPosition windowPosition, WindowSize windowSize)
    {
        int consoleWidth = Console.WindowWidth;
        int consoleHeight = Console.WindowHeight;

        if (windowSize == WindowSize.FullScreen)
            return (consoleWidth, consoleHeight, new Coordinate(0, 0));

        string[] messageLines = string.IsNullOrEmpty(message) ? Array.Empty<string>() : message.Split('\n');
        int maxLineLen = 0;
        foreach (var l in messageLines)
            if (l.Length > maxLineLen) maxLineLen = l.Length;

        int maxItemLen = 0;
        if (items != null)
            foreach (var it in items)
                if (it != null && it.Length > maxItemLen) maxItemLen = it.Length;

        int messageWidth = maxLineLen + 4;     // padding for borders
        int itemsWidth = maxItemLen + 6;       // "[ {item} ]" + small margin
        int effWidth = Math.Clamp(Math.Max(messageWidth, itemsWidth), 10, consoleWidth);

        int messageHeight = messageLines.Length + 2;
        int itemsHeight = items == null || items.Length == 0 ? 0 : (items.Length == 1 ? 3 : items.Length * 2 + 1);
        int effHeight = Math.Clamp(Math.Max(messageHeight, itemsHeight), 3, consoleHeight);

        Coordinate effPosition = windowPosition switch
        {
            WindowPosition.Center => new Coordinate(
                Math.Max(0, (consoleWidth - effWidth) / 2),
                Math.Max(0, (consoleHeight - effHeight) / 2)),
            WindowPosition.Left => new Coordinate(0, Math.Max(0, (consoleHeight - effHeight) / 2)),
            WindowPosition.Right => new Coordinate(
                Math.Max(0, consoleWidth - effWidth),
                Math.Max(0, (consoleHeight - effHeight) / 2)),
            WindowPosition.Top => new Coordinate(Math.Max(0, (consoleWidth - effWidth) / 2), 0),
            WindowPosition.Bottom => new Coordinate(
                Math.Max(0, (consoleWidth - effWidth) / 2),
                Math.Max(0, consoleHeight - effHeight)),
            _ => new Coordinate(0, 0)
        };

        return (effWidth, effHeight, effPosition);
    }

    /// <summary>
    /// Interactive logic moved here; base.Show() will call this then ClearScreen().
    /// </summary>
    protected override int ShowInternal()
    {
        bool shouldContinue = true;
        while (shouldContinue)
        {
            DrawMenu();

            ConsoleKey input = Console.ReadKey(true).Key;
            switch (input)
            {
                case ConsoleKey.Enter:
                    shouldContinue = false;
                    break;
                case ConsoleKey.UpArrow:
                case ConsoleKey.LeftArrow:
                    selectedItemIndex = selectedItemIndex - 1 >= 0
                        ? selectedItemIndex - 1
                        : items.Length - 1;
                    break;
                case ConsoleKey.DownArrow:
                case ConsoleKey.RightArrow:
                    selectedItemIndex = selectedItemIndex + 1 < items.Length
                        ? selectedItemIndex + 1
                        : 0;
                    break;
            }
        }

        return selectedItemIndex;
    }

    private void DrawMenu()
    {
        base.Draw();

        if (items == null || items.Length == 0)
            return;

        int baseY = position.Y + height - 3;

        if (items.Length == 1)
            DrawButton(items[0], 0, position.X + width / 2, baseY, centered: true);
        else if (items.Length == 2)
        {
            DrawButton(items[0], 0, position.X + 4, baseY);
            DrawButton(items[1], 1, position.X + width - items[1].Length - 6, baseY);
        }
        else
        {
            int totalHeight = items.Length * 2 - 1;
            int startY = position.Y + (height - totalHeight) / 2;
            for (int i = 0; i < items.Length; i++)
                DrawButton(items[i], i, position.X + width / 2, startY + i * 2, centered: true);
        }
    }

    /// <summary>
    /// Отрисовка одной кнопки
    /// </summary>
    private void DrawButton(string text, int index, int x, int y, bool centered = false)
    {
        string b = $"[ {text} ]";
        int bx = centered ? x - b.Length / 2 : x;

        Console.SetCursorPosition(bx, y);
        if (selectedItemIndex == index)
        {
            Console.BackgroundColor = ConsoleColor.Gray;
            Console.ForegroundColor = ConsoleColor.Black;
        }
        Console.Write(b);
        Console.ResetColor();
    }
}
