using StrategyGame.ConsoleGame.Game.PlayerTypes;
using StrategyGame.ConsoleGame.Game.Units;

namespace StrategyGame.ConsoleGame.Game.Buildings.CastleBuilding;

public abstract class CastleBuilding
{
    /// <summary>
    /// Отображаемое имя модуля здания.
    /// </summary>
    public string Name { get; }

    /// <summary>
    /// Признак того, что здание построено.
    /// </summary>
    public bool IsBuilt { get; private set; }

    /// <summary>
    /// Стоимость в ресурсах для постройки: тип ресурса -> количество.
    /// </summary>
    public IReadOnlyDictionary<Type, int> ResourceCosts { get; }

    /// <summary>
    /// Юниты, производимые и хранимые в здании (тип юнита -> количество).
    /// </summary>
    protected readonly Dictionary<Type,int> producedUnits = new();
    public IReadOnlyDictionary<Type,int> ProducedUnits => producedUnits;

    /// <summary>
    /// Стоимость покупки произведённого юнита: тип юнита -> (тип ресурса -> количество за единицу).
    /// </summary>
    protected readonly Dictionary<Type, Dictionary<Type, int>> unitPurchaseCosts = new();
    public IReadOnlyDictionary<Type, Dictionary<Type, int>> UnitPurchaseCosts => unitPurchaseCosts;

    protected CastleBuilding(string name, Dictionary<Type, int>? resourceCosts = null, Dictionary<Type, Dictionary<Type, int>>? unitCosts = null)
    {
        Name = name;
        IsBuilt = false;
        ResourceCosts = resourceCosts != null ? new Dictionary<Type, int>(resourceCosts) : new Dictionary<Type, int>();
        if (unitCosts != null)
        {
            foreach (var kv in unitCosts)
            {
                unitPurchaseCosts[kv.Key] = new Dictionary<Type, int>(kv.Value);
            }
        }
    }

    public void Build() => IsBuilt = true;
    public void Demolish() => IsBuilt = false;

    /// <summary>
    /// Применить пассивный еженедельный эффект здания (вызывается при еженедельном тике).
    /// </summary>
    public virtual void ApplyWeeklyEffect(Castle castle, Player player) { }

    /// <summary>
    /// Некоторые здания предоставляют активное действие (например, Колодец). Переопределите HasAction и UseAction.
    /// </summary>
    public virtual bool HasAction => false;
    public virtual void UseAction(Castle castle, Player player) { }

    /// <summary>
    /// Распечатать статус здания, при необходимости используя контекст замка.
    /// </summary>
    public virtual string Print(Castle castle)
    {
        return $"{Name}: {(IsBuilt ? "построено" : "не построено")}";
    }

    // Вспомогательные методы для произведённых юнитов (Type-based internals)
    protected void ProduceUnit(Type unitType, int amount)
    {
        if (amount <= 0) return;
        if (!producedUnits.ContainsKey(unitType)) producedUnits[unitType] = 0;
        producedUnits[unitType] += amount;
    }

    public int GetProducedCount(Type unitType)
    {
        return producedUnits.TryGetValue(unitType, out var v) ? v : 0;
    }

    public bool ConsumeProduced(Type unitType, int amount)
    {
        if (amount <= 0) return true;
        if (!producedUnits.TryGetValue(unitType, out var v) || v < amount) return false;
        producedUnits[unitType] = v - amount;
        return true;
    }

    public IReadOnlyDictionary<Type, int>? GetUnitCost(Type unitType)
    {
        return unitPurchaseCosts.TryGetValue(unitType, out var dict) ? dict : null;
    }

    // Generic convenience wrappers using UnitBase type
    /// <summary>
    /// Добавить произведённые единицы для конкретного типа юнита (generic).
    /// </summary>
    protected void ProduceUnit<TUnit>(int amount) where TUnit : UnitBase
    {
        ProduceUnit(typeof(TUnit), amount);
    }

    /// <summary>
    /// Получить количество произведённых юнитов конкретного типа (generic).
    /// </summary>
    public int GetProducedCount<TUnit>() where TUnit : UnitBase
    {
        return GetProducedCount(typeof(TUnit));
    }

    /// <summary>
    /// Потребить произведённые юниты конкретного типа (generic).
    /// </summary>
    public bool ConsumeProduced<TUnit>(int amount) where TUnit : UnitBase
    {
        return ConsumeProduced(typeof(TUnit), amount);
    }

    /// <summary>
    /// Получить стоимость покупки произведённого юнита в виде словаря Resource type -> amount (generic).
    /// </summary>
    public IReadOnlyDictionary<Type, int>? GetUnitCost<TUnit>() where TUnit : UnitBase
    {
        return GetUnitCost(typeof(TUnit));
    }

    // --- New public helpers for save/load to avoid reflection ---

    /// <summary>
    /// Replace producedUnits using a dictionary keyed by Type.
    /// </summary>
    public void SetProducedUnits(Dictionary<Type,int> map)
    {
        producedUnits.Clear();
        if (map == null) return;
        foreach (var kv in map)
        {
            if (kv.Key != null && kv.Value > 0)
                producedUnits[kv.Key] = kv.Value;
        }
    }

    /// <summary>
    /// Import produced units from a dictionary keyed by full type name (string).
    /// Type resolution attempts Type.GetType first, then scans loaded assemblies.
    /// Only non-abstract types assignable to UnitBase are applied.
    /// </summary>
    public void ImportProducedUnitsByTypeName(Dictionary<string,int>? map)
    {
        producedUnits.Clear();
        if (map == null) return;
        foreach (var kv in map)
        {
            if (string.IsNullOrWhiteSpace(kv.Key)) continue;
            string typeName = kv.Key.Trim();
            Type? t = Type.GetType(typeName);
            if (t == null)
            {
                t = AppDomain.CurrentDomain.GetAssemblies()
                    .SelectMany(a =>
                    {
                        try { return a.GetTypes(); } catch { return Array.Empty<Type>(); }
                    })
                    .FirstOrDefault(x => x.FullName != null && string.Equals(x.FullName, typeName, StringComparison.OrdinalIgnoreCase));
            }

            if (t != null && typeof(UnitBase).IsAssignableFrom(t) && !t.IsAbstract)
            {
                producedUnits[t] = kv.Value;
            }
        }
    }
}
