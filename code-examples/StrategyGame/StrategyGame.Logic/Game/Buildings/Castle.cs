using StrategyGame.ConsoleGame.Game.MapTypes;
using StrategyGame.ConsoleGame.Game.PlayerTypes;
using StrategyGame.ConsoleGame.Game.Units;
using StrategyGame.ConsoleGame.Game.Units.Playable;
using System.Reflection;
using StrategyGame.ConsoleGame.Game.Buildings.CastleBuilding;
using CastleBuildingBase = StrategyGame.ConsoleGame.Game.Buildings.CastleBuilding.CastleBuilding;

namespace StrategyGame.ConsoleGame.Game.Buildings;

public class Castle : Building
{
    // internal mutable storage for garrison (3) - contains ICombatant (unit stacks)
    private readonly ICombatant[] garrison = new ICombatant[3];

    /// <summary>
    /// Read-only view of garrison slots.
    /// </summary>
    public IReadOnlyList<ICombatant> Garrison => Array.AsReadOnly(garrison);

    public int GarrisonSlots => garrison.Length;

    // internal storage for castle buildings; expose read-only list
    private readonly List<CastleBuildingBase> buildings = new();
    public IReadOnlyList<CastleBuildingBase> Buildings => buildings.AsReadOnly();

    public Castle(Coordinate pos) : base(pos, MapCell.Castle)
    {
        // инициализаци€ пустых стеков
        garrison[0] = new UnitStack<InfantryUnit>(0);
        garrison[1] = new UnitStack<ArcherUnit>(0);
        garrison[2] = new UnitStack<BeastUnit>(0);

        PopulateBuildingsViaReflection();
    }

    private void PopulateBuildingsViaReflection()
    {
        var asm = Assembly.GetExecutingAssembly();

        // namespace for CastleBuilding types
        var buildingNamespace = typeof(ArcheryBuilding).Namespace ?? "StrategyGame.ConsoleGame.Game.Buildings.CastleBuilding";

        var types = asm.GetTypes()
            .Where(t => t.IsClass && !t.IsAbstract && t.Namespace == buildingNamespace && typeof(CastleBuildingBase).IsAssignableFrom(t));

        foreach (var t in types)
        {
            try
            {
                var inst = (CastleBuildingBase?)Activator.CreateInstance(t);
                if (inst != null)
                {
                    buildings.Add(inst);
                }
            }
            catch
            {
                // ignore
            }
        }

        // сортировка по имени
        buildings.Sort((a, b) => string.Compare(a.Name, b.Name, StringComparison.Ordinal));
    }

    /// <summary>
    /// ѕолучить слот гарнизона по индексу.
    /// </summary>
    public ICombatant? GetGarrisonSlot(int index)
    {
        if (index < 0 || index >= garrison.Length) return null;
        return garrison[index];
    }

    /// <summary>
    /// ѕопытатьс€ установить слот гарнизона (используетс€ дл€ пополнени€).
    /// </summary>
    public bool TrySetGarrisonSlot(int index, ICombatant unit)
    {
        if (index < 0 || index >= garrison.Length) return false;
        garrison[index] = unit;
        return true;
    }

    /// <summary>
    /// ƒобавить указанное количество юнитов в гарнизон, возвращает остаток неразмещенных.
    /// </summary>
    public int AddUnitsToGarrison(ICombatant prototype, int qty)
    {
        if (qty <= 0) return 0;
        int remaining = qty;
        for (int slot = 0; slot < garrison.Length && remaining > 0; slot++)
        {
            var gs = garrison[slot];
            IUnitStack stack = gs as IUnitStack;
            if (stack == null)
            {
                try
                {
                    var created = (ICombatant?)Activator.CreateInstance(prototype.GetType(), new object[] { 0 });
                    if (created != null)
                    {
                        garrison[slot] = created;
                        stack = created as IUnitStack;
                    }
                }
                catch
                {
                    stack = null;
                }
            }

            if (stack == null) continue;

            int space = 99 - stack.Count;
            int add = Math.Min(space, remaining);
            stack.Add(add);
            remaining -= add;
        }

        return remaining;
    }

    /// <summary>
    /// ≈женедельный тик: примен€ет эффекты построек, вызываетс€ извне.
    /// </summary>
    public void WeeklyTick(Player player)
    {
        if (player == null) return;

        foreach (var b in buildings)
        {
            if (!b.IsBuilt) continue;
            b.ApplyWeeklyEffect(this, player);
        }
    }

    /// <summary>
    /// ѕрименить бонус защитников: временный бонус к защите дл€ зан€тых стеков.
    /// </summary>
    public void ApplyDefenderBonus(bool apply)
    {
        foreach (var u in garrison)
        {
            if (u is IUnitStack s && s.Count > 0)
            {
                s.TempDefenseBonus = apply ? (int)Math.Ceiling(s.BaseDefense * 0.5) : 0;
            }
        }
    }
}
