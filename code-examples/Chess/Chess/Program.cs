using System;
using System.Collections.Generic;
namespace TempCs
{
    class Program
    {
        private const int cellSizeY = 3;
        private const int cellSizeX = 5;
        private const int fieldWidth = 8;
        private const int fieldHeight = 8;
        private const int margin = 5;

        private static int messageStartX = margin + cellSizeX * fieldWidth + 3;
        private static int messageStartY = margin;

        private const string wrongCoordinatesMessage = " так ходить не может!";

        enum FigureType
        {
            Pawn = 'P', Knight = 'H', Rook = 'R', Bishop = 'B', Queen = 'Q', King = 'K' 
        }

        static void Main(string[] args)
        {
            Dictionary<char, Dictionary<char, char>> board = InitBoard();

            do
            {
                DrawBoard(board);
                
                string[] coords = ReadFigurePositions();
                string start = coords[0];
                string end = coords[1];

                TryMoveFigure(board, start, end);
            } while (true);
        }

        private static Dictionary<char, Dictionary<char, char>> InitBoard()
        {
            return new Dictionary<char, Dictionary<char, char>>()
            {
                { 'A', new Dictionary<char, char> { { '1', ' ' }, { '2', ' ' }, { '3', ' ' }, { '4', ' ' }, { '5', ' ' }, { '6', ' ' }, { '7', ' ' }, { '8', ' ' } } },
                { 'B', new Dictionary<char, char> { { '1', ' ' }, { '2', ' ' }, { '3', ' ' }, { '4', ' ' }, { '5', ' ' }, { '6', ' ' }, { '7', ' ' }, { '8', ' ' } } },
                { 'C', new Dictionary<char, char> { { '1', ' ' }, { '2', ' ' }, { '3', ' ' }, { '4', ' ' }, { '5', ' ' }, { '6', ' ' }, { '7', ' ' }, { '8', ' ' } } },
                { 'D', new Dictionary<char, char> { { '1', ' ' }, { '2', ' ' }, { '3', ' ' }, { '4', ' ' }, { '5', ' ' }, { '6', ' ' }, { '7', ' ' }, { '8', ' ' } } },
                { 'E', new Dictionary<char, char> { { '1', ' ' }, { '2', ' ' }, { '3', ' ' }, { '4', ' ' }, { '5', ' ' }, { '6', ' ' }, { '7', ' ' }, { '8', ' ' } } },
                { 'F', new Dictionary<char, char> { { '1', ' ' }, { '2', ' ' }, { '3', ' ' }, { '4', ' ' }, { '5', ' ' }, { '6', ' ' }, { '7', ' ' }, { '8', ' ' } } },
                { 'G', new Dictionary<char, char> { { '1', 'P' }, { '2', 'P' }, { '3', 'P' }, { '4', 'P' }, { '5', 'P' }, { '6', 'P' }, { '7', 'P' }, { '8', 'P' } } },
                { 'H', new Dictionary<char, char> { { '1', 'R' }, { '2', 'B' }, { '3', 'H' }, { '4', 'Q' }, { '5', 'K' }, { '6', 'B' }, { '7', 'H' }, { '8', 'R' } } }
            };
        }
        
