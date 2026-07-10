using StrategyGame.ConsoleGame.Game.PlayerTypes;
using StrategyGame.ConsoleGame.Game.Resources;
using StrategyGame.ConsoleGame.Game.Units.Playable;

namespace StrategyGame.ConsoleGame.Game.Buildings.CastleBuilding;

public class FarmBuilding : CastleBuilding
{
    public FarmBuilding() : base("Ферма", new Dictionary<Type, int>
    {
        { typeof(Wood), 3 },
        { typeof(Stone), 1 },
        { typeof(Gold), 1 }
    }, new Dictionary<Type, Dictionary<Type, int>>
    {
        { typeof(BeastUnit), new Dictionary<Type,int> { { typeof(Gold), 1 } } }
    }) { }

    /// <inheritdoc/>
    public override void ApplyWeeklyEffect(Castle castle, Player player)
    {
        if (!IsBuilt) return;
        // производим 1 зверя в неделю, храним в здании
        ProduceUnit(typeof(BeastUnit), 1);
    }

    /// <inheritdoc/>
    public override string Print(Castle castle)
    {
        int available = GetProducedCount(typeof(BeastUnit));
        return $"{Name}: {(IsBuilt ? "построено" : "не построено")} | доступно: {available}";
    }
}
