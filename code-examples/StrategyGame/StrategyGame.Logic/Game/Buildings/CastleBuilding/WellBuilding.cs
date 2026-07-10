using StrategyGame.ConsoleGame.Game.PlayerTypes;
using StrategyGame.ConsoleGame.Game.Resources;
using StrategyGame.ConsoleGame.Game.Units;

namespace StrategyGame.ConsoleGame.Game.Buildings.CastleBuilding;

public class WellBuilding : CastleBuilding
{
    public WellBuilding() : base("Колодец", new Dictionary<Type, int>
    {
        { typeof(Wood), 2 },
        { typeof(Stone), 2 },
        { typeof(Gold), 2 }
    }) { }

    public override bool HasAction => true;

    /// <inheritdoc/>
    public override void UseAction(Castle castle, Player player)
    {
        if (!IsBuilt) return;
        // исцелить все юниты в гарнизоне замка и восстановить магию игрока
        foreach (var u in castle.Garrison)
        {
            if (u is IUnitStack s && s.Count > 0)
            {
                s.RestoreFullHp();
            }
        }

        player.MagicRemaining = player.MaxMagic;
    }

    /// <inheritdoc/>
    public override string Print(Castle castle)
    {
        return $"{Name}: {(IsBuilt ? "построено" : "не построено")}";
    }
}
