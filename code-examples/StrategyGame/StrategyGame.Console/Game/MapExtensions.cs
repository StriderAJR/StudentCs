namespace StrategyGame.ConsoleGame.Game;

public static class MapExtensions
{
    public static string ToSymbol(this MapCell cell)
    {
        var symbols = Map.Symbols;
        if (symbols.UseMonospace)
        {
            return cell switch
            {
                MapCell.Empty => symbols.EmptyMonospace,
                MapCell.Wall => symbols.WallMonospace,
                MapCell.Gold => symbols.GoldMonospace,
                MapCell.Wood => symbols.TreeMonospace,
                MapCell.Stone => symbols.StoneMonospace,
                _ => symbols.EmptyMonospace,
            } ?? " ";
        }

        return cell switch
        {
            MapCell.Empty => symbols.Empty,
            MapCell.Wall => symbols.Wall,
            MapCell.Gold => symbols.Gold,
            MapCell.Wood => symbols.Tree,
            MapCell.Stone => symbols.Stone,
            _ => symbols.Empty,
        } ?? " ";
    }

    public static char ToAscii(this MapCell cell)
    {
        return cell switch
        {
            MapCell.Empty => ' ',
            MapCell.Wall => '#',
            MapCell.Gold => 'G',
            MapCell.Wood => 'W',
            MapCell.Stone => 'S',
            _ => ' ',
        };
    }
}
