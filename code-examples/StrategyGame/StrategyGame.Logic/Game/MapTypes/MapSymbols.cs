using System.Text.Json;

namespace StrategyGame.ConsoleGame.Game.MapTypes;

/// <summary>
/// Набор глифов, используемых для разных типов ячеек карты.
/// </summary>
public sealed class MapSymbolSet
{
    public string Wood { get; set; }
    public string Stone { get; set; }
    public string Gold { get; set; }
    public string Player { get; set; }
    public string Castle { get; set; }
    public string Monster { get; set; }
    public string Wall { get; set; }
    public string Empty { get; set; }
}

/// <summary>
/// Настройки с наборами глифов по умолчанию и запасными вариантами для отображения и парсинга карт.
/// </summary>
public sealed class MapSymbolSettings
{
    /// <summary>
    /// Основные (эмодзи/богатые) глифы.
    /// </summary>
    public MapSymbolSet Default { get; set; }

    /// <summary>
    /// Моноширинные запасные глифы (одноколонные символы).
    /// </summary>
    public MapSymbolSet Fallback { get; set; }

    /// <summary>
    /// Если true — использовать моноширинные запасные глифы вместо основных.
    /// </summary>
    public bool UseMonospace { get; set; } = false;
}

/// <summary>
/// Глобальная таблица соответствия символов и настроек для отображения карт.
/// </summary>
public static class MapSymbols
{
    private static MapSymbolSettings settings;
    private static Dictionary<MapCell, string[]> cellToGlyph;
    private static Dictionary<string, MapCell> glyphToCell;

    /// <summary>
    /// Текущие настройки, загружаемые из mapsymbols.json при первом обращении.
    /// </summary>
    public static MapSymbolSettings Settings
    {
        get
        {
            if (settings == null)
                Load();
            return settings;
        }

        private set => settings = value;
    }

    /// <summary>
    /// Отображение MapCell -> возможные строки-глифы (основной + запасной).
    /// </summary>
    public static Dictionary<MapCell, string[]> CellToGlyph
    {
        get
        {
            if (Settings == null)
                Load();
            return cellToGlyph;
        }
        private set => cellToGlyph = value;
    }

    /// <summary>
    /// Обратная таблица: строковый глиф -> MapCell.
    /// </summary>
    public static Dictionary<string, MapCell> GlyphToCell
    {
        get
        {
            if (Settings == null)
                Load();
            return glyphToCell;
        }
        private set => glyphToCell = value;
    }

    /// <summary>
    /// Загрузить настройки глифов из JSON-файла и инициализировать таблицы соответствий.
    /// </summary>
    /// <param name="path">Путь к JSON-файлу с настройками.</param>
    public static void Load(string path)
    {
        if (!File.Exists(path))
            return;

        var json = File.ReadAllText(path);
        var opts = new JsonSerializerOptions { PropertyNameCaseInsensitive = true };
        Settings = JsonSerializer.Deserialize<MapSymbolSettings>(json, opts);

        CellToGlyph = new Dictionary<MapCell, string[]>
        {
            { MapCell.Wood, new string[] {Settings.Default.Wood, Settings.Fallback.Wood } },
            { MapCell.Stone, new string[] {Settings.Default.Stone, Settings.Fallback.Stone} },
            { MapCell.Gold, new string[] {Settings.Default.Gold, Settings.Fallback.Gold} },
            { MapCell.Empty, new string[] {Settings.Default.Empty, Settings.Fallback.Empty} },
            { MapCell.Wall, new string[] {Settings.Default.Wall, Settings.Fallback.Wall} },
            // включаем глифы игрока в соответствие
            { MapCell.Player, new string[] {Settings.Default.Player, Settings.Fallback.Player} },
            // глифы монстров при наличии
            { MapCell.Monster, new string[] {Settings.Default.Monster, Settings.Fallback.Monster} },
            // глифы замка
            { MapCell.Castle, new string[] {Settings.Default.Castle, Settings.Fallback.Castle} }
        };

        GlyphToCell = new Dictionary<string, MapCell>
        {
            { Settings.Default.Wood, MapCell.Wood },
            { Settings.Fallback.Wood, MapCell.Wood },
            { Settings.Default.Stone, MapCell.Stone },
            { Settings.Fallback.Stone, MapCell.Stone },
            { Settings.Default.Gold, MapCell.Gold },
            { Settings.Fallback.Gold, MapCell.Gold },
            { Settings.Default.Wall, MapCell.Wall },
            { Settings.Fallback.Wall, MapCell.Wall },
            { Settings.Default.Empty, MapCell.Empty },
            // включаем глифы игрока в обратную таблицу
            { Settings.Default.Player, MapCell.Player },
            { Settings.Fallback.Player, MapCell.Player },
            // глифы монстров
            { Settings.Default.Monster, MapCell.Monster },
            { Settings.Fallback.Monster, MapCell.Monster },
            // глифы замка
            { Settings.Default.Castle, MapCell.Castle },
            { Settings.Fallback.Castle, MapCell.Castle }
        };
    }

    /// <summary>
    /// Загрузить настройки из стандартного места (AppContext.BaseDirectory/settings/mapsymbols.json).
    /// Безопасно вызывать несколько раз — загрузка произойдёт только если файл найден.
    /// </summary>
    public static void Load()
    {
        try
        {
            string baseDir = AppContext.BaseDirectory;
            string candidate = Path.Combine(baseDir, "settings", "mapsymbols.json");
            if (File.Exists(candidate))
            {
                Load(candidate);
            }
        }
        catch { }
    }
}