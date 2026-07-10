using StrategyGame.ConsoleGame.Game.AttackStrategies;
using StrategyGame.ConsoleGame.Game.TargetingStrategies;

namespace StrategyGame.ConsoleGame.Game.Units.Monsters
{
    /// <summary>
    /// Юнит типа "Wraith" (призрак). Иногда может частично игнорировать защиту цели.
    /// </summary>
    public class WraithUnit : UnitBase
    {
        public override string TypeName => "Wraith";
        public override int Attack { get; }
        public override int BaseDefense { get; }
        public override int MaxHp { get; }

        public WraithUnit(int attack, int maxHp, int baseDefense = 0)
        {
            Attack = attack;
            MaxHp = maxHp;
            BaseDefense = baseDefense;
            CurrentHp = MaxHp;

            // призрак предпочитает случайный выбор цели, и может фазироваться
            TargetingStrategy = new RandomTargetingStrategy();
            AttackStrategy = new WraithAttackStrategy();
        }
    }
}
