using System;
using System.Reflection.Metadata.Ecma335;

namespace TempProject;

// ветвления
// циклы
// методы
// юнит тесты

internal class Program
{
    public static int ParseToNumber(string buffer)
    {
        // "1234"

        int result = 0;
        foreach (char c in buffer)
        {
            int digit = c - '0';
            result = result * 10 + digit;
        }

        for (int i = 0; i < buffer.Length; i++)
        {
            char c = buffer[i];

            int digit = c - '0';
            result = result * 10 + digit;
        }

        return result;

        // 1 = 1
        // 2 = 1 * 10 + 2 = 12
        // 3 = 12 * 10 + 3 = 120 + 3 = 123
        // 4 = 123 * 10 + 4 = 1234
    }


    public static void Main()
    {
        int a = 10;

        // for 
        // while 
        // do-while

        for (int i = 5; i <= 5; i++)
        {
            if (i % 2 == 0) continue;

            Console.WriteLine(i);
        }

        int i2 = 5;
        while(a <= 5)
        {
            if (i2 % 2 == 0)
            {
                i2++;
                continue;
            }
            Console.WriteLine(i2);
            i2++;
        }

        do
        {
            Console.WriteLine("...");
        }
        while (a <= 5);

        int num = Add(1, 2);
        PrintMessage("Hello");
    }
}