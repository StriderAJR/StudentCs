using StrategyGame.ConsoleGame.Game.AttackStrategies;
using StrategyGame.ConsoleGame.Game.PlayerTypes;
using StrategyGame.ConsoleGame.Game.TargetingStrategies;

namespace StrategyGame.ConsoleGame.Game.Units;

/// <summary>
/// Базовый интерфейс для боевых сущностей (юнит или стек).
/// </summary>
public interface ICombatant
{
    /// <summary>
    /// Отображаемое имя типа юнита.
    /// </summary>
    string TypeName { get; }

    /// <summary>
    /// Сила атаки.
    /// </summary>
    int Attack { get; }

    /// <summary>
    /// Базовое значение защиты.
    /// </summary>
    int BaseDefense { get; }

    /// <summary>
    /// Максимальное здоровье юнита.
    /// </summary>
    int MaxHp { get; }

    /// <summary>
    /// Текущее здоровье юнита/фронтового юнита в стеке.
    /// </summary>
    int CurrentHp { get; set; }

    /// <summary>
    /// Признак что объект жив.
    /// </summary>
    bool IsAlive { get; }

    /// <summary>
    /// Временное бонусное значение защиты (например, бонус защитников в замке).
    /// </summary>
    int TempDefenseBonus { get; set; }

    /// <summary>
    /// Количество единиц в стеке (1 для одиночного юнита).
    /// </summary>
    int Count { get; }

    /// <summary>
    /// Стратегия выбора цели.
    /// </summary>
    ITargetingStrategy TargetingStrategy { get; set; }

    /// <summary>
    /// Стратегия нанесения урона (поведение атаки).
    /// </summary>
    IAttackStrategy AttackStrategy { get; set; }

    /// <summary>
    /// Владелец (игрок) данного боевого объекта — может быть null для монстров.
    /// </summary>
    Player Owner { get; set; }

    /// <summary>
    /// Применить получение урона к этому объекту.
    /// </summary>
    void TakeDamage(int damage, int additionalDefense);

    /// <summary>
    /// Моментально уничтожить объект (сделать негодным).
    /// </summary>
    void Kill();

    /// <summary>
    /// Восстановить здоровье фронтового юнита до максимума.
    /// </summary>
    void RestoreFullHp();

    /// <summary>
    /// Выполнить атаку по цели (делегирует стратегии).
    /// </summary>
    void AttackTarget(ICombatant target);

    /// <summary>
    /// Текстовое отображение статуса (для UI).
    /// </summary>
    string DisplayInfo();
}
