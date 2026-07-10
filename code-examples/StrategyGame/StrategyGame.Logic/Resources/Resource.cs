namespace StrategyGame.Logic.Resources;

public abstract class Resource
{
    public abstract string Name { get; }
    public virtual string Description => $"{Name}: {Amount}";
    public int Amount { get; set; }

    protected Resource(int amount = 0)
    {
        Amount = amount;
    }

    public override string ToString()
    {
        return $"{Name}: {Amount}";
    }
}
