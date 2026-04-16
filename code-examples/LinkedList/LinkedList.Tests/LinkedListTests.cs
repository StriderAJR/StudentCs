namespace LinkedList.Tests;

[TestFixture]
internal class LinkedListTests
{
    [Test]
    public void AddToTheBegginning()
    {
        LinkedList<int> list = new LinkedList<int>();
        list.Add(1);

        Assert.IsTrue(list.Count == 1, "Added only one element to the list");
    }

    [Test]
    public void RemoveByIndex_RemoveFromMiddle()
    {
        LinkedList<int> list = new LinkedList<int>();
        list.Add(1);
        list.Add(2);
        list.Add(3);

        list.RemoveByIndex(1);

        Assert.That(2, Is.EqualTo(list.Count));
        Assert.That(1, Is.EqualTo(list[0]));
        Assert.That(3, Is.EqualTo(list[1]));
    }

    [Test]
    public void Add_WithIndex_InsertInMiddle()
    {
        LinkedList<int> list = new LinkedList<int>();
        list.Add(1);
        list.Add(2);
        list.Add(3);

        list.Add(4, 2);

        Assert.That(4, Is.EqualTo(list.Count));
        Assert.That(1, Is.EqualTo(list[0]));
        Assert.That(2, Is.EqualTo(list[1]));
        Assert.That(4, Is.EqualTo(list[2]));
        Assert.That(3, Is.EqualTo(list[3]));
    }

    [Test]
    public void TryGetByIndex_IndexLessThenZero()
    {
        LinkedList<int> list = new LinkedList<int>();
        Assert.Throws<ArgumentException>(() =>
            {
                var _ = list[-1];
            });
    }
}
