using System.IO;
using StrategyGame.Wpf.Models;

namespace StrategyGame.Wpf.Game;

public static class MapFileLoader
{
    public static MapLoadResult Load(string filePath)
    {
        if (!File.Exists(filePath))
        {
            throw new FileNotFoundException("Map file was not found.", filePath);
        }

        string[] lines = File.ReadAllLines(filePath);
        if (lines.Length == 0)
        {
            throw new InvalidOperationException("Map file is empty.");
        }

        int rows = lines.Length;
        int columns = lines.Max(line => line.Length);
        if (columns == 0)
        {
            throw new InvalidOperationException("Map file does not contain any cells.");
        }

        MapCell[,] map = new MapCell[rows, columns];
        Coordinate? playerStart = null;

        for (int x = 0; x < rows; x++)
        {
            string line = lines[x];

            for (int y = 0; y < columns; y++)
            {
                char symbol = y < line.Length ? line[y] : ' ';

                if (symbol == '@')
                {
                    playerStart = new Coordinate(x, y);
                    map[x, y] = MapCell.Empty;
                    continue;
                }

                map[x, y] = symbol switch
                {
                    '#' => MapCell.Wall,
                    'W' => MapCell.Wood,
                    'S' => MapCell.Stone,
                    'G' => MapCell.Gold,
                    ' ' => MapCell.Empty,
                    _ => MapCell.Empty
                };
            }
        }

        return new MapLoadResult
        {
            Map = map,
            PlayerStart = playerStart ?? new Coordinate(1, 1)
        };
    }
}
