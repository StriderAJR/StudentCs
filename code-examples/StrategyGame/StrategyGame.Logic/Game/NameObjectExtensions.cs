using StrategyGame.ConsoleGame.Game.Resources;

namespace StrategyGame.ConsoleGame.Game;

public static class NameObjectExtensions
{
    /// <summary>
    /// Produces a string for a sequence of named objects
    /// </summary>
    public static string ToDescriptionString(this IEnumerable<Resource> objects, string separator = " ")
    {
        if (objects == null) return string.Empty;
        return string.Join(separator, objects.Select(r => r.Description));
    }
}
