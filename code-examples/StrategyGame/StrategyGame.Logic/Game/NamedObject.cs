namespace StrategyGame.ConsoleGame.Game;

/// <summary>
/// Базовый абстрактный класс для объектов с именем.
/// </summary>
public abstract class NamedObject
{
    public abstract string Name { get; }

    public abstract string Description { get; }
}
