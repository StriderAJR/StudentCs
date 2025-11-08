namespace StrategyGame.ConsoleGame;

public class Player(string name, PlayerType type, Coordinate position)
{
    public Coordinate position { get; private set; } = position;
    public int X { get => position.X; }
    public int Y { get => position.Y; }
    public uint Health { get; private set; } = CalculateHealth();

    public readonly string Name = name;
    public readonly PlayerType Type = type;

    public void Move(Coordinate shift)
    {
        position = new Coordinate(position.X + shift.X, position.Y + shift.Y);

        // Если бы стукртура Coordinate не была бы readonly, то можно было бы записать так:
        // position.X += shift.X;
        // position.Y += shift.Y;

        // TODO рассказать про перегрузку операторов
    }

    private static uint CalculateHealth()
    {
        return 100;
        // TODO different health for different types
    }

    // TODO рисовать окно с информацией об игроке
}
