using StrategyGame.Wpf.Models;

namespace StrategyGame.Wpf.Game;

public class MapLoadResult
{
    public required MapCell[,] Map { get; init; }
    public required Coordinate PlayerStart { get; init; }
}
