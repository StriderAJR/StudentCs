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

    private void DrawFrame(string? title)
    {
        if (Width <= 0 || Height <= 0)
            return;

        var prevColor = GameConsole.ForegroundColor;
        GameConsole.ForegroundColor = ConsoleColor.Gray;

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
            GameConsole.SetCursorPosition(X + 2, Y);
            GameConsole.Write($"[{title}]");
        }

        GameConsole.ForegroundColor = prevColor;
    }
}
