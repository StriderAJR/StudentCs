using StrategyGame.ConsoleGame.Game.PlayerTypes;
using StrategyGame.ConsoleGame.Game.Units;

namespace StrategyGame.ConsoleGame.Game.AttackStrategies
{
    /// <summary>
    /// Стратегия атаки для призрака (Wraith): иногда игнорирует часть защиты цели.
    /// </summary>
    public class WraithAttackStrategy : IAttackStrategy
    {
        private readonly Random rng = new();

        /// <inheritdoc/>
        public void PerformAttack(ICombatant attacker, ICombatant defender, Player owner, int ownerAttackBonus)
        {
            // призрак наносит обычный урон, но иногда частично игнорирует защиту (фаза)
            int damage = attacker.Attack + ownerAttackBonus;
            bool phased = rng.NextDouble() < 0.2;
            defender.TakeDamage(damage, phased ? 0 : (owner != null ? owner.GetDefenseForUnit(defender) : 0));
        }
    }
}
