using UnitTestsDemo.MainProject;

namespace UnitTestsDemo.Tests;

public class Tests
{
    [Test]
    public void Add_TwoNumbers_ReturnsSum()
    {
        var result = Program.Add(2, 3);

        Assert.That(result, Is.EqualTo(5));
    }

    [Test]
    public void Max_FirstNumberIsGreater_ReturnsFirstNumber()
    {
        var result = Program.Max(10, 5);

        Assert.That(result, Is.EqualTo(10));
    }

    [Test]
    public void IsEven_EvenNumber_ReturnsTrue()
    {
        var result = Program.IsEven(10);

        Assert.That(result, Is.True);
    }

    [Test]
    public void IsEven_OddNumber_ReturnsFalse()
    {
        var result = Program.IsEven(11);

        Assert.That(result, Is.False);
    }

    [Test]
    public void SumFromOneTo_Five_ReturnsFifteen()
    {
        var result = Program.SumFromOneTo(5);

        Assert.That(result, Is.EqualTo(15));
    }

    [Test]
    public void CountDigits_FourDigitNumber_ReturnsFour()
    {
        var result = Program.CountDigits(1234);

        Assert.That(result, Is.EqualTo(4));
    }

    [Test]
    public void GetNumberSign_PositiveNumber_ReturnsPositive()
    {
        var result = Program.GetNumberSign(10);

        Assert.That(result, Is.EqualTo("Positive"));
    }

    [Test]
    public void Repeat_TextThreeTimes_ReturnsRepeatedText()
    {
        var result = Program.Repeat("abc", 3);

        Assert.That(result, Is.EqualTo("abcabcabc"));
    }
}