using StrategyGame.ConsoleGame.Game.AttackStrategies;
using StrategyGame.ConsoleGame.Game.PlayerTypes;
using StrategyGame.ConsoleGame.Game.TargetingStrategies;

namespace StrategyGame.ConsoleGame.Game.Units;

/// <summary>
/// Абстрактный класс для описания свойств одного юнита (не стека).
/// Содержит дефолтные стратегии и реализацию базовой логики урона.
/// </summary>
public abstract class UnitBase : ICombatant
{
    /// <inheritdoc/>
    public virtual string TypeName => GetUnitName();

    /// <inheritdoc/>
    public abstract int Attack { get; }

    /// <inheritdoc/>
    public abstract int BaseDefense { get; }

    /// <inheritdoc/>
    public abstract int MaxHp { get; }

    /// <inheritdoc/>
    public int CurrentHp { get; set; }

    /// <inheritdoc/>
    public bool IsAlive => CurrentHp > 0;

    /// <inheritdoc/>
    public int TempDefenseBonus { get; set; } = 0;

    /// <inheritdoc/>
    public virtual int Count => 1;

    /// <inheritdoc/>
    public ITargetingStrategy TargetingStrategy { get; set; }

    /// <inheritdoc/>
    public IAttackStrategy AttackStrategy { get; set; }

    /// <inheritdoc/>
    public Player Owner { get; set; }

    /// <summary>
    /// Инициализация дефолтных стратегий (целевой выбор и атака).
    /// </summary>
    public UnitBase()
    {
        TargetingStrategy = new RandomTargetingStrategy();
        AttackStrategy = new SimpleAttackStrategy();
    }

    /// <summary>
    /// Возвращает строку статуса для UI по умолчанию.
    /// </summary>
    public virtual string DisplayInfo() => $"{TypeName} HP:{CurrentHp}/{MaxHp} ATK:{Attack}";

    /// <summary>
    /// Формирование имени типа по имени класса по умолчанию (удаляет суффикс "Unit").
    /// </summary>
    protected string GetUnitName() => GetType().Name.Replace("Unit", string.Empty);

    /// <inheritdoc/>
    public virtual void TakeDamage(int damage, int additionalDefense)
    {
        int totalDefense = BaseDefense + additionalDefense + TempDefenseBonus;
        int reduced = damage - totalDefense;
        if (reduced < 0) reduced = 0;
        CurrentHp -= reduced;
        if (CurrentHp < 0) CurrentHp = 0;
    }

    /// <inheritdoc/>
    public virtual void Kill() => CurrentHp = 0;

    /// <inheritdoc/>
    public virtual void RestoreFullHp()
    {
        CurrentHp = MaxHp;
    }

    /// <inheritdoc/>
    public virtual void AttackTarget(ICombatant target)
    {
        AttackStrategy?.PerformAttack(this, target, Owner, Owner != null ? Owner.GetWeaponAttackBonus(this) : 0);
    }
}
