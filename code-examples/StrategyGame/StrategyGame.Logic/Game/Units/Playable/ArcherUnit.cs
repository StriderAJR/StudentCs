namespace StrategyGame.ConsoleGame.Game.Units.Playable;

public class ArcherUnit : UnitBase
{
    /// <inheritdoc/>
    public override string TypeName => "Archer";

    /// <inheritdoc/>
    public override int Attack { get; } = 5;

    /// <inheritdoc/>
    public override int BaseDefense { get; } = 0;

    /// <inheritdoc/>
    public override int MaxHp { get; } = 10;

    public ArcherUnit() { CurrentHp = MaxHp; }
}
