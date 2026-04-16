using System.Collections;
using System.Collections.Concurrent;
using System.Diagnostics;
using System.Linq;

namespace TempProject;

// + yield return
// + интерфейсы
// + списки, словари, стек, очереди, компоновщик
// + Exceptions
// + перегрузка операторов
// + IEnumerable, IEnumerator
// + дженерики
// + делегаты
// + LINQ
// + unit тесты

// разбиение программы на проекты

// Многопоточное программирование. Класс Thread. Потоки и блокировки. async и await. Блокирование потока GUI. BackgroundWorker
// Потокобезопасность. Что такое race condition, зачем нужен lock, чем async/await отличается от многопоточности.

class Program
{
    public static async Task Main()
    {
        Stopwatch sp = Stopwatch.StartNew();

        ConcurrentBag<int> list = new ConcurrentBag<int>();

        // ThreadPool
        Parallel.For(0, 1000, i =>
        {
            list.Add(i);
        });

        var thread = new Thread(() =>
        {
            Console.WriteLine("Hello");
        });

        thread.Start();


        Console.WriteLine(list.Count);
        Console.WriteLine(sp.ElapsedMilliseconds);
    }
}