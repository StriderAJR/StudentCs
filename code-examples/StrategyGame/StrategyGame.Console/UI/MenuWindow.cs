using System;

namespace StrategyGame.ConsoleGame.UI;

public class MenuWindow : ConsoleWindow<int>
{
    private readonly string[] items;
    private readonly ButtonPosition buttonPosition;
    private int selectedItemIndex = 0;

    /// <summary>
    /// Custom constructor: explicit width/height and position
    /// </summary>
    public MenuWindow(
        string message,
        string[] items,
        string? title,
        int width,
        int height,
        Coordinate position,
        ButtonPosition buttonPosition = ButtonPosition.CenterVertically)
        : base(message, title, width, height, position)
    {
        this.items = items ?? Array.Empty<string>();
        this.buttonPosition = buttonPosition;
    }

    /// <summary>
    /// Auto constructor: computes size based on message + menu items and then calls base custom ctor.
    /// This avoids calling virtual methods from the base constructor.
    /// </summary>
    public MenuWindow(
        string message,
        string[] items,
        string? title = null,
        WindowPosition windowPosition = WindowPosition.Center,
        WindowSize windowSize = WindowSize.Auto,
        ButtonPosition buttonPosition = ButtonPosition.CenterVertically)
        : this(
            message,
            items,
            title,
            CalculateAutoParams(message, items, windowPosition, windowSize, buttonPosition),
            buttonPosition)
    {
    }

    // Private helper constructor that accepts the precomputed tuple (width, height, position)
    private MenuWindow(
        string message,
        string[] items,
        string? title,
        (int width, int height, Coordinate position) autoParams,
        ButtonPosition buttonPosition)
        : base(message, title, autoParams.width, autoParams.height, autoParams.position)
    {
        this.items = items ?? Array.Empty<string>();
        this.buttonPosition = buttonPosition;
    }

