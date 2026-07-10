using StrategyGame.ConsoleGame.Game.Units.Playable;

namespace StrategyGame.ConsoleGame.Game.Units;

public static class UnitFactory
{
    private static readonly Dictionary<string, Func<int, IUnitStack>> creators = new(StringComparer.OrdinalIgnoreCase);

    static UnitFactory()
    {
        // register known unit types
        Register<InfantryUnit>("Infantry");
        Register<ArcherUnit>("Archer");
        Register<BeastUnit>("Beast");
    }

    public static void Register<TUnit>(string name) where TUnit : UnitBase, new()
    {
        creators[name] = count => new UnitStack<TUnit>(count);
    }

    public static IUnitStack Create(string name, int count = 1)
    {
        if (creators.TryGetValue(name, out var f))
            return f(Math.Clamp(count, 0, 99));
        // fallback: try to find by type name and register dynamically
        var asm = typeof(UnitFactory).Assembly;
        var type = Type.GetType(name) ?? Array.Find(asm.GetTypes(), t => string.Equals(t.Name, name, StringComparison.OrdinalIgnoreCase));
        if (type != null && typeof(UnitBase).IsAssignableFrom(type))
        {
            var method = typeof(UnitFactory).GetMethod(nameof(Register))!.MakeGenericMethod(type);
            method.Invoke(null, new object[] { name });
            return creators[name](count);
        }

        // default fallback to Infantry
        return new UnitStack<InfantryUnit>(count);
    }
}
