using System;
using System.Reflection.Metadata.Ecma335;

namespace TempProject;

// ветвления
// циклы
// методы
// юнит тесты

internal class Program
{
    public static int ConvertToNumber(string buffer)
    {
        // ...
        // нельзя использовать Parse, TryParse, Convert.ToInt32() и т.д. подобное
        // вернуть число

        // привидение - (int)
        // преобразование - Parse, Convert.ToInt32()

        // "1234"

        int result = 0;
        foreach(char letter in buffer)
        {
            int digit = letter - '0'; // 0 = 48, 1 = 49 ... 
            result = result * 10 + digit;
        }

        // 1 = 1 * 10^3
        // 2 = 2 * 10^2
        // 3 = 3 * 10^1
        // 4
        // 1234

        // 1 => result = 1
        // 2 => result = 1 * 10 + 2 = 12
        // 3 => result = 12 * 10 + 3 = 123
        // ...

        return result;
    }

    public static void Main()
    {
        int num = ConvertToNumber(Console.ReadLine());
    }
}