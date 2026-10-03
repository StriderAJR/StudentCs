using System.IO;
using NUnit.Framework;

[TestFixture]
public class ProgramTests
{
    private string[] _lines = null!;

    [SetUp]
    public void SetUp() => _lines = File.ReadAllLines("event_server.log");

    [Test]
    public void FilterByDate_ReturnsOnlyMaintenanceDayEntries()
    {
        var result = Program.FilterByDate(_lines, "2026-09-02");
        Assert.That(result, Has.Length.EqualTo(6));
    }

    [Test]
    public void FilterByLevel_ReturnsAllDatabaseErrors()
    {
        var result = Program.FilterByLevel(_lines, "Error");

        Assert.That(result, Has.Length.EqualTo(3));
        Assert.That(result[1], Does.Contain("[Database]"));
        Assert.That(result[2], Does.Contain("[Database]"));
    }

    [Test]
    public void FilterByCategory_ReturnsOnlyCombatRecords()
    {
        var result = Program.FilterByCategory(_lines, "Combat");

        Assert.That(result, Is.Not.Empty);
        Assert.That(result[0], Does.Contain("[Combat]"));
    }

    [Test]
    public void Search_IsCaseInsensitive()
    {
        var result = Program.Search(_lines, "сердце эмберфанга");
        Assert.That(result, Has.Length.EqualTo(4));
    }

    [Test]
    public void CountEntries_CountsFatalRecords()
    {
        Assert.That(Program.CountEntries(_lines, "[Fatal]"), Is.EqualTo(1));
    }

    [Test]
    public void GetServerStatus_ReturnsCriticalWhenFatalServerEntryExists()
    {
        Assert.That(Program.GetServerStatus(_lines),
            Is.EqualTo("КРИТИЧЕСКАЯ ОШИБКА: сервер остановлен"));
    }
}
