- [Стиль и структура кода](#стиль-и-структура-кода)
  - [Скобочные и бесскобочные блоки кода](#скобочные-и-бесскобочные-блоки-кода)
  - [Именование переменных](#именование-переменных)
    - [Говорящие названия](#говорящие-названия)
    - [Однобуквенные имена](#однобуквенные-имена)
    - [Аккуратность с именами переменных](#аккуратность-с-именами-переменных)
- [Логические конструкции и условия](#логические-конструкции-и-условия)
  - [Неявное возвращение true/false](#неявное-возвращение-truefalse)
  - [Ветвления и return](#ветвления-и-return)
- [Работа с кодом и методами](#работа-с-кодом-и-методами)
  - [Не создавать "висящие" блоки кода](#не-создавать-висящие-блоки-кода)
  - [Не использовать вложенные методы](#не-использовать-вложенные-методы)
  - [Разбиение программы на методы](#разбиение-программы-на-методы)
- [Циклы](#циклы)
- [Эффективность кода](#эффективность-кода)
  - [Вычисления и повторяемость](#вычисления-и-повторяемость)
- [Работа с файлами](#работа-с-файлами)
- [Калькулятор с проверкой ввода](#калькулятор-с-проверкой-ввода)

# Стиль и структура кода

## Скобочные и бесскобочные блоки кода

Не надо смешивать скобочные и бесскобочные блоки кода. Или все делаем со скобками или все без скобок.
```cs
if (isActive)
{
    if (isReady)
        Process();
    else
    {
        Stop();
    }
}

```

Вложенный if внутри else всегда идет или с табуляцией (если это беcскобочная запись) или со скобками + табуляцией. Или если хочется сэкономить место (любите однострочные записи), то else + if идут на одной строчке. Лучше всегда пользоваться или 2, или 3 вариантом.

```csharp
// вариант 1
if(...) some_operation
else
  if(...) another operation
  else last_operation
```

```csharp
// вариант 2
if(...) 
{
  some_operation
}
else
{
  if(...) another operation
  else last_operation
}
```

```csharp
// вариант 3
if(...) some_operation
else if(...) another operation
else last_operation
```

А вот так не надо
```cs
if (condition1)
{
    FirstOperation();
}
else
if (condition2)
{
    SecondOperation();
}
else
{
    LastOperation();
}
```

## Именование переменных

### Говорящие названия

Нужно использовать говорящие названия переменных

```csharp
int num, count, maxValue, result;
string str, buffer, input, output, fileContent;
char symbol;
int[] array;
int[][] matrix;

int studentCount;
double percent;
string fullName, name, surname;
int[] coordinates;
int arraySize;
double currentDeposit;
int vectorLength;
double squareArea, circleArea;
int[] studentGrades;
```

### Однобуквенные имена

Использование однобуквенных имен переменных только для общепринятых вещей

```csharp
int i, j, k; // счетчики цикла
char c; // просто символ, например, при посимвольном просмотре строки
int n, m; // размеры массивов и матриц
```

### Аккуратность с именами переменных

Будьте аккуратны с именами переменных. Плохие примеры:

```csharp
string number; // в коде будет казаться, что это число, но на деле это строка
int c; // с - зарезервированное имя для символа
int[] digit; // здесь не одна цифра, а массив чисел
```

Для этих примеров хорошие варианты именования:

```csharp
string numberStr, strNumber, numberAsStr; // это дает понять, что в строке записано число, но это все-таки строка
int number, num, count, length; // названия именно для чисел
int[] digits, numbers; // если это массив, то название обязательно во множественном числе
```

## Не создавать лишние переменные

Не нужно создавать промежуточные переменные, которые не добавляют коду читаемости и используются только один раз.

Плохо:

```csharp
var number = 10;
var multiplier = 2;
var result = number * multiplier;
var output = result.ToString();

Console.WriteLine(output);
```

Если промежуточные значения не имеют самостоятельного смысла, код можно упростить.

```csharp
var number = 10;

Console.WriteLine(number * 2);
```

Однако не нужно стремиться избавиться от всех промежуточных переменных.

Если выражение сложное, именованная переменная может значительно улучшить читаемость.

Например:

```csharp
var isEligible = age >= 18 && hasPassport && !isBlocked;

if (isEligible)
{
    Register();
}
```

Главный критерий — переменная должна делать код понятнее, а не просто увеличивать количество строк.

## Не использовать `double` для денежных расчётов

Типы `float` и `double` используют двоичное представление чисел с плавающей точкой.

Из-за этого некоторые десятичные дроби невозможно представить точно.

Плохо:

```csharp
var price = 0.1;
var tax = 0.2;

Console.WriteLine(price + tax == 0.3);
```

Результат будет `False`.

Для денежных расчётов обычно лучше использовать `decimal`.

```csharp
var price = 0.1m;
var tax = 0.2m;

Console.WriteLine(price + tax == 0.3m);
```

Результат будет `True`.

Суффикс `m` указывает, что числовой литерал имеет тип `decimal`.

При этом `decimal` не обеспечивает абсолютную точность любых математических операций. Например, деление также может потребовать округления.

## Не оставлять закомментированный код

Не нужно оставлять в программе старые версии алгоритмов в виде комментариев.

Плохо:

```csharp
var result = CalculateSum(numbers);

// var result = 0;
// foreach (var number in numbers)
// {
//     result += number;
// }

Console.WriteLine(result);
```

Закомментированный код затрудняет чтение программы. Со временем становится непонятно, зачем он оставлен и актуален ли вообще.

Если старая реализация больше не нужна, удалите её.

Для хранения истории изменений существуют системы контроля версий, например Git.

Комментарии должны объяснять неочевидные решения, а не хранить старые версии программы.

# Логические конструкции и условия

## Неявное возвращение true/false

Не нужно возвращать явно значение true или false в случаях, когда идет проверка условия.

```csharp
private bool CorrectMonth(int month)
{
    return (month >= 1 && month <= 12)
        ? true
        : false;
}
```

Правильнее писать сразу возврат результата условия.

```csharp
private bool CorrectMonth(int month)
{
    return month >= 1 && month <= 12;
}
```

## Не дублировать одинаковые проверки

Не нужно повторять одну и ту же проверку, если её результат уже известен и данные с момента проверки не изменились.

Плохо:

```csharp
if (numbers.Length > 0)
{
    Console.WriteLine(numbers[0]);
}

if (numbers.Length > 0)
{
    Console.WriteLine(numbers[numbers.Length - 1]);
}
```

Лучше:

```csharp
if (numbers.Length > 0)
{
    Console.WriteLine(numbers[0]);
    Console.WriteLine(numbers[numbers.Length - 1]);
}
```

Повторные проверки усложняют код и создают лишние точки, в которых при изменении программы можно допустить ошибку.

## Ветвления и return

В ветвлениях, если в одной из веток идет return, то в else нет смысла.

Неправильно:

```csharp
if (count % 10 == 0 || count % 10 >= 5 || (count % 100 >= 10 && count % 100 <= 19)) return "рублей";
else if (count % 10 == 1) return "рубль";
else return "рубля";
```

Как нужно:

```csharp
if (count % 10 == 0 || count % 10 >= 5 || (count % 100 >= 10 && count % 100 <= 19)) return "рублей";
if (count % 10 == 1) return "рубль";
return "рубля";
```

Не нужно в явном виде возвращать true и false, если есть какая-то проверка.

```csharp
if (condition)
{
    return false;
}
return true;
```

Можно сразу возвращать результат проверки.

```csharp
return condition;
return !condition;
```

## Избегать избыточной вложенности
Плохо:
```cs
public void ProcessOrder(Order order)
{
    if (order != null)
    {
        if (order.Items.Count > 0)
        {
            if (order.IsPaid)
            {
                ShipOrder(order);
            }
        }
    }
}
```

Лучше:
```cs
public void ProcessOrder(Order order)
{
    if (order == null)
    {
        return;
    }

    if (order.Items.Count == 0)
    {
        return;
    }

    if (!order.IsPaid)
    {
        return;
    }

    ShipOrder(order);
}
```

## Не использовать исключения для обработки обычных ситуаций

Исключения предназначены для обработки исключительных ситуаций.

Например, пользователь вполне может ввести текст вместо числа. Это ожидаемая ситуация, для которой существуют специальные методы проверки.

Плохо:

```csharp
try
{
    var number = int.Parse(Console.ReadLine());
    Console.WriteLine(number);
}
catch (FormatException)
{
    Console.WriteLine("Некорректное число");
}
```

Лучше:

```csharp
if (int.TryParse(Console.ReadLine(), out var number))
{
    Console.WriteLine(number);
}
else
{
    Console.WriteLine("Некорректное число");
}
```

Метод `TryParse` позволяет проверить возможность преобразования без генерации исключения.

Не нужно использовать `try-catch` там, где можно выполнить обычную проверку условия.

# Магические числа и константы

## Не использовать магические числа

Плохо:
```cs
if (temperature > 38)
{
    Console.WriteLine("Высокая температура");
}

if (previousTemperature > 38)
{
    Console.WriteLine("Температура была высокой");
}
```

Лучше:
```cs
const int HighTemperatureThreshold = 38;

if (temperature > HighTemperatureThreshold)
{
    Console.WriteLine("Высокая температура");
}

if (previousTemperature > HighTemperatureThreshold)
{
    Console.WriteLine("Температура была высокой");
}
```
Константа объясняет смысл значения и позволяет менять его в одном месте.

Но не нужно превращать в константы буквально каждую единицу или ноль.

# Работа с кодом и методами

## Не создавать "висящие" блоки кода

Не делайте висящие в воздухе блоки кода. Почти наверняка вы недоразбили метод на более мелкие части из-за чего вам понадобились переменные с одним именем

```cs
public void ProcessOrders()
{
    {
        var result = CalculateTotal();
        SaveResult(result);
    }

    {
        var result = CalculateDiscount();
        SaveResult(result);
    }
}
```
Здесь блоки `{ }` используются исключительно для ограничения области видимости переменной result, чтобы повторно объявить её с тем же именем.

Правильно — выделить независимые операции в отдельные методы:
```cs
public void ProcessOrders()
{
    ProcessTotal();
    ProcessDiscount();
}

private void ProcessTotal()
{
    var result = CalculateTotal();
    SaveResult(result);
}

private void ProcessDiscount()
{
    var result = CalculateDiscount();
    SaveResult(result);
}
```

## Метод должен выполнять одну логическую задачу

Разбиение программы на методы необходимо не только для устранения повторяющегося кода.

Каждый метод должен выполнять одну логически завершённую задачу.

Плохо:

```csharp
public static void ProcessStudent()
{
    Console.WriteLine("Введите имя студента:");
    var name = Console.ReadLine();

    name = name.Trim();

    File.AppendAllText("students.txt", name + Environment.NewLine);

    Console.WriteLine("Студент сохранён");
}
```

В одном методе смешаны ввод данных, их обработка, сохранение в файл и вывод результата.

Лучше:

```csharp
public static void ProcessStudent()
{
    var name = ReadStudentName();
    SaveStudent(name);

    Console.WriteLine("Студент сохранён");
}

private static string ReadStudentName()
{
    Console.WriteLine("Введите имя студента:");
    return Console.ReadLine().Trim();
}

private static void SaveStudent(string name)
{
    File.AppendAllText("students.txt", name + Environment.NewLine);
}
```

Теперь каждый метод имеет конкретное назначение.

При этом не нужно доводить декомпозицию до абсурда и создавать отдельный метод для каждой строки кода.

Главный критерий — улучшение читаемости и упрощение сопровождения программы.

## Не использовать глобальное состояние для передачи данных между методами

Не нужно создавать поля класса только ради того, чтобы несколько методов могли использовать одни и те же переменные.

Плохо:

```csharp
static int number;
static int result;

static void Calculate()
{
    result = number * number;
}

static void Main()
{
    number = int.Parse(Console.ReadLine());

    Calculate();

    Console.WriteLine(result);
}
```

Здесь методы зависят от общего изменяемого состояния.

Лучше передавать необходимые данные через параметры и возвращать результат работы метода.

```csharp
static int Calculate(int number)
{
    return number * number;
}

static void Main()
{
    var number = int.Parse(Console.ReadLine());
    var result = Calculate(number);

    Console.WriteLine(result);
}
```

Такой метод проще читать, тестировать и повторно использовать.

Поля класса сами по себе не являются плохой практикой. Они нужны для хранения состояния объекта или класса, но не должны заменять параметры и возвращаемые значения без необходимости.

## Не изменять входные данные неожиданным образом

Метод не должен неожиданно изменять переданные ему данные.

Особенно внимательно относитесь к массивам и другим ссылочным типам.

Плохо:

```csharp
public static int GetMax(int[] numbers)
{
    Array.Sort(numbers);
    return numbers[numbers.Length - 1];
}
```

Метод называется `GetMax`, но помимо поиска максимального значения он сортирует исходный массив.

Это побочный эффект, о котором вызывающий код может не знать.

Лучше:

```csharp
public static int GetMax(int[] numbers)
{
    var max = numbers[0];

    foreach (var number in numbers)
    {
        if (number > max)
        {
            max = number;
        }
    }

    return max;
}
```

Теперь метод выполняет только заявленную задачу и не изменяет исходный массив.

В обоих вариантах предполагается, что массив не пустой.

Если метод должен изменять переданные данные, это должно быть очевидно из его назначения.

## Не использовать вложенные методы
Да, иногда их действительно используют, но для этого нужна веская и объяснимая причина. На время обучения вам запрещается их использовать.

```csharp
public static void Main()
{
    int Sum(int num)
    {
        int sum = 0;
        while (num > 0)
        {
            sum += num % 10;
            num /= 10;             
        }
        return sum;
    }
    int num = int.Parse(Console.ReadLine());
    Console.WriteLine(Sum(num))
}
```

Нужно было так:

```csharp
int Sum(int num)
{
    int sum = 0;
    while (num > 0)
    {
        sum += num % 10;
        num /= 10;             
    }
    return sum;
}

public static void Main()
{
    int num = int.Parse(Console.ReadLine());
    Console.WriteLine(Sum(num))
}
```

## Разбиение программы на методы

Методы используются не только для уменьшения повторяемости кода, но и для разбиения программы на логические куски.

```csharp
public static void Main()
{
    string numberStr = Console.ReadLine();    
    int x = Convert.ToInt32(numberStr);

    if (x >= 0)
    {
        int countDigits = str.Length;
        int sum = 0;
        while (countDigits != 0)
        {
            sum += Convert.ToInt32(str.Substring(0, 1));
            countDigits -= 1;
            str = str.Substring(1, countDigits);
        }

        if (sum % 2 == 0)
        {
            Console.WriteLine("чётное");
        }
        if (x % 2 != 0)
        {
            string reverString = "";
            while (str != "")
            {
                reverString += str.Substring(str.Length - 1, 1);
                str = str.Substring(0, str.Length - 1);
            }

            Console.WriteLine(reverString);
        }
    }
    else
    {
        return;
    }
}
```

Лучше (более читаемо и удобно в обслуживании) вот так:

```csharp
static int SumOfDigits(string str)
{
    int countDigits = str.Length;
    int sum = 0;
    while (countDigits != 0)
    {
        sum += Convert.ToInt32(str.Substring(0, 1));
        countDigits -= 1;
        str = str.Substring(1, countDigits);
    }
    return sum;
}

static string ReverseString(string str)
{
    string reverString = "";
    while (str != "")
    {
        reverString += str.Substring(str.Length - 1, 1);
        str = str.Substring(0, str.Length - 1);
    }
    return reverString;
}

public static void Main()
{
    string numberStr = Console.ReadLine();    
    int x = Convert.ToInt32(numberStr);

    if (x >= 0)
    {
        if (SumOfDigits(numberStr) % 2 == 0)
        {
            Console.WriteLine("чётное");
        }
        if (x % 2 != 0)
        {
            Console.WriteLine(ReverseString(numberStr));
        }
    }
    else
    {
        return;
    }
}
```

# Циклы

## Выбирать тип цикла под конкретную задачу

Выбирайте правильно тип цикла. Бесконечные циклы лучше делать через while, а если известно кол-во итераций, то for

```csharp
for (; ;) {} // плохой выбор
while(true){} // хороший выбор
```

Создавайте переменные как можно ближе к их использованию

```csharp
int a = Convert.ToInt32(Console.ReadLine());
int fac = 1;
if (a > 0)
{
    for (int i = 1; i < a + 1; i++)
    {
        fac = fac * i;
    }
    Console.WriteLine(fac);
}
```

нужно вот так:

```csharp
int a = Convert.ToInt32(Console.ReadLine());
if (a > 0)
{
    int fac = 1;
    for (int i = 1; i < a + 1; i++)
    {
        fac = fac * i;
    }
    Console.WriteLine(fac);
}
```

## Не создавать бесконечные циклы без необходимости

Не нужно создавать бесконечный цикл с `break`, если условие его завершения известно заранее.

Плохо:

```csharp
var i = 0;

while (true)
{
    if (i >= numbers.Length)
    {
        break;
    }

    Console.WriteLine(numbers[i]);
    i++;
}
```

Лучше:

```csharp
for (var i = 0; i < numbers.Length; i++)
{
    Console.WriteLine(numbers[i]);
}
```

Бесконечные циклы оправданны, когда момент завершения определяется непосредственно в процессе работы, например при ожидании корректного пользовательского ввода.

## Не создавать строки многократной конкатенацией в больших циклах

Строки в C# неизменяемы. При конкатенации создаётся новая строка.

Если многократно добавлять данные к строке в цикле, это может приводить к большому количеству лишних выделений памяти.

Неэффективно:

```csharp
var result = "";

for (var i = 0; i < 10000; i++)
{
    result += i + ", ";
}
```

Для многократного построения строк лучше использовать `StringBuilder`.

```csharp
var result = new StringBuilder();

for (var i = 0; i < 10000; i++)
{
    result.Append(i);
    result.Append(", ");
}

Console.WriteLine(result.ToString());
```

Для небольшого количества объединений обычная конкатенация вполне допустима.

Не нужно заменять на `StringBuilder` любое использование оператора `+` со строками.

# Эффективность кода

## Вычисления и повторяемость

Неэффективный код. Если число нужно возвести в квадрат или куб, лучше сделать это с помощью умножения, не используя более общий, но менее быстрый Math.Pow.

```csharp
return 0.5 * Math.Asin(distance * g / Math.Pow(v,2));
```

Если есть одно и то же повторяющееся вычисление, лучше выделять его в отдельную переменную

```csharp
int lastTwoDigits = count % 100;
int lastDigit = count % 10;
if (lastTwoDigits >= 5 && lastTwoDigits <= 20) return "рублей";
if (lastDigit >= 2 && lastDigit <= 4) return "рубля";
if (lastDigit == 1) return "рубль";
else return "рублей";
```

Повторяемость кода нужно избегать

```csharp
double ak = Math.Sqrt((x - ax) * (x - ax) + (y - ay) * (y - ay));
double kb = Math.Sqrt((x - bx) * (x - bx) + (y - by) * (y - by));
double ab = Math.Sqrt((ax - bx) * (ax - bx) + (ay - by) * (ay - by));
```

достаточно создать метод

```csharp
double ak = GetDistanceBetweenPoints(ax, ay, x, y);
double kb = GetDistanceBetweenPoints(bx, by, x, y);
double ab = GetDistanceBetweenPoints(bx, by, ax, ay);

double GetDistanceBetweenPoints(double x1, double y1, double x2, double y2)
{
    return Math.Sqrt((x2 - x1)*(x2 - x1) + (y2 - y1)*(y2 - y1));
}
```

# Работа с файлами

Никогда не используйте абсолютные пути к файлам.

```csharp
string[] lines = File.ReadAllLines(@"C:\VasyaPupkin\my_cool_program\bin\Debug\net8.0\example.txt");
```

Используйте относительные пути:

```csharp
string[] lines = File.ReadAllLines(@".\example.txt");
string[] lines = File.ReadAllLines(@"..\..\..\..\example.txt");
```

Пример исправленного кода с Main и методами для работы с балансом:

```csharp
public static void Main(string[] args)
{
    string[] lines = File.ReadAllLines("./bank-balance.txt"); 
    var initialBalance = int.Parse(lines[0]); 
    
    int currentBalance = initialBalance;
    Stack<int> balanceHistory = new Stack<int>(new List<int>{ 0 }); 
    for (int i = 1; i < lines.Length; i++)
    {
        (string command, int operationSum) = ParseLine(lines[i]);
        currentBalance = ApplyOperation(command, currentBalance, operationSum, balanceHistory);

        if (currentBalance < 0)
        {
            Console.WriteLine("Ошибка. Недостаточно средств");
            break; 
        }
    }
    Console.WriteLine(currentBalance);
}

public static (string, int) ParseLine(string line)
{
    string[] parts = line.Split('|', StringSplitOptions.TrimEntries); 
    string datetime = parts[0];

    string command;
    int operationSum = 0;
    if (parts.Length < 3)
    {
        command = parts[1];
    }
    else
    {
        operationSum = int.Parse(parts[1]);
        command = parts[2];
    }

    return (command, operationSum);
}

public static int ApplyOperation(string command, int currentBalance, int operationSum, Stack<int> balanceHistory)
{
    if (command == "revert")
        return balanceHistory.Pop();

    balanceHistory.Push(currentBalance);
    if (command == "in")
        currentBalance = currentBalance + operationSum;
    else if (command == "out")
        currentBalance = currentBalance - operationSum;

    return currentBalance;
}
```

# Калькулятор с проверкой ввода

```csharp
public static void Main(string[] args)
{
    int num1 = ReadNumber().Value;
    int? num2 = ReadNumber(true);
    string operation = ReadOperation();

    double result = 0;
    switch (operation)
    {
        case "+": result = num1 + num2.Value; break;
        case "-": result = num1 - num2.Value; break;
        case "*": result = num1 * num2.Value; break;
        case "/": result = num1 / num2.Value; break;
        case "!": result = Factorial(num1); break;
        case "log": result = Math.Log(num1, num2.Value); break;
        case "dsum": result = GetDigitsSum(num1); break;
    }

    Console.WriteLine(result);
}

public static int? ReadNumber(bool allowEmpty = false)
{
    string buf = Console.ReadLine();

    if (allowEmpty && string.IsNullOrEmpty(buf)) return null;

    int num;
    while (int.TryParse(buf, out num))
    {
        Console.WriteLine("Некорректный ввод. Введите целое число, положительное или отрицательное");
        buf = Console.ReadLine();
    }

    return num;
}

public static string ReadOperation()
{
    string[] allowedOperations = { "+", "-", "*", "/", "!", "dsum" };

    do
    {
        string operation = Console.ReadLine();
        if (allowedOperations.Contains(operation)) return operation;

        Console.WriteLine($"Неизвестная операция '{operation}'. Повторите ввод");
    } while (true);
}

public static int Factorial(int num)
{
    int factorial = 1;
    for (int i = 2; i < num; i++)
        factorial *= i;
    return factorial;
}

public static int GetDigitsSum(int num)
{
    int dsum = 0;
    while (num != 0)
    {
        int lastDigit = num / 10;
        num = num % 10;
        dsum += lastDigit;
    }
    return dsum;
}
```
