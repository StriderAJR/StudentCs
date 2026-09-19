namespace UnitTestsDemo.MainProject;

public class Program
{
    public static int ConvertToNumber(string buffer)
    {
        int result = 0;
        foreach (char letter in buffer)
        {
            int digit = letter - '0'; // 0 = 48, 1 = 49 ... 
            result = result * 10 + digit;
        }

        return result;
    }

    public static int Add(int a, int b)
    {
        return a + b;
    }

    public static int Max(int a, int b)
    {
        if (a > b)
        {
            return a;
        }

        return b;
    }

    public static bool IsEven(int number)
    {
        return number % 2 == 0;
    }

    public static bool IsPositive(int number)
    {
        return number > 0;
    }

    public static int SumFromOneTo(int number)
    {
        var sum = 0;

        for (var i = 1; i <= number; i++)
        {
            sum += i;
        }

        return sum;
    }

    public static int CountDigits(int number)
    {
        if (number == 0)
        {
            return 1;
        }

        var count = 0;

        while (number != 0)
        {
            number /= 10;
            count++;
        }

        return count;
    }

    public static string GetNumberSign(int number)
    {
        if (number > 0)
        {
            return "Positive";
        }

        if (number < 0)
        {
            return "Negative";
        }

        return "Zero";
    }

    public static string Repeat(string text, int count)
    {
        var result = "";

        for (var i = 0; i < count; i++)
        {
            result += text;
        }

        return result;
    }

    public static void Main()
    {
        Add(1, 2);
    }
}