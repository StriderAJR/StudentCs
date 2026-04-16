using StrategyGame.Logic.Game;
using StrategyGame.Logic.Models;

namespace StrategyGame.Logic;

public class StrategyGameState
{
    public StrategyGameState(uint width, uint height)
    {
        Width = width;
        Height = height;
        Map = GenerateMap(height, width);
    }

    public uint Width { get; private set; }
    public uint Height { get; private set; }
    public MapCell[,] Map { get; private set; }
    public Player? Player { get; private set; }

    public void StartNewGame(string playerName, PlayerType playerType)
    {
        Map = GenerateMap(Height, Width);
        Player = new Player(playerName, playerType, new Coordinate(1, 1));
    }

    public void StartNewGame(string playerName, PlayerType playerType, string mapFilePath)
    {
        MapLoadResult loadedMap = MapFileLoader.Load(mapFilePath);

        Map = loadedMap.Map;
        Height = (uint)Map.GetLength(0);
        Width = (uint)Map.GetLength(1);
        Player = new Player(playerName, playerType, loadedMap.PlayerStart);
    }

    public bool MovePlayer(Coordinate shift)
    {
        if (Player is null)
        {
            return false;
        }

        Coordinate newPosition = Player.Position + shift;

        if (!IsInsideMap(newPosition))
        {
            return false;
        }

        if (Map[newPosition.X, newPosition.Y] == MapCell.Wall)
        {
            return false;
        }

        Player.Move(shift);
        return true;
    }

    private bool IsInsideMap(Coordinate position)
    {
        return position.X >= 0
            && position.Y >= 0
            && position.X < Map.GetLength(0)
            && position.Y < Map.GetLength(1);
    }

    private static MapCell[,] GenerateMap(uint height, uint width)
    {
        MapCell[,] map = new MapCell[height, width];

        for (int i = 0; i < height; i++)
        {
            for (int j = 0; j < width; j++)
            {
                map[i, j] = i == 0 || i == height - 1 || j == 0 || j == width - 1
                    ? MapCell.Wall
                    : MapCell.Empty;
            }
        }

        return map;
    }
}
