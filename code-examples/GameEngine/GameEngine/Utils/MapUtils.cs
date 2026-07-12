using GameEngine.Models;

namespace GameEngine.Utils;

public static class MapUtils
{
    public static GameMap CreateDefaultMap()
    {
        return new GameMap
        {
            Width = GameSettings.DefaultMapWidth,
            Height = GameSettings.DefaultMapHeight
        };
    }

    public static Position CreateCenterPosition(GameMap map)
    {
        return new Position
        {
            X = map.Width / 2,
            Y = map.Height / 2
        };
    }

    public static bool IsInside(GameMap map, int x, int y)
    {
        return x >= 0 && x < map.Width && y >= 0 && y < map.Height;
    }

    public static double Distance(Position first, Position second)
    {
        var xDifference = second.X - first.X;
        var yDifference = second.Y - first.Y;
        return Math.Sqrt((xDifference * xDifference) + (yDifference * yDifference));
    }

    public static bool IsNear(Position first, Position second, double maximumDistance)
    {
        return maximumDistance >= 0 && Distance(first, second) <= maximumDistance;
    }

    public static Position ClampPosition(GameMap map, Position position)
    {
        return new Position
        {
            X = Math.Clamp(position.X, 0, map.Width - 1),
            Y = Math.Clamp(position.Y, 0, map.Height - 1)
        };
    }
}
