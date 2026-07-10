using StrategyGame.ConsoleGame.Game.Items;
using StrategyGame.ConsoleGame.Game.PlayerTypes;
using StrategyGame.ConsoleGame.Game.Units;

namespace StrategyGame.ConsoleGame.Game.AttackStrategies;

/// <summary>
/// ѕроста€ стратеги€ атаки: наносит базовый урон и сообщает экипировке о попадании.
/// </summary>
public class SimpleAttackStrategy : IAttackStrategy
{
    /// <inheritdoc/>
    public void PerformAttack(ICombatant attacker, ICombatant defender, Player owner, int ownerAttackBonus)
    {
        // просто: урон = атака атакующего + бонус от экипировки
        int damage = attacker.Attack + ownerAttackBonus;
        defender.TakeDamage(damage, owner != null ? owner.GetDefenseForUnit(defender) : 0);
        // уведомл€ем экипировку о попадании
        if (owner.EquippedWeapon is IEquipmentEffect weap) weap.OnHit(owner, attacker as UnitBase, defender as UnitBase, damage);
        if (owner.EquippedArtifact is IEquipmentEffect art) art.OnHit(owner, attacker as UnitBase, defender as UnitBase, damage);
    }
}
