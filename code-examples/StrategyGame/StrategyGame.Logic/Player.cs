using StrategyGame.Logic.Models;
using StrategyGame.Logic.Resources;

namespace StrategyGame.Logic;

public class Player
{
    public Player(string name, PlayerType type, Coordinate position, PlayerColor color = PlayerColor.Red)
    {
        Name = name;
        Type = type;
        Position = position;
        Color = color;

        MaxMoves = GetMaxMoves(type);
        MovesRemaining = MaxMoves;

        MaxMagic = GetMaxMagic(type);
        MagicRemaining = MaxMagic;

        Health = CalculateHealth(type);

        Resources.Add(new Wood(0));
        Resources.Add(new Stone(0));
        Resources.Add(new Gold(0));
    }

    public string Name { get; }
    public PlayerType Type { get; }
    public PlayerColor Color { get; set; }
    public Coordinate Position { get; private set; }
    public int X => Position.X;
    public int Y => Position.Y;
    public uint Health { get; }

    public int MaxMoves { get; }
    public int MovesRemaining { get; set; }
    public int TempMoveBonusPercent { get; set; }

    public int MaxMagic { get; }
    public int MagicRemaining { get; set; }

    public List<Resource> Resources { get; } = [];

    public void Move(Coordinate shift)
    {
        Position += shift;
    }

    public Resource? GetResource<T>() where T : Resource
    {
        return Resources.FirstOrDefault(r => r.GetType() == typeof(T));
    }

    public int GetResourceAmount<T>() where T : Resource
    {
        Resource? resource = GetResource<T>();
        return resource?.Amount ?? 0;
    }

    public void AddResource<T>(int amount) where T : Resource
    {
        Resource? resource = GetResource<T>();
        if (resource is null)
        {
            resource = (Resource?)Activator.CreateInstance(typeof(T), 0);
            if (resource is not null)
            {
                Resources.Add(resource);
            }
        }

        if (resource is not null)
        {
            resource.Amount += amount;
        }
    }

    public bool TryConsumeResource(Type resourceType, int amount)
    {
        Resource? resource = Resources.FirstOrDefault(r => r.GetType() == resourceType);
        if (resource is null || resource.Amount < amount)
        {
            return false;
        }

        resource.Amount -= amount;
        return true;
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

    private static int GetMaxMoves(PlayerType type)
    {
        return type switch
        {
            PlayerType.Ranger => 6,
            PlayerType.Mage => 4,
            PlayerType.Knight => 3,
            _ => 4
        };
    }

    private static int GetMaxMagic(PlayerType type)
    {
        return type switch
        {
            PlayerType.Mage => 20,
            PlayerType.Knight => 8,
            PlayerType.Ranger => 3,
            _ => 5
        };
    }
}
