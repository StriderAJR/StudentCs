namespace StrategyGame.Logic.Models;

public readonly struct Coordinate
{
    public Coordinate(int x, int y)
    {
        X = x;
        Y = y;
    }

    public int X { get; }
    public int Y { get; }

    public static Coordinate operator +(Coordinate left, Coordinate right)
    {
        return new Coordinate(left.X + right.X, left.Y + right.Y);
    }
}