    // Helper to compute required width/height/position for menu auto mode.
    private static (int width, int height, Coordinate position) CalculateAutoParams(
        string message,
        string[] items,
        WindowPosition windowPosition,
        WindowSize windowSize,
        ButtonPosition buttonPosition)
    {
        int consoleWidth = Console.WindowWidth;
        int consoleHeight = Console.WindowHeight;

        if (windowSize == WindowSize.FullScreen)
            return (consoleWidth, consoleHeight, new Coordinate(0, 0));

        string[] messageLines = string.IsNullOrEmpty(message)
            ? Array.Empty<string>()
            : message.Split('\n');

        int maxLineLen = 0;
        foreach (var l in messageLines)
            if (l.Length > maxLineLen)
                maxLineLen = l.Length;

        int messageWidth = maxLineLen + 4; // padding for borders

        int effWidth;
        int itemsInteriorRows = 0;

        if (items != null && items.Length > 0 &&
            buttonPosition == ButtonPosition.Horizontal)
        {
            // Horizontal layout: place all buttons on one row -> compute required
            // horizontal space
            int n = items.Length;
            int[] btnLens = new int[n];
            int totalBtnLen = 0;
            for (int i = 0; i < n; i++)
            {
                int len = (items[i]?.Length ?? 0) + 4; // "[ {text} ]" => +4
                btnLens[i] = len;
                totalBtnLen += len;
            }

            int minSpacing = 3; // minimal spaces between buttons
            int totalNeededInterior = totalBtnLen + Math.Max(0, (n - 1) * minSpacing);

            // totalNeededInterior is interior width; convert to window width by adding
            // 4 (padding)
            int itemsWidth = totalNeededInterior + 4;

            effWidth = Math.Clamp(Math.Max(messageWidth, itemsWidth), 10, consoleWidth);

            // single row for items
            itemsInteriorRows = 1;
        }
        else
        {
            // Vertical/default layout (existing behavior)
            int maxItemLen = 0;
            if (items != null)
                foreach (var it in items)
                    if (it != null && it.Length > maxItemLen)
                        maxItemLen = it.Length;

            int itemsWidth = maxItemLen + 6; // "[ {item} ]" + small margin
            effWidth = Math.Clamp(Math.Max(messageWidth, itemsWidth), 10, consoleWidth);

            // compute interior rows
            if (items != null && items.Length > 0)
                itemsInteriorRows = items.Length <= 2 ? 1 : items.Length * 2 - 1;
            else
                itemsInteriorRows = 0;
        }

        int separator = messageLines.Length > 0 ? 1 : 0;
        int interiorRows = messageLines.Length + separator + itemsInteriorRows;

        int effHeight = Math.Clamp(interiorRows + 2, 3, consoleHeight); // +2 for borders

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

        string[] messageLines = string.IsNullOrEmpty(message)
            ? Array.Empty<string>()
            : message.Split('\n');
        int messageCount = messageLines.Length;
        int separator = messageCount > 0 ? 1 : 0;

        int contentStartY = position.Y + 1 + messageCount + separator; // below message + sep
        int baseY = position.Y + height - 3; // legacy bottom row

        if (buttonPosition == ButtonPosition.Horizontal)
        {
            int y = messageCount > 0 ? contentStartY : baseY;

            int interiorStart = position.X + 2;
            int interiorEnd = position.X + width - 3;
            int availableSpan = Math.Max(1, interiorEnd - interiorStart + 1);

            int n = items.Length;
            int[] btnLens = new int[n];
            int totalBtnLen = 0;
            for (int i = 0; i < n; i++)
            {
                int len = (items[i]?.Length ?? 0) + 4; // "[ {text} ]"
                btnLens[i] = len;
                totalBtnLen += len;
            }

            if (n == 1)
            {
                // single button centered
                DrawButton(items[0], 0, position.X + width / 2, y, centered: true);
            }
            else if (n == 2)
            {
                // two buttons at edges (left and right)
                int start0 = interiorStart;
                int start1 = interiorEnd - btnLens[1] + 1;
                DrawButton(items[0], 0, start0, y);
                DrawButton(items[1], 1, start1, y);
            }
            else if (n == 3)
            {
                // two at edges + one center
                int start0 = interiorStart;
                int centerX = position.X + width / 2;
                int start2 = interiorEnd - btnLens[2] + 1;
                DrawButton(items[0], 0, start0, y);
                DrawButton(items[1], 1, centerX, y, centered: true);
                DrawButton(items[2], 2, start2, y);
            }
            else
            {
                // n >= 4: distribute centers evenly between interiorStart and interiorEnd
                double avail = availableSpan;
                for (int i = 0; i < n; i++)
                {
                    // positions at fractions (i+1)/(n+1) of the available span
                    double center = interiorStart + ((i + 1) * avail) / (n + 1);
                    int startX = (int)Math.Round(center - btnLens[i] / 2.0);

                    // clamp
                    startX = Math.Max(startX, interiorStart);
                    startX = Math.Min(startX, interiorEnd - btnLens[i] + 1);

                    DrawButton(items[i], i, startX, y);
                }
            }
        }
        else // CenterVertically (stacked by default)
        {
            if (messageCount > 0)
            {
                int contentStart = contentStartY;

                if (items.Length == 1)
                    DrawButton(items[0], 0, position.X + width / 2, contentStart, centered: true);
                else if (items.Length == 2)
                {
                    DrawButton(items[0], 0, position.X + 4, contentStart);
                    DrawButton(items[1], 1, position.X + width - items[1].Length - 6, contentStart);
                }
                else
                {
                    for (int i = 0; i < items.Length; i++)
                        DrawButton(items[i], i, position.X + width / 2, contentStart + i * 2, centered: true);
                }
            }
            else
            {
                // legacy behavior when there's no message (centered / bottom layout)
                int bottomBaseY = baseY;

                if (items.Length == 1)
                    DrawButton(items[0], 0, position.X + width / 2, bottomBaseY, centered: true);
                else if (items.Length == 2)
                {
                    DrawButton(items[0], 0, position.X + 4, bottomBaseY);
                    DrawButton(items[1], 1, position.X + width - items[1].Length - 6, bottomBaseY);
                }
                else
                {
                    int totalHeight = items.Length * 2 - 1;
                    int startY = position.Y + (height - totalHeight) / 2;
                    for (int i = 0; i < items.Length; i++)
                        DrawButton(items[i], i, position.X + width / 2, startY + i * 2, centered: true);
                }
            }
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
