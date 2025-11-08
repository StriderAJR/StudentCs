using StrategyGame.ConsoleGame.UI.CustomConsole;

namespace StrategyGame.ConsoleGame.UI;

public sealed class Button
{
    public string Text { get; }
    // X is either the start X (when Centered == false) or center X (when Centered == true)
    public int X { get; }
    public int Y { get; }
    public int Width { get; }
    public bool Centered { get; }

    public Button(string text, int x, int y, int width, bool centered = false)
    {
        Text = text;
        X = x;
        Y = y;
        Width = width;
        Centered = centered;
    }

    public int StartX()
    {
        return Centered ? X - Width / 2 : X;
    }

    public string Rendered => $"[ {Text} ]";

    public void Draw(bool selected)
    {
        int buttonStartX = StartX();
        var originalForeground = GameConsole.ForegroundColor;
        var originalBackground = GameConsole.BackgroundColor;

        if (selected)
        {
            GameConsole.BackgroundColor = ConsoleColor.Gray;
            GameConsole.ForegroundColor = ConsoleColor.Black;
        }

        GameConsole.SetCursorPosition(buttonStartX, Y);
        GameConsole.Write(Rendered);

        GameConsole.ForegroundColor = originalForeground;
        GameConsole.BackgroundColor = originalBackground;
    }
}
