﻿using System;
using System.Drawing;

namespace FirstApp
{
    static class Player
    {
        public static int X, Y;

        public static void MovePlayer(int shiftX, int shiftY)
        {
            // ...
        }
    }

    class Auto
    {
        public string Color; // 8 byte
        public int Wheels; // 4 byte
    }

    class Map
    {
        public char[,] map;
        public static int Gold;
        public static int Wood;
        public void Print()
        {
            Console.WriteLine($"Map = {map.LongLength}, Gold = {Map.Gold}");
            Map.Gold--;
        }
    }

    class Programm
    {
        private static string[][] map; // public protected
        private static int playerX, playerY;

        private void Print()
        {
            // ...
        }

        private static void Main()
        {
            Player.X = 0;
            Player.Y = 0;

            Auto auto = new Auto();
            auto.Color = "red"; //  заполненные данные = экземпляр класса = объект
            auto.Wheels = 4;

            Map map1 = new Map();
            Map map2 = new Map();

            map1.map = new char[10,10];
            map2.map = new char[15,20];
            Map.Gold = 1000;
            map1.Print();
            map2.Print();

            map = GenerateMap();
            
            int startX = 1;
            int startY = 1;
            map[startX][startY] = "@";

            LoopGame();
        }

        private static string[][] GenerateMap()
        {
            string[][] playGround = new string[10][];
            for (int i = 0; i < 10; i++)
            {
                if (i == 9 || i == 0)
                {
                    playGround[i] = new string[10] { "#", "#", "#", "#", "#", "#", "#", "#", "#", "#" };                    
                }
                else
                {
                    playGround[i] = new string[10] { "#", " ", " ", " ", " ", " ", " ", " ", " ", "#" };
                }
            }

            return playGround;
        }

        static void MovePlayer(int shiftX, int shiftY)
        {
            if (map[playerY + shiftY][playerX + shiftX] != "#")
            {
                playerY--;
                map[playerY][playerX] = "@";
                map[playerY - shiftY][playerX - shiftX] = " ";
            }
        }

        //             ТВЗ имя_метода(список_входных_параметров)
        private static void LoopGame()
        {
            while (true)
            {
                Console.Clear();
                PrintPlayGround();

                string commanda = Console.ReadLine();
                if (commanda == "w")
                {
                    MovePlayer(0, -1);
                }
                if (commanda == "s")
                {
                    MovePlayer(0, 1);
                }
                if (commanda == "d")
                {
                    MovePlayer(1, 0);
                }
                if (commanda == "a")
                {
                    MovePlayer(-1, 0);
                }
            }
        }

        private static void PrintPlayGround()
        {
            for (int i = 0; i < map.Length; i++)
            {
                Console.WriteLine(string.Join("", map[i]));
            }
        }

        private static void GameOver()
        {
            Console.Clear();
            Console.WriteLine("Game over");
        }
    }
}  