namespace StrategyGame.ConsoleGame.UI;

public class MenuWindow(string message, string[] items, string? title = null, int? width = null, int? height = null, Coordinate? position = null)
    : ConsoleWindow(message, title, width, height, position)
{
    // В inline-конструкторе можно не создавать отдельные поля для всех параметров,
    // если не нужны дополнительные модификаторы или логика.
    // Например, параметр `items` можно использовать напрямую, без отдельного private поля.
    //
    // Если же нужно сделать поле readonly, или добавить инициализацию/валидацию, 
    // тогда создаём явное поле с нужным модификатором и присваиваем значение из параметра.

    private int selectedItemIndex = 0;

    /// <summary>
    /// Отобразить меню
    /// </summary>
    /// <returns>Возвращает индекс выбранной кнопки</returns>
    /// <remarks>В будущем переделать так, чтобы у кнопки было событие срабатывания</remarks>
    public int Show()
    {
        bool shouldContinue = true;
        while (shouldContinue)
        {
            DrawMenu();

            ConsoleKey input = Console.ReadKey(true).Key;
            switch (input)
            {
                case ConsoleKey.Enter:
                    shouldContinue = false;
                    break;
                case ConsoleKey.UpArrow:
                case ConsoleKey.LeftArrow:
                    selectedItemIndex = selectedItemIndex - 1 >= 0
                        ? selectedItemIndex - 1
                        : items.Length - 1;
                    break;
                case ConsoleKey.DownArrow:
                case ConsoleKey.RightArrow:
                    selectedItemIndex = selectedItemIndex + 1 < items.Length
                        ? selectedItemIndex + 1
                        : 0;
                    break;
            }
        }

        return selectedItemIndex;
    }

    private void DrawMenu()
    {
        base.Draw();

        if (items == null || items.Length == 0)
            return;

        int baseY = position.Y + height - 3;

        // if buttons count = 1 - draw in the window center
        // if buttons count = 2 - draw on the left and right
        // if buttons count > 2 - draw buttons vertically in the window center 

        if (items.Length == 1)
            DrawButton(items[0], 0, position.X + width / 2, baseY, centered: true);
        else if (items.Length == 2)
        {
            DrawButton(items[0], 0, position.X + 4, baseY);
            DrawButton(items[1], 1, position.X + width - items[1].Length - 6, baseY);
        }
        else
        {
            int totalHeight = items.Length * 2 - 1;
            int startY = position.Y + (height - totalHeight) / 2;
            for (int i = 0; i < items.Length; i++)
                DrawButton(items[i], i, position.X + width / 2, startY + i * 2, centered: true);
        }
    }

    /// <summary>
    /// Отрисовка одной кнопки
    /// </summary>
    private void DrawButton(string text, int index, int x, int y, bool centered = false)
    {
        string b = $"[ {text} ]";
        int bx = centered ? x - b.Length / 2 : x;

        Console.SetCursorPosition(bx, y);
        if (selectedItemIndex == index)
        {
            Console.BackgroundColor = ConsoleColor.Gray;
            Console.ForegroundColor = ConsoleColor.Black;
        }
        Console.Write(b);
        Console.ResetColor();
    }
}
