using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;

namespace ChessWithClasses
{
    class Program
    {
        static void Main(string[] args)
        {
            Game game = new Game();
            do
            {
                game.Refresh();
                
                string[] coords = game.ReadFigurePositions();
                string start = coords[0];
                string end = coords[1];

                game.TryMoveFigure(start, end);
            } while (true);
        }
    }
}