using StrategyGame.ConsoleGame.Game.PlayerTypes;
using StrategyGame.ConsoleGame.Game.Units;

namespace StrategyGame.ConsoleGame.Game.Items;

/// <summary>
/// Слот брони на голове/теле.
/// </summary>
public enum ArmorSlot { Head, Body }

/// <summary>
/// Armor item that provides defense and optional other effects.
/// Implements IEquipmentEffect to apply effects to units/players.
/// </summary>
public class Armor : Item, IEquipmentEffect
{
    public ArmorSlot Slot { get; }

    /// <summary>
    /// Defense value provided by this armor.
    /// </summary>
    public int Defense { get; }

    public override string Name => "Доспех";

    public override string Description => $"{Name} для {Slot.ToString()}";

    public Armor(string name, int defense) : base()
    {
        Defense = defense;
    }

    /// <inheritdoc/>
    public int GetAttackBonus(Player player, UnitBase unit) => 0;

    /// <inheritdoc/>
    public int GetDefenseBonus(Player player, UnitBase unit) => Defense;

    /// <inheritdoc/>
    public void OnHit(Player player, UnitBase attacker, UnitBase defender, int damage) { }

    /// <inheritdoc/>
    public void OnTurnStart(Player player, UnitBase unit) { }
}
