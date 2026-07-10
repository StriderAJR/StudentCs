using StrategyGame.ConsoleGame.UI.CustomConsole;
using StrategyGame.ConsoleGame.Game.PlayerTypes;

namespace StrategyGame.ConsoleGame.UI.Panels;

public class SidePanel : ListPanel
{
    private Player? _player;
    private readonly Action<int>? _onSelect;

    public SidePanel(int x, int y, int width, int height, string[] buttons, Player? player, Action<int>? onSelect = null)
        : base(x, y, width, height, buttons, Orientation.Vertical, true)
    {
        _player = player;
        _onSelect = onSelect;
    }

    public void UpdateButtons(string[] buttons)
    {
        UpdateItems(buttons);
    }

    public void UpdatePlayer(Player? player)
    {
        _player = player;
    }

    public bool HandleKey(ConsoleKey key)
    {
        var buttons = _items;
        if (buttons.Length == 0) return false;

        if (key == ConsoleKey.UpArrow)
        {
            SelectedIndex = (SelectedIndex - 1 + buttons.Length) % buttons.Length;
            return true;
        }
        if (key == ConsoleKey.DownArrow)
        {
            SelectedIndex = (SelectedIndex + 1) % buttons.Length;
            return true;
        }
        if (key == ConsoleKey.Enter)
        {
            _onSelect?.Invoke(SelectedIndex);
            return true;
        }

        // Not handled
        return false;
    }

    // Color the frame using player's color
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

    // Custom content: center buttons and space them vertically
    protected override void DrawContent()
    {
        int innerX = X + 1;
        int innerY = Y + 1;
        int innerW = Math.Max(0, Width - 2);
        int innerH = Math.Max(0, Height - 2);

        if (innerW <= 0 || innerH <= 0)
            return;

        var prevBg = GameConsole.BackgroundColor;
        var prevFg = GameConsole.ForegroundColor;

        ClearInterior(innerX, innerY, innerW, innerH, prevBg);

        string[] buttons = _items;
        DrawButtonsCentered(buttons, innerX, innerY, innerW, innerH, prevBg, prevFg);

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

    private void DrawButtonsCentered(string[] buttons, int innerX, int innerY, int innerW, int innerH, ConsoleColor prevBg, ConsoleColor prevFg)
    {
        int startY = innerY + 1;

        var player = _player; // capture local
        bool panelFocused = IsFocused;

        for (int i = 0; i < buttons.Length; i++)
        {
            string text = buttons[i] ?? string.Empty;
            if (text.Length > innerW)
                text = text.Substring(0, innerW);

            int posX = innerX + Math.Max(0, (innerW - text.Length) / 2);
            int posY = startY + i * 2;
            if (posY >= innerY && posY < innerY + innerH)
            {
                GameConsole.SetCursorPosition(posX, posY);
                if (panelFocused && player != null && i == SelectedIndex)
                {
                    // highlight selection: use selected player color as background
                    GameConsole.BackgroundColor = UITheme.FromPlayerColorSelected(player.Color);
                    GameConsole.ForegroundColor = prevBg; // keep it readable
                }
                else
                {
                    GameConsole.BackgroundColor = prevBg;
                    if (!panelFocused && player != null)
                    {
                        // when panel is not focused, show labels in dimmed player color
                        GameConsole.ForegroundColor = UITheme.FromPlayerColorDimmed(player.Color);
                    }
                    else
                    {
                        GameConsole.ForegroundColor = prevFg; // use standard text color
                    }
                }

                GameConsole.Write(text);
            }
        }
    }
}
