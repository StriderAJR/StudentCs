using NUnit.Framework;

public class MonitorTests
{
    [Test]
    public void RenderGraph_HasTwentyColumns()
    {
        Assert.That(Monitor.RenderGraph(new[] { 0, 50, 100 }).Length, Is.EqualTo(20));
    }
}
