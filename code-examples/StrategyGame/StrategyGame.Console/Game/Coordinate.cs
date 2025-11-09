namespace StrategyGame.ConsoleGame.Game;
public readonly struct Coordinate
{
    public Coordinate(int x, int y)
    {
        X = x;
        Y = y;
    }

    public readonly int X;
    public readonly int Y;

    public static Coordinate operator +(Coordinate a, Coordinate b)
        => new Coordinate(a.X + b.X, a.Y + b.Y);

    public static Coordinate operator -(Coordinate a, Coordinate b)
        => new Coordinate(a.X - b.X, a.Y - b.Y);
}