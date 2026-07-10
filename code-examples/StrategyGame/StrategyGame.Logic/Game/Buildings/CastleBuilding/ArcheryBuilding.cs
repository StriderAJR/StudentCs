using StrategyGame.ConsoleGame.Game.PlayerTypes;
using StrategyGame.ConsoleGame.Game.Resources;
using StrategyGame.ConsoleGame.Game.Units.Playable;

namespace StrategyGame.ConsoleGame.Game.Buildings.CastleBuilding;

public class ArcheryBuilding : CastleBuilding
{
    public ArcheryBuilding() : base("—трельбище", new Dictionary<Type, int>
    {
        { typeof(Wood), 4 },
        { typeof(Stone), 2 },
        { typeof(Gold), 2 }
    }, new Dictionary<Type, Dictionary<Type, int>>
    {
        { typeof(ArcherUnit), new Dictionary<Type,int> { { typeof(Gold), 1 } } }
    })
    { }

    /// <inheritdoc/>
    public override void ApplyWeeklyEffect(Castle castle, Player player)
    {
        if (!IsBuilt) return;
        // производим 2 лучника в неделю, храним в здании
        ProduceUnit(typeof(ArcherUnit), 2);
    }

    /// <inheritdoc/>
    public override string Print(Castle castle)
    {
        int available = GetProducedCount(typeof(ArcherUnit));
        return $"{Name}: {(IsBuilt ? "построено" : "не построено")} | доступно: {available}";
    }
}
