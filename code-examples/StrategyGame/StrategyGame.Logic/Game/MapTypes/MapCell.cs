namespace StrategyGame.ConsoleGame.Game.MapTypes;

/// <summary>
/// Тип ячейки на карте.
/// </summary>
public enum MapCell
{
    /// <summary>
    /// Пустая ячейка.
    /// </summary>
    Empty,
    /// <summary>
    /// Стена (недоступная для передвижения).
    /// </summary>
    Wall,
    /// <summary>
    /// Золото.
    /// </summary>
    Gold,
    /// <summary>
    /// Дерево.
    /// </summary>
    Wood,
    /// <summary>
    /// Камень.
    /// </summary>
    Stone,
    /// <summary>
    /// Позиция игрока (не хранится как отдельная сущность на карте).
    /// </summary>
    Player,
    /// <summary>
    /// Монстр.
    /// </summary>
    Monster,
    /// <summary>
    /// Замок.
    /// </summary>
    Castle
}
