namespace StrategyGame.ConsoleGame.Game.MapTypes;

public static class MapExtensions
{
    /// <summary>
    /// Получить отображаемый глиф для <see cref="MapCell"/> в соответствии с текущими настройками.
    /// </summary>
    /// <param name="symbol">Значение MapCell.</param>
    /// <returns>Строковый глиф для рендеринга.</returns>
    public static string ToSymbol(this MapCell symbol)
    {
        var settings = MapSymbols.Settings;
        var symbols = settings.UseMonospace ? settings.Fallback : settings.Default;
        return symbol switch
        {
            MapCell.Empty => symbols.Empty,
            MapCell.Wall => symbols.Wall,
            MapCell.Gold => symbols.Gold,
            MapCell.Wood => symbols.Wood,
            MapCell.Stone => symbols.Stone,
            MapCell.Player => symbols.Player,
            MapCell.Monster => symbols.Monster,
            MapCell.Castle => symbols.Castle,
            _ => symbols.Empty,
        };
    }

    /// <summary>
    /// Возвращает массив возможных глифов (основной + запасной) для <see cref="MapCell"/>, или пустой массив, если не найдено.
    /// </summary>
    /// <param name="cell">Значение MapCell.</param>
    /// <returns>Массив строк-глифов.</returns>
    public static string[] GetGlyphs(this MapCell cell)
    {
        if (MapSymbols.CellToGlyph != null && MapSymbols.CellToGlyph.TryGetValue(cell, out var arr))
            return arr ?? Array.Empty<string>();
        return Array.Empty<string>();
    }

    /// <summary>
    /// Преобразует строковый глиф в <see cref="MapCell"/> используя глобальную таблицу соответствия; возвращает <see cref="MapCell.Empty"/>, если глиф неизвестен.
    /// </summary>
    /// <param name="glyph">Строковый глиф.</param>
    /// <returns>Соответствующее значение MapCell или MapCell.Empty.</returns>
    public static MapCell ToMapCell(this string glyph)
    {
        if (string.IsNullOrEmpty(glyph))
            return MapCell.Empty;

        if (MapSymbols.GlyphToCell != null && MapSymbols.GlyphToCell.TryGetValue(glyph, out var cell))
            return cell;
        return MapCell.Empty;
    }

    /// <summary>
    /// Возвращает упорядоченный список глифов (по убывающей длине) для жадного сопоставления.
    /// </summary>
    /// <returns>Массив строк-глифов, упорядоченных по убывающей длине.</returns>
    public static string[] GetOrderedGlyphs()
    {
        if (MapSymbols.GlyphToCell == null)
            return Array.Empty<string>();
        return MapSymbols.GlyphToCell.Keys
            .Where(k => !string.IsNullOrEmpty(k))
            .OrderByDescending(k => k.Length)
            .ToArray();
    }
}
