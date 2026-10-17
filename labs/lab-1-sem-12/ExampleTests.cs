using NUnit.Framework;

public class WorldTests
{
    [Test]
    public void Load_CreatesMonsterFromM()
    {
        var world = World.Load("world.txt");
        Assert.That(world.GetAt(5, 1), Is.TypeOf<Monster>());
    }
}
