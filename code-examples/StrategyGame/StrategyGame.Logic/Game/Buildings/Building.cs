using StrategyGame.ConsoleGame.Game.MapTypes;
using StrategyGame.ConsoleGame.Game.PlayerTypes;
using StrategyGame.ConsoleGame.UI.CustomConsole;

namespace StrategyGame.ConsoleGame.Game.Buildings;

/// <summary>
/// Представляет общее строение на карте (шахта, лесопилка, карьер и т.д.).
/// Содержит позицию, тип, владельца и базовый доход.
/// </summary>
public class Building
{
    /// <summary>
    /// Позиция строения на карте.
    /// </summary>
    public Coordinate Position { get; }

    /// <summary>
    /// Тип ячейки, соответствующий строению.
    /// </summary>
    public MapCell Type { get; }

    /// <summary>
    /// Владелец строения (игрок) или null, если незахвачено.
    /// </summary>
    public Player Owner { get; private set; }

    /// <summary>
    /// Доход в ресурсах в неделю от этого строения.
    /// </summary>
    public int IncomePerWeek { get; protected set; }

    public Building(Coordinate position, MapCell type)
    {
        Position = position;
        Type = type;

        IncomePerWeek = type switch
        {
            MapCell.Gold => 2,
            MapCell.Wood => 3,
            MapCell.Stone => 2,
            _ => 0
        };
    }

    protected Building() { }

    public bool IsCaptured => Owner != null;

    public void Capture(Player player)
    {
        Owner = player;
    }

    /// <summary>
    /// Применить начисление дохода к указанному игроку (добавляет ресурсы через параметры по ссылке).
    /// </summary>
    public virtual void ApplyIncome(Player player, ref int wood, ref int stone, ref int gold)
    {
        if (player == null) return;
        // поведение по умолчанию базируется на Type
        switch (Type)
        {
            case MapCell.Wood: wood += IncomePerWeek; break;
            case MapCell.Stone: stone += IncomePerWeek; break;
            case MapCell.Gold: gold += IncomePerWeek; break;
        }
    }

    /// <summary>
    /// Получить цвет для отображения строения в консоли (по владельцу).
    /// </summary>
    public ConsoleColor GetColor()
    {
        if (Owner == null)
            return ConsoleColor.Gray;

        return UITheme.FromPlayerColor(Owner.Color);
    }
}
