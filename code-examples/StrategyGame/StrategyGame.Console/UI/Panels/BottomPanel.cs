using StrategyGame.ConsoleGame.UI.CustomConsole;

namespace StrategyGame.ConsoleGame.UI.Panels;

public class BottomPanel : UIPanel
{
    private readonly Func<PanelData> _dataProvider;

    public BottomPanel(int x, int y, int width, int height,
        Func<PanelData> dataProvider)
        : base(x, y, width, height)
    {
        _dataProvider = dataProvider;
    }

    protected override void DrawContent()
    {
        int innerX = X + 1;
        int innerY = Y + 1;
        int innerW = Math.Max(0, Width - 2);
        int innerH = Math.Max(0, Height - 2);

        if (innerW <= 0 || innerH <= 0)
            return;

        var data = _dataProvider();
        int day = data.Day, week = data.Week;
        int wood = data.Wood, stone = data.Stone, gold = data.Gold;
        int woodInc = data.WoodIncome, stoneInc = data.StoneIncome, goldInc = data.GoldIncome;

        string line1 = $"День: {day}   Неделя: {week}";
        GameConsole.SetCursorPosition(innerX, innerY);
        GameConsole.ForegroundColor = ConsoleColor.Cyan;
        GameConsole.Write(line1.Length > innerW ? line1[..innerW] : line1);

        int row = innerY + 1;
        int col = innerX;

        void DrawResource(string label, int value, int inc)
        {
            GameConsole.SetCursorPosition(col, row);
            GameConsole.ForegroundColor = ConsoleColor.Green;
            string text = $"{label}: {value}";
            GameConsole.Write(text);
            if (inc > 0)
            {
                GameConsole.ForegroundColor = ConsoleColor.DarkGray;
                GameConsole.Write($" (+{inc})");
            }
            col += text.Length + 8;
        }

        DrawResource("Дерево", wood, woodInc);
        DrawResource("Камень", stone, stoneInc);
        DrawResource("Золото", gold, goldInc);

        GameConsole.ForegroundColor = ConsoleColor.Gray;
    }
}
