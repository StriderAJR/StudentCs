namespace StrategyGame.ConsoleGame.Game;

public class Building
{
    public Coordinate Position { get; }
    public MapCell Type { get; }
    public Player Owner { get; private set; }
    public int IncomePerWeek { get; }

    public Building(Coordinate position, MapCell type)
    {
        Position = position;
        Type = type;

        IncomePerWeek = type switch
        {
            MapCell.Gold => 2,
            MapCell.Wood => 3,
            MapCell.Stone => 2,
            _ => 0
        };
    }

    public bool IsCaptured => Owner != null;

    public void Capture(Player player)
    {
        Owner = player;
    }

    public ConsoleColor GetColor()
    {
        if (Owner == null)
            return ConsoleColor.Gray;

        return Owner.Color switch
        {
            PlayerColor.Red => ConsoleColor.Red,
            PlayerColor.Blue => ConsoleColor.Cyan,
            PlayerColor.Green => ConsoleColor.Green,
            PlayerColor.Yellow => ConsoleColor.Yellow,
            _ => ConsoleColor.White
        };
    }
}
