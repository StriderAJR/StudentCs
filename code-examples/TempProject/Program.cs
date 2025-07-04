using Microsoft.VisualBasic.CompilerServices;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;

namespace Table
{
    public class YieldExample
    {
        public IEnumerator GetTestEnumerator()
        {
            for (int i = 0; i < 6; i++)
            {
                yield return i;
            }
        }

        private string[] books = new[] { "book1", "book2", "book3" };
        public IEnumerator GetBooks()
        {
            Console.WriteLine("Возвращаем книгу 1");
            yield return books[0];
            Console.WriteLine("Возвращаем книгу 2");
            yield return books[1];
            Console.WriteLine("Возвращаем книгу 3");
            yield return books[2];
            Console.WriteLine("Все. Книг больше нет.");
        }

        public IEnumerator GetFilms()
        {
            for (int i = 0; i < 5; i++)
            {
                yield return $"Film{i + 1}";
            }
        }

        private double xStart = 1, xEnd = 5, delta = 1;
        private double Calculate(double x)
        {
            return x * 2 + Math.Log(x) / 5;
        }
        
        public IEnumerator GetEnumerator()
        {
            for(double i = xStart; i < xEnd; i += delta)
            {
                yield return Calculate(i);
            }
        }


    }

    class Program
    {
        private static void Example1()
        {
            YieldExample yieldExample = new YieldExample();
            foreach (double n in yieldExample)
            {
                Console.WriteLine(n);
            }
        }

        private static void Example2()
        {
            YieldExample yieldExample = new YieldExample();

            Console.Write("Получить итератор...");
            var bookEnumerator = yieldExample.GetBooks();
            Console.WriteLine("done");
            do
            {
                Console.Write("Выводим значение элемента...");
                Console.WriteLine(bookEnumerator.Current);
                Console.WriteLine("done");

                Console.WriteLine("Если можем сдвинуть интератор дальше, то продолжаем...");
            }
            while (bookEnumerator.MoveNext());
            Console.WriteLine("Сдвинуть итератор не удалось. Перебор коллекции завершен.");
        }

        private static void Example3()
        {
            YieldExample yieldExample = new YieldExample();

            Console.Write("Получить итератор...");
            var enumerator = yieldExample.GetFilms();
            Console.WriteLine("done");
            while (enumerator.MoveNext())
            {
                Console.Write("Выводим значение элемента...");
                Console.WriteLine(enumerator.Current);

                Console.WriteLine("Если можем сдвинуть интератор дальше, то продолжаем...");
            }
            Console.WriteLine("Сдвинуть итератор не удалось. Перебор коллекции завершен.");
        }

        public static void Main()
        {
            //Example1();
            //Example2();
            //Example3();

            ComplexNumber cn1 = new ComplexNumber(1, 1);
            ComplexNumber cn2 = new ComplexNumber(2, 1);

            Console.WriteLine(cn1 + cn2);
            Console.WriteLine(cn1 * 5);
            Console.WriteLine(cn2++);
            Console.WriteLine(++cn2);

            //Example4();
        }

        public static void Example4()
        {
            OperatorOverloadingExample example = new OperatorOverloadingExample();

            Console.WriteLine(example + "???");
            Console.WriteLine(example + 2);
            Console.WriteLine(example == 2);
            Console.WriteLine(example != 2);
        }
    }

    public class ComplexNumber
    {
        private int imaginaryNumber, realNumber;

        public ComplexNumber(int imaginaryNumber, int realNumber)
        {
            this.imaginaryNumber = imaginaryNumber;
            this.realNumber = realNumber;
        }

        public static ComplexNumber operator+(ComplexNumber left, ComplexNumber right)
        {
            return new ComplexNumber(
                left.imaginaryNumber + right.imaginaryNumber, 
                left.realNumber + right.realNumber);
        }

        public static int operator*(ComplexNumber left, int number)
        {
            return left.imaginaryNumber * number * number;
        }

        public static ComplexNumber operator++(ComplexNumber left)
        {
            return new ComplexNumber(left.imaginaryNumber++, left.realNumber++);
        }

        public override string ToString()
        {
            return $"{imaginaryNumber}i + {realNumber}";
        }
    }

    public class OperatorOverloadingExample
    {
        private string someField = "Hello!";

        public static string operator+(OperatorOverloadingExample left, string right)
        {
            return $"{left.someField} + {right} = success";
        }

        public static int operator+(OperatorOverloadingExample left, int right)
        {
            return left.someField.Length * right;
        }

        public static bool operator==(OperatorOverloadingExample left, int right)
        {
            return left.someField.Length == right;
        }

        public static bool operator!=(OperatorOverloadingExample left, int right)
        {
            return !(left == right);
        }
    }
}
