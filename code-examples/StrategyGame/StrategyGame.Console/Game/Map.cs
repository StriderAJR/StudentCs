using StrategyGame.ConsoleGame.UI.CustomConsole;

namespace StrategyGame.ConsoleGame.Game;

public class Map
{
    private MapCell[,] cells;
    private bool[,] explored;
    private readonly List<Building> buildings = new();

    public IReadOnlyList<Building> Buildings => buildings;
    public int Rows => cells.GetLength(0);
    public int Cols => cells.GetLength(1);

    public void LoadFromFile(string path, out Coordinate? playerPos)
    {
        playerPos = null;
        string[] lines = File.ReadAllLines(path);
        int fileH = lines.Length;
        int fileW = lines.Any() ? lines.Max(l => l.Length) : 0;

        cells = new MapCell[fileH, fileW];
        explored = new bool[fileH, fileW];
        buildings.Clear();

        for (int i = 0; i < fileH; i++)
        {
            string line = lines[i];
            for (int j = 0; j < fileW; j++)
            {
                char c = j < line.Length ? line[j] : ' ';
                switch (c)
                {
                    case '#':
                        cells[i, j] = MapCell.Wall;
                        break;
                    case 'G':
                        cells[i, j] = MapCell.Gold;
                        buildings.Add(new Building(new Coordinate(i, j), MapCell.Gold));
                        break;
                    case 'W':
                        cells[i, j] = MapCell.Wood;
                        buildings.Add(new Building(new Coordinate(i, j), MapCell.Wood));
                        break;
                    case 'S':
                        cells[i, j] = MapCell.Stone;
                        buildings.Add(new Building(new Coordinate(i, j), MapCell.Stone));
                        break;
                    case '@':
                        cells[i, j] = MapCell.Empty;
                        playerPos = new Coordinate(i, j);
                        break;
                    default:
                        cells[i, j] = MapCell.Empty;
                        break;
                }
            }
        }
    }

    public Coordinate? FindFirstEmptyCell()
    {
        for (int i = 0; i < Rows; i++)
            for (int j = 0; j < Cols; j++)
                if (cells[i, j] == MapCell.Empty)
                    return new Coordinate(i, j);
        return null;
    }

    public bool CanMove(Coordinate playerPos, Coordinate shift)
    {
        int targetRow = playerPos.X + shift.X;
        int targetCol = playerPos.Y + shift.Y;

        if (targetRow < 0 || targetRow >= Rows || targetCol < 0 || targetCol >= Cols)
            return false;

        return cells[targetRow, targetCol] != MapCell.Wall;
    }

    public void RevealAround(Coordinate pos, int radius = 3)
    {
        for (int dr = -radius; dr <= radius; dr++)
        {
            for (int dc = -radius; dc <= radius; dc++)
            {
                int r = pos.X + dr;
                int c = pos.Y + dc;
                if (r >= 0 && r < Rows && c >= 0 && c < Cols)
                    explored[r, c] = true;
            }
        }
    }

    public Building? GetBuildingAt(Coordinate pos)
    {
        return buildings.FirstOrDefault(b => b.Position.Equals(pos));
    }

    public void Draw(int x, int y, int width, int height, Coordinate playerPos)
    {
        int innerX = x + 1;
        int innerY = y + 1;
        int innerW = Math.Max(0, width - 2);
        int innerH = Math.Max(0, height - 2);

        if (cells == null || innerW <= 0 || innerH <= 0)
            return;

        int top = playerPos.X - innerH / 2;
        int left = playerPos.Y - innerW / 2;

        top = Math.Clamp(top, 0, Math.Max(0, Rows - innerH));
        left = Math.Clamp(left, 0, Math.Max(0, Cols - innerW));

        for (int row = 0; row < innerH; row++)
        {
            for (int col = 0; col < innerW; col++)
            {
                int mr = top + row;
                int mc = left + col;

                if (mr < 0 || mr >= Rows || mc < 0 || mc >= Cols)
                    continue;

                GameConsole.SetCursorPosition(innerX + col, innerY + row);

                char ch = ' ';
                ConsoleColor color = ConsoleColor.Gray;

                if (explored[mr, mc])
                {
                    MapCell cell = cells[mr, mc];
                    ch = cell.ToChar();

                    var bld = buildings.FirstOrDefault(b => b.Position.X == mr && b.Position.Y == mc);
                    if (bld != null && bld.Owner != null)
                        color = bld.GetColor();
                }

                GameConsole.ForegroundColor = color;
                GameConsole.Write(ch);
            }
        }

        // игрок
        int screenRow = playerPos.X - top;
        int screenCol = playerPos.Y - left;
        if (screenRow >= 0 && screenRow < innerH && screenCol >= 0 && screenCol < innerW)
        {
            GameConsole.ForegroundColor = ConsoleColor.Red;
            GameConsole.SetCursorPosition(innerX + screenCol, innerY + screenRow);
            GameConsole.Write('@');
        }

        GameConsole.ForegroundColor = ConsoleColor.Gray;
    }
}
