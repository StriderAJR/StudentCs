using StrategyGame.ConsoleGame.Game.PlayerTypes;
using StrategyGame.ConsoleGame.Game.Units;

namespace StrategyGame.ConsoleGame.Game.AttackStrategies;

/// <summary>
/// Интерфейс стратегии атаки. Определяет поведение при выполнении атаки между двумя участниками боя.
/// </summary>
public interface IAttackStrategy
{
    /// <summary>
    /// Выполнить атаку от `attacker` к `defender`.
    /// </summary>
    /// <param name="attacker">Атакующий участник.</param>
    /// <param name="defender">Защищающейся участник.</param>
    /// <param name="owner">Игрок-владелец атакующего (может быть null для монстров).</param>
    /// <param name="ownerAttackBonus">Бонус к атаке от экипировки/артефактов владельца.</param>
    void PerformAttack(ICombatant attacker, ICombatant defender, Player owner, int ownerAttackBonus);
}
