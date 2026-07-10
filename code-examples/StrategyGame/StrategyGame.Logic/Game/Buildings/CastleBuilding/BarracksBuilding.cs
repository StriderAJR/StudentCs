using StrategyGame.ConsoleGame.Game.PlayerTypes;
using StrategyGame.ConsoleGame.Game.Resources;
using StrategyGame.ConsoleGame.Game.Units.Playable;

namespace StrategyGame.ConsoleGame.Game.Buildings.CastleBuilding;

public class BarracksBuilding : CastleBuilding
{
    public BarracksBuilding() : base("Казармы", new Dictionary<Type,int>
    {
        { typeof(Wood), 5 },
        { typeof(Stone), 3 },
        { typeof(Gold), 2 }
    }, new Dictionary<Type, Dictionary<Type,int>>
    {
        { typeof(InfantryUnit), new Dictionary<Type,int> { { typeof(Gold), 1 } } }
    }) { }

    /// <inheritdoc/>
    public override void ApplyWeeklyEffect(Castle castle, Player player)
    {
        if (!IsBuilt) return;
        // производим 2 пехотинца в неделю, храним в здании
        ProduceUnit(typeof(InfantryUnit), 2);
    }

    /// <inheritdoc/>
    public override string Print(Castle castle)
    {
        int available = GetProducedCount(typeof(InfantryUnit));
        return $"{Name}: {(IsBuilt ? "построено" : "не построено")} | доступно: {available}";
    }
}
