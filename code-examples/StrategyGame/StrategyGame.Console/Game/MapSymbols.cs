using System.Text.Json;

namespace StrategyGame.ConsoleGame.Game;

public sealed class MapSymbols
{
    // Primary (emoji / rich) glyphs — not used by default now
    public string Tree { get; set; } = "??";
    public string Stone { get; set; } = "?";
    public string Gold { get; set; } = "??";
    public string Person { get; set; } = "??";
    public string Castle { get; set; } = "??";
    public string Monster { get; set; } = "??";
    public string Wall { get; set; } = "?";
    public string Empty { get; set; } = " ";

    // Monospace fallbacks (single-column characters) - defaults used now
    public string TreeMonospace { get; set; } = "T";
    public string StoneMonospace { get; set; } = "^";
    public string GoldMonospace { get; set; } = "$";
    public string PersonMonospace { get; set; } = "@";
    public string CastleMonospace { get; set; } = "#";
    public string MonsterMonospace { get; set; } = "M";
    public string WallMonospace { get; set; } = "#";
    public string EmptyMonospace { get; set; } = " ";

    // When true, code should use monospace fallbacks instead of primary glyphs
    public bool UseMonospace { get; set; } = true;

    private MapSymbols() { }

    public static MapSymbols LoadFromFile(string path)
    {
        if (!File.Exists(path))
            return new MapSymbols();

        try
        {
            var json = File.ReadAllText(path);
            var opts = new JsonSerializerOptions { PropertyNameCaseInsensitive = true };
            var ms = JsonSerializer.Deserialize<MapSymbols>(json, opts);
            return ms ?? new MapSymbols();
        }
        catch
        {
            return new MapSymbols();
        }
    }
}