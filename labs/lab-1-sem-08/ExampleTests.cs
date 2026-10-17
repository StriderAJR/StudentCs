using NUnit.Framework;

public class MapTests
{
    [Test]
    public void Restore_ReturnsSavedPlayerPosition()
    {
        var map = Map.Load("map.txt");
        var snapshot = map.CreateSnapshot();
        map.MovePlayer(new Position(2, 2));
        map.Restore(snapshot);
        Assert.That(map.Player.Position, Is.EqualTo(snapshot.PlayerPosition));
    }
}
