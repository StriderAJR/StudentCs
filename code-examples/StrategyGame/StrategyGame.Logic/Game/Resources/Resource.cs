namespace StrategyGame.ConsoleGame.Game.Resources;

/// <summary>
/// Базовый абстрактный класс для представления ресурса (например, дерево/камень/золото).
/// Хранит имя ресурса и текущее количество.
/// </summary>
public abstract class Resource : NamedObject
{
    public abstract override string Name { get; }
    public override string Description => $"{Name}: {Amount}";
    public int Amount { get; set; }

    protected Resource(int amount = 0)
    {
        Amount = amount;
    }

    public override string ToString() => $"{Name}: {Amount}";
}
