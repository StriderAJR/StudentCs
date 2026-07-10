using StrategyGame.ConsoleGame.Game.MapTypes;
using StrategyGame.ConsoleGame.Game.PlayerTypes;

namespace StrategyGame.ConsoleGame.Game.Buildings;

/// <summary>
/// «олота€ шахта Ч генерирует золото еженедельно.
/// </summary>
public class GoldMine : Building
{
    public GoldMine(Coordinate pos) : base(pos, MapCell.Gold)
    {
        IncomePerWeek = 2;
    }

    /// <inheritdoc/>
    public override void ApplyIncome(Player player, ref int wood, ref int stone, ref int gold)
    {
        if (player == null) return;
        gold += IncomePerWeek;
    }
}
