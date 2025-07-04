using System;

namespace HangmanGame
{
    internal class HangmanGameConsolePrinter : IHangmanGamePrinter
    {
        private IHangmanGamePrinter _hangmanGamePrinterImplementation;

        public void Render(GameState gameState)
        {
            Console.Clear();
            Console.WriteLine(gameState.WordState);
            Console.WriteLine($"У вас осталось {gameState.TriesCount} попыток");
            if(gameState.Message != string.Empty)
                Console.WriteLine(gameState.Message);
            Console.Write("Введите букву: ");
        }
    }
}