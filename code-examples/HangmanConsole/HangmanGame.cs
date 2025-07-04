using System;
using System.Linq;

namespace HangmanGame
{
    internal class GameState
    {
        public char[] WordState { get; set; }
        public int TriesCount { get; set; }
        public bool IsError { get; set; }
        public string Message { get; set; }
    }
    
    public class HangmanGame
    {
        private static readonly string[] dictionary = new[] {"привет", "мир", "василиск"};
        private string word;
        private GameState gameState;

        private IHangmanGamePrinter printer;
        

        public HangmanGame()
        {
            printer = new HangmanGameConsolePrinter();
            
            StartNewGame();
        }

        private readonly Random random = new Random();
        public void StartNewGame()
        {
            int randomIndex = random.Next(0, dictionary.Length);
            word = dictionary[randomIndex];

            gameState = new GameState
            {
                WordState = Enumerable.Repeat('_', word.Length).ToArray(),
                IsError = false,
                Message = string.Empty,
                TriesCount = 5
            };
        }

        public void Render()
        {
            printer.Render(gameState);
        }

        public void Check(string input)
        {
            if (input.Length == 0)
            {
                gameState.IsError = true;
                gameState.Message = "Введите только одну букву";
                return;
            }

            char letter = input[0];
            if (input.Length > 1)
            {
                gameState.IsError = true;
                gameState.Message = "Введите только одну букву";
            } else if (word.Contains(letter))
            {
                for (int i = 0; i < word.Length; i++)
                {
                    if (word[i] == letter) gameState.WordState[i] = letter;
                }
            }
            else
            {
                gameState.IsError = true;
                gameState.Message = "Такой буквы нет";
                gameState.TriesCount--;
            }
        }
    }
}