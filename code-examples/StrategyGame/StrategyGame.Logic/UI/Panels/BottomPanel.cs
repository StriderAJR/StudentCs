using StrategyGame.ConsoleGame.UI.CustomConsole;
using StrategyGame.ConsoleGame.Game.MapTypes;
using StrategyGame.ConsoleGame.Game.PlayerTypes;

namespace StrategyGame.ConsoleGame.UI.Panels;

public class BottomPanel : ListPanel
{
    private PanelData _data;
    private readonly Player? _player;

    public BottomPanel(int x, int y, int width, int height,
        PanelData data, Player? player)
        : base(x, y, width, height, Array.Empty<string>(), Orientation.Horizontal, false)
    {
        _data = data;
        _player = player;
    }

    public void UpdateData(PanelData data)
    {
        _data = data;
    }

    protected override ConsoleColor GetBorderColor()
    {
        try
        {
            var p = _player;
            if (p != null)
            {
                return UITheme.FromPlayerColor(p.Color);
            }
        }
        catch { }

        return ConsoleColor.Gray;
    }

    protected override void DrawContent()
    {
        int innerX = X + 1;
        int innerY = Y + 1;
        int innerW = Math.Max(0, Width - 2);
        int innerH = Math.Max(0, Height - 2);

        if (innerW <= 0 || innerH <= 0)
            return;

        // Fill interior with previous background (do not color with player bg)
        var prevBg = GameConsole.BackgroundColor;
        var prevFg = GameConsole.ForegroundColor;

        ClearInterior(innerX, innerY, innerW, innerH, prevBg);

        DrawMainLine(innerX, innerY, innerW, prevFg);

        int row = innerY + 1;
        int col = innerX;

        int wood = GetResourceAmount(typeof(Game.Resources.Wood));
        int stone = GetResourceAmount(typeof(Game.Resources.Stone));
        int gold = GetResourceAmount(typeof(Game.Resources.Gold));

        int woodInc = GetIncomeFor(typeof(Game.Resources.Wood));
        int stoneInc = GetIncomeFor(typeof(Game.Resources.Stone));
        int goldInc = GetIncomeFor(typeof(Game.Resources.Gold));

        int movesRem = _data.MovesRemaining, maxMoves = _data.MaxMoves;

        void DrawResource(MapCell type, string label, int value, int inc)
        {
            string icon = GetGlyph(type);
            GameConsole.SetCursorPosition(col, row);

            var prev = GameConsole.ForegroundColor;
            // keep icon color default
            GameConsole.ForegroundColor = ConsoleColor.Gray;

            try
            {
                var p = _player;
                if (p != null)
                {
                    // use selected variant for icon so it stands out
                    GameConsole.ForegroundColor = UITheme.FromPlayerColorSelected(p.Color);
                }
                else
                {
                    GameConsole.ForegroundColor = ConsoleColor.Gray;
                }
            }
            catch { GameConsole.ForegroundColor = ConsoleColor.Gray; }

            GameConsole.Write(icon + " ");
            GameConsole.ForegroundColor = prev;

            GameConsole.ForegroundColor = prevFg;
            string text = $"{label}: {value}";
            GameConsole.Write(text);
            if (inc > 0)
            {
                // show income as dimmed player color when player exists, else use dark gray
                var p = _player;
                GameConsole.ForegroundColor = p != null ? UITheme.FromPlayerColorDimmed(p.Color) : ConsoleColor.DarkGray;
                GameConsole.Write($" (+{inc})");
            }
            col += icon.Length + 1 + text.Length + 8;
        }

        DrawResource(MapCell.Wood, "Дерево", wood, woodInc);
        DrawResource(MapCell.Stone, "Камень", stone, stoneInc);
        DrawResource(MapCell.Gold, "Золото", gold, goldInc);

        // Draw moves info on the right side of the panel
        string movesText = $"Ходы: {movesRem}/{maxMoves}";
        int movesPos = innerX + Math.Max(0, innerW - movesText.Length);
        GameConsole.SetCursorPosition(movesPos, row);
        GameConsole.ForegroundColor = ConsoleColor.Magenta;
        GameConsole.Write(movesText);

        GameConsole.ForegroundColor = prevFg;
        GameConsole.BackgroundColor = prevBg;
    }

    private void ClearInterior(int innerX, int innerY, int innerW, int innerH, ConsoleColor prevBg)
    {
        for (int ry = 0; ry < innerH; ry++)
        {
            GameConsole.SetCursorPosition(innerX, innerY + ry);
            GameConsole.BackgroundColor = prevBg;
            GameConsole.Write(new string(' ', innerW));
        }
    }

    private void DrawMainLine(int innerX, int innerY, int innerW, ConsoleColor prevFg)
    {
        var data = _data;
        int day = data.Day, week = data.Week;

        string line1 = $"День: {day}   Неделя: {week}";
        GameConsole.SetCursorPosition(innerX, innerY);
        GameConsole.ForegroundColor = prevFg;
        GameConsole.Write(line1.Length > innerW ? line1[^innerW..] : line1);
    }

    private int GetResourceAmount(Type t)
    {
        return _data.Resources?.FirstOrDefault(r => r.GetType() == t)?.Amount ?? 0;
    }

    private int GetIncomeFor(Type t)
    {
        return _data.IncomeByType?.TryGetValue(t, out var v) == true ? v : 0;
    }

    private string GetGlyph(MapCell type)
    {
        try
        {
            return type.GetGlyphs().FirstOrDefault() ?? "";
        }
        catch { return ""; }
    }
}
