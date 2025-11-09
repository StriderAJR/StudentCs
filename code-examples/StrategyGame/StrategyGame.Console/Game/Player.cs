namespace StrategyGame.ConsoleGame.Game;

public class Player(string name, PlayerType type, Coordinate position, PlayerColor color)
{
    public Coordinate position { get; private set; } = position;
    public int X { get => position.X; }
    public int Y { get => position.Y; }
    public uint Health { get; private set; } = CalculateHealth();
    public PlayerColor Color { get; private set; } = color;

    public readonly string Name = name;
    public readonly PlayerType Type = type;

    public void Move(Coordinate shift)
    {
        position = position + shift;
    }

    private static uint CalculateHealth()
    {
        return 100;
        // TODO different health for different types
    }

    // TODO рисовать окно с информацией об игроке
}
