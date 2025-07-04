using System;
using HangmanGame;

namespace HangmanConsole
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("Hello World!");
            HangmanGame.HangmanGame game = new HangmanGame.HangmanGame();
            do
            {
                game.Render();
                var letter = Console.ReadLine();
                game.Check(letter);
            } while (true);
        }
    }
}