using StrategyGame.ConsoleGame.Game.MapTypes;
using StrategyGame.ConsoleGame.Game.PlayerTypes;

namespace StrategyGame.ConsoleGame.Game.Buildings;

/// <summary>
/// Карьер — генерирует камень еженедельно.
/// </summary>
public class StoneQuarry : Building
{
    public StoneQuarry(Coordinate pos) : base(pos, MapCell.Stone)
    {
        IncomePerWeek = 2;
    }

    /// <inheritdoc/>
    public override void ApplyIncome(Player player, ref int wood, ref int stone, ref int gold)
    {
        if (player == null) return;
        stone += IncomePerWeek;
    }
}
