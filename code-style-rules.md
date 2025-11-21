# Рекомендации по C# с примерами

## 1. Форматирование кода

### Скобки и отступы

Выбирайте единый стиль для всего кода: либо со скобками, либо без.

**Варианты:**

```csharp
// Вариант 1: без скобок, с табуляцией
if(...) some_operation
else
  if(...) another_operation
  else last_operation

// Вариант 2: со скобками
if(...) 
{
  some_operation
}
else
{
  if(...) another_operation
  else last_operation
}

// Вариант 3: однострочный else if
if(...) some_operation
else if(...) another_operation
else last_operation
```

**Чего не делать:**

```csharp
if(...) some_operation
else
if(...) another_operation
else last_operation
```

### Висящие блоки кода

Не оставляйте блоки, обрамленные просто фигурными скобками без смысла:

```csharp
int a = 10;
{
   int b = 5;
   Console.WriteLine(a + b);
}
```

---

## 2. Именование переменных

### Говорящие имена

```csharp
int studentCount;
double percent;
string fullName;
int[] coordinates;
double currentDeposit;
int vectorLength;
double squareArea, circleArea;
int[] studentGrades;
```

### Допустимые однобуквенные имена

```csharp
int i, j, k; // счетчики цикла
char c;      // символ
int n, m;    // размеры массивов и матриц
```

### Плохие примеры

```csharp
string number; // кажется число, но это строка
int c;         // символ
int[] digit;   // массив, а не одна цифра
```

### Хорошие варианты имен

```csharp
string numberStr, strNumber, numberAsStr;
int number, num, count, length;
int[] digits, numbers;
```

---

## 3. Операторы и арифметика

* Отделяйте операторы пробелами:

```csharp
return 0.5 * Math.Asin(distance * 9.8 / (v * v));
```

* Не создавайте переменные только для возврата значения:

```csharp
var angle = 0.5 * Math.Asin((distance * g) / (v * v));
return angle;
```

Лучше сразу:

```csharp
return 0.5 * Math.Asin((distance * g) / (v * v));
```

* Для возведения в квадрат или куб лучше использовать умножение:

```csharp
return 0.5 * Math.Asin(distance * g / Math.Pow(v, 2));
```

* Магические числа выносите в константы:

```csharp
float g = 9.8;
return 0.5 * Math.Asin((g * distance) / (v * v));
```

---

## 4. Булевы выражения

Не нужно явно возвращать `true` или `false` при проверке условия.

**Плохо:**

```csharp
private bool CorrectMonth(int month)
{
    return (month >= 1 && month <= 12) ? true : false;
}
```

**Правильно:**

```csharp
private bool CorrectMonth(int month)
{
    return month >= 1 && month <= 12;
}
```

**Пример инверсии:**

```csharp
return !condition;
```

---

## 5. Ветвления и циклы

### If / Else

Если в ветке идет `return`, `else` не нужен:

```csharp
if (count % 10 == 0 || count % 10 >= 5 || (count % 100 >= 10 && count % 100 <= 19)) return "рублей";
if (count % 10 == 1) return "рубль";
return "рубля";
```

### Вложенные ветвления

```csharp
int lastTwoDigits = count % 100;
int lastDigit = count % 10;
if (lastTwoDigits >= 5 && lastTwoDigits <= 20) return "рублей";
if (lastDigit >= 2 && lastDigit <= 4) return "рубля";
if (lastDigit == 1) return "рубль";
return "рублей";
```

### Циклы

* Если известен счёт итераций — `for`.
* Бесконечные циклы — `while(true)`.

```csharp
for(; ;) {} // плохо
while(true){} // хорошо
```

---

## 6. Методы и повторяемость кода

* Разделяйте программу на методы, чтобы улучшить читаемость.
* Не используйте вложенные методы.

**Плохо (вложенные методы):**

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
    Console.WriteLine(Sum(num));
}
```

**Правильно:**

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
    Console.WriteLine(Sum(num));
}
```

