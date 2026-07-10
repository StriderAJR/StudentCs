using StrategyGame.ConsoleGame.Game.MapTypes;
using StrategyGame.ConsoleGame.Game.PlayerTypes;

namespace StrategyGame.ConsoleGame.Game.Buildings;

/// <summary>
/// Ћесопилка Ч генерирует древесину еженедельно.
/// </summary>
public class WoodMill : Building
{
    public WoodMill(Coordinate pos) : base(pos, MapCell.Wood)
    {
        IncomePerWeek = 3;
    }

    /// <inheritdoc/>
    public override void ApplyIncome(Player player, ref int wood, ref int stone, ref int gold)
    {
        if (player == null) return;
        wood += IncomePerWeek;
    }
}
