using System;

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
        int bx = StartX();
        Console.SetCursorPosition(bx, Y);
        if (selected)
        {
            Console.BackgroundColor = ConsoleColor.Gray;
            Console.ForegroundColor = ConsoleColor.Black;
        }
        Console.Write(Rendered);
        Console.ResetColor();
    }
}
