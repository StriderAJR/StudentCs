namespace StrategyGame.ConsoleGame.Game.Units.Playable;

public class BeastUnit : UnitBase
{
    /// <inheritdoc/>
    public override string TypeName => "Beast";

    /// <inheritdoc/>
    public override int Attack { get; } = 7;

    /// <inheritdoc/>
    public override int BaseDefense { get; } = 0;

    /// <inheritdoc/>
    public override int MaxHp { get; } = 8;

    public BeastUnit() { CurrentHp = MaxHp; }
}
