namespace StrategyGame.ConsoleGame.Game.Units.Monsters;

/// <summary>
/// Фабрика монстров: регистрация и создание экземпляров монстров по ключу.
/// </summary>
public static class MonsterFactory
{
    private static readonly Dictionary<string, Func<UnitBase>> registry = new();
    private static readonly List<string> keys = new();

    /// <summary>
    /// Зарегистрировать производителя монстра по ключу.
    /// </summary>
    public static void Register(string key, Func<UnitBase> creator)
    {
        if (string.IsNullOrEmpty(key) || creator == null) return;
        registry[key] = creator;
        if (!keys.Contains(key)) keys.Add(key);
    }

    /// <summary>
    /// Создать монстра по ключу. Возвращает null, если ключ не зарегистрирован.
    /// </summary>
    public static UnitBase Create(string key)
    {
        if (registry.TryGetValue(key, out var c)) return c();
        return null;
    }

    /// <summary>
    /// Создать случайного монстра из зарегестрированных (использует переданный RNG).
    /// </summary>
    public static UnitBase CreateRandom(Random rng)
    {
        if (keys.Count == 0) return null;
        var k = keys[rng.Next(keys.Count)];
        return Create(k);
    }

    /// <summary>
    /// Зарегистрировать стандартный набор монстров, если они ещё не зарегистрированы.
    /// </summary>
    public static void RegisterDefaults()
    {
        // избегаем дублирования регистраций
        if (keys.Contains("goblin") || keys.Contains("wraith"))
            return;

        Register("goblin", () => new GoblinUnit(4, 8));
        Register("wraith", () => new WraithUnit(6, 10));
    }
}
