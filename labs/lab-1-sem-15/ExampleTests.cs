using NUnit.Framework;

public class PlayerExtensionsTests
{
    [Test]
    public void IsDead_IsAvailableAsExtension()
    {
        var player = new Player { Health = 0 };
        Assert.That(player.IsDead(), Is.True);
    }
}
