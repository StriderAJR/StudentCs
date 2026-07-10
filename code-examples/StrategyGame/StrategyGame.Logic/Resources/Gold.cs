namespace StrategyGame.Logic.Resources;

public class Gold : Resource
{
    public Gold(int amount = 0) : base(amount)
    {
    }

    public override string Name => "Gold";
}
