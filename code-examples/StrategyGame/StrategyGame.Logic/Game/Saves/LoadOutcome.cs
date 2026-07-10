using StrategyGame.ConsoleGame.Game.MapTypes;
using StrategyGame.ConsoleGame.Game.PlayerTypes;

namespace StrategyGame.ConsoleGame.Game.Save;

public class LoadOutcome
{
    public Map Map { get; set; } = null!;
    public List<Player> Players { get; set; } = new();
    public int CurrentPlayerIndex { get; set; }
    public int Day { get; set; }
    public int Week { get; set; }
    public string SaveFileName { get; set; } = string.Empty;
}
