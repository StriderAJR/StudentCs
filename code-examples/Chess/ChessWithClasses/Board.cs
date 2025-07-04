using System;
using System.Collections.Generic;
using System.Text;

namespace ChessWithClasses
{
    public class Board
    {
        private const int fieldWidth = 8;
        private const int fieldHeight = 8;

        private int cellSizeX;
        private int cellSizeY;
        private int margin;

        private Dictionary<char, Dictionary<char, Figure>> board;

        enum FigureType
        {
            Pawn = 'P', Knight = 'H', Rook = 'R', Bishop = 'B', Queen = 'Q', King = 'K' 
        }

        public Board(int cellSizeX, int cellSizeY, int margin)
        {
            this.cellSizeX   = cellSizeX;
            this.cellSizeY   = cellSizeY;
            this.margin      = margin;

            board = new Dictionary<char, Dictionary<char, Figure>>()
            {
                { 'A', new Dictionary<char, Figure> { { '1', null }, { '2', null }, { '3', null }, { '4', null }, { '5', null }, { '6', null }, { '7', null }, { '8', null } } },
                { 'B', new Dictionary<char, Figure> { { '1', null }, { '2', null }, { '3', null }, { '4', null }, { '5', null }, { '6', null }, { '7', null }, { '8', null } } },
                { 'C', new Dictionary<char, Figure> { { '1', null }, { '2', null }, { '3', null }, { '4', null }, { '5', null }, { '6', null }, { '7', null }, { '8', null } } },
                { 'D', new Dictionary<char, Figure> { { '1', null }, { '2', null }, { '3', null }, { '4', null }, { '5', null }, { '6', null }, { '7', null }, { '8', null } } },
                { 'E', new Dictionary<char, Figure> { { '1', null }, { '2', null }, { '3', null }, { '4', null }, { '5', null }, { '6', null }, { '7', null }, { '8', null } } },
                { 'F', new Dictionary<char, Figure> { { '1', null }, { '2', null }, { '3', new Pawn() }, { '4', null }, { '5', null }, { '6', null }, { '7', null }, { '8', null } } },
                { 'G', new Dictionary<char, Figure> { { '1', new Pawn() }, { '2', new Pawn() }, { '3', null }, { '4', new Pawn() }, { '5', new Pawn() }, { '6', new Pawn() }, { '7', new Pawn() }, { '8', new Pawn() } } },
                { 'H', new Dictionary<char, Figure> { { '1', new Rook() }, { '2', new Bishop() }, { '3', new Knight() }, { '4', new Queen() }, { '5', new King() }, { '6', new Knight() }, { '7', new Bishop() }, { '8', new Rook() } } }
            };
        }

        public Figure Get(char letter, char number)
        {
            return board[letter][number];
        }

        public Figure Get(string position)
        {
            return Get(position[0], position[1]);
        }

        public void DrawBoard()
        {
            PrintBorder();
            PrintFigures();
            PrintCoordinates();
        }

        public bool TryMoveFigure(string start, string end)
        {
            var figure = board[start[0]][start[1]];

            // TODO нужно пробрасывать ошибку обратно в Game
            //if(figure == null)
            //{
            //    Console.WriteLine("На стартовой точке нет фигуры. Нажмите любую клавишу, чтобы повторить...");
            //}

            return TryMove(start, end, figure.IsCorrectMove, figure);
        }

        private bool TryMove(string start, string end, Func<Board, string, string, bool> checkMethod, Figure figure)
        {
            if(!checkMethod(this, start, end))
            {
                return false;
            }

            // меняем позицию фигуры на доске
            MoveFigure(start, end, figure);
            return true;
        }

        private void MoveFigure(string start, string end, Figure figure)
        {
            board[start[0]][start[1]] = null;
            board[end[0]]  [end[1]]   = figure;
        }

        private void PrintBorder()
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

        private void PrintCoordinates()
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

        private void PrintFigures()
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
                    Console.Write(board[letter][num]?.Abbreviation);
                }

        }
    }
}
