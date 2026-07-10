using StrategyGame.ConsoleGame.UI.CustomConsole;
using StrategyGame.ConsoleGame.Game.MapTypes;

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
        // compute raw start
        int raw = Centered ? X - Width / 2 : X;
        // clamp to console bounds so button never draws outside the buffer
        raw = Math.Max(0, raw);
        raw = Math.Min(raw, Math.Max(0, GameConsole.WindowWidth - Width));
        return raw;
    }

    public string Rendered => $"[ {Text} ]";

    public void Draw(bool selected)
    {
        int buttonStartX = StartX();
        var originalForeground = GameConsole.ForegroundColor;
        var originalBackground = GameConsole.BackgroundColor;

        // compute inner width (space between vertical borders)
        int inner = Math.Max(0, Width - 2);

        // Prepare centered inner text
        string innerText;
        int textLen = Text?.Length ?? 0;
        if (textLen >= inner)
        {
            innerText = Text.Substring(0, Math.Min(textLen, inner));
        }
        else
        {
            int padLeft = (inner - textLen) / 2;
            int padRight = inner - textLen - padLeft;
            innerText = new string(' ', padLeft) + Text + new string(' ', padRight);
        }

        // Determine coordinates for a 3-row box (top, middle, bottom) using Y as middle row
        int topY = Y - 1;
        int midY = Y;
        int botY = Y + 1;

        bool canDrawBox = topY >= 0 && botY < GameConsole.WindowHeight &&
                          buttonStartX >= 0 && (buttonStartX + Width - 1) < GameConsole.WindowWidth;

        if (selected)
        {
            GameConsole.BackgroundColor = ConsoleColor.Gray;
            GameConsole.ForegroundColor = ConsoleColor.Black;
        }

        if (canDrawBox)
        {
            // Choose glyphs: prefer Unicode box drawing unless configuration requests monospace fallback
            bool useMonospace = false;
            try { useMonospace = MapSymbols.Settings.UseMonospace; } catch { useMonospace = false; }

            char tl, tr, bl, br, hor, ver;
            if (useMonospace)
            {
                tl = '+'; tr = '+'; bl = '+'; br = '+'; hor = '-'; ver = '|';
            }
            else
            {
                // Use explicit Unicode codepoints to avoid any encoding issues in source files
                tl = '\u250C'; // '?'
                tr = '\u2510'; // '?'
                bl = '\u2514'; // '?'
                br = '\u2518'; // '?'
                hor = '\u2500'; // '?'
                ver = '\u2502'; // '?'
            }

            // Top border
            GameConsole.SetCursorPosition(buttonStartX, topY);
            GameConsole.Write(tl);
            GameConsole.Write(new string(hor, inner));
            GameConsole.Write(tr);

            // Middle with text
            GameConsole.SetCursorPosition(buttonStartX, midY);
            GameConsole.Write(ver);
            GameConsole.Write(innerText);
            GameConsole.Write(ver);

            // Bottom border
            GameConsole.SetCursorPosition(buttonStartX, botY);
            GameConsole.Write(bl);
            GameConsole.Write(new string(hor, inner));
            GameConsole.Write(br);
        }
        else
        {
            // Fallback: single-line rendering (old style)
            GameConsole.SetCursorPosition(buttonStartX, Y);
            GameConsole.Write(Rendered);
        }

        GameConsole.ForegroundColor = originalForeground;
        GameConsole.BackgroundColor = originalBackground;
    }
}
