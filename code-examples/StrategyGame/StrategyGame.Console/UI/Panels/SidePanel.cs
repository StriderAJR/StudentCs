using StrategyGame.ConsoleGame.UI.CustomConsole;

namespace StrategyGame.ConsoleGame.UI.Panels;

public class SidePanel : UIPanel
{
    public SidePanel(int x, int y, int width, int height)
        : base(x, y, width, height)
    {
    }

    protected override void DrawContent()
    {
        int innerX = X + 1;
        int innerY = Y + 1;
        int innerW = Math.Max(0, Width - 2);
        int innerH = Math.Max(0, Height - 2);

        if (innerW <= 0 || innerH <= 0)
            return;

        string[] buttons = new[] { "[I] Информация об игроке", "[E] Завершить день", "[M] Меню" };
        int startY = innerY + 1;

        for (int i = 0; i < buttons.Length; i++)
        {
            string text = buttons[i];
            if (text.Length > innerW)
                text = text[..innerW];

            int posX = innerX + Math.Max(0, (innerW - text.Length) / 2);
            int posY = startY + i * 2;
            if (posY >= innerY && posY < innerY + innerH)
            {
                GameConsole.SetCursorPosition(posX, posY);
                GameConsole.ForegroundColor = ConsoleColor.Yellow;
                GameConsole.Write(text);
            }
        }

        GameConsole.ForegroundColor = ConsoleColor.Gray;
    }
}
