using StrategyGame.Logic;
using StrategyGame.Logic.Models;

namespace StrategyGame.Logic.Game;

public class MapLoadResult
{
    public required MapCell[,] Map { get; init; }
    public required Coordinate PlayerStart { get; init; }
}
