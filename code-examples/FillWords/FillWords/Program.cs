using System;

namespace FillWords
{
    class Program
    {
        static void Main(string[] args)
        {
            // TEST
            Console.CursorVisible = false;

            FillWords fillWords = new FillWords(new ConsoleDrawer(), 6, 6);

            do 
            {
                fillWords.Draw();

                ConsoleKeyInfo cki = Console.ReadKey();

                if(cki.Key == ConsoleKey.DownArrow)  fillWords.SetHover(0, 1);
                if(cki.Key == ConsoleKey.UpArrow)    fillWords.SetHover(0, -1);
                if(cki.Key == ConsoleKey.RightArrow) fillWords.SetHover(1, 0);
                if(cki.Key == ConsoleKey.LeftArrow)  fillWords.SetHover(-1, 0);
                if(cki.Key == ConsoleKey.Enter)      fillWords.SwitchSelectionMode();
            } while (true);
        }
    }
}
