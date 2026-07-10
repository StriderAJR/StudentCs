namespace StrategyGame.ConsoleGame.Game.Items;

/// <summary>
/// Base class for wearable equipment (armor, weapons, artifacts).
/// Provides a display `Name` for UI and identifying purposes.
/// </summary>
public abstract class Item : NamedObject
{
    public override abstract string Name { get; }

    public override abstract string Description { get; }
}