        private static bool IsCorrectCoordinate(string coord)
        {
            if(string.IsNullOrEmpty(coord)) return false;

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

        private static void TryMoveFigure(Dictionary<char, Dictionary<char, char>> board, string start, string end)
        {
            char figureSymbol = board[start[0]][start[1]];
            if(figureSymbol == ' ')
            {
                Console.WriteLine("На стартовой точке нет фигуры. Нажмите любую клавишу, чтобы повторить...");
                return;
            }

            bool isCorrect = false;
            FigureType figure = (FigureType)figureSymbol;
            switch(figure)
            {
                case FigureType.Pawn:   TryMovePawn  (board, start, end); break;
                case FigureType.Rook:   TryMoveRook  (board, start, end); break;
                case FigureType.Bishop: TryMoveBishop(board, start, end); break;
                case FigureType.Knight: TryMoveKnight(board, start, end); break;
                case FigureType.Queen:  TryMoveQueen (board, start, end); break;
                case FigureType.King:   TryMoveKing  (board, start, end); break;
            }

            if(!isCorrect)
            {
                // Вывести ошибку
                return;
            }
        }

        private static void MoveFigure(Dictionary<char, Dictionary<char, char>> board, string start, string end, FigureType figure)
        {
            board[start[0]][start[1]] = ' ';
            board[end[0]]  [end[1]]   = (char) figure;
        }

        private static void TryMoveKing(Dictionary<char, Dictionary<char, char>> board, string start, string end)
        {
            if(!IsKingCorrect(start, end))
            {
                Console.WriteLine("Король" + wrongCoordinatesMessage);
                return;
            }
        }

        private static void TryMoveQueen(Dictionary<char, Dictionary<char, char>> board, string start, string end)
        {
            if(!IsQueenCorrect(start, end))
            {
                Console.WriteLine("Королева" + wrongCoordinatesMessage);
                return;
            }
        }

        private static void TryMoveKnight(Dictionary<char, Dictionary<char, char>> board, string start, string end)
        {
            if(!IsKnightCorrect(start, end))
            {
                Console.WriteLine("Конь" + wrongCoordinatesMessage);
                return;
            }
        }

        private static void TryMoveBishop(Dictionary<char, Dictionary<char, char>> board, string start, string end)
        {
            if(!IsBishopCorrect(board, start, end))
            {
                Console.WriteLine("Ферзь" + wrongCoordinatesMessage);
                return;
            }

            // меняем позицию фигуры на доске
            MoveFigure(board, start, end, FigureType.Bishop);
        }

        private static void TryMoveRook(Dictionary<char, Dictionary<char, char>> board, string start, string end)
        {
            if(!IsRookCorrect(start, end))
            {
                Console.WriteLine("Ладья" + wrongCoordinatesMessage);
                return;
            }

            // меняем позицию фигуры на доске
            MoveFigure(board, start, end, FigureType.Bishop);
        }

        private static void TryMovePawn(Dictionary<char, Dictionary<char, char>> board, string start, string end)
        {
            if(!IsPawnCorrect(start, end))
            {
                Console.WriteLine("Пешка" + wrongCoordinatesMessage);
                return;
            }

            // меняем позицию фигуры на доске
            MoveFigure(board, start, end, FigureType.Bishop);
        }

        private static bool IsKnightCorrect(string start, string end)
        {
            int dx = Math.Abs(end[0] - start[0]);
            int dy = Math.Abs(end[1] - start[1]);

            return dx + dy == 3 && dx * dy == 2;
        }

        private static bool IsBishopCorrect(Dictionary<char, Dictionary<char, char>> board, string start, string end)
        {
            // Перепрыгивать через фигуры может только конь, 
            // поэтому при проверке нужно убедить, что никто не стоит на пути

            // определеяем паттерн движения, который хочет проверить пользователь
            int deltaX = Math.Abs(end[0] - start[0]);
            int deltaY = Math.Abs(end[1] - start[1]);

            // ферзь ходит только по диагонале
            if(deltaX != deltaY)
                return false;

            // а теперь проверяем не перепрыгнул ли он через кого-нибудь

            // определеяем положительный или трицательный шаг
            int stepX = 1, stepY = 1;
            char startX = start[0], startY = start[1], endX = end[0], endY = end[1];

            if(end[0] < start[0]) stepX = -1;
            if(end[1] < start[1]) stepY = -1;

            // пробегаем все ячейки от начальной позиции до конечной и проверяем, чтобы там никто не стоял
            bool isCorrect = true;
            char currentX = (char) (startX + stepX), currentY = (char) (startY + stepY);
            while(currentX != endX && currentY != endY)
            {
                if(board[currentX][currentY] != ' ')
                {
                    isCorrect = false;
                    break;
                }

                currentX = (char) (currentX + stepX);
                currentY = (char) (currentY + stepY);
            }

            return isCorrect;
        }

        private static bool IsKingCorrect(string start, string end)
        {
            throw new NotImplementedException();
        }

        private static bool IsQueenCorrect(string start, string end)
        {
            throw new NotImplementedException();
        }

        private static bool IsRookCorrect(string start, string end)
        {
            throw new NotImplementedException();
        }

        private static bool IsPawnCorrect(string start, string end)
        {
            throw new NotImplementedException();
        }
        

        private static void DrawBoard(Dictionary<char, Dictionary<char, char>> board)
        {
            PrintBorder();
            PrintFigures(board);
            PrintCoordinates();
        }

        private static void ClearMessages()
        {
            int xStart = messageStartX, yStart = messageStartY;
            for(int x = xStart; x < xStart + 50; x++)
                for(int y = yStart; y < yStart + 3; y++)
                {
                    Console.SetCursorPosition(x, y);
                    Console.Write(' ');
                }
        }

        private static string[] ReadFigurePositions()
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

        public static void PrintBorder()
        {
            Console.SetCursorPosition(margin, margin);

            int maxX = fieldWidth * cellSizeX - fieldWidth;
            int maxY = fieldHeight * cellSizeY - fieldHeight;

            for (int y = 0; y <= maxY; y++)
            {
                bool isFirstRow = y == 0;
                bool isLastRow = y == maxY;
                bool isBorderHorizontal = y % (cellSizeY - 1) == 0;

                for (int x = 0; x <= maxX; x++)
                {
                    Console.SetCursorPosition(margin + x, margin + y);

                    bool isFirstColumn = x == 0;
                    bool isLastColumn = x == maxX;
                    bool isBorderVertical = x % (cellSizeX - 1) == 0;
                    bool isBorderCross = isBorderHorizontal && isBorderVertical;

                    if (isBorderCross)
                    {
                        if (isFirstColumn && isFirstRow)
                            Console.Write("┌");
                        else if (isFirstRow && !isFirstColumn && !isLastColumn)
                            Console.Write("┬");
                        else if (isFirstRow && isLastColumn)
                            Console.Write("┐");
                        else if (isFirstColumn && !isFirstRow && !isLastRow)
                            Console.Write("├");
                        else if (!isFirstRow && !isLastRow && !isFirstColumn && !isLastColumn)
                            Console.Write("┼");
                        else if (isLastColumn && !isFirstRow && !isLastRow)
                            Console.Write("┤");
                        else if (isLastRow && isFirstColumn)
                            Console.Write("└");
                        else if (isLastRow && !isFirstColumn && !isLastColumn)
                            Console.Write("┴");
                        else if (isLastColumn && isLastRow)
                            Console.Write("┘");
                    }
                    else
                    {
                        if (isBorderVertical) Console.Write("│");
                        else if (isBorderHorizontal) Console.Write("─");
                        else Console.Write(" ");
                    }

                    Console.ResetColor();
                }
            }
        }

        private static void PrintCoordinates()
        {
            int x = margin + cellSizeX / 2, y1 = margin - 1, y2 = margin + fieldHeight * (cellSizeY-1) + 1;

            for (char letter = '1'; letter <= '8'; letter++)
            {
                Console.SetCursorPosition(x, y1);
                Console.Write(letter);

                Console.SetCursorPosition(x, y2);
                Console.Write(letter);

                x += cellSizeX - 1;
            }

            int y = margin + cellSizeY / 2, x1 = margin - 1, x2 = margin + fieldWidth * (cellSizeX-1) + 1;
            for (char letter = 'A'; letter <= 'H'; letter++)
            {
                Console.SetCursorPosition(x1, y);
                Console.Write(letter);

                Console.SetCursorPosition(x2, y);
                Console.Write(letter);

                y += cellSizeY - 1;
            }
        }

        private static void PrintFigures(Dictionary<char, Dictionary<char, char>> board)
        {
            int xStep = cellSizeX - 1;
            int yStep = cellSizeY - 1;
            for (char letter = 'A'; letter <= 'H'; letter++)
                for (char num = '1'; num <= '8'; num++)
                {
                    int iRow    = num - '1';
                    int iColumn = letter - 'A';                    

                    int x, y;
                    if (iRow == 0) x = margin + cellSizeX / 2;
                    else           x = margin + iRow * xStep + xStep - 2;

                    if (iColumn == 0) y = margin + cellSizeY / 2;
                    else              y = margin + iColumn * yStep + yStep - 1;

                    Console.SetCursorPosition(x, y);
                    Console.Write(board[letter][num]);
                }

        }
    }
}