using System;
using System.Collections.Generic;
using System.Text;

namespace ChessWithClasses
{
    public abstract class Figure
    {
        public abstract char Abbreviation { get; }
        public abstract string RusName { get; }
        public abstract bool IsCorrectMove(Board board, string start, string end);
        
    }

    public class Pawn : Figure
    {
        public override char Abbreviation => 'P';

        public override string RusName => "Пешка";

        public override bool IsCorrectMove(Board board, string start, string end)
        {
            throw new NotImplementedException();
        }
    }

    public class Rook : Figure
    {
        public override char Abbreviation => 'R';

        public override string RusName => "Ладья";

        public override bool IsCorrectMove(Board board, string start, string end)
        {
            throw new NotImplementedException();
        }
    }

    public class Knight : Figure
    {
        public override char Abbreviation => 'H';

        public override string RusName => "Конь";

        public override bool IsCorrectMove(Board board, string start, string end)
        {
            // конь может перепрыгивать через всех, поэтому не нужно проверять стоит ли кто-то перед ним
            int dx = Math.Abs(end[0] - start[0]);
            int dy = Math.Abs(end[1] - start[1]);

            return dx + dy == 3 && dx * dy == 2;
        }
    }

    public class Bishop : Figure
    {
        public override char Abbreviation => 'B';

        public override string RusName => "Ферзь";

        public override bool IsCorrectMove(Board board, string start, string end)
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
                if(board.Get(currentX, currentY) != null)
                {
                    isCorrect = false;
                    break;
                }

                currentX = (char) (currentX + stepX);
                currentY = (char) (currentY + stepY);
            }

            return isCorrect;
        }
    }

    public class Queen : Figure
    {
        public override char Abbreviation => 'Q';

        public override string RusName => "Королева";

        public override bool IsCorrectMove(Board board, string start, string end)
        {
            throw new NotImplementedException();
        }
    }

    public class King : Figure
    {
        public override char Abbreviation => 'K';

        public override string RusName => "Король";

        public override bool IsCorrectMove(Board board, string start, string end)
        {
            throw new NotImplementedException();
        }
    }
}
