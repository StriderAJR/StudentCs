using StrategyGame.ConsoleGame.Game.Units;

namespace StrategyGame.ConsoleGame.Game.TargetingStrategies;

public interface ITargetingStrategy
{
    UnitBase SelectTarget(UnitBase source, IEnumerable<UnitBase> possibleTargets, Random rng);
}
