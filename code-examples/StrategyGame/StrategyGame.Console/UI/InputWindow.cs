namespace StrategyGame.ConsoleGame.UI;

/// <summary>
/// Окно с полем ввода текста и кнопкой Ok (Enter)
/// </summary>
public class InputWindow(string message,string? title = null, int? width = null, int? height = null, Coordinate? position = null)
    : ConsoleWindow(message, title, width, height, position)
{
    private string input = string.Empty;

    /// <summary>
    /// Отображает окно с полем ввода и возвращает введённый текст после нажатия Enter
    /// </summary>
    public string Show()
    {
        bool finished = false;

        while (!finished)
        {
            base.Draw();

            // поле ввода
            int inputY = position.Y + 2;
            int inputX = position.X + 2;
            Console.SetCursorPosition(inputX, inputY);
            Console.Write(new string(' ', width - 4));
            Console.SetCursorPosition(inputX, inputY);
            Console.Write(input);

            Console.SetCursorPosition(inputX + input.Length, inputY);

            ConsoleKeyInfo key = Console.ReadKey(true);

            if (key.Key == ConsoleKey.Enter)
            {
                finished = true;
            }
            else if (key.Key == ConsoleKey.Backspace)
            {
                if (input.Length > 0)
                    input = input[..^1];
            }
            else if (!char.IsControl(key.KeyChar))
            {
                input += key.KeyChar;
            }
        }

        return input;
    }
}
