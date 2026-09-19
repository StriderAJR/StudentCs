- [Создание проекта с NUnit-тестами](#создание-проекта-с-nunit-тестами)
  - [Создание проекта с тестами (Visual Studio)](#создание-проекта-с-тестами-visual-studio)
- [Добавление ссылки на основной проект](#добавление-ссылки-на-основной-проект)
  - [Создание проекта с тестами (VS Code)](#создание-проекта-с-тестами-vs-code)
    - [Создание NUnit-проекта](#создание-nunit-проекта)
    - [Добавление ссылки на MainProject](#добавление-ссылки-на-mainproject)
- [Если используется Solution-файл](#если-используется-solution-файл)
- [Полная последовательность команд](#полная-последовательность-команд)
- [Проверка](#проверка)
- [Подготовка методов к тестированию](#подготовка-методов-к-тестированию)
- [Создание первого теста](#создание-первого-теста)
  - [`[Test]`](#test)
- [Вызов тестируемого метода](#вызов-тестируемого-метода)
- [Проверка результата](#проверка-результата)
- [Проверка `bool`](#проверка-bool)
- [Несколько тестов одного метода](#несколько-тестов-одного-метода)
- [`[TestCase]`](#testcase)
- [Основные виды проверок](#основные-виды-проверок)
- [Запуск тестов](#запуск-тестов)
- [Тесты для лабораторной работы](#тесты-для-лабораторной-работы)


# Создание проекта с NUnit-тестами

Для лабораторной работы у вАас уже должен быть создан основной проект с программой. В дальнейшем будем считать, что он называется:

```text
MainProject
```

Теперь рядом с ним создадим отдельный проект, в котором будут находиться автоматические тесты.

> **Обратите внимание на названия проектов на скриншотах**
>
> В примерах и на скриншотах названия проектов могут выглядеть так:
>
> ```text
> UnitTestsDemo.MainProject
> UnitTestsDemo.ProgramTests
> ```
>
> Префикс `UnitTestsDemo` связан только с устройством большого хранилища кода, в котором подготовлен пример.
>
> В вашей лабораторной такого префикса **не будет**. Ваши проекты должны называться просто:
>
> ```text
> MainProject
> ProgramTests
> ```

## Создание проекта с тестами (Visual Studio)

В `Solution Explorer` нажмите правой кнопкой мыши на решение (`Solution`) и выберите:

```text
Add → New Project
```
![](./img/unit-tests/1.png)

В списке шаблонов найдите:

```text
NUnit Test Project
```

Выберите его и нажмите `Next`.

![](./img/unit-tests/2.png)

В качестве имени проекта укажите:

```text
ProgramTests
```

После создания проекта в решении должно быть два проекта:

```text
Solution
├── MainProject
└── ProgramTests
```

`MainProject` содержит вашу программу.

`ProgramTests` будет содержать тесты для методов из `MainProject`.

---

# Добавление ссылки на основной проект

Проект `ProgramTests` пока ничего не знает о коде, находящемся в `MainProject`.

Поэтому необходимо добавить ссылку на основной проект.

Нажмите правой кнопкой мыши на проект:

```text
ProgramTests
```

и выберите:

```text
Add → Project Reference
```

![](./img/unit-tests/3.png)

В открывшемся окне поставьте галочку напротив:

```text
MainProject
```

и нажмите `OK`.

![](./img/unit-tests/4.png)

После этого из проекта `ProgramTests` можно будет обращаться к публичным методам из `MainProject`.

## Создание проекта с тестами (VS Code)

В VS Code проекты обычно создаются через терминал с помощью команды `dotnet`.

> **Обратите внимание!**
> Структура solution должно быть такой:
> 
> есть директория для solution и в ней каждый проект лежит отдельно в своей папке. Т.е. структура такая:
> 
> ```text
> Solution
> ├── Lab02.sln
> ├── MainProject
> |   ├── Program.cs
> |   └── MainProject.csproj
> └── ProgramTests
>     ├── Program.cs
>     └── ProgramTests.csproj
> 
> Причем sln файл скорее всего у вас будет даже отсутствовать. Пока вы его вручную не создадите команды создания проекта dotnet new его не создадут. В целом, он и не особо нужен. Но если хотите, чтобы было, то команда для создания
> ```
> dotnet sln add Lab02
> dotnet sln add MainProject
> dotnet sln add ProgramTests
> ```
```

Предположим, что основной проект лабораторной уже существует и называется:

```text
MainProject
```

Нужно создать рядом с ним второй проект:

```text
ProgramTests
```

Структура должна получиться такой:

```text
Solution
├── MainProject
└── ProgramTests
```

### Создание NUnit-проекта

Откройте терминал в папке, где находится `MainProject`.

Например:

```text
Solution
└── MainProject
```

Находясь в папке `Solution`, выполните:

```bash
dotnet new nunit -n ProgramTests
```

После этого рядом с `MainProject` появится новый проект:

```text
Solution
├── MainProject
└── ProgramTests
```

Команда:

```bash
dotnet new nunit -n ProgramTests
```

означает:

* `dotnet new` — создать новый проект;
* `nunit` — использовать шаблон NUnit;
* `-n ProgramTests` — назвать проект `ProgramTests`.

---

### Добавление ссылки на MainProject

Сам по себе проект `ProgramTests` пока не имеет доступа к коду из `MainProject`.

Нужно добавить ссылку на основной проект.

Находясь в папке `Solution`, выполните:

```bash
dotnet add ProgramTests reference MainProject
```

После этого `ProgramTests` сможет использовать публичные методы из `MainProject`.

Фактически в файл:

```text
ProgramTests/ProgramTests.csproj
```

будет добавлена ссылка примерно такого вида:

```xml
<ItemGroup>
    <ProjectReference Include="..\MainProject\MainProject.csproj" />
</ItemGroup>
```

Редактировать `.csproj` вручную не требуется.

---

# Если используется Solution-файл

Если у вас есть файл решения, например:

```text
Solution.sln
```

новый проект желательно также добавить в него.

Команда:

```bash
dotnet sln add ProgramTests
```

Если основной проект ещё не был добавлен в solution, можно добавить и его:

```bash
dotnet sln add MainProject
```

После этого:

```bash
dotnet sln list
```

покажет оба проекта.

Например:

```text
MainProject/MainProject.csproj
ProgramTests/ProgramTests.csproj
```

---

# Полная последовательность команд

Если `MainProject` уже существует, обычно достаточно выполнить:

```bash
dotnet new nunit -n ProgramTests
dotnet add ProgramTests reference MainProject
dotnet sln add ProgramTests
```

Если `.sln`-файла ещё нет, его можно создать:

```bash
dotnet new sln
```

Затем добавить оба проекта:

```bash
dotnet sln add MainProject
dotnet sln add ProgramTests
```

И добавить ссылку:

```bash
dotnet add ProgramTests reference MainProject
```

---

# Проверка

После этого можно выполнить:

```bash
dotnet test
```

Команда найдёт проект с NUnit-тестами, соберёт оба проекта и запустит тесты.

Если всё настроено правильно, `ProgramTests` сможет обращаться к методам из `MainProject`:

```csharp
using MainProject;

namespace ProgramTests;

public class Tests
{
    [Test]
    public void Add_TwoNumbers_ReturnsSum()
    {
        var result = Program.Add(2, 3);

        Assert.That(result, Is.EqualTo(5));
    }
}
```

Итоговая структура:

```text
Solution
│
├── MainProject
│   ├── MainProject.csproj
│   └── Program.cs
│
├── ProgramTests
│   ├── ProgramTests.csproj
│   └── UnitTest1.cs
│
└── Solution.sln
```

> В примерах преподавателя проекты могут называться:
>
> ```text
> UnitTestsDemo.MainProject
> UnitTestsDemo.ProgramTests
> ```
>
> Префикс `UnitTestsDemo` относится только к структуре большого хранилища кода преподавателя.
>
> У вас проекты будут называться:
>
> ```text
> MainProject
> ProgramTests
> ```


---

# Подготовка методов к тестированию

Поскольку классы и объекты мы пока не изучали, методы, которые необходимо тестировать, будем делать:

```csharp
public static
```

Например, в `Program.cs` проекта `MainProject` может находиться такой метод:

```csharp
namespace MainProject;

public class Program
{
    public static int Add(int a, int b)
    {
        return a + b;
    }

    public static bool IsEven(int number)
    {
        return number % 2 == 0;
    }

    public static void Main()
    {
    }
}
```

Здесь:

```csharp
public
```

позволяет обращаться к методу из другого проекта, а:

```csharp
static
```

позволяет вызвать метод непосредственно через `Program`, без создания объекта.

Например:

```csharp
Program.Add(2, 3);
```

---

# Создание первого теста

После создания NUnit-проекта в нём уже будет создан файл с тестами. Обычно он называется примерно так:

```text
UnitTest1.cs
```

Его можно переименовать, например, в:

```text
ProgramTests.cs
```

Содержимое файла можно заменить на следующее:

```csharp
using MainProject;

namespace ProgramTests;

public class Tests
{
    [Test]
    public void Add_TwoNumbers_ReturnsSum()
    {
        var result = Program.Add(2, 3);

        Assert.That(result, Is.EqualTo(5));
    }
}
```

Разберём этот тест.

## `[Test]`

Перед методом находится:

```csharp
[Test]
```

Этот атрибут сообщает NUnit:

> Этот метод является тестом, его необходимо запустить при запуске тестирования.

Сам метод:

```csharp
public void Add_TwoNumbers_ReturnsSum()
```

является обычным методом.

---

# Вызов тестируемого метода

Внутри теста сначала вызывается метод из основной программы:

```csharp
var result = Program.Add(2, 3);
```

Метод должен вернуть:

```text
5
```

Полученный результат сохраняется в переменную `result`.

---

# Проверка результата

После этого необходимо проверить, действительно ли программа получила ожидаемый результат:

```csharp
Assert.That(result, Is.EqualTo(5));
```

Эта запись читается примерно так:

> Проверить, что `result` равен `5`.

Общий вид проверки:

```csharp
Assert.That(фактический_результат, условие);
```

В данном случае:

```csharp
result
```

— фактический результат работы программы,

а:

```csharp
Is.EqualTo(5)
```

— условие, которому этот результат должен соответствовать.

Если `result` действительно равен `5`, тест будет успешно пройден.

Если результат отличается, тест завершится ошибкой.

---

# Проверка `bool`

Например, у нас есть метод:

```csharp
public static bool IsEven(int number)
{
    return number % 2 == 0;
}
```

Можно проверить, что для чётного числа он возвращает `true`:

```csharp
[Test]
public void IsEven_EvenNumber_ReturnsTrue()
{
    var result = Program.IsEven(10);

    Assert.That(result, Is.True);
}
```

И отдельно проверить нечётное число:

```csharp
[Test]
public void IsEven_OddNumber_ReturnsFalse()
{
    var result = Program.IsEven(11);

    Assert.That(result, Is.False);
}
```

---

# Несколько тестов одного метода

Одного теста обычно недостаточно.

Например, для метода:

```csharp
public static int Add(int a, int b)
{
    return a + b;
}
```

можно проверить несколько случаев:

```csharp
[Test]
public void Add_PositiveNumbers_ReturnsSum()
{
    var result = Program.Add(2, 3);

    Assert.That(result, Is.EqualTo(5));
}

[Test]
public void Add_NegativeNumbers_ReturnsSum()
{
    var result = Program.Add(-2, -3);

    Assert.That(result, Is.EqualTo(-5));
}

[Test]
public void Add_PositiveAndNegativeNumbers_ReturnsSum()
{
    var result = Program.Add(10, -3);

    Assert.That(result, Is.EqualTo(7));
}
```

Таким образом мы проверяем работу одного метода в разных ситуациях.

---

# `[TestCase]`

Если необходимо несколько раз проверить один и тот же метод с разными значениями, вместо нескольких почти одинаковых тестов можно использовать `[TestCase]`.

Например:

```csharp
[TestCase(2, 3, 5)]
[TestCase(10, 20, 30)]
[TestCase(-2, -3, -5)]
[TestCase(10, -3, 7)]
public void Add_TwoNumbers_ReturnsSum(int a, int b, int expected)
{
    var result = Program.Add(a, b);

    Assert.That(result, Is.EqualTo(expected));
}
```

Каждая строка:

```csharp
[TestCase(...)]
```

описывает отдельный запуск теста.

Например:

```csharp
[TestCase(2, 3, 5)]
```

передаст в метод:

```text
a = 2
b = 3
expected = 5
```

После чего выполнится:

```csharp
var result = Program.Add(a, b);

Assert.That(result, Is.EqualTo(expected));
```

Затем NUnit запустит тот же тест со следующим набором значений.

---

# Основные виды проверок

Для сравнения результата с конкретным значением:

```csharp
Assert.That(result, Is.EqualTo(5));
```

Проверка, что значения **не равны**:

```csharp
Assert.That(result, Is.Not.EqualTo(5));
```

Проверка значения `bool`:

```csharp
Assert.That(result, Is.True);
```

```csharp
Assert.That(result, Is.False);
```

Проверка, что число больше некоторого значения:

```csharp
Assert.That(result, Is.GreaterThan(0));
```

Меньше:

```csharp
Assert.That(result, Is.LessThan(10));
```

Больше или равно:

```csharp
Assert.That(result, Is.GreaterThanOrEqualTo(0));
```

Меньше или равно:

```csharp
Assert.That(result, Is.LessThanOrEqualTo(100));
```

---

# Запуск тестов

Откройте окно:

```text
Test → Test Explorer
```

В `Test Explorer` будут отображаться найденные NUnit-тесты.

Для запуска всех тестов нажмите:

```text
Run All
```

Если тест прошёл успешно, он будет отмечен как успешно выполненный.

Если тест не прошёл, NUnit покажет:

* какой тест завершился ошибкой;
* какой результат ожидался;
* какой результат был получен на самом деле.

Например, если написать:

```csharp
var result = Program.Add(2, 3);

Assert.That(result, Is.EqualTo(6));
```

тест завершится ошибкой, потому что программа вернула `5`, а тест ожидал `6`.

Сообщение будет содержать информацию примерно следующего вида:

```text
Expected: 6
But was:  5
```

Это позволяет быстро понять, какой тест не прошёл и чем фактический результат отличается от ожидаемого.

---

# Тесты для лабораторной работы

Тесты для методов из лабораторной необходимо размещать в проекте:

```text
ProgramTests
```

Сама реализация методов остаётся в:

```text
MainProject
```

Таким образом структура решения будет выглядеть примерно так:

```text
Solution
│
├── MainProject
│   └── Program.cs
│
└── ProgramTests
    └── ProgramTests.cs
```

В `Program.cs` находятся методы лабораторной:

```csharp
public static int SomeMethod(int value)
{
    // Implementation
}
```

А в `ProgramTests.cs` находятся их тесты:

```csharp
[Test]
public void SomeMethod_SomeInput_ReturnsExpectedResult()
{
    var result = Program.SomeMethod(10);

    Assert.That(result, Is.EqualTo(20));
}
```

Основная идея unit-тестирования:

```text
Задать входные данные
        ↓
Вызвать тестируемый метод
        ↓
Получить фактический результат
        ↓
Сравнить его с ожидаемым результатом
```

В NUnit последняя часть обычно записывается через:

```csharp
Assert.That(actual, constraint);
```
