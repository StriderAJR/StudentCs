using NUnit.Framework;

public class InteractionTests
{
    [Test]
    public void Interact_UsesPolymorphicObjectBehavior()
    {
        var monster = new Monster("Orc", 50, 10);
        var result = monster.Interact(new Player("Hero", 100));
        Assert.That(result, Does.Contain("атак"));
    }
}
