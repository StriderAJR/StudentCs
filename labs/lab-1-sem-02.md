# Лабораторная работа 2. Проверка конфигурации сервера

## Имя

sem-1-lab-02-server-config

## Сложность

1 / 5

## Темы

Операторы ветвления, логический тип данных, операторы, методы, строка, рефакторинг и улучшение кода.

## Задание

Перед запуском игрового сервера автоматически выполняется проверка его конфигурации.

Некоторые параметры могут конфликтовать друг с другом. В отдельных случаях сервер можно запускать только после предупреждения администратора, а некоторые конфигурации делают запуск невозможным.

Разработайте программу, которая получает сведения о конфигурации сервера, анализирует их и определяет один из возможных результатов:

- сервер готов к запуску;
- сервер можно запустить, но необходимо предупредить администратора;
- запуск невозможен.

Какие именно параметры используются при проверке и какие правила между ними существуют — определите самостоятельно. Постарайтесь сделать проверку максимально правдоподобной.

Все сообщения программы должны быть понятны человеку, который отвечает за работу сервера.

<details>
<summary>Unit тесты для самопроверки</summary>

```cs
using NUnit.Framework;

public class ProgramTests
{
    [Test]
    public void CheckConfiguration_ValidConfiguration_ServerIsReady()
    {
        var result = Program.CheckConfiguration(50, 8, true, false);

        Assert.That(result, Is.EqualTo("Сервер готов к запуску."));
    }

    [Test]
    public void CheckConfiguration_ZeroPlayers_LaunchIsImpossible()
    {
        var result = Program.CheckConfiguration(0, 8, true, false);

        Assert.That(
            result,
            Is.EqualTo("Запуск невозможен: количество игроков должно быть больше нуля."));
    }

    [Test]
    public void CheckConfiguration_NotEnoughMemory_LaunchIsImpossible()
    {
        var result = Program.CheckConfiguration(50, 1, true, false);

        Assert.That(
            result,
            Is.EqualTo("Запуск невозможен: серверу недостаточно оперативной памяти."));
    }

    [Test]
    public void CheckConfiguration_PublicServerWithPassword_LaunchWithWarning()
    {
        var result = Program.CheckConfiguration(50, 8, true, true);

        Assert.That(
            result,
            Is.EqualTo("Запуск возможен с предупреждением: публичный сервер защищён паролем."));
    }

    [Test]
    public void CheckConfiguration_TooManyPlayersForAvailableMemory_LaunchWithWarning()
    {
        var result = Program.CheckConfiguration(150, 4, true, false);

        Assert.That(
            result,
            Is.EqualTo(
                "Запуск возможен с предупреждением: для такого количества игроков рекомендуется больше оперативной памяти."));
    }

    [Test]
    public void CheckConfiguration_InvalidPlayersAndPublicServerWithPassword_LaunchIsImpossible()
    {
        var result = Program.CheckConfiguration(0, 8, true, true);

        Assert.That(
            result,
            Is.EqualTo("Запуск невозможен: количество игроков должно быть больше нуля."));
    }
}
```

</details>

## Подсказки

<details>
<summary>Показать подсказки</summary>

- Перед написанием программы полезно выписать все возможные ситуации на бумаге.
- Некоторые проверки могут зависеть сразу от нескольких параметров.
- Если в разных местах программы начинают повторяться одинаковые действия, подумайте, можно ли сделать решение более понятным.
- Хорошее сообщение не просто сообщает об ошибке, а помогает понять её причину.

</details>

## Дополнительное задание

Добавьте возможность выводить сразу все найденные проблемы конфигурации, а не только первую обнаруженную.
