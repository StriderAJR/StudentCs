using StrategyGame.ConsoleGame.Game.PlayerTypes;
using StrategyGame.ConsoleGame.Game.Resources;

namespace StrategyGame.ConsoleGame.Game.Buildings.CastleBuilding;

public class StableBuilding : CastleBuilding
{
    public StableBuilding() : base("Конюшня", new Dictionary<Type, int>
    {
        { typeof(Wood), 4 },
        { typeof(Stone), 2 },
        { typeof(Gold), 3 }
    }) { }

    /// <inheritdoc/>
    public override void ApplyWeeklyEffect(Castle castle, Player player)
    {
        if (!IsBuilt) return;
        // запланировать бонус к передвижению на следующую неделю. Установить временный бонус игроку.
        player.TempMoveBonusPercent = Math.Max(player.TempMoveBonusPercent, 50);
    }

    /// <inheritdoc/>
    public override string Print(Castle castle)
    {
        return $"{Name}: {(IsBuilt ? "построено" : "не построено")}";
    }
}
