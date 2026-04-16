namespace StrategyGame.Wpf.Models;

public class Player
{
    public Player(string name, PlayerType type, Coordinate position)
    {
        Name = name;
        Type = type;
        Position = position;
        Health = CalculateHealth(type);
    }

    public string Name { get; }
    public PlayerType Type { get; }
    public Coordinate Position { get; private set; }
    public int X => Position.X;
    public int Y => Position.Y;
    public uint Health { get; }

    public void Move(Coordinate shift)
    {
        Position += shift;
    }

    private static uint CalculateHealth(PlayerType type)
    {
        return type switch
        {
            PlayerType.Knight => 120,
            PlayerType.Ranger => 100,
            PlayerType.Mage => 80,
            _ => 100
        };
    }
}
