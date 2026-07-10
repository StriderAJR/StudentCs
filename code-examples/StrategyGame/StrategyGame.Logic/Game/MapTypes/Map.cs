using StrategyGame.ConsoleGame.UI.CustomConsole;
using StrategyGame.ConsoleGame.Game.Units;
using StrategyGame.ConsoleGame.Game.Units.Monsters;
using StrategyGame.ConsoleGame.Game.Buildings;
using StrategyGame.ConsoleGame.Game.PlayerTypes;

namespace StrategyGame.ConsoleGame.Game.MapTypes;

public class Map
{
    private MapCell[,] cells;
    // global explored kept for backward compatibility; but we'll use per-player fog
    private bool[,] explored;
    private readonly Dictionary<Player, bool[,]> playerExplored = new();

    private readonly List<Building> buildings = new();

    // монстры по позиции: ключ = (ряд, столбец)
    private readonly Dictionary<(int, int), List<UnitBase>> monsters = new();
    private readonly Random random = new();

    public IReadOnlyList<Building> Buildings => buildings;
    public int Rows => cells.GetLength(0);
    public int Cols => cells.GetLength(1);

    public Map()
    {
        // регистрация стандартных монстров
        MonsterFactory.RegisterDefaults();
    }

    /// <summary>
    /// Загрузить карту из текстового файла и вернуть позицию игрока, если она задана.
    /// </summary>
    /// <param name="path">Путь к файлу карты.</param>
    /// <returns>Координата игрока, если найдена; иначе null.</returns>
    public Coordinate? LoadFromFile(string path)
    {
        string[] lines = File.ReadAllLines(path);

        var orderedGlyphs = MapExtensions.GetOrderedGlyphs();

        var (rows, playerPos) = ParseLinesToRows(lines, orderedGlyphs);

        FillCellsAndBuildings(rows);

        return playerPos;
    }

    /// <summary>
    /// Разобрать строки текста в матрицу строк MapCell и обнаружить позицию игрока за один проход.
    /// </summary>
    private static (List<List<MapCell>> rows, Coordinate? playerPos) ParseLinesToRows(string[] lines, string[] orderedGlyphs)
    {
        var rows = new List<List<MapCell>>();
        Coordinate? playerPos = null;

        for (int lineIndex = 0; lineIndex < lines.Length; lineIndex++)
        {
            string line = lines[lineIndex] ?? string.Empty;
            var rowCells = new List<MapCell>();

            int charIndex = 0;
            int colIndex = 0; // логический индекс столбца (увеличивается на каждую ячейку карты)

            while (charIndex < line.Length)
            {
                if (TryMatchGlyph(line, charIndex, orderedGlyphs, out var glyphCandidate))
                {
                    // найден многосимвольный глиф в текущей позиции
                    var cell = glyphCandidate.ToMapCell();
                    if (cell == MapCell.Player && playerPos is null)
                    {
                        playerPos = new Coordinate(lineIndex, colIndex);
                        rowCells.Add(MapCell.Empty); // сохраняем поведение: в ячейке игрока хранится пустая клетка
                    }
                    else
                    {
                        rowCells.Add(cell);
                    }

                    charIndex += glyphCandidate.Length;
                    colIndex++;
                }
                else
                {
                    // Не найден многосимвольный глиф; потребляем один символ и пытаемся сопоставить ASCII-глиф
                    char c = line[charIndex];
                    string key = c.ToString();
                    var cell = key.ToMapCell();
                    if (cell == MapCell.Player && playerPos is null)
                    {
                        playerPos = new Coordinate(lineIndex, colIndex);
                        rowCells.Add(MapCell.Empty);
                    }
                    else
                    {
                        rowCells.Add(cell);
                    }

                    charIndex++;
                    colIndex++;
                }
            }

            rows.Add(rowCells);
        }

        return (rows, playerPos);
    }

