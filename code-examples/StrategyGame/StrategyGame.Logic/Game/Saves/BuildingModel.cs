using StrategyGame.ConsoleGame.Game.MapTypes;

namespace StrategyGame.ConsoleGame.Game.Saves;

public class BuildingModel
{
    public MapCell Type { get; set; }
    public int X { get; set; }
    public int Y { get; set; }
    public int? OwnerIndex { get; set; }

    // castle-specific state
    public bool IsCastle { get; set; }
    public List<UnitModel>? Garrison { get; set; }
    public List<CastleBuildingState>? CastleBuildings { get; set; }
}

public class CastleBuildingState
{
    public string Name { get; set; } = string.Empty;
    public bool IsBuilt { get; set; }
    // produce counts per unit type name
    public Dictionary<string, int>? ProducedUnits { get; set; }
}
