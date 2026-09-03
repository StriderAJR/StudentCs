# Лекция 5: циклы и управление повторением

## Основные циклы

`for` подходит, когда есть счётчик:

``` csharp
for (int i = 0; i < 5; i++)
{
    Console.WriteLine($"Проверка {i + 1}");
}
```

`foreach` проходит элементы коллекции:

``` csharp
string[] events = { "Login", "Warning", "Logout" };

foreach (string eventName in events)
{
    Console.WriteLine(eventName);
}
```

`while` повторяет тело, пока условие истинно:

``` csharp
int attempts = 0;

while (attempts < 3)
{
    Console.WriteLine("Попытка подключения");
    attempts++;
}
```

Если управляющее значение не меняется, цикл становится бесконечным.

## `do while`

Иногда тело нужно выполнить хотя бы один раз — например, показать меню до первой проверки выбора пользователя.

``` csharp
int choice;

do
{
    Console.WriteLine("1 — статус сервера");
    Console.WriteLine("0 — выход");
    choice = int.Parse(Console.ReadLine());
}
while (choice != 0);
```

У `while` условие проверяется до тела, у `do while` — после тела. Поэтому `do while` гарантирует одну итерацию.

## Вложенные циклы

Цикл может находиться внутри другого цикла. В этом случае внутреннее тело выполняется для каждого элемента внешнего цикла.

``` csharp
for (int row = 0; row < 2; row++)
{
    for (int column = 0; column < 3; column++)
    {
        Console.Write($"[{row},{column}] ");
    }

    Console.WriteLine();
}
```

Два цикла по `n` элементов дают примерно `n * n` повторений. На маленькой карте это нормально, но рост количества операций нужно учитывать.

## `break` и `continue`

`break` завершает цикл, `continue` пропускает текущую итерацию.

``` csharp
string[] events = { "Info", "Ignore", "Critical", "Info" };

foreach (string eventName in events)
{
    if (eventName == "Ignore")
    {
        continue;
    }

    Console.WriteLine(eventName);

    if (eventName == "Critical")
    {
        break;
    }
}
```

## Цикл внутри метода

Цикл лучше выделить в метод, если он выполняет самостоятельную задачу. Так главная программа описывает сценарий, а детали обхода остаются внутри метода.

``` csharp
static int CountErrors(string[] lines)
{
    int errorCount = 0;

    foreach (string line in lines)
    {
        if (line.StartsWith("ERROR"))
        {
            errorCount++;
        }
    }

    return errorCount;
}
```

Метод возвращает число ошибок, но не печатает их. Разделение вычисления и вывода упрощает повторное использование и проверку кода.

Если внутри цикла появляется большой повторяющийся фрагмент, его можно вынести в отдельный метод. Это один из самых полезных вариантов рефакторинга.

## Цикл и память

Переменная `i` — отдельное значение в кадре метода. Массив `events` — объект в куче, а переменная массива содержит ссылку на него.

![Массив и переменная цикла в памяти](../img/lectures/07/array-memory.png)

## Итог

`for` работает со счётчиком, `foreach` — с элементами, `while` — с условием. Следите за границами, изменением счётчика и тем, где находятся данные, по которым вы проходите.