    /// <summary>
    /// Попытаться сопоставить один из упорядоченных глифов в данной позиции строки.
    /// </summary>
    private static bool TryMatchGlyph(string line, int idx, string[] orderedGlyphs, out string matched)
    {
        for (int i = 0; i < orderedGlyphs.Length; i++)
        {
            var g = orderedGlyphs[i];
            if (string.IsNullOrEmpty(g))
                continue;

            if (idx + g.Length <= line.Length && string.Compare(line, idx, g, 0, g.Length, StringComparison.Ordinal) == 0)
            {
                matched = g;
                return true;
            }
        }

        matched = string.Empty;
        return false;
    }

    /// <summary>
    /// Заполнить внутренние массивы cells/explored и обнаружить строения на карте.
    /// </summary>
    private void FillCellsAndBuildings(List<List<MapCell>> rows)
    {
        int fileH = rows.Count;
        int fileW = rows.Any() ? rows.Max(r => r.Count) : 0;

        cells = new MapCell[fileH, fileW];
        explored = new bool[fileH, fileW];
        buildings.Clear();
        monsters.Clear();
        playerExplored.Clear();

        for (int row = 0; row < fileH; row++)
        {
            var rowList = rows[row];
            for (int col = 0; col < fileW; col++)
            {
                var cell = col < rowList.Count ? rowList[col] : MapCell.Empty;
                cells[row, col] = cell;

                if (cell == MapCell.Gold || cell == MapCell.Wood || cell == MapCell.Stone)
                {
                    buildings.Add(BuildingFactory.CreateBuilding(new Coordinate(row, col), cell));
                }

                if (cell == MapCell.Monster)
                {
                    // разместить 1-3 случайных монстра здесь через фабрику
                    int monsterCount = random.Next(1, 4);
                    var list = new List<UnitBase>(monsterCount);
                    for (int monIndex = 0; monIndex < monsterCount; monIndex++)
                    {
                        // выбрать случайного зарегистрированного монстра
                        var monsterInstance = MonsterFactory.CreateRandom(random);
                        if (monsterInstance != null) list.Add(monsterInstance);
                    }
                    monsters[(row, col)] = list;
                }
                if (cell == MapCell.Castle)
                {
                    var castle = (Castle)BuildingFactory.CreateBuilding(new Coordinate(row, col), MapCell.Castle);
                    buildings.Add(castle);
                }
            }
        }
    }

    /// <summary>
    /// Найти первую пустую ячейку на карте.
    /// </summary>
    public Coordinate? FindFirstEmptyCell()
    {
        for (int row = 0; row < Rows; row++)
            for (int col = 0; col < Cols; col++)
                if (cells[row, col] == MapCell.Empty)
                    return new Coordinate(row, col);
        return null;
    }

    /// <summary>
    /// Найти случайную пустую клетку на карте. Игнорирует клетки, занятые списком occupied (если задан).
    /// </summary>
    public Coordinate? FindRandomEmptyCell(Random random, IEnumerable<Coordinate>? occupied = null)
    {
        var occupiedSet = new HashSet<(int, int)>();
        if (occupied != null)
            foreach (var occupiedCoord in occupied) occupiedSet.Add((occupiedCoord.X, occupiedCoord.Y));

        var empties = new List<Coordinate>();
        for (int row = 0; row < Rows; row++)
            for (int col = 0; col < Cols; col++)
                if (cells[row, col] == MapCell.Empty && !occupiedSet.Contains((row, col)))
                    empties.Add(new Coordinate(row, col));

        if (empties.Count == 0) return null;
        return empties[random.Next(empties.Count)];
    }

    /// <summary>
    /// Возвращает true, если игрок может переместиться из playerPos на заданный сдвиг.
    /// </summary>
    public bool CanMove(Coordinate playerPos, Coordinate shift)
    {
        int targetRow = playerPos.X + shift.X;
        int targetCol = playerPos.Y + shift.Y;

        if (targetRow < 0 || targetRow >= Rows || targetCol < 0 || targetCol >= Cols)
            return false;

        return cells[targetRow, targetCol] != MapCell.Wall;
    }

