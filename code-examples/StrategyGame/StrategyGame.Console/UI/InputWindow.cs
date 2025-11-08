using System;

namespace StrategyGame.ConsoleGame.UI;

/// <summary>
/// Окно с полем ввода текста и кнопкой Ok (Enter)
/// </summary>
public class InputWindow : ConsoleWindow<string>
{
    private string input = string.Empty;

    /// <summary>
    /// Custom constructor: explicit size and position
    /// </summary>
    public InputWindow(string message, string? title, int width, int height, Coordinate position)
        : base(message, title, width, height, position)
    {
    }

    /// <summary>
    /// Auto constructor: size/position by enums
    /// </summary>
    public InputWindow(string message, string? title = null, WindowPosition windowPosition = WindowPosition.Center, WindowSize windowSize = WindowSize.Auto)
        : base(message, title, windowPosition, windowSize)
    {
    }

    /// <summary>
    /// Interactive logic moved here; base.Show() will call this then ClearScreen().
    /// </summary>
    protected override string ShowInternal()
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
