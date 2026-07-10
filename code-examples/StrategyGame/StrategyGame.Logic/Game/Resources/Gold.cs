namespace StrategyGame.ConsoleGame.Game.Resources;

public class Gold : Resource
{
    public Gold(int amount = 0) : base(amount) { }

    public override string Name => "Золото";

    public override string Description => $"{Name}: {Amount}";
}