    /// <summary>
    /// Инициализировать массивы тумана войны для списка игроков. Вызывать после загрузки карты и создания игроков.
    /// </summary>
    public void InitializePlayerFog(IEnumerable<Player> players)
    {
        playerExplored.Clear();
        foreach (var player in players)
        {
            var arr = new bool[Rows, Cols];
            playerExplored[player] = arr;
        }
    }

    /// <summary>
    /// Открыть клетки вокруг позиции для заданного игрока.
    /// </summary>
    public void RevealAround(Player player, Coordinate pos, int radius = 3)
    {
        if (!playerExplored.TryGetValue(player, out var arr))
            return;

        for (int deltaRow = -radius; deltaRow <= radius; deltaRow++)
        {
            for (int deltaCol = -radius; deltaCol <= radius; deltaCol++)
            {
                int r = pos.X + deltaRow;
                int c = pos.Y + deltaCol;
                if (r >= 0 && r < Rows && c >= 0 && c < Cols)
                    arr[r, c] = true;
            }
        }
    }

    // backward-compatible: reveal without player does nothing
    public void RevealAround(Coordinate pos, int radius = 3) { }

    /// <summary>
    /// Вернуть строение в позиции, если оно присутствует.
    /// </summary>
    public Building? GetBuildingAt(Coordinate pos)
    {
        return buildings.FirstOrDefault(building => building.Position.Equals(pos));
    }

    /// <summary>
    /// Попытаться захватить строение в позиции для игрока.
    /// </summary>
    public bool TryCaptureBuilding(Coordinate pos, Player player)
    {
        if (player is null) return false;

        var building = GetBuildingAt(pos);
        if (building == null) return false;

        // если уже захвачено тем же игроком — ничего не делаем
        if (building.IsCaptured && ReferenceEquals(building.Owner, player))
            return false;

        building.Capture(player);
        return true;
    }

    /// <summary>
    /// Возвращает копию массива тумана войны для указанного игрока (или null если не найден).
    /// Используется для сериализации/сохранения состояния игры.
    /// </summary>
    public bool[,]? GetPlayerExplored(Player player)
    {
        if (playerExplored.TryGetValue(player, out var arr))
        {
            var copy = new bool[arr.GetLength(0), arr.GetLength(1)];
            for (int i = 0; i < arr.GetLength(0); i++)
                for (int j = 0; j < arr.GetLength(1); j++)
                    copy[i, j] = arr[i, j];
            return copy;
        }
        return null;
    }

    /// <summary>
    /// Установить массив тумана войны для указанного игрока (копируется).
    /// Используется при загрузке сохранения.
    /// </summary>
    public void SetPlayerExplored(Player player, bool[,] arr)
    {
        if (arr == null) return;
        var copy = new bool[arr.GetLength(0), arr.GetLength(1)];
        for (int i = 0; i < arr.GetLength(0); i++)
            for (int j = 0; j < arr.GetLength(1); j++)
                copy[i, j] = arr[i, j];

        playerExplored[player] = copy;
    }

    /// <summary>
    /// Отрисовать карту, центрированную на playerPos.
    /// </summary>
    public void Draw(int x, int y, int width, int height, Coordinate playerPos)
    {
        // unused in new flow
    }

