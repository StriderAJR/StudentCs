using StrategyGame.ConsoleGame.UI.CustomConsole;

namespace StrategyGame.ConsoleGame.UI;

public class MenuWindow : ConsoleWindow<int>
{
    private readonly string[] items;
    private readonly ButtonPosition buttonPosition;
    private readonly Button[] buttons;
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
        this.buttons = CalculateButtons(this.items, this.buttonPosition, this.position,
            this.width, this.height, this.message);
    }

    /// <summary>
    /// Auto constructor: computes size based on message + menu items and then calls
    /// base custom ctor. This avoids calling virtual methods from the base ctor.
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
            CalculateAutoParams(message, items, windowPosition, windowSize,
                buttonPosition),
            buttonPosition)
    {
    }

    // Private helper constructor that accepts the precomputed tuple
    // (width, height, position)
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
        this.buttons = CalculateButtons(this.items, this.buttonPosition, this.position,
            this.width, this.height, this.message);
    }

    // Helper to compute required width/height/position for menu auto mode.
    private static (int width, int height, Coordinate position) CalculateAutoParams(
        string message,
        string[] items,
        WindowPosition windowPosition,
        WindowSize windowSize,
        ButtonPosition buttonPosition)
    {
        int consoleWidth = GameConsole.WindowWidth;
        int consoleHeight = GameConsole.WindowHeight;

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
            // Horizontal layout: compute required horizontal space
            int n = items.Length;
            int[] buttonLengths = new int[n];
            int totalButtonLength = 0;
            for (int i = 0; i < n; i++)
            {
                int len = (items[i]?.Length ?? 0) + 4; // "[ {text} ]" => +4
                buttonLengths[i] = len;
                totalButtonLength += len;
            }

            int minSpacing = 3; // minimal spaces between buttons
            int totalNeededInterior = totalButtonLength + Math.Max(0, (n - 1) * minSpacing);

            // interior width -> window width (+4 padding)
            int itemsWidth = totalNeededInterior + 4;

            effWidth = Math.Clamp(Math.Max(messageWidth, itemsWidth), 10, consoleWidth);

            // single row for items
            itemsInteriorRows = 1;
        }
        else
        {
            // Vertical/default layout
            int maxItemLen = 0;
            if (items != null)
                foreach (var it in items)
                    if (it != null && it.Length > maxItemLen)
                        maxItemLen = it.Length;

            int itemsWidth = maxItemLen + 6; // "[ {item} ]" + margin
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

            ConsoleKey input = ReadKey(true).Key;
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

        if (buttons == null || buttons.Length == 0)
            return;

        ConsoleBuffer buffer = GameConsole.Buffer;

        for (int i = 0; i < buttons.Length; i++)
        {
            var button = buttons[i];
            button.Draw(selected: selectedItemIndex == i);
        }

        buffer.Flush();
    }

    // Build buttons positions and sizes based on layout rules.
    private static Button[] CalculateButtons(
        string[] items,
        ButtonPosition buttonPosition,
        Coordinate position,
        int width,
        int height,
        string? message)
    {
        if (items == null || items.Length == 0)
            return Array.Empty<Button>();

        string[] messageLines = string.IsNullOrEmpty(message)
            ? Array.Empty<string>()
            : message.Split('\n');

        int messageCount = messageLines.Length;
        int separator = messageCount > 0 ? 1 : 0;
        int contentStartY = position.Y + 1 + messageCount + separator; // below message
        int baseY = position.Y + height - 3; // legacy bottom row

        int interiorStart = position.X + 2;
        int interiorEnd = position.X + width - 3;
        int availableSpan = Math.Max(1, interiorEnd - interiorStart + 1);

        int n = items.Length;
        Button[] result = new Button[n];

        if (buttonPosition == ButtonPosition.Horizontal)
        {
            int y = messageCount > 0 ? contentStartY : baseY;

            // calculate button rendered lengths
            int[] buttonLengths = new int[n];
            for (int i = 0; i < n; i++)
                buttonLengths[i] = (items[i]?.Length ?? 0) + 4; // "[ {text} ]"

            if (n == 1)
            {
                int centerX = position.X + width / 2;
                result[0] = new Button(items[0], centerX, y, buttonLengths[0], centered: true);
            }
            else if (n == 2)
            {
                int start0 = interiorStart;
                int start1 = interiorEnd - buttonLengths[1] + 1;
                result[0] = new Button(items[0], start0, y, buttonLengths[0]);
                result[1] = new Button(items[1], start1, y, buttonLengths[1]);
            }
            else if (n == 3)
            {
                int start0 = interiorStart;
                int centerX = position.X + width / 2;
                int start2 = interiorEnd - buttonLengths[2] + 1;
                result[0] = new Button(items[0], start0, y, buttonLengths[0]);
                result[1] = new Button(items[1], centerX, y, buttonLengths[1], centered: true);
                result[2] = new Button(items[2], start2, y, buttonLengths[2]);
            }
            else
            {
                double avail = availableSpan;
                for (int i = 0; i < n; i++)
                {
                    double center = interiorStart + ((i + 1) * avail) / (n + 1);
                    int startX = (int)Math.Round(center - buttonLengths[i] / 2.0);

                    // clamp
                    startX = Math.Max(startX, interiorStart);
                    startX = Math.Min(startX, interiorEnd - buttonLengths[i] + 1);

                    result[i] = new Button(items[i], startX, y, buttonLengths[i]);
                }
            }
        }
        else // CenterVertically (stacked)
        {
            if (messageCount > 0)
            {
                int contentStart = contentStartY;

                if (n == 1)
                    result[0] = new Button(items[0], position.X + width / 2, contentStart,
                        (items[0]?.Length ?? 0) + 4, centered: true);
                else if (n == 2)
                {
                    result[0] = new Button(items[0], position.X + 4, contentStart,
                        (items[0]?.Length ?? 0) + 4);
                    result[1] = new Button(items[1], interiorEnd - ((items[1]?.Length ?? 0) + 4) + 1,
                        contentStart, (items[1]?.Length ?? 0) + 4);
                }
                else
                {
                    for (int i = 0; i < n; i++)
                        result[i] = new Button(items[i], position.X + width / 2, contentStart + i * 2,
                            (items[i]?.Length ?? 0) + 4, centered: true);
                }
            }
            else
            {
                // legacy behavior when there's no message
                int bottomBaseY = baseY;

                if (n == 1)
                    result[0] = new Button(items[0], position.X + width / 2, bottomBaseY,
                        (items[0]?.Length ?? 0) + 4, centered: true);
                else if (n == 2)
                {
                    result[0] = new Button(items[0], position.X + 4, bottomBaseY,
                        (items[0]?.Length ?? 0) + 4);
                    result[1] = new Button(items[1], interiorEnd - ((items[1]?.Length ?? 0) + 4) + 1,
                        bottomBaseY, (items[1]?.Length ?? 0) + 4);
                }
                else
                {
                    int totalHeight = n * 2 - 1;
                    int startY = position.Y + (height - totalHeight) / 2;
                    for (int i = 0; i < n; i++)
                        result[i] = new Button(items[i], position.X + width / 2, startY + i * 2,
                            (items[i]?.Length ?? 0) + 4, centered: true);
                }
            }
        }

        return result;
    }
}
