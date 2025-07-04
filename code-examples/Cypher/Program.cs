using System;
namespace TempCs
{
    class Program
    {
        private static bool IsCorrectCoordinate(string coord)
        {
            char letter = coord[0];
            char num = coord[1];
            return coord.Length == 2 && letter >= 'A' && letter <= 'H' && num >= '1' && num <= '8';
        }

        private static string ReadCoord()
        {
            do
            {
                string input = Console.ReadLine().ToUpper();
                if (IsCorrectCoordinate(input)) 
                    return input;
                else                           
                    Console.WriteLine("Координата не корректна!");
            }
            while (true);
        }

        enum FigureType
        {
            Knight
        }

        private static FigureType ReadFigureType()
        {
            do
            {
                string input = Console.ReadLine();
                if (input == "0" || input.ToLower() == "knight")
                    return FigureType.Knight;
                else                           
                    Console.WriteLine("Тип фигуры не корректен!");
            }
            while (true);
        }

        private static bool IsKnightCorrect(string start, string end)
        {
            int dx = Math.Abs(end[0] - start[0]);
            int dy = Math.Abs(end[1] - start[1]);

            return dx + dy == 3 && dx * dy == 2;
        }

        static void Main(string[] args)
        {
            Console.Write("Введите тип фигуры (0 - Конь):");
            FigureType figure = ReadFigureType();

            Console.Write("Введите стартовую координату: ");
            string start = ReadCoord();

            Console.Write("Введите конечную координату: ");
            string end = ReadCoord();

            bool isCorrect = false;
            switch(figure)
            {
                case FigureType.Knight: 
                    isCorrect = IsKnightCorrect(start, end); 
                    break;
            }

            if (isCorrect)
                Console.WriteLine("Ваша фигура ходит правильно.");
            else
                Console.WriteLine("Такой ход не возможен для данной фигуры.");
        }
    }
}