using StrategyGame.ConsoleGame.Game.PlayerTypes;
using StrategyGame.ConsoleGame.Game.Units;

namespace StrategyGame.ConsoleGame.Game.Items;

/// <summary>
/// Weapon types.
/// </summary>
public enum WeaponType
{
    Sword,
    Bow,
    Staff
}

/// <summary>
/// Weapon item which can grant attack bonuses in certain conditions.
/// </summary>
public class Weapon : Item, IEquipmentEffect
{
    public WeaponType Type { get; }

    /// <summary>
    /// Attack bonus when appropriate.
    /// </summary>
    public int AttackBonus { get; }

    public override string Name => Type.ToString("G");

    public override string Description => $"Оружие {Type}";

    public Weapon(WeaponType type, int attackBonus) : base()
    {
        Type = type;
        AttackBonus = attackBonus;
    }

    /// <inheritdoc/>
    public int GetAttackBonus(Player player, UnitBase unit)
    {
        if (player == null || unit == null) return 0;
        // Provide bonus when unit/player specializes in weapon
        return player.Type == PlayerType.Mage && Type == WeaponType.Staff
            || player.Type == PlayerType.Ranger && Type == WeaponType.Bow
            || player.Type == PlayerType.Knight && Type == WeaponType.Sword
            ? AttackBonus
            : 0;
    }

    /// <inheritdoc/>
    public int GetDefenseBonus(Player player, UnitBase unit) => 0;

    /// <inheritdoc/>
    public void OnHit(Player player, UnitBase attacker, UnitBase defender, int damage) { }

    /// <inheritdoc/>
    public void OnTurnStart(Player player, UnitBase unit) { }
}
