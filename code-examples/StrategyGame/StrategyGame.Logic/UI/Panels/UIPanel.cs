using StrategyGame.ConsoleGame.UI.CustomConsole;

namespace StrategyGame.ConsoleGame.UI.Panels;

public abstract class UIPanel
{
    protected int X { get; }
    protected int Y { get; }
    protected int Width { get; }
    protected int Height { get; }

    protected UIPanel(int x, int y, int width, int height)
    {
        X = x;
        Y = y;
        Width = width;
        Height = height;
    }

    public void Draw(string? title = null)
    {
        DrawFrame(title);
        DrawContent();
    }

    protected abstract void DrawContent();

    // Derived panels can override this to color the frame (borders) differently,
    // e.g. to reflect player color. Default is gray.
    protected virtual ConsoleColor GetBorderColor() => ConsoleColor.Gray;

    private void DrawFrame(string? title)
    {
        if (Width <= 0 || Height <= 0)
            return;

        var prevColor = GameConsole.ForegroundColor;
        GameConsole.ForegroundColor = GetBorderColor();

        for (int row = 0; row < Height; row++)
        {
            GameConsole.SetCursorPosition(X, Y + row);

            if (row == 0)
            {
                GameConsole.Write('┌');
                if (Width > 1) GameConsole.Write(new string('─', Width - 2));
                if (Width > 1) GameConsole.Write('┐');
            }
            else if (row == Height - 1)
            {
                GameConsole.Write('└');
                if (Width > 1) GameConsole.Write(new string('─', Width - 2));
                if (Width > 1) GameConsole.Write('┘');
            }
            else
            {
                GameConsole.Write('│');
                if (Width > 1) GameConsole.Write(new string(' ', Width - 2));
                if (Width > 1) GameConsole.Write('│');
            }
        }

        if (!string.IsNullOrEmpty(title) && Width >= 6)
        {
            // write title using border color to match frame
            GameConsole.SetCursorPosition(X + 2, Y);
            GameConsole.ForegroundColor = GetBorderColor();
            GameConsole.Write($"[{title}]");
        }

        // ensure interior/content uses standard text color (not border color)
        GameConsole.ForegroundColor = ConsoleColor.Gray;
    }
}
