using NUnit.Framework;

public class ProgramTests
{
    [Test]
    public void CheckConfiguration_ValidConfiguration_ServerIsReady()
    {
        Assert.That(Program.CheckConfiguration(50, 8, true, false),
            Is.EqualTo("Сервер готов к запуску."));
    }

    [Test]
    public void CheckConfiguration_ZeroPlayers_LaunchIsImpossible()
    {
        Assert.That(Program.CheckConfiguration(0, 8, true, false),
            Is.EqualTo("Запуск невозможен: количество игроков должно быть больше нуля."));
    }

    [Test]
    public void CheckConfiguration_NotEnoughMemory_LaunchIsImpossible()
    {
        Assert.That(Program.CheckConfiguration(50, 1, true, false),
            Is.EqualTo("Запуск невозможен: серверу недостаточно оперативной памяти."));
    }

    [Test]
    public void CheckConfiguration_PublicServerWithPassword_LaunchWithWarning()
    {
        Assert.That(Program.CheckConfiguration(50, 8, true, true),
            Is.EqualTo("Запуск возможен с предупреждением: публичный сервер защищён паролем."));
    }

    [Test]
    public void CheckConfiguration_TooManyPlayersForAvailableMemory_LaunchWithWarning()
    {
        Assert.That(Program.CheckConfiguration(150, 4, true, false), Is.EqualTo(
            "Запуск возможен с предупреждением: для такого количества игроков рекомендуется больше оперативной памяти."));
    }

    [Test]
    public void CheckConfiguration_InvalidPlayers_HasPriorityOverWarning()
    {
        Assert.That(Program.CheckConfiguration(0, 8, true, true),
            Is.EqualTo("Запуск невозможен: количество игроков должно быть больше нуля."));
    }
}
