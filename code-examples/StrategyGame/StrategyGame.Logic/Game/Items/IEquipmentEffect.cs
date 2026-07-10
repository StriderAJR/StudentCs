using StrategyGame.ConsoleGame.Game.PlayerTypes;
using StrategyGame.ConsoleGame.Game.Units;

namespace StrategyGame.ConsoleGame.Game.Items;

/// <summary>
/// Интерфейс для эффектов экипировки (оружие, броня, артефакты).
/// Позволяет добавлять бонусы атаки/защиты и хуки на события боя/хода.
/// </summary>
public interface IEquipmentEffect
{
    /// <summary>
    /// Возвращает бонус к атаке, который должен применяться к атакам юнита.
    /// </summary>
    /// <param name="player">Игрок-владелец экипировки.</param>
    /// <param name="unit">Юнит, который атакует.</param>
    /// <returns>Целочисленный бонус к атаке.</returns>
    int GetAttackBonus(Player player, UnitBase unit);

    /// <summary>
    /// Возвращает дополнительную защиту, которая применяется при атаке по юниту.
    /// </summary>
    /// <param name="player">Игрок-владелец экипировки.</param>
    /// <param name="unit">Юнит, который защищается.</param>
    /// <returns>Целочисленный бонус к защите.</returns>
    int GetDefenseBonus(Player player, UnitBase unit);

    /// <summary>
    /// Опциональный хук, вызываемый когда юнит наносит удар.
    /// Реализация по умолчанию ничего не делает.
    /// </summary>
    /// <param name="player">Игрок-владелец экипировки.</param>
    /// <param name="attacker">Атакующий юнит.</param>
    /// <param name="defender">Защищающийся юнит.</param>
    /// <param name="damage">Нанесённый урон.</param>
    void OnHit(Player player, UnitBase attacker, UnitBase defender, int damage) { }

    /// <summary>
    /// Опциональный хук, вызываемый в начале хода игрока.
    /// Реализация по умолчанию ничего не делает.
    /// </summary>
    /// <param name="player">Игрок-владелец экипировки.</param>
    /// <param name="unit">Юнит, к которому применяется эффект.</param>
    void OnTurnStart(Player player, UnitBase unit) { }
}
