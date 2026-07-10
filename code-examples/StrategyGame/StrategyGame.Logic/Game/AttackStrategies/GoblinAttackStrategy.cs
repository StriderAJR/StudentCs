using StrategyGame.ConsoleGame.Game.PlayerTypes;
using StrategyGame.ConsoleGame.Game.Units;

namespace StrategyGame.ConsoleGame.Game.AttackStrategies
{
    /// <summary>
    /// Стратегия атаки для гоблина: случайно даёт шанс нанести двойной урон.
    /// </summary>
    public class GoblinAttackStrategy : IAttackStrategy
    {
        private readonly Random rng = new();

        /// <inheritdoc/>
        public void PerformAttack(ICombatant attacker, ICombatant defender, Player owner, int ownerAttackBonus)
        {
            // у гоблина небольшой шанс нанести двойной урон
            int damage = attacker.Attack + ownerAttackBonus;
            if (rng.NextDouble() < 0.15)
                damage *= 2; // критический удар
            defender.TakeDamage(damage, owner != null ? owner.GetDefenseForUnit(defender) : 0);
        }
    }
}
