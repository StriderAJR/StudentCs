using System.IO;
using NUnit.Framework;

[TestFixture]
public class ProgramTests
{
    private string[] _lines = null!;

    [SetUp]
    public void SetUp() => _lines = File.ReadAllLines("event_server.log");

    [Test]
    public void Build_UsesOnlyTheEventTimeRange()
    {
        var report = Program.BuildReport(_lines);
        Assert.That(report, Does.Not.Contain("ежедневное техническое обслуживание"));
    }

    [Test]
    public void Build_ReportsWinnerAndEventItem()
    {
        var report = Program.BuildReport(_lines);
        Assert.That(report, Does.Contain("Победитель: Ночные совы"));
        Assert.That(report, Does.Contain("Очки победителя: 2500"));
    }

    [Test]
    public void Build_ReportsClanWithoutEventItem()
    {
        var report = Program.BuildReport(_lines);
        Assert.That(report, Does.Contain("Ивентовый предмет: Сердце Эмберфанга"));
        Assert.That(report, Does.Contain("Утешительная награда Железных волков: 800 очков"));
    }

    [Test]
    public void Build_ReportsTechnicalCounters()
    {
        var report = Program.BuildReport(_lines);
        Assert.That(report, Does.Contain("Предупреждений во время события: 6"));
        Assert.That(report, Does.Contain("Ошибок во время события: 1"));
    }
}
