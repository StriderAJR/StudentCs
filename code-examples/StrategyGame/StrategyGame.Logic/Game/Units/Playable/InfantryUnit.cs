namespace StrategyGame.ConsoleGame.Game.Units.Playable;

public class InfantryUnit : UnitBase
{
    /// <inheritdoc/>
    public override string TypeName => "Infantry";

    /// <inheritdoc/>
    public override int Attack { get; } = 6;

    /// <inheritdoc/>
    public override int BaseDefense { get; } = 0;

    /// <inheritdoc/>
    public override int MaxHp { get; } = 12;

    public InfantryUnit() { CurrentHp = MaxHp; }
}