**Пример разбиения на методы:**

```csharp
static int SumOfDigits(string str) { ... }
static string ReverseString(string str) { ... }

public static void Main()
{
    string numberStr = Console.ReadLine();    
    int x = Convert.ToInt32(numberStr);

    if (x >= 0)
    {
        if (SumOfDigits(numberStr) % 2 == 0)
            Console.WriteLine("чётное");
        if (x % 2 != 0)
            Console.WriteLine(ReverseString(numberStr));
    }
}
```

---

## 7. Избегание повторяемого кода

**Плохо:**

```csharp
double ak = Math.Sqrt((x - ax) * (x - ax) + (y - ay) * (y - ay));
double kb = Math.Sqrt((x - bx) * (x - bx) + (y - by) * (y - by));
double ab = Math.Sqrt((ax - bx) * (ax - bx) + (ay - by) * (ay - by));
```

**Правильно:**

```csharp
double ak = GetDistanceBetweenPoints(ax, ay, x, y);
double kb = GetDistanceBetweenPoints(bx, by, x, y);
double ab = GetDistanceBetweenPoints(bx, by, ax, ay);

double GetDistanceBetweenPoints(double x1, double y1, double x2, double y2)
{
    return Math.Sqrt((x2 - x1)*(x2 - x1) + (y2 - y1)*(y2 - y1));
}
```

---

## 8. Работа с файлами

* Никогда не используйте абсолютные пути.

```csharp
string[] lines = File.ReadAllLines(@"C:\VasyaPupkin\my_cool_program\bin\Debug\net8.0\example.txt");
```

* Используйте относительные пути:

```csharp
string[] lines = File.ReadAllLines(@".\example.txt");
string[] lines = File.ReadAllLines(@"..\..\..\..\example.txt");
```

**Пример улучшенной работы с файлами:**

```csharp
string[] lines = File.ReadAllLines("./bank-balance.txt");
var initialBalance = int.Parse(lines[0]);
int currentBalance = initialBalance;
Stack<int> balanceHistory = new Stack<int>(new List<int>{0});

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

public static (string, int) ParseLine(string line) { ... }
public static int ApplyOperation(string command, int currentBalance, int operationSum, Stack<int> balanceHistory) { ... }
```

---

## 9. Пример калькулятора

**Плохо:**

```csharp
string buf = Console.ReadLine();
int num1;
while (int.TryParse(buf, out num1))
{
    Console.WriteLine("Некорректный ввод...");
    buf = Console.ReadLine();
}

string operation = Console.ReadLine();
buf = Console.ReadLine();
int? num2 = null;
if (!string.IsNullOrEmpty(buf))
{
    int tempNum;
    while (int.TryParse(buf, out tempNum))
    {
        Console.WriteLine("Некорректный ввод...");
        buf = Console.ReadLine();
    }
    num2 = tempNum;
}

if (operation == "+") Console.WriteLine(num1 + num2);
else if (operation == "-") Console.WriteLine(num1 - num2);
```

**Правильно (с методами и `switch`):**

```csharp
public static void Main(string[] args)
{
    int num1 = ReadNumber().Value;
    int? num2 = ReadNumber(true);
    string operation = ReadOperation();

    double result = operation switch
    {
        "+" => num1 + num2.Value,
        "-" => num1 - num2.Value,
        "*" => num1 * num2.Value,
        "/" => num1 / num2.Value,
        "!" => Factorial(num1),
        "log" => Math.Log(num1, num2.Value),
        "dsum" => GetDigitsSum(num1),
        _ => 0
    };

    Console.WriteLine(result);
}

public static int? ReadNumber(bool allowEmpty = false) { ... }
public static string ReadOperation() { ... }
public static int Factorial(int num)
{
    int factorial = 1;
    for (int i = 2; i < num; i++) factorial *= i;
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
