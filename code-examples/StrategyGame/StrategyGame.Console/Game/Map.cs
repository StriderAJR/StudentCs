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

    public static MapSymbols Symbols { get; private set; } = MapSymbols.LoadFromFile(Path.Combine(AppContext.BaseDirectory, "mapsymbols.json"));

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

    public bool TryCaptureBuilding(Coordinate pos, Player player)
    {
        if (player is null) return false;

        var b = GetBuildingAt(pos);
        if (b == null) return false;

        // если уже захвачено тем же игроком — ничего не делаем
        if (b.IsCaptured && ReferenceEquals(b.Owner, player))
            return false;

        b.Capture(player);
        return true;
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

                string glyph = Map.Symbols.Empty;
                ConsoleColor color = ConsoleColor.Gray;

                if (explored[mr, mc])
                {
                    var cell = cells[mr, mc];
                    glyph = cell.ToSymbol();

                    var bld = buildings.FirstOrDefault(b => b.Position.X == mr && b.Position.Y == mc);
                    if (bld != null && bld.Owner != null)
                        color = bld.GetColor();
                }

                // ensure glyph fits in remaining space; if not, fallback to ASCII char
                int remaining = innerW - col;
                int glyphLen = Math.Max(1, glyph?.Length ?? 1);
                if (glyphLen > remaining)
                {
                    var ascii = cells[top + row, left + col].ToAscii();
                    glyph = ascii.ToString();
                    glyphLen = 1;
                }

                GameConsole.ForegroundColor = color;
                GameConsole.Write(glyph);

                // skip columns consumed by multi-char glyph
                col += glyphLen - 1;
            }
        }

        // игрок
        int screenRow = playerPos.X - top;
        int screenCol = playerPos.Y - left;
        if (screenRow >= 0 && screenRow < innerH && screenCol >= 0 && screenCol < innerW)
        {
            GameConsole.ForegroundColor = ConsoleColor.Red;
            GameConsole.SetCursorPosition(innerX + screenCol, innerY + screenRow);
            var pg = Map.Symbols.UseMonospace ? Map.Symbols.PersonMonospace ?? "@" : Map.Symbols.Person ?? "@";
            // if player symbol is multi-char and would overflow, write single char fallback
            if (pg.Length > innerW - screenCol)
                GameConsole.Write('@');
            else
                GameConsole.Write(pg);
        }

        GameConsole.ForegroundColor = ConsoleColor.Gray;
    }

    public void DrawFrame(int x, int y, int width, int height, string? title = null)
    {
        if (width <= 0 || height <= 0)
            return;

        var prevColor = GameConsole.ForegroundColor;
        GameConsole.ForegroundColor = ConsoleColor.Gray;

        for (int row = 0; row < height; row++)
        {
            GameConsole.SetCursorPosition(x, y + row);

            if (row == 0)
            {
                if (width == 1)
                    GameConsole.Write('┌');
                else
                {
                    GameConsole.Write('┌');
                    GameConsole.Write(new string('─', Math.Max(0, width - 2)));
                    if (width > 1) GameConsole.Write('┐');
                }
            }
            else if (row == height - 1)
            {
                if (width == 1)
                    GameConsole.Write('└');
                else
                {
                    GameConsole.Write('└');
                    GameConsole.Write(new string('─', Math.Max(0, width - 2)));
                    if (width > 1) GameConsole.Write('┘');
                }
            }
            else
            {
                if (width == 1)
                    GameConsole.Write('│');
                else
                {
                    GameConsole.Write('│');
                    GameConsole.Write(new string(' ', Math.Max(0, width - 2)));
                    GameConsole.Write('│');
                }
            }
        }

        if (!string.IsNullOrEmpty(title) && width >= 6)
        {
            GameConsole.SetCursorPosition(x + 2, y);
            GameConsole.Write($"[{title}]");
        }

        GameConsole.ForegroundColor = prevColor;
    }

    /// <summary>
    /// Отрисовать видимую часть карты, центрированную на игроке.
    /// Повторяет логику старого DrawMapPanel, использует internal поля cells/explored/buildings.
    /// </summary>
    public void DrawVisible(int x, int y, int width, int height, Player player)
    {
        int innerX = x + 1;
        int innerY = y + 1;
        int innerW = Math.Max(0, width - 2);
        int innerH = Math.Max(0, height - 2);

        if (cells == null || innerW <= 0 || innerH <= 0)
            return;

        int mapRows = Rows;
        int mapCols = Cols;

        // центрируем вид на игроке
        int top = player.position.X - innerH / 2;
        int left = player.position.Y - innerW / 2;

        top = Math.Clamp(top, 0, Math.Max(0, mapRows - innerH));
        left = Math.Clamp(left, 0, Math.Max(0, mapCols - innerW));

        for (int row = 0; row < innerH; row++)
        {
            for (int col = 0; col < innerW; col++)
            {
                int mr = top + row;
                int mc = left + col;

                GameConsole.SetCursorPosition(innerX + col, innerY + row);

                string glyph = Map.Symbols.Empty;
                ConsoleColor color = ConsoleColor.Gray;

                if (mr >= 0 && mr < mapRows && mc >= 0 && mc < mapCols)
                {
                    if (explored[mr, mc])
                    {
                        var cell = cells[mr, mc];
                        glyph = cell.ToSymbol();

                        var bld = buildings.FirstOrDefault(b => b.Position.X == mr && b.Position.Y == mc);
                        if (bld != null)
                        {
                            if (bld.Owner != null)
                                color = bld.GetColor();
                            else
                                color = ConsoleColor.Gray;
                        }
                        else
                        {
                            color = ConsoleColor.Gray;
                        }
                    }
                    else
                    {
                        glyph = Map.Symbols.Empty;
                        color = ConsoleColor.DarkGray;
                    }
                }
                else
                {
                    glyph = Map.Symbols.Empty;
                    color = ConsoleColor.Gray;
                }

                int remaining = innerW - col;
                int glyphLen = Math.Max(1, glyph?.Length ?? 1);
                if (glyphLen > remaining)
                {
                    var ascii = cells[Math.Clamp(mr, 0, Rows - 1), Math.Clamp(mc, 0, Cols - 1)].ToAscii();
                    glyph = ascii.ToString();
                    glyphLen = 1;
                }

                GameConsole.ForegroundColor = color;
                GameConsole.Write(glyph);

                col += glyphLen - 1;
            }
        }

        // Отрисовать игрока поверх карты (игрок всегда видим)
        int screenRow = player.position.X - top;
        int screenCol = player.position.Y - left;
        if (screenRow >= 0 && screenRow < innerH && screenCol >= 0 && screenCol < innerW)
        {
            GameConsole.ForegroundColor = ConsoleColor.Red;
            GameConsole.SetCursorPosition(innerX + screenCol, innerY + screenRow);
            var pg = Map.Symbols.UseMonospace ? Map.Symbols.PersonMonospace ?? "@" : Map.Symbols.Person ?? "@";
            if (pg.Length > innerW - screenCol)
                GameConsole.Write('@');
            else
                GameConsole.Write(pg);
        }

        GameConsole.ForegroundColor = ConsoleColor.Gray;
    }
}
