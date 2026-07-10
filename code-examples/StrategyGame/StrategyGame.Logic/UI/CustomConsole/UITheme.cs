namespace StrategyGame.ConsoleGame.UI.CustomConsole;

public static class UITheme
{
    // Current color used for borders and frames. Default is Gray.
    public static ConsoleColor CurrentBorderColor { get; set; } = ConsoleColor.Gray;

    public static ConsoleColor FromPlayerColor(Game.PlayerTypes.PlayerColor pc)
    {
        // keep mapping consistent with previous visual choices (Blue => Cyan)
        return pc switch
        {
            Game.PlayerTypes.PlayerColor.Red => ConsoleColor.Red,
            Game.PlayerTypes.PlayerColor.Blue => ConsoleColor.Cyan,
            Game.PlayerTypes.PlayerColor.Green => ConsoleColor.Green,
            Game.PlayerTypes.PlayerColor.Yellow => ConsoleColor.Yellow,
            _ => ConsoleColor.Gray,
        };
    }

    /// <summary>
    /// Dimmed variant for less prominent UI elements.
    /// </summary>
    public static ConsoleColor FromPlayerColorDimmed(Game.PlayerTypes.PlayerColor pc)
    {
        return pc switch
        {
            Game.PlayerTypes.PlayerColor.Red => ConsoleColor.DarkRed,
            Game.PlayerTypes.PlayerColor.Blue => ConsoleColor.DarkCyan,
            Game.PlayerTypes.PlayerColor.Green => ConsoleColor.DarkGreen,
            Game.PlayerTypes.PlayerColor.Yellow => ConsoleColor.DarkYellow,
            _ => ConsoleColor.DarkGray,
        };
    }

    /// <summary>
    /// Variant for selected/highlighted elements (uses the normal mapping, kept for semantic clarity).
    /// </summary>
    public static ConsoleColor FromPlayerColorSelected(Game.PlayerTypes.PlayerColor pc)
    {
        return FromPlayerColor(pc);
    }
}
