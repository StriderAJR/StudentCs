using System;
using System.Collections.Generic;
using System.Text;

namespace ChessWithClasses
{
    public class Game
    {
        // захаркодено, но можно сделать изменяемыми, например, в настройках игры
        private const int cellSizeY = 3;
        private const int cellSizeX = 5;
        private const int fieldWidth = 8;
        private const int fieldHeight = 8;
        private const int margin = 5;

        private static int messageStartX = margin + cellSizeX * fieldWidth + 3;
        private static int messageStartY = margin;

        private const string wrongCoordinatesMessage = " так ходить не может!";
        readonly Board board = new Board(cellSizeX, cellSizeY, margin);

        public string[] ReadFigurePositions()
        {
            ClearMessages();

            Console.SetCursorPosition(messageStartX, messageStartY);

            Console.Write("Введите стартовую координату: ");
            string start = ReadCoord();

            Console.SetCursorPosition(messageStartX, messageStartY + 1);
            Console.Write("Введите конечную координату: ");
            string end = ReadCoord();

            return new string[] {start, end};
        }

        public void Refresh()
        {
            board.DrawBoard();
        }

        public void TryMoveFigure(string start, string end)
        {
            bool isSuccess = board.TryMoveFigure(start, end);
            if(!isSuccess)
            {
                var figure = board.Get(start);
                if(figure == null)
                {
                    Console.WriteLine("На начальной позиции нет фигуры.");
                }
                else
                {
                    Console.WriteLine(figure.RusName + wrongCoordinatesMessage);
                }
            }
        }

        private void ClearMessages()
        {
            int xStart = messageStartX, yStart = messageStartY;
            for(int x = xStart; x < xStart + 50; x++)
                for(int y = yStart; y < yStart + 3; y++)
                {
                    Console.SetCursorPosition(x, y);
                    Console.Write(' ');
                }
        }

        private string ReadCoord()
        {
            do
            {
                string input = Console.ReadLine().ToUpper();
                if (IsCorrectCoordinate(input))
                {
                    return input;
                }
                else
                {
                    // TODO чтобы строка не уезжала вправо
                    Console.WriteLine("Координата не корректна!");
                }
            }
            while (true);
        }

        private bool IsCorrectCoordinate(string coord)
        {
            if(string.IsNullOrEmpty(coord)) return false;

            char letter = coord[0];
            char num = coord[1];
            return coord.Length == 2 && letter >= 'A' && letter <= 'H' && num >= '1' && num <= '8';
        }
    }
}
