using StrategyGame.ConsoleGame.Game.Units;

namespace StrategyGame.ConsoleGame.Game.TargetingStrategies;

public class RandomTargetingStrategy : ITargetingStrategy
{
    public UnitBase SelectTarget(UnitBase source, IEnumerable<UnitBase> possibleTargets, Random rng)
    {
        var alive = possibleTargets.Where(p => p != null && p.IsAlive).ToList();
        if (alive.Count == 0) return null;
        return alive[rng.Next(alive.Count)];
    }
}
