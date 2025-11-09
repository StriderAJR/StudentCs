namespace StrategyGame.ConsoleGame.Game;

public static class MapExtensions
{
    public static char ToChar(this MapCell cell)
    {
        switch (cell)
        {
            case MapCell.Empty: return ' ';
            case MapCell.Wall: return '#';
            case MapCell.Gold: return 'G';
            case MapCell.Wood: return 'W';
            case MapCell.Stone: return 'S';
        }

        throw new ArgumentException("Unknown cell");
    }
}
