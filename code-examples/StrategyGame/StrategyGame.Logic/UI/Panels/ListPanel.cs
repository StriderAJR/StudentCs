using StrategyGame.ConsoleGame.UI.CustomConsole;

namespace StrategyGame.ConsoleGame.UI.Panels
{
    public enum Orientation { Vertical, Horizontal }

    public class ListPanel : UIPanel
    {
        // make items mutable so callers can update them without recreating the panel
        protected string[] _items;
        protected readonly Orientation _orientation;
        protected readonly bool _selectable;

        public bool IsFocused { get; set; }
        public int SelectedIndex { get; set; }

        public ListPanel(int x, int y, int width, int height, string[] items, Orientation orientation = Orientation.Vertical, bool selectable = true)
            : base(x, y, width, height)
        {
            _items = items ?? Array.Empty<string>();
            _orientation = orientation;
            _selectable = selectable;
            SelectedIndex = 0;
        }

        // Allow updating items without recreating the panel
        public void UpdateItems(string[] items)
        {
            _items = items ?? Array.Empty<string>();
            // clamp selected index
            if (_items.Length == 0)
                SelectedIndex = 0;
            else
                SelectedIndex = Math.Clamp(SelectedIndex, 0, _items.Length - 1);
        }

        protected override void DrawContent()
        {
            var items = _items ?? Array.Empty<string>();
            int innerX = X + 1;
            int innerY = Y + 1;
            int innerW = Math.Max(0, Width - 2);
            int innerH = Math.Max(0, Height - 2);

            if (_orientation == Orientation.Vertical)
            {
                DrawVertical(items, innerX, innerY, innerW, innerH);
            }
            else // Horizontal
            {
                DrawHorizontal(items, innerX, innerY, innerW, innerH);
            }
        }

        private void DrawVertical(string[] items, int innerX, int innerY, int innerW, int innerH)
        {
            int maxLines = Math.Max(0, innerH);
            int textWidth = Math.Max(0, innerW - 2); // reserve 2 chars for marker

            for (int i = 0; i < maxLines; i++)
            {
                GameConsole.SetCursorPosition(innerX, innerY + i);
                string text = i < items.Length ? items[i] : string.Empty;
                if (text.Length > textWidth) text = text.Substring(0, Math.Max(0, textWidth - 3)) + "...";

                bool isSelected = _selectable && i == SelectedIndex;

                // draw marker
                string marker = "  ";
                if (isSelected)
                    marker = IsFocused ? "? " : "> ";

                var prevFg = GameConsole.ForegroundColor;
                var prevBg = GameConsole.BackgroundColor;

                if (IsFocused && isSelected)
                {
                    // invert whole area including marker
                    GameConsole.BackgroundColor = prevFg;
                    GameConsole.ForegroundColor = prevBg;
                    GameConsole.Write(marker + text.PadRight(textWidth));
                    GameConsole.ForegroundColor = prevFg;
                    GameConsole.BackgroundColor = prevBg;
                }
                else
                {
                    // draw marker with highlight color if selected but unfocused
                    if (isSelected)
                    {
                        GameConsole.ForegroundColor = ConsoleColor.Yellow;
                        GameConsole.Write(marker);
                        GameConsole.ForegroundColor = prevFg;
                    }
                    else
                    {
                        GameConsole.Write(marker);
                    }

                    GameConsole.Write(text.PadRight(textWidth));
                }
            }
        }

        private void DrawHorizontal(string[] items, int innerX, int innerY, int innerW, int innerH)
        {
            int row = innerY;
            int col = innerX;
            int maxItems = items.Length;
            for (int i = 0; i < maxItems; i++)
            {
                string text = items[i] ?? string.Empty;
                int itemLen = text.Length + 2; // include marker/icon area

                bool isSelected = _selectable && i == SelectedIndex;
                var prevFg = GameConsole.ForegroundColor;
                var prevBg = GameConsole.BackgroundColor;

                if (isSelected && IsFocused)
                {
                    GameConsole.BackgroundColor = prevFg;
                    GameConsole.ForegroundColor = prevBg;
                    GameConsole.SetCursorPosition(col, row);
                    GameConsole.Write(text);
                    GameConsole.ForegroundColor = prevFg;
                    GameConsole.BackgroundColor = prevBg;
                }
                else
                {
                    GameConsole.SetCursorPosition(col, row);
                    if (isSelected)
                    {
                        GameConsole.ForegroundColor = ConsoleColor.Yellow;
                    }
                    GameConsole.Write(text);
                    GameConsole.ForegroundColor = prevFg;
                }

                col += itemLen + 2;
                if (col >= innerX + innerW) break;
            }
        }
    }
}
