using NUnit.Framework;

public class GameServerTests
{
    [Test]
    public void FindByLogin_ReturnsExpectedPlayer()
    {
        var server = GameServer.Load("players.csv");
        Assert.That(server.FindByLogin("PixelMage").Level, Is.EqualTo(12));
    }
}
