namespace StrategyGame.ConsoleGame.UI;

/// <summary>
/// Базовое консольное окно с рамкой, опциональным заголовком и текстом
/// </summary>
public class ConsoleWindow(string message, string? title = null, int? width = null, int? height = null, Coordinate? position = null)
{
    // Вверху представлена запись inline конструктора
    //
    // Полная запись inline-конструктора эквивалентна обычному конструктору:
    // public Menu(Coordinate position, int width, int height, string title, params string[] items)
    // {
    //     this.position = position;
    //     this.width = width;
    //     this.height = height;
    //     this.title = title;
    //     this.items = items;
    // }
    //
    //
    // Inline-конструктор (C# 12) — это компактная запись конструктора прямо в объявлении класса.
    // Преимущества и случаи использования:
    //
    // 1. Подходит, когда класс в основном хранит данные (поля), 
    //    и логика конструктора сводится к простому присваиванию параметров полям.
    //    Пример: небольшие классы GUI (Menu, Button), модели координат, DTO.
    //
    // 2. Экономит место и делает код более читаемым, особенно если класс небольшой.
    //
    // Когда не подходит:
    //
    // 1. Если конструктор содержит сложную логику (валидацию, вычисления, вызовы методов, условные блоки).
    //    Inline-конструктор допускает только прямое присваивание полям, не больше.
    //
    // 2. Если нужно использовать перегрузки конструкторов, вызовы `this(...)` или `base(...)`.
    //    Inline-конструктор не позволяет явно вызывать другие конструкторы.
    //
    // 3. Если класс наследуется и конструктор должен вызывать базовый конструктор с логикой,
    //    лучше использовать обычный конструктор.
    //
    // Итого: inline-конструктор — для компактных, "простых" классов-данных;
    // сложные или логически насыщенные классы — обычный конструктор.

    protected readonly Coordinate position = position ?? new Coordinate(0, 0);
    protected readonly int width = width ?? Console.WindowWidth;
    protected readonly int height = height ?? Console.WindowHeight;
    protected readonly string? title = title;
    protected readonly string? message = message;

    /// <summary>
    /// Рисует окно: рамку, заголовок и текст
    /// </summary>
    public virtual void Draw()
    {
        Console.ForegroundColor = ConsoleColor.Gray;

        // рамка
        for (int i = 0; i < height; i++)
        {
            Console.SetCursorPosition(position.X, position.Y + i);
            for (int j = 0; j < width; j++)
            {
                char c =
                    i == 0 && j == 0 ? '┌' :
                    i == 0 && j == width - 1 ? '┐' :
                    i == height - 1 && j == 0 ? '└' :
                    i == height - 1 && j == width - 1 ? '┘' :
                    i == 0 || i == height - 1 ? '─' :
                    j == 0 || j == width - 1 ? '│' : ' ';
                Console.Write(c);
            }
        }

        // заголовок
        if (!string.IsNullOrEmpty(title))
        {
            Console.SetCursorPosition(position.X + 2, position.Y);
            Console.Write($"[{title}]");
        }

        // текст
        if (!string.IsNullOrEmpty(message))
        {
            string[] lines = message.Split('\n');
            for (int i = 0; i < lines.Length && i < height - 2; i++)
            {
                Console.SetCursorPosition(position.X + 2, position.Y + 1 + i);
                Console.Write(lines[i]);
            }
        }

        Console.ResetColor();
    }
}