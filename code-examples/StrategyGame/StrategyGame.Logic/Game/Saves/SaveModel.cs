namespace StrategyGame.ConsoleGame.Game.Saves;

public class SaveModel
{
    public DateTime SavedAt { get; set; }
    public int Day { get; set; }
    public int Week { get; set; }
    public int CurrentPlayerIndex { get; set; }
    public List<PlayerModel> Players { get; set; } = new();
    public List<BuildingModel> Buildings { get; set; } = new();
    public List<bool[][]> PlayerExplored { get; set; } = new();
    public List<MonsterModel>? Monsters { get; set; }
}

public class MonsterModel
{
    public int X { get; set; }
    public int Y { get; set; }
    public List<UnitModel> Units { get; set; } = new();
}
