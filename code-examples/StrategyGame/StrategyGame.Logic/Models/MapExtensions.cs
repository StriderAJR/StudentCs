using StrategyGame.Logic;

namespace StrategyGame.Logic.Models;

public static class MapExtensions
{
    public static char ToChar(this MapCell cell)
    {
        return cell switch
        {
            MapCell.Empty => ' ',
            MapCell.Wall => '#',
            MapCell.Gold => 'G',
            MapCell.Wood => 'W',
            MapCell.Stone => 'S',
            _ => throw new ArgumentException("Unknown cell", nameof(cell))
        };
    }
}
