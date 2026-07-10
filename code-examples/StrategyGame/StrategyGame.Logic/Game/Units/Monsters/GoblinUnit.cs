using StrategyGame.ConsoleGame.Game.TargetingStrategies;
using StrategyGame.ConsoleGame.Game.AttackStrategies;

namespace StrategyGame.ConsoleGame.Game.Units.Monsters
{
    /// <summary>
    /// Юнит типа "Гоблин". Использует прицельную стратегию и специальную стратегию атаки с шансом критического удара.
    /// </summary>
    public class GoblinUnit : UnitBase
    {
        public override string TypeName => "Goblin";
        public override int Attack { get; }
        public override int BaseDefense { get; }
        public override int MaxHp { get; }

        public GoblinUnit(int attack, int maxHp, int baseDefense = 0)
        {
            Attack = attack;
            MaxHp = maxHp;
            BaseDefense = baseDefense;
            CurrentHp = MaxHp;

            // гоблины немного чаще выбирают самые слабые цели
            TargetingStrategy = new PreferWeakestTargetingStrategy();
            // у гоблина небольшой шанс критического удара: используем специальную стратегию атаки
            AttackStrategy = new GoblinAttackStrategy();
        }
    }
}
