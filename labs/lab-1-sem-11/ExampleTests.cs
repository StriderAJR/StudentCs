using System.Linq;
using NUnit.Framework;

public class ItemStackTests
{
    [Test]
    public void Split_DistributesRemainderToFirstStack()
    {
        var result = new ItemStack("Камень", 10).Split(3);
        Assert.That(result.Select(x => x.Count), Is.EqualTo(new[] { 4, 3, 3 }));
    }
}
