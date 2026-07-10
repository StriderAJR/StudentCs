using StrategyGame.ConsoleGame.Game.MapTypes;

namespace StrategyGame.ConsoleGame.Game.Buildings;

public static class BuildingFactory
{
    /// <summary>
    /// Создать экземпляр строения по типу MapCell.
    /// </summary>
    public static Building CreateBuilding(Coordinate pos, MapCell type)
    {
        return type switch
        {
            MapCell.Wood => new WoodMill(pos),
            MapCell.Stone => new StoneQuarry(pos),
            MapCell.Gold => new GoldMine(pos),
            MapCell.Castle => new Castle(pos),
            _ => new Building(pos, type)
        };
    }
}
