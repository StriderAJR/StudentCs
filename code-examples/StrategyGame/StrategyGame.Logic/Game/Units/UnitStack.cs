using StrategyGame.ConsoleGame.Game.TargetingStrategies;
using StrategyGame.ConsoleGame.Game.AttackStrategies;
using StrategyGame.ConsoleGame.Game.PlayerTypes;

namespace StrategyGame.ConsoleGame.Game.Units;

/// <summary>
/// Интерфейс стека юнитов, расширяющий <see cref="ICombatant"/> и добавляющий операцию добавления.
/// </summary>
public interface IUnitStack : ICombatant
{
    /// <summary>
    /// Количество единиц в стеке.
    /// </summary>
    new int Count { get; }

    /// <summary>
    /// Добавить (или убрать, если отрицательное) количество юнитов в стек.
    /// </summary>
    void Add(int amount);
}

/// <summary>
/// Непараметризованная реализация стека одинаковых юнитов в одной ячейке.
/// Не наследуется от <see cref="UnitBase"/>, а делегирует характеристики прототипу.
/// </summary>
public class UnitStack : IUnitStack
{
    private readonly UnitBase prototype;

    /// <summary>
    /// Создаёт новый стек с указанным прототипом и начальным количеством.
    /// </summary>
    public UnitStack(UnitBase prototype, int count = 1)
    {
        this.prototype = prototype ?? throw new ArgumentNullException(nameof(prototype));
        Count = Math.Clamp(count, 0, 99);
        CurrentHp = Count > 0 ? MaxHp : 0;
        // Инициализируем временный бонус у прототипа
        this.prototype.TempDefenseBonus = 0;

        // стандартные стратегии
        TargetingStrategy = new RandomTargetingStrategy();
        AttackStrategy = new SimpleAttackStrategy();
    }

    /// <summary>
    /// Имя типа, делегируется прототипу.
    /// </summary>
    public string TypeName => prototype.TypeName;

    /// <summary>
    /// Сила атаки для одного юнита (делегируется прототипу).
    /// </summary>
    public int Attack => prototype.Attack;

    /// <summary>
    /// Базовая защита для одного юнита (делегируется прототипу).
    /// </summary>
    public int BaseDefense => prototype.BaseDefense;

    /// <summary>
    /// Максимальное HP одного юнита (делегируется прототипу).
    /// </summary>
    public int MaxHp => prototype.MaxHp;

    /// <summary>
    /// Количество единиц в стеке.
    /// </summary>
    public int Count { get; private set; }

    /// <summary>
    /// Текущее здоровье фронтового юнита в стеке.
    /// </summary>
    public int CurrentHp { get; set; }

    /// <summary>
    /// Временный бонус к защите — делегируется прототипу, поскольку это временный эффект на тип внутри слота.
    /// </summary>
    public int TempDefenseBonus { get => prototype.TempDefenseBonus; set => prototype.TempDefenseBonus = value; }

    /// <summary>
    /// Стратегия выбора цели для стека.
    /// </summary>
    public ITargetingStrategy TargetingStrategy { get; set; }

    /// <summary>
    /// Стратегия атаки для стека.
    /// </summary>
    public IAttackStrategy AttackStrategy { get; set; }

    /// <summary>
    /// Владелец стека (игрок) — может быть null для монстров.
    /// </summary>
    public Player Owner { get; set; }

    /// <summary>
    /// Проверка, жив ли стек (есть ли юниты и здоровье).
    /// </summary>
    public bool IsAlive => Count > 0 && CurrentHp > 0;

    /// <summary>
    /// Добавляет или убирает указанное количество юнитов в стеке (клинчится 0..99).
    /// </summary>
    public void Add(int amount)
    {
        if (amount == 0) return;
        int prev = Count;
        Count = Math.Clamp(Count + amount, 0, 99);
        if (prev == 0 && Count > 0 && CurrentHp <= 0)
            CurrentHp = MaxHp;
    }

    /// <summary>
    /// Возвращает строковое описание стека для UI.
    /// </summary>
    public string DisplayInfo() => Count > 0 ? $"{TypeName} x{Count} HP:{CurrentHp}/{MaxHp} ATK:{Attack}" : "пуст";

    /// <summary>
    /// Применяет урон к стеку; урон распределяется по юнитам последовательно.
    /// </summary>
    public void TakeDamage(int damage, int additionalDefense)
    {
        if (Count == 0) return;

        int totalDefense = BaseDefense + additionalDefense + TempDefenseBonus;
        int reduced = damage - totalDefense;
        if (reduced < 0) reduced = 0;

        int remainingDamage = reduced;

        while (remainingDamage > 0 && Count > 0)
        {
            if (CurrentHp > remainingDamage)
            {
                CurrentHp -= remainingDamage;
                remainingDamage = 0;
            }
            else
            {
                remainingDamage -= CurrentHp;
                Count--;
                if (Count > 0)
                {
                    CurrentHp = MaxHp;
                }
                else
                {
                    CurrentHp = 0;
                }
            }
        }

        if (Count == 0)
            CurrentHp = 0;
    }

    /// <summary>
    /// Мгновенно уничтожить весь стек.
    /// </summary>
    public void Kill()
    {
        Count = 0;
        CurrentHp = 0;
    }

    /// <summary>
    /// Восстановить HP фронтового юнита до максимума, если стек не пуст.
    /// </summary>
    public void RestoreFullHp()
    {
        if (Count > 0) CurrentHp = MaxHp;
    }

    /// <summary>
    /// Выполнить атаку по цели, делегируя логику стратегии атаки.
    /// </summary>
    public void AttackTarget(ICombatant target)
    {
        AttackStrategy?.PerformAttack(this, target, Owner, Owner != null ? Owner.GetWeaponAttackBonus(prototype) : 0);
    }
}

/// <summary>
/// Небольшая generic-обёртка для удобного создания стека по типу юнита.
/// </summary>
public class UnitStack<TUnit> : UnitStack where TUnit : UnitBase, new()
{
    /// <summary>
    /// Создать стек конкретного типа с указанным количеством.
    /// </summary>
    public UnitStack(int count = 1) : base(new TUnit(), count) { }
}
