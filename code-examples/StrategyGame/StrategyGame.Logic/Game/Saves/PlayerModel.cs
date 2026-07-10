namespace StrategyGame.ConsoleGame.Game.Saves;

public class PlayerModel
{
    public string Type { get; set; } = string.Empty;
    public string Color { get; set; } = string.Empty;
    public int X { get; set; }
    public int Y { get; set; }
    public int MaxMoves { get; set; }
    public int MovesRemaining { get; set; }
    public int MaxMagic { get; set; }
    public int MagicRemaining { get; set; }
    public int TempMoveBonusPercent { get; set; }
    public int UnitSlots { get; set; }
    public List<UnitModel> Units { get; set; } = new();

    // serialized player resources (e.g. Wood, Stone, Gold)
    public List<ResourceModel> Resources { get; set; } = new();
}