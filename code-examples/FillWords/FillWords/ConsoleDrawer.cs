using System;

namespace FillWords
{
    public class ConsoleDrawer : IDrawer
    {
        private readonly int _margin;
        private readonly int _cellSizeY;
        private readonly int _cellSizeX;

        private const ConsoleColor HoverColor     = ConsoleColor.Red;
        private const ConsoleColor SelectionColor = ConsoleColor.Cyan;
        private const ConsoleColor DefaultColor   = ConsoleColor.Black;
        private const ConsoleColor GuessedColor   = ConsoleColor.Gray;

        public ConsoleDrawer(int margin = 5, int cellSizeX = 5, int cellSizeY = 3)
        {
            _cellSizeX = cellSizeX;
            _cellSizeY = cellSizeY;
            _margin = margin;
        }
        
        public void Draw(FillWords game, int hoverX, int hoverY)
        {
            PrintField(game);
            PrintLetters(game);
            Console.SetCursorPosition(0, 0);
        }

        public void DrawMessage(string message)
        {
            int marginX = 10, marginY = 10;
            Console.ForegroundColor = ConsoleColor.Red;
            PrintMessageBox(marginX, marginY, message.Length * 2);
            Console.SetCursorPosition(marginX + message.Length / 2, marginY+1);
            Console.Write(message);
            Console.ReadKey();
        }

        private void PrintField(FillWords game)
        {
            Console.SetCursorPosition(_margin, _margin);

            int maxX = game.GetFieldWidth()  * _cellSizeX - game.GetFieldWidth();
            int maxY = game.GetFieldHeight() * _cellSizeY - game.GetFieldHeight();

            for (int y = 0; y <= maxY; y++)
            {
                bool isFirstRow = y == 0;
                bool isLastRow = y == maxY;
                bool isBorderHorizontal = y % (_cellSizeY - 1) == 0;

                for (int x = 0; x <= maxX; x++)
                {
                    bool isFirstColumn    = x == 0;
                    bool isLastColumn     = x == maxX;
                    bool isBorderVertical = x % (_cellSizeX - 1) == 0;
                    bool isBorderCross    = isBorderHorizontal && isBorderVertical;
                    
                    int fieldX = (x - 1) / (_cellSizeX - 1);
                    int fieldY = (y - 1) / (_cellSizeY - 1);
                    
                    Console.SetCursorPosition(_margin + x, _margin + y);
                    
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
                        if (isBorderVertical)
                        {
                            Console.Write("│");
                        }
                        else if (isBorderHorizontal)
                        {
                            Console.Write("─");
                        }
                        else
                        {
                            ConsoleColor color = GetColor(game, fieldX, fieldY);
                            Console.BackgroundColor = color;
                            Console.Write(" ");
                        }
                    }

                    Console.ResetColor();
                }
            }
        }

        private void PrintLetters(FillWords game)
        {
            int xStep = _cellSizeX - 1;
            int yStep = _cellSizeY - 1;
            for (int fieldY = 0; fieldY < game.GetFieldHeight(); fieldY++)
            for (int fieldX = 0; fieldX < game.GetFieldWidth(); fieldX++)
            {
                int x, y;
                if (fieldX == 0) x = _margin + _cellSizeX / 2;
                else x = _margin + fieldX * xStep + xStep - 2;

                if (fieldY == 0) y = _margin + _cellSizeY / 2;
                else y = _margin + fieldY * yStep + yStep - 1;

                Console.BackgroundColor = GetColor(game, fieldX, fieldY);
                Console.SetCursorPosition(x, y);
                Console.Write(game.GetLetter(fieldX, fieldY));
                Console.ResetColor();
            }
        }

        private ConsoleColor GetColor(FillWords game, int x, int y)
        {
            return game.IsCellHovered(x, y)
                    ? HoverColor
                    : game.IsCellSelected(x, y)
                        ? SelectionColor
                        : game.IsCellGuessed(x, y)
                            ? GuessedColor
                            : DefaultColor;
        }

        private void PrintMessageBox(int marginX, int marginY, int width)
        {
            Console.SetCursorPosition(marginX, marginY++);
            Console.Write("╔" + new string('═', width) + "╗");
            Console.SetCursorPosition(marginX, marginY++);
            Console.Write("║" + new string(' ', width) + "║");
            Console.SetCursorPosition(marginX, marginY);
            Console.Write("╚" + new string('═', width) + "╝");
        }
    }
}