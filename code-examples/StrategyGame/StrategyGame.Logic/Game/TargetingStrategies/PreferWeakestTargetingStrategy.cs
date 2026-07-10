using StrategyGame.ConsoleGame.Game.Units;

namespace StrategyGame.ConsoleGame.Game.TargetingStrategies
{
    public class PreferWeakestTargetingStrategy : ITargetingStrategy
    {
        public UnitBase SelectTarget(UnitBase source, IEnumerable<UnitBase> possibleTargets, Random rng)
        {
            var alive = possibleTargets.Where(p => p != null && p.IsAlive).ToList();
            if (alive.Count == 0) return null;
            // choose weakest by current HP (lowest)
            var ordered = alive.OrderBy(u => u.CurrentHp).ToList();
            return ordered.First();
        }
    }
}
