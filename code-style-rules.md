# Стиль и структура кода

## Скобочные и безскобочные блоки кода

Не надо смешивать скобочные и безскобочные блоки кода. Или все делаем со скобками или все без скобок.
Изображение

Вложенный if внутри else всегда идет или с табуляцией (если это беcскобочная запись) или со скобками + табуляцией. Или если хочется сэкономить место (любите однострочные записи), то else + if идут на одной строчке. Лучше всегда пользоваться или 2 или 3 вариантом. 1 ущербен и никто так не делает по описанным причинам. Или экономим место, или делаем как нужно.

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
Изображение

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

# Работа с кодом и методами

## Не использовать вложенные методы

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
