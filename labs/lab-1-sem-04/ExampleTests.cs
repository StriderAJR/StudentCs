using System;
using System.IO;
using NUnit.Framework;

[TestFixture]
public class ProgramTests
{
    private LogEntry[] _entries = null!;

    [SetUp]
    public void SetUp()
    {
        string[] lines = File.ReadAllLines("event_server.log");
        _entries = Program.ParseLog(lines);
    }

    [Test]
    public void ParseLog_SplitsTimestampLevelCategoryAndMessage()
    {
        LogEntry first = _entries[0];

        Assert.That(first.Timestamp, Is.EqualTo(new DateTime(2026, 9, 1, 12, 10, 1, 101)));
        Assert.That(first.Level, Is.EqualTo("Info"));
        Assert.That(first.Category, Is.EqualTo("Server"));
        Assert.That(first.Message, Does.Contain("Событие"));
    }

    [Test]
    public void FilterByDate_ReturnsOnlyMaintenanceDayEntries()
    {
        LogEntry[] result = Program.FilterByDate(_entries, new DateTime(2026, 9, 2));
        Assert.That(result, Has.Length.EqualTo(6));
    }

    [Test]
    public void FilterByLevel_ReturnsAllDatabaseErrors()
    {
        LogEntry[] result = Program.FilterByLevel(_entries, "Error");

        Assert.That(result, Has.Length.EqualTo(3));
        Assert.That(result[1].Category, Is.EqualTo("Database"));
        Assert.That(result[2].Category, Is.EqualTo("Database"));
    }

    [Test]
    public void FilterByCategory_ReturnsOnlyCombatRecords()
    {
        LogEntry[] result = Program.FilterByCategory(_entries, "Combat");

        Assert.That(result, Is.Not.Empty);
        Assert.That(result[0].Category, Is.EqualTo("Combat"));
    }

    [Test]
    public void Search_IsCaseInsensitive()
    {
        LogEntry[] result = Program.Search(_entries, "сердце эмберфанга");
        Assert.That(result, Has.Length.EqualTo(4));
    }

    [Test]
    public void CountByLevel_CountsFatalRecords()
    {
        Assert.That(Program.CountByLevel(_entries, "Fatal"), Is.EqualTo(1));
    }

    [Test]
    public void GetServerStatus_ReturnsCriticalWhenFatalServerEntryExists()
    {
        Assert.That(Program.GetServerStatus(_entries),
            Is.EqualTo("КРИТИЧЕСКАЯ ОШИБКА: сервер остановлен"));
    }
}