    /// <summary>
    /// Отрисовать видимую часть карты,центрированную на текущем игроке, и показать других игроков цветными иконками.
    /// </summary>
    public void DrawVisible(int x, int y, int width, int height, List<Player> players, Player currentPlayer)
    {
        int innerX = x + 1;
        int innerY = y + 1;
        int innerW = Math.Max(0, width - 2);
        int innerH = Math.Max(0, height - 2);

        if (cells == null || innerW <= 0 || innerH <= 0)
            return;

        int mapRows = Rows;
        int mapCols = Cols;

        // центрируем вид на текущем игроке
        int top = currentPlayer.position.X - innerH / 2;
        int left = currentPlayer.position.Y - innerW / 2;

        top = Math.Clamp(top, 0, Math.Max(0, mapRows - innerH));
        left = Math.Clamp(left, 0, Math.Max(0, mapCols - innerW));

        // use current player's explored array
        playerExplored.TryGetValue(currentPlayer, out var exploredArr);

        for (int row = 0; row < innerH; row++)
        {
            for (int col = 0; col < innerW; col++)
            {
                int mapRow = top + row;
                int mapCol = left + col;

                GameConsole.SetCursorPosition(innerX + col, innerY + row);

                string glyph = MapCell.Empty.GetGlyphs().First();

                if (mapRow >= 0 && mapRow < mapRows && mapCol >= 0 && mapCol < mapCols)
                {
                    bool isExplored = exploredArr != null && exploredArr[mapRow, mapCol];
                    if (isExplored)
                    {
                        var cell = cells[mapRow, mapCol];
                        glyph = cell.ToSymbol();

                        var buildingAt = buildings.FirstOrDefault(b => b.Position.X == mapRow && b.Position.Y == mapCol);
                        if (buildingAt != null)
                        {
                            if (buildingAt.Owner != null)
                                GameConsole.ForegroundColor = buildingAt.GetColor();
                            else
                                GameConsole.ForegroundColor = ConsoleColor.Gray;
                        }
                        else
                        {
                            GameConsole.ForegroundColor = ConsoleColor.Gray;
                        }
                    }
                    else
                    {
                        glyph = MapCell.Empty.GetGlyphs().First();
                        GameConsole.ForegroundColor = ConsoleColor.DarkGray;
                    }
                }
                else
                {
                    glyph = MapCell.Empty.GetGlyphs().First();
                    GameConsole.ForegroundColor = ConsoleColor.Gray;
                }

                int glyphLen = Math.Max(1, glyph?.Length ?? 1);

                GameConsole.Write(glyph);

                col += glyphLen - 1;
            }
        }

        // draw players visible to current player
        foreach (var player in players)
        {
            int screenRow = player.position.X - top;
            int screenCol = player.position.Y - left;
            if (screenRow >= 0 && screenRow < innerH && screenCol >= 0 && screenCol < innerW)
            {
                if (exploredArr != null && (exploredArr[player.position.X, player.position.Y] || ReferenceEquals(player, currentPlayer)))
                {
                    GameConsole.SetCursorPosition(innerX + screenCol, innerY + screenRow);
                    GameConsole.ForegroundColor = UITheme.FromPlayerColor(player.Color);

                    // используем первый глиф для игрока из настроек MapCell.Player
                    var playerGlyphs = MapCell.Player.GetGlyphs();
                    string primary = playerGlyphs.FirstOrDefault() ?? "@";
                    string fallback = playerGlyphs.Skip(1).FirstOrDefault() ?? (primary.Length > 0 ? primary.Substring(0, 1) : "@");

                    if (primary.Length > innerW - screenCol)
                        GameConsole.Write(fallback);
                    else
                        GameConsole.Write(primary);
                }
            }
        }

        GameConsole.ForegroundColor = UITheme.CurrentBorderColor;
    }

    /// <summary>
    /// Вернуть список монстров в позиции или пустой список.
    /// </summary>
    public List<UnitBase> GetMonstersAt(Coordinate pos)
    {
        if (monsters.TryGetValue((pos.X, pos.Y), out var list))
            return list;
        return new List<UnitBase>();
    }

    /// <summary>
    /// Удалить монстров в позиции (используется после их победы).
    /// </summary>
    public void RemoveMonstersAt(Coordinate pos)
    {
        monsters.Remove((pos.X, pos.Y));
        // очищаем ячейку, чтобы карта больше не показывала глиф монстра
        if (pos.X >= 0 && pos.X < Rows && pos.Y >= 0 && pos.Y < Cols)
            cells[pos.X, pos.Y] = MapCell.Empty;
    }

    /// <summary>
    /// Отрисовать рамку в заданной области с опциональным заголовком.
    /// </summary>
    public void DrawFrame(int x, int y, int width, int height, string? title = null)
    {
        if (width <= 0 || height <= 0)
            return;

        var prevColor = GameConsole.ForegroundColor;
        GameConsole.ForegroundColor = UITheme.CurrentBorderColor;

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
}
