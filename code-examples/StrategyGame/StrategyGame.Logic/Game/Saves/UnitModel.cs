namespace StrategyGame.ConsoleGame.Game.Saves;

public class UnitModel
{
    public string TypeName { get; set; } = string.Empty;
    public int Count { get; set; }
    public int CurrentHp { get; set; }
    public int MaxHp { get; set; }
}
